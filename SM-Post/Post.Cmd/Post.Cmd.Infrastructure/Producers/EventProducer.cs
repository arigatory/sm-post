using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Producers;
using Microsoft.Extensions.Options;

namespace Post.Cmd.Infrastructure.Producers;

public class EventProducer : IEventProducer
{
    private readonly ProducerConfig _config;
    public EventProducer(IOptions<ProducerConfig> config)
    {
        _config = config.Value;
    }

    public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
    {
        using var producer = new ProducerBuilder<string, string>(_config)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(Serializers.Utf8)
            .SetErrorHandler((_, e) => Console.WriteLine($"Kafka Error: {e.Reason}"))
            .Build();

        var eventMessage = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(@event, @event.GetType())
        };

        try
        {
            var deliveryResult = await producer.ProduceAsync(topic, eventMessage);

            if (deliveryResult.Status == PersistenceStatus.NotPersisted)
            {
                throw new Exception($"Could not produce {(string.IsNullOrEmpty(@event.GetType().Name) ? "event" : @event.GetType().Name)} message to topic - {topic} due to the following reason: {deliveryResult.Message}.");
            }

            Console.WriteLine($"Successfully produced event to {topic}: {deliveryResult.Value}");
        }
        catch (ProduceException<string, string> ex)
        {
            Console.WriteLine($"Failed to produce message to {topic}: {ex.Error.Reason}");
            throw;
        }
    }
}