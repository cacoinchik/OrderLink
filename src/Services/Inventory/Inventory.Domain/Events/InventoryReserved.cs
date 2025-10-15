namespace Inventory.Domain.Events
{
    public class InventoryReserved
    {
        public Guid ReservationId { get; set; }
        public Guid OrderId { get; set; }
        public DateTime TimeExpired { get; set; }
    }
}