using FluentValidation;
using Inventory.API.DTOs;

namespace Inventory.API.Validators
{
    public class ManageStockQuantityRequestValidator : AbstractValidator<ManageStockQuantityRequest>
    {
        public ManageStockQuantityRequestValidator()
        {
            RuleFor(request => request.WarehouseId)
                .NotNull().WithMessage("ID склада обязателен")
                .NotEmpty().WithMessage("ID склада обязателен")
                .NotEqual(Guid.Empty).WithMessage("ID склада обязателен");

            RuleFor(request => request.Sku)
                .NotNull().WithMessage("SKU обязателен")
                .NotEmpty().WithMessage("SKU обязателен")
                .MaximumLength(100).WithMessage("SKU не может быть длиннее 100 символов");

            RuleFor(request => request.Quantity)
                .GreaterThanOrEqualTo(-100000).WithMessage("Количество не можеть быть меньше -100000")
                .LessThanOrEqualTo(100000).WithMessage("Количество не может быть больше 100000")
                .NotEqual(0).WithMessage("Нельзя менять количество на 0");

        }
    }
}
