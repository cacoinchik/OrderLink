using Inventory.API.DTOs;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly InventoryDbContext _context;
        public WarehouseController(InventoryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetWarehouses(bool? isActive)
        {
            var warehouses = _context.Warehouses.AsQueryable();

            if (isActive.HasValue)
                warehouses = warehouses.Where(w => w.IsActive == isActive);

            var sortWarehouses = await warehouses.OrderBy(w => w.Name).ToListAsync();

            var result = sortWarehouses.Select(MapToDto).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWarehouseById(Guid id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);

            if (warehouse is null)
                return NotFound("Склад не найден");

            return Ok(MapToDto(warehouse));
        }

        [HttpPost]
        public async Task<IActionResult> CreateWarehouse(CreateWarehouseRequest request)
        {
            var warehouse = new Warehouse(
                name: request.Name,
                region: request.Region,
                city: request.City,
                address: request.Address
            );

            await _context.Warehouses.AddAsync(warehouse);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                actionName: nameof(GetWarehouseById),
                routeValues: new { id = warehouse.Id },
                value: MapToDto(warehouse)
                );
        }

        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> ActivateWarehouse(Guid id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);

            if (warehouse is null)
                return NotFound("Склад не найден");

            warehouse.Activate();

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivateWarehouse(Guid id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);

            if (warehouse is null)
                return NotFound("Склад не найден");

            warehouse.Deactivate();

            await _context.SaveChangesAsync();

            return Ok();
        }

        private WarehouseDto MapToDto(Warehouse warehouse)
        {
            return new WarehouseDto
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Region = warehouse.Region,
                City = warehouse.City,
                Address = warehouse.Address,
                IsActive = warehouse.IsActive,
                TimeCreate = warehouse.TimeCreate
            };
        }
    }
}
