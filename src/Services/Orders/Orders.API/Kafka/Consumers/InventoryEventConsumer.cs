using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Options;
using Orders.API.Kafka.Configuration;

namespace Orders.API.Kafka.Consumers
{
    public class InventoryEventConsumer : BackgroundService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly KafkaSettings _kafkaSettings;
        private readonly IServiceProvider _serviceProvider;

        public InventoryEventConsumer(IOptions<KafkaSettings> kafkaSettings, IServiceProvider serviceProvider)
        {
            _kafkaSettings = kafkaSettings.Value;
            _serviceProvider = serviceProvider;

            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaSettings.BootstrapServers,
                GroupId = _kafkaSettings.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            _consumer = new ConsumerBuilder<string, string>(config: config).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_kafkaSettings.Topics.InventoryEvents);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(1));

                        if(consumeResult is not null)
                        {
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
            await Task.CompletedTask;
        }
    }
}
