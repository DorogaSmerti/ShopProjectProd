using FluentValidation;
using MyFirstProject.Models;

namespace MyFirstProject.Validators;

public class AddToCartDtoValidator : AbstractValidator<AddToCartDto>
{
    public AddToCartDtoValidator()
    {
        RuleFor(x => x.ProductId)
        .GreaterThan(0).WithMessage("ProductId must be greater than 0");

        RuleFor(x => x.Quantity)
        .GreaterThan(0).WithMessage("Quantity must be greater than 0");
    }
}

public class ChangeQuantityDtoValidator : AbstractValidator<ChangeQuantityDto>
{
    public ChangeQuantityDtoValidator()
    {
        RuleFor(x => x.Quantity)
        .GreaterThan(0).WithMessage("Quantity must be greater than 0");
    }
}