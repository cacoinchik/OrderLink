namespace Inventory.Domain.Entities
{
    public class Stock
    {
        public Guid Id { get; private set; }
        public Guid WarehouseId { get; private set; }
        public string Sku { get; private set; }
        public int AvailableQuantity { get; private set; }
        public int ReservedQuantity { get; private set; }
        public int TotalQuantity { get; private set; }
        public DateTime TimeUpdate { get; private set; }
        public byte[] RowVersion { get; private set; }

        private Stock() { }

        public Stock(Guid warehouseId, string sku, int initialQuantity)
        {
            Id = Guid.NewGuid();
            WarehouseId = warehouseId;
            Sku = sku;
            AvailableQuantity = initialQuantity;
            ReservedQuantity = 0;
            TotalQuantity = initialQuantity;
            TimeUpdate = DateTime.UtcNow;
        }

        public void Reserve(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Количество должно быть больше 0");

            if (AvailableQuantity < quantity)
                throw new InvalidOperationException($"Недостаточно товара на складе. Доступный остаток: {AvailableQuantity}");

            AvailableQuantity -= quantity;
            ReservedQuantity += quantity;
            TimeUpdate = DateTime.UtcNow;
        }

        public void Release(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Количество должно быть больше 0");

            if (ReservedQuantity < quantity)
                throw new InvalidOperationException($"Недостаточно зарезервированного товара. Зарезервировано: {ReservedQuantity}");

            ReservedQuantity -= quantity;
            AvailableQuantity += quantity;
            TimeUpdate = DateTime.UtcNow;
        }

        public void Commit(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Количество должно быть больше 0");

            if (ReservedQuantity < quantity)
                throw new InvalidOperationException($"Недостаточно зарезервированного товара для списания. Зарезервировано: {ReservedQuantity}");

            ReservedQuantity -= quantity;
            TotalQuantity -= quantity;
            TimeUpdate = DateTime.UtcNow;
        }

        public void ManageQuantity(int quantity)
        {
            var newTotal = TotalQuantity + quantity;

            if (newTotal <= 0)
                throw new InvalidOperationException("Итоговое количество не может быть отрицательным");
            if (newTotal < ReservedQuantity)
                throw new InvalidOperationException("Остаток ниже зарезервированного количества");

            TotalQuantity = newTotal;
            AvailableQuantity = TotalQuantity - ReservedQuantity;
            TimeUpdate = DateTime.UtcNow;
        }
    }
}