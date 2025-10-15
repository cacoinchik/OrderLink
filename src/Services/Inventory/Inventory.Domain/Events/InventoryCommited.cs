namespace Inventory.Domain.Events
{
    public class InventoryCommited
    {
        public Guid ReservationId { get; set; }
        public Guid OrderId { get; set; }
    }

}
