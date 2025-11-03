using Inventory.Domain.Enums;

namespace Inventory.Domain.Entities
{
    public class Reservation
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid StockId { get; private set; }
        public Guid WarehouseId { get; private set; }
        public string Sku { get; private set; }
        public int Count { get; private set; }
        public ReservationStatus Status { get; private set; }
        public DateTime TimeCreate { get; private set; }
        public DateTime TimeExpired { get; private set; }
        public DateTime? TimeCommitted { get; private set; }
        public DateTime? TimeReleased { get; private set; }

        private Reservation() { }

        //TODO настраиваемое время для TTL (пока 15 по умолчанию)
        public Reservation(Guid orderId, Guid stockId, Guid warehouseId, string sku, int count, int ttlMinutes = 15)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            StockId = stockId;
            WarehouseId = warehouseId;
            Sku = sku;
            Count = count;
            Status = ReservationStatus.Reserved;
            TimeCreate = DateTime.UtcNow;
            TimeExpired = DateTime.UtcNow.AddMinutes(ttlMinutes);
        }

        //TODO статусы на русском языке
        public void Commit()
        {
            if (Status != ReservationStatus.Reserved)
                throw new InvalidOperationException($"Невозможно подтвердить резерв в статусе {Status}");

            Status = ReservationStatus.Committed;
            TimeCommitted = DateTime.UtcNow;
        }

        public void Release()
        {
            if (Status == ReservationStatus.Committed)
                throw new InvalidOperationException("Невозможно снять подтвержденный резерв");

            if (Status == ReservationStatus.Released)
                return;

            Status = ReservationStatus.Released;
            TimeReleased = DateTime.UtcNow;
        }

        public void MarkAsExpired()
        {
            if (Status != ReservationStatus.Reserved)
                return;

            Status = ReservationStatus.Expired;
            TimeReleased = DateTime.UtcNow;
        }

        public bool IsExpired()
        {
            var result = DateTime.UtcNow > TimeExpired && Status == ReservationStatus.Reserved;
            return result;
        }
    }
}