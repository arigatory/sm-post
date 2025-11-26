using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Converters;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers;

public class EventConsumer : IEventConsumer
{
    private readonly ConsumerConfig _config;
    private readonly IEventHandler _eventHandler;

    public EventConsumer(IOptions<ConsumerConfig> consumerConfig, IEventHandler eventHandler)
    {
        _config = consumerConfig.Value;
        _eventHandler = eventHandler;
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

                var handlerMethod = _eventHandler.GetType().GetMethod("On", new Type[] { @event.GetType() });

                if (handlerMethod == null)
                {
                    Console.WriteLine($"Could not find handler method for event type: {@event.GetType().Name}");
                    consumer.Commit(consumerResult);
                    continue;
                }

                // Invoke the async handler method and await it
                var task = (Task)handlerMethod.Invoke(_eventHandler, new object[] { @event });
                if (task != null)
                {
                    task.Wait();
                }

                consumer.Commit(consumerResult);

                Console.WriteLine($"Successfully processed and committed event: {@event.Type}");
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
