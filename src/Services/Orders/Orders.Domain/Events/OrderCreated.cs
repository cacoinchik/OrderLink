using System.Runtime.InteropServices;

namespace Orders.Domain.Events
{
    public class OrderCreated
    {
        public Guid Id { get; set; }
        public string CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public DateTime TimeCreate { get; set; }
    }
}
