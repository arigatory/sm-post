using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using MediatR;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Converters;

namespace Post.Query.Infrastructure.Consumers;

public class EventConsumer : IEventConsumer
{
    private readonly ConsumerConfig _config;
    private readonly IMediator _mediator;

    public EventConsumer(IOptions<ConsumerConfig> consumerConfig, IMediator mediator)
    {
        _config = consumerConfig.Value;
        _mediator = mediator;
    }

    public void Consume(string topic)
    {
        using var consumer = new ConsumerBuilder<string, string>(_config)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(Deserializers.Utf8)
            .SetErrorHandler((_, e) => Console.WriteLine($"Kafka Consumer Error: {e.Reason}"))
            .Build();

        consumer.Subscribe(topic);
        Console.WriteLine($"Kafka consumer subscribed to topic: {topic}");

        while (true)
        {
            try
            {
                var consumerResult = consumer.Consume();

                if (consumerResult?.Message == null) continue;

                Console.WriteLine($"Received message: {consumerResult.Message.Value}");

                var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };
                var @event = JsonSerializer.Deserialize<BaseEvent>(consumerResult.Message.Value, options);

                if (@event == null)
                {
                    Console.WriteLine("Failed to deserialize event");
                    continue;
                }

                Console.WriteLine($"Processing event type: {@event.Type}");

                if (@event is INotification notification)
                {
                    // Publish via MediatR - all registered handlers will be invoked
                    _mediator.Publish(notification).GetAwaiter().GetResult();
                    consumer.Commit(consumerResult);
                    Console.WriteLine($"Successfully processed and committed event: {@event.Type}");
                }
                else
                {
                    Console.WriteLine($"Event does not implement INotification: {@event.GetType().Name}");
                    consumer.Commit(consumerResult);
                }
            }
            catch (ConsumeException ex)
            {
                Console.WriteLine($"Consume error: {ex.Error.Reason}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner stack trace: {ex.InnerException.StackTrace}");

                    if (ex.InnerException.InnerException != null)
                    {
                        Console.WriteLine($"Inner inner exception: {ex.InnerException.InnerException.Message}");
                    }
                }
            }
        }
    }
}
