
using Confluent.Kafka;
using Inventory.API.Kafka.Configuration;
using Microsoft.Extensions.Options;

namespace Inventory.API.Kafka.Consumers
{
    public class OrderEventConsumer : BackgroundService
    {
        private IConsumer<string, string>? _consumer;
        private readonly KafkaSettings _kafkaSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrderEventConsumer> _logger;

        public OrderEventConsumer(IOptions<KafkaSettings> kafkaSettings, IServiceProvider serviceProvider, ILogger<OrderEventConsumer> logger)
        {
            _kafkaSettings = kafkaSettings.Value;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            _logger.LogInformation("Starting Kafka Consumer...");
            _logger.LogInformation($"Kafka BootstrapServers: {_kafkaSettings.BootstrapServers}");
            _logger.LogInformation($"Kafka GroupId {_kafkaSettings.GroupId}");
            _logger.LogInformation($"Kafka Topics {_kafkaSettings.Topics.InventoryEvents}");

            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaSettings.BootstrapServers,
                GroupId = _kafkaSettings.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            _logger.LogInformation("Kafka creating consumer");
            _consumer = new ConsumerBuilder<string, string>(config: config).Build();
            _logger.LogInformation("Consumer created");

            _logger.LogInformation($"Subscribing to topic: {_kafkaSettings.Topics.OrderEvents}");
            _consumer.Subscribe(_kafkaSettings.Topics.OrderEvents);
            _logger.LogInformation("Successfully subscribed to topic");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(1));

                        if (consumeResult is not null)
                        {
                            _logger.LogInformation($"Message recieved from Kafka: {consumeResult.Message.Key}");
                            await HandleMessageAsync(consumeResult, stoppingToken);
                            _consumer.Commit(consumeResult);
                        }
                    }
                    catch (Exception ex) { }

                }
            }
            finally
            {
                _consumer.Close();
            }
        }

        public override void Dispose()
        {
            _consumer?.Dispose();
            base.Dispose();
        }

        private async Task HandleMessageAsync(ConsumeResult<string, string> result, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Message received from Kafka - Key: {result.Message.Key}, Value: {result.Message.Value}");

            //TODO Обработка сообщения

            await Task.CompletedTask;
        }
    }
}
