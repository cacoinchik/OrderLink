using FluentValidation;
using Inventory.API.DTOs;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
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

        public ReservationsController(InventoryDbContext context, ILogger logger)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationById(Guid id, CancellationToken cancellationToken = default)
        {
            var reservation = await _context.Reservations.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

            if (reservation is null)
                return NotFound("Резерв не найден");

            return Ok(MapToDto(reservation));
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation(
            CreateReservationRequest request,
            [FromServices] IValidator<CreateReservationRequest> validator,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    message = "Ошибка валидации",
                    errors = validationResult.Errors.Select(error => new
                    {
                        field = error.PropertyName,
                        errorMessage = error.ErrorMessage
                    })
                });
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var stock = await _context.Stocks
                .FirstOrDefaultAsync(
                    s => s.WarehouseId == request.WarehouseId && s.Sku == request.Sku, cancellationToken);

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

                await _context.Reservations.AddAsync(reservation, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return CreatedAtAction(
                    actionName: nameof(GetReservationById),
                    routeValues: new { id = reservation.Id },
                    value: MapToDto(reservation)
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/commit")]
        public async Task<IActionResult> CommitReservation(Guid id, CancellationToken cancellationToken = default)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

                if (reservation is null)
                    return NotFound("Резерв не найден");

                if (reservation.Status == ReservationStatus.Committed)
                    return Ok(new { message = "Резерв уже подтвержден" });

                var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == reservation.StockId, cancellationToken);

                if (stock is null)
                    return NotFound("Остатки товара не найдены");

                reservation.Commit();
                stock.Commit(reservation.Count);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return Ok(new { message = "Резерв успешно подтвержден" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/release")]
        public async Task<IActionResult> ReleaseReservation(Guid id, CancellationToken cancellationToken = default)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

                if (reservation is null)
                    return NotFound("Резерв не найден");

                if (reservation.Status == ReservationStatus.Released)
                    return Ok("Резерв уже освобожден");

                var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == reservation.StockId, cancellationToken);

                if (stock is null)
                    return NotFound("Остатки товара не найдены");

                reservation.Release();
                stock.Release(reservation.Count);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return Ok(new { message = "Резерв уже освобожден" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
