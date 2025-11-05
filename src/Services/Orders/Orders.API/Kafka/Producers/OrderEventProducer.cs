using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Orders.API.Kafka.Configuration;
using Orders.Domain.Events;
using System.Text.Json;

namespace Orders.API.Kafka.Producers
{
    public class OrderEventProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly KafkaSettings _kafkaSettings;

        public OrderEventProducer(IOptions<KafkaSettings> kafkaSettings)
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

        public async Task PublishOrderCreatedAsync(OrderCreated orderCreatedEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                var message = new Message<string, string>
                {
                    Key = orderCreatedEvent.Id.ToString(),
                    Value = JsonSerializer.Serialize(orderCreatedEvent)
                };

                var result = await _producer.ProduceAsync(_kafkaSettings.Topics.OrderEvents, message, cancellationToken);
            }
            catch (Exception ex)
            {

            }

        }
    }
}
