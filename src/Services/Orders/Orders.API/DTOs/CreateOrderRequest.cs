namespace Orders.API.DTOs
{
    public class CreateOrderRequest
    {
        public string CustomerId { get; set; }
        public List<OrderItemDto> Items { get; set; }
        public ShippingAddressDto ShippingAddress { get; set; }
    }
    public class OrderItemDto
    {
        public string Sku { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
    }
    public class ShippingAddressDto
    {
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string AddressLine { get; set; }
    }
}
