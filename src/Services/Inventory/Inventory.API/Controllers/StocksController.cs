using Inventory.API.DTOs;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class StocksController : ControllerBase
    {
        private readonly InventoryDbContext _context;

        public StocksController(InventoryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetStocks(string? sku = null, Guid? warehouseId = null)
        {
            var stocks = _context.Stocks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(sku))
                stocks = stocks.Where(s => s.Sku == sku);

            if (warehouseId.HasValue)
                stocks = stocks.Where(s => s.WarehouseId == warehouseId);

            var sortStocks = await stocks.OrderByDescending(s => s.Sku).ThenBy(s => s.WarehouseId).ToListAsync();

            var result = stocks.Select(MapToDto).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStockById(Guid id)
        {
            var stock = await _context.Stocks.FindAsync(id);

            if (stock is null)
                return NotFound("Остатки данного товара не найдены");

            return Ok(MapToDto(stock));
        }

        [HttpPost("manage")]
        public async Task<IActionResult> ManageStock(ManageStockQuantityRequest request)
        {
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.WarehouseId ==  request.WarehouseId && s.Sku == request.Sku);

            if (stock is null)
            {
                stock = new Stock(
                    warehouseId: request.WarehouseId,
                    sku: request.Sku,
                    initialQuantity: request.Quantity
                );

                await _context.Stocks.AddAsync(stock);
            }
            else
            {
                stock.ManageQuantity(request.Quantity);
            }

            await _context.SaveChangesAsync();

            return Ok(MapToDto(stock));
        }

        private StockDto MapToDto(Stock stock)
        {
            return new StockDto
            {
                Id = stock.Id,
                WarehouseId = stock.WarehouseId,
                Sku = stock.Sku,
                AvailableQuantity = stock.AvailableQuantity,
                ReservedQuantity = stock.ReservedQuantity,
                TotalQuantity = stock.TotalQuantity,
                TimeUpdate = stock.TimeUpdate,
            };
        }

    }
}
