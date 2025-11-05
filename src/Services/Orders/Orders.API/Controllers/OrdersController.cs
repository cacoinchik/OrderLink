using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orders.API.DTOs;
using Orders.API.Kafka.Producers;
using Orders.Domain.Entities;
using Orders.Domain.Events;
using Orders.Infrastructure.Data;

namespace Orders.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersDbContext _context;
        private readonly OrderEventProducer _producer;
        public OrdersController(OrdersDbContext context, OrderEventProducer producer)
        {
            _context = context;
            _producer = producer;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id, CancellationToken cancellationToken = default)
        {
            var order = await _context.Orders.Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

            if (order is null)
                return NotFound(new { message = "Данный заказ не найден" });

            var result = MapToDto(order);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderRequest(CreateOrderRequest request, CancellationToken cancellationToken = default)
        {
            try
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

                await _context.Orders.AddAsync(order, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var orderCreatedEvent = new OrderCreated
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    TotalAmount = order.TotalAmount,
                    Currency = order.Currency,
                    TimeCreate = order.TimeCreate
                };

                await _producer.PublishOrderCreatedAsync(orderCreatedEvent, cancellationToken);

                var result = MapToDto(order);

                return CreatedAtAction(
                    actionName: nameof(GetOrderById),
                    routeValues: new { id = order.Id },
                    value: result
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
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
                TimeCreate = order.TimeCreate,
                Country = order.ShippingCountry,
                PostalCode = order.ShippingPostalCode,
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
