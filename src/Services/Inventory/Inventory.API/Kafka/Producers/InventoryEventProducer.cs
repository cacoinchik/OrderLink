using Confluent.Kafka;
using Inventory.API.Kafka.Configuration;
using Inventory.Domain.Events;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Inventory.API.Kafka.Producers
{
    public class InventoryEventProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly KafkaSettings _kafkaSettings;

        public InventoryEventProducer(IOptions<KafkaSettings> kafkaSettings)
        {
            _kafkaSettings = kafkaSettings.Value;

            var config = new ProducerConfig
            {
                BootstrapServers = _kafkaSettings.BootstrapServers,
                Acks = Acks.All,
                MessageTimeoutMs = 10000,
                EnableIdempotence = true
            };

            _producer = new ProducerBuilder<string, string>(config: config).Build();
        }

        public async Task PublishInventoryReservedAsync(InventoryReserved inventoryReservedEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                var message = new Message<string, string>
                {
                    Key = inventoryReservedEvent.OrderId.ToString(),
                    Value = JsonSerializer.Serialize(inventoryReservedEvent)
                };

                var result = await _producer.ProduceAsync(_kafkaSettings.Topics.InventoryEvents, message, cancellationToken);
            }
            catch (Exception ex) { }
        }

        public async Task PublishInventoryReserveFailedAsync(InventoryReserveFailed inventoryReserveFailedEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                var message = new Message<string, string>
                {
                    Key = inventoryReserveFailedEvent.OrderId.ToString(),
                    Value = JsonSerializer.Serialize(inventoryReserveFailedEvent)
                };

                var result = await _producer.ProduceAsync(_kafkaSettings.Topics.InventoryEvents, message, cancellationToken);
            }
            catch (Exception ex) { }
        }
    }
}
