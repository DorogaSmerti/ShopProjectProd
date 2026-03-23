using FluentValidation;
using MyFirstProject.Models;

namespace MyFirstProject.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Name is required.")
        .Length(3, 50).WithMessage("Name must be between 3 and 50 characters.");

        RuleFor(x => x.Price)
        .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.Stock)
        .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative");
    }
}