using Orders.Domain.Enums;

namespace Orders.Domain.Entities
{
    public class Order
    {
        //Общая инфомраиця о заказе
        public Guid Id { get; private set; }
        public string CustomerId { get; private set; }
        public OrderStatus Status { get; private set; }
        public decimal TotalAmount { get; private set; }
        public string Currency { get; private set; }
        public DateTime CreateTime { get; private set; }
        public DateTime? UpdateTime { get; private set; }

        //Информация об адресе доставки
        public string ShippingCountry { get; private set; }
        public string ShippinPostalCode { get; private set; }
        public string ShippingCity { get; private set; }
        public string ShippingAddress { get; private set; }

        //Детали заказа
        private readonly List<OrderItem> orderItems = new();
        public IReadOnlyCollection<OrderItem> Items => orderItems;

        private Order() { }

        public Order(string customerId, List<OrderItem> items, string country, string postalCode, string city, string address)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            Status = OrderStatus.New;
            CreateTime = DateTime.UtcNow;
            orderItems = items;
            TotalAmount = items.Sum(x => x.Price * x.Count);
            Currency = items.First().Currency;
            ShippingCountry = country;
            ShippinPostalCode = postalCode;
            ShippingCity = city;
            ShippingAddress = address;
        }
    }
}
