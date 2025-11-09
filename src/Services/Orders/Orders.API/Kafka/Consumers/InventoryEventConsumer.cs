using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Options;
using Orders.API.Kafka.Configuration;
using System.Security.Cryptography;

namespace Orders.API.Kafka.Consumers
{
    public class InventoryEventConsumer : BackgroundService
    {
        private IConsumer<string, string>? _consumer;
        private readonly KafkaSettings _kafkaSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InventoryEventConsumer> _logger;


        public InventoryEventConsumer(IOptions<KafkaSettings> kafkaSettings, IServiceProvider serviceProvider, ILogger<InventoryEventConsumer> logger)
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
            _logger.LogInformation($"Kafka GroupId: {_kafkaSettings.GroupId}");
            _logger.LogInformation($"Kafka Topics: {_kafkaSettings.Topics.InventoryEvents}");

            try
            {
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

                _logger.LogInformation($"Subscribing to topic: {_kafkaSettings.Topics.InventoryEvents}");
                _consumer.Subscribe(_kafkaSettings.Topics.InventoryEvents);
                _logger.LogInformation("Successfully subscribed to topic");

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
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Error consuming message from Kafka");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unexpected error");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Fatal Error - service will stop");
                throw;
            }
            finally
            {
                _logger.LogInformation("CLosing Kafka consumer");
                _consumer?.Close();
                _logger.LogInformation("Kafka consumer closed");
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
