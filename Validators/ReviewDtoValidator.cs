using FluentValidation;
using MyFirstProject.Models;

namespace MyFirstProject.Validators;

public class ReviewDtoValidator : AbstractValidator<ReviewsDto>
{
    public ReviewDtoValidator()
    {
        RuleFor(x => x.Username)
        .NotEmpty().WithMessage("Username is required.")
        .Length(3, 50).WithMessage("Username must be between 3 and 50 characters.");

        RuleFor(x => x.Rating)
        .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Body)
        .MaximumLength(500).WithMessage("Review body cannot exceed 500 characters.");
    }
}