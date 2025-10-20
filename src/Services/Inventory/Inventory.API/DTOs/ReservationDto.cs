namespace Inventory.API.DTOs
{
    public class ReservationDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid StockId { get; set; }
        public Guid WarehouseId { get; set; }
        public string Sku { get; set; }
        public int Count { get; set; }
        public string Status { get; set; }
        public DateTime TimeCreate { get; set; }
        public DateTime TimeExpired { get; set; }
        public DateTime? TimeCommited { get; set; }
        public DateTime? TimeReleased { get; set; }
    }

    public class CreateReservationRequest
    {
        public Guid OrderId { get; set; }
        public Guid WarehouseId { get; set; }
        public string Sku { get; set; }
        public int Count { get; set; }
        public int TtlMinutes { get; set; } = 15;
    }
}
