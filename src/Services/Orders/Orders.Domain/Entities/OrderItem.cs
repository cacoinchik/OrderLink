namespace Orders.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public string Sku {  get; private set; }
        public int Count { get; private set; }
        public decimal Price { get; private set; }
        public string Currency { get; private set; }

        private OrderItem() { }

        public OrderItem(string sku, int count, decimal price, string currency)
        {
            Id = Guid.NewGuid();
            Sku = sku;
            Count = count;
            Price = price;
            Currency = currency;
        }

    }
}
