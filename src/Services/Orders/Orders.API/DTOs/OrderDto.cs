using Orders.Domain.Entities;

namespace Orders.API.DTOs
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public string CustomerId { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency {  get; set; }   
        public DateTime TimeCreate { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string AddressLine { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
