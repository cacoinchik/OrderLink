using FluentValidation;
using Inventory.API.DTOs;

namespace Inventory.API.Validators
{
    public class CreateWarehouseRequestValidator : AbstractValidator<CreateWarehouseRequest>
    {
        public CreateWarehouseRequestValidator()
        {
            RuleFor(request => request.Name)
                .NotNull().WithMessage("Название склада обязательно")
                .NotEmpty().WithMessage("Название склада обязательно")
                .MaximumLength(150).WithMessage("Навзание склада не может быть длиннее 150 символов");

            RuleFor(request => request.Region)
                .NotNull().WithMessage("Регион обязателен")
                .NotEmpty().WithMessage("Регион обязателен")
                .MaximumLength(100).WithMessage("Навзание регоина не может быть длиннее 100 символов");

            RuleFor(request => request.City)
                .NotNull().WithMessage("Город обязателен")
                .NotEmpty().WithMessage("Город обязателен")
                .MaximumLength(100).WithMessage("Навзание города не может быть длиннее 100 символов");

            RuleFor(request => request.Address)
                .NotNull().WithMessage("Адрес обязателен")
                .NotEmpty().WithMessage("Адрес обязателен")
                .MaximumLength(300).WithMessage("Адрес не может быть длиннее 300 символов");
        }
    }
}
