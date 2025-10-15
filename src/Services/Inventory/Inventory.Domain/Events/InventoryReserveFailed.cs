namespace Inventory.Domain.Events
{
    public class InventoryReserveFailed
    {
        public Guid OrderId { get; set; }
        public string Reason { get; set; }
    }
}