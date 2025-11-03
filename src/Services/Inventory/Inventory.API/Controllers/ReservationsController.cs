using Inventory.API.DTOs;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly InventoryDbContext _context;

        public ReservationsController(InventoryDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationById(Guid id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation is null)
                return NotFound("Резерв не найден");

            return Ok(MapToDto(reservation));
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation(CreateReservationRequest request)
        {
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.WarehouseId == request.WarehouseId && s.Sku == request.Sku);

            if (stock is null)
                return NotFound("Остаток по данному товару не найден на складе");

            stock.Reserve(request.Count);

            var reservation = new Reservation(
                orderId: request.OrderId,
                stockId: stock.Id,
                warehouseId: request.WarehouseId,
                sku: request.Sku,
                count: request.Count,
                ttlMinutes: request.TtlMinutes
            );

            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                actionName: nameof(GetReservationById),
                routeValues: new { id = reservation.Id },
                value: MapToDto(reservation)
            );
        }

        [HttpPost("{id}/commit")]
        public async Task<IActionResult> CommitReservation(Guid id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation is null)
                return NotFound("Резерв не найден");

            var stock = await _context.Stocks.FindAsync(reservation.StockId);

            if (stock is null)
                return NotFound("Остатки товара не найдены");

            reservation.Commit();
            stock.Commit(reservation.Count);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{id}/release")]
        public async Task<IActionResult> ReleaseReservation(Guid id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation is null)
                return NotFound("Резерв не найден");

            var stock = await _context.Stocks.FindAsync(reservation.StockId);

            if (stock is null)
                return NotFound("Остатки товара не найдены");

            reservation.Release();
            stock.Release(reservation.Count);

            await _context.SaveChangesAsync();

            return Ok();
        }


        private ReservationDto MapToDto(Reservation reservation)
        {
            return new ReservationDto
            {
                Id = reservation.Id,
                OrderId = reservation.OrderId,
                StockId = reservation.StockId,
                WarehouseId = reservation.WarehouseId,
                Sku = reservation.Sku,
                Count = reservation.Count,
                Status = reservation.Status.ToString(),
                TimeCreate = reservation.TimeCreate,
                TimeExpired = reservation.TimeExpired,
                TimeCommitted = reservation.TimeCommitted,
                TimeReleased = reservation.TimeReleased
            };
        }
    }
}
