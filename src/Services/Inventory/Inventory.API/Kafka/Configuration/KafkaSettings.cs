namespace Inventory.API.Kafka.Configuration
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public KafkaTopics Topics { get; set; } = new();
    }
    public class KafkaTopics
    {
        public string OrderEvents { get; set; } = string.Empty;
        public string InventoryEvents { get; set; } = string.Empty;
    }
}
