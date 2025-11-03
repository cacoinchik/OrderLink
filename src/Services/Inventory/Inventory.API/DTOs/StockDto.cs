namespace Inventory.API.DTOs
{
    public class StockDto
    {
        public Guid Id { get; set; }
        public Guid WarehouseId { get; set; }
        public string Sku { get; set; }
        public int AvailableQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int TotalQuantity { get; set; }
        public DateTime TimeUpdate { get; set; }
    }

    public class ManageStockQuantityRequest
    {
        public Guid WarehouseId { get; set; }
        public string Sku { get; set; }
        public int Quantity { get; set; }
    }
}
