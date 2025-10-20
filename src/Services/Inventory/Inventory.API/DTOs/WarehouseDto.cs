namespace Inventory.API.DTOs
{
    public class CreateWarehouseRequest
    {
        public string Name { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
    }

    public class WarehouseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime TimeCreate { get; set; }
    }
}
