namespace Inventory.Domain.Events
{
    public class InventoryReleased
    {
        public Guid ReservationId { get; set; }
        public Guid OrderId { get; set; }
    }
}
