using FluentValidation;
using Inventory.API.DTOs;

namespace Inventory.API.Validators
{
    public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
    {
        public CreateReservationRequestValidator()
        {
            RuleFor(request => request.OrderId)
                .NotNull().WithMessage("ID заказа обязателен")
                .NotEmpty().WithMessage("ID заказа обязателен")
                .NotEqual(Guid.Empty).WithMessage("ID заказа обязателен");

            RuleFor(request => request.WarehouseId)
                .NotNull().WithMessage("ID склада обязателен")
                .NotEmpty().WithMessage("ID склада обязателен")
                .NotEqual(Guid.Empty).WithMessage("ID склада обязателен");

            RuleFor(request => request.Sku)
                .NotNull().WithMessage("SKU обязателен")
                .NotEmpty().WithMessage("SKU обязателен")
                .MaximumLength(100).WithMessage("SKU не может быть длиннее 100 символов");

            RuleFor(request => request.Count)
                .GreaterThan(0).WithMessage("Количество должно быть больше 0")
                .LessThanOrEqualTo(1000).WithMessage("Количество должно быть меньше 1000");

            RuleFor(request => request.TtlMinutes)
                .GreaterThanOrEqualTo(1).WithMessage("TTL должен быть минимум 1 минута")
                .LessThanOrEqualTo(480).WithMessage("TTL не должен быть больше 480 минут");
        }
    }
}
