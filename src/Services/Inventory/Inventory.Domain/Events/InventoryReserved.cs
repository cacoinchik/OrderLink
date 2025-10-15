namespace Inventory.Domain.Events
{
    public class InvenotryReserved
    {
        public Guid ReservationId { get; set; }
        public Guid OrderId { get; set; }
        public DateTime TimeExpired { get; set; }
    }
}