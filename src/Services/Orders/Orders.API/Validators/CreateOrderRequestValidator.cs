using FluentValidation;
using Orders.API.DTOs;

namespace Orders.API.Validators
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator()
        {
            RuleFor(request => request.CustomerId)
                .NotNull().WithMessage("ID клиента обязателен")
                .NotEmpty().WithMessage("ID клиента обязателен")
                .MaximumLength(100).WithMessage("ID клиента не может быть длиннее 100 символов");

            RuleFor(request => request.Items)
                .NotNull().WithMessage("Список товаров не может быть пуст")
                .NotEmpty().WithMessage("В заказе должен быть хотя бы 1 товар")
                .Must(x => x.Count <= 50).WithMessage("В заказе не может быть больше 50 товаров");

            RuleForEach(request => request.Items).SetValidator(new OrderItemDtoValidator());

            RuleFor(request => request.ShippingAddress)
                .NotNull().WithMessage("Адрес доставки должен быть заполнен")
                .SetValidator(new ShippingAddressDtoValidator());
        }
    }
}
