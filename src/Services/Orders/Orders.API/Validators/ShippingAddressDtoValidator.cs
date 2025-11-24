using FluentValidation;
using Orders.API.DTOs;

namespace Orders.API.Validators
{
    public class ShippingAddressDtoValidator : AbstractValidator<ShippingAddressDto>
    {
        public ShippingAddressDtoValidator()
        {
            RuleFor(address => address.Country)
                .NotNull().WithMessage("Страна обязательна")
                .NotEmpty().WithMessage("Страна обязательна")
                .MaximumLength(100).WithMessage("Название страны не может быть длиннее 100 символов");

            RuleFor(address => address.City)
                .NotNull().WithMessage("Город обязателен")
                .NotEmpty().WithMessage("Город обязателен")
                .MaximumLength(100).WithMessage("Название города не может быть длиннее 100 символов");

            RuleFor(address => address.PostalCode)
                .NotNull().WithMessage("Почтовый индекс обязателен")
                .NotEmpty().WithMessage("Почтовый индекс обязателен")
                .MaximumLength(20).WithMessage("Почтовый индекс не может быть длиннее 20 символов");

            RuleFor(address => address.AddressLine)
                .NotNull().WithMessage("Полный адрес обязателен")
                .NotEmpty().WithMessage("Полный адрес обязателен")
                .MaximumLength(300).WithMessage("Адрес не может быть длиннее 300 символов");
        }
    }
}
