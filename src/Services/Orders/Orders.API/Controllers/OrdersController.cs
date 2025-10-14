using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orders.API.DTOs;
using Orders.Domain.Entities;
using Orders.Infrastructure.Data;

namespace Orders.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersDbContext _context;
        public OrdersController(OrdersDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _context.Orders.Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

            if (order is null)
                return NotFound("Данный заказ не найден");

            var result = MapToDto(order);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderRequest(CreateOrderRequest request)
        {
            var orderItems = request.Items.Select(item => new OrderItem(
                sku: item.Sku,
                count: item.Count,
                price: item.Price,
                currency: item.Currency
            )).ToList();

            var order = new Order(
                customerId: request.CustomerId,
                items: orderItems,
                country: request.ShippingAddress.Country,
                postalCode: request.ShippingAddress.PostalCode,
                city: request.ShippingAddress.City,
                address: request.ShippingAddress.AddressLine
            );

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            var result = MapToDto(order);

            return CreatedAtAction(
                actionName: nameof(GetOrderById),
                routeValues: new { id = order.Id },
                value: result
            );
        }

        private OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                Currency = order.Currency,
                TimeCreate = order.CreateTime,
                Country = order.ShippingCountry,
                PostalCode = order.ShippinPostalCode,
                City = order.ShippingCity,
                AddressLine = order.ShippingAddress,
                Items = order.Items.Select(item => new OrderItemDto
                {
                    Sku = item.Sku,
                    Count = item.Count,
                    Price = item.Price,
                    Currency = item.Currency
                }).ToList()
            };
        }
    }
}
