using FluentValidation;
using Orders.API.DTOs;

namespace Orders.API.Validators
{
    public class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
    {
        public OrderItemDtoValidator()
        {
            RuleFor(item => item.Sku)
                .NotNull().WithMessage("SKU не может быть пустым")
                .NotEmpty().WithMessage("SKU не может быть пустым")
                .MaximumLength(100).WithMessage("Максимальная длина SKU 100 символов");

            RuleFor(item => item.Count)
                .GreaterThan(0).WithMessage("Количество должно быть больше 0")
                .LessThanOrEqualTo(1000).WithMessage("Количество должно быть не больше 1000");

            RuleFor(item => item.Price)
                .GreaterThan(0).WithMessage("Цена должна быть больше 0");

            RuleFor(item => item.Currency)
                .NotNull().WithMessage("Валюта обязательна")
                .NotEmpty().WithMessage("Валюта обязательна")
                .Length(3).WithMessage("Код валюты должен состоять из 3 символов")
                .Must(BeValidCurrency).WithMessage("Валюта должна быть одной из: RUB, EUR, USD");
        }

        private bool BeValidCurrency(string currency)
        {
            var validCurrencies = new[] { "RUB", "EUR", "USD" };
            return validCurrencies.Contains(currency?.ToUpper());
        }
    }
}
