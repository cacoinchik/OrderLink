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
        public DateTime TimeCreate { get; private set; }
        public DateTime? TimeUpdate { get; private set; }

        //Информация об адресе доставки
        public string ShippingCountry { get; private set; }
        public string ShippingPostalCode { get; private set; }
        public string ShippingCity { get; private set; }
        public string ShippingAddress { get; private set; }

        //Детали заказа
        private readonly List<OrderItem> _orderItems = new();
        public IReadOnlyCollection<OrderItem> Items => _orderItems;

        private Order() { }

        public Order(string customerId, List<OrderItem> items, string country, string postalCode, string city, string address)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            Status = OrderStatus.New;
            TimeCreate = DateTime.UtcNow;
            _orderItems = items;
            TotalAmount = items.Sum(x => x.Price * x.Count);
            Currency = items.First().Currency;
            ShippingCountry = country;
            ShippingPostalCode = postalCode;
            ShippingCity = city;
            ShippingAddress = address;
        }

        //Методы для смены статуса заказа
        //TODO метод для русифицирования статусов :)
        public void SetReserved()
        {
            if (Status is not OrderStatus.New)
                throw new InvalidOperationException($"Невозможно сменить статус с '{Status}' на 'Резерв'");

            Status = OrderStatus.Reserved;
            TimeUpdate = DateTime.UtcNow;
        }

        public void SetPaymentPending()
        {
            if (Status is not OrderStatus.Reserved)
                throw new InvalidOperationException($"Невозможно сменить статус с {Status} на 'Отправка платежа'");

            Status = OrderStatus.PaymentPending;
            TimeUpdate = DateTime.UtcNow;
        }

        public void SetPaid()
        {
            if (Status is not OrderStatus.PaymentPending)
                throw new InvalidOperationException($"Невозможно сменить статус с {Status} на 'Оплачено'");

            Status = OrderStatus.Paid;
            TimeUpdate = DateTime.UtcNow;
        }

        public void SetCompleted()
        {
            if (Status is not OrderStatus.Paid)
                throw new InvalidOperationException($"Невозможно сменить статус с {Status} на 'Завершен'");

            Status = OrderStatus.Completed;
            TimeUpdate = DateTime.UtcNow;
        }

        public void CancelOrder()
        {
            if (Status is OrderStatus.Completed)
                throw new InvalidOperationException("Невозможно отменить завершенный заказ'");

            if (Status is OrderStatus.Cancelled)
                throw new InvalidOperationException("Данный заказ уже отменен");

            Status = OrderStatus.Cancelled;
            TimeUpdate = DateTime.UtcNow;
        }

        public void SetFailed()
        {
            if (Status is OrderStatus.Cancelled || Status is OrderStatus.Completed)
                throw new InvalidOperationException($"Невозможно сменить статус с {Status} на 'Платёж не прошел'");

            Status = OrderStatus.Failed;
            TimeUpdate = DateTime.UtcNow;
        }

        public void SetExpired()
        {
            if (Status is OrderStatus.Cancelled || Status is OrderStatus.Completed)
                throw new InvalidOperationException($"Невозможно сменить статус с {Status} на 'Истекший");

            Status = OrderStatus.Expired;
            TimeUpdate = DateTime.UtcNow;
        }
    }
}
