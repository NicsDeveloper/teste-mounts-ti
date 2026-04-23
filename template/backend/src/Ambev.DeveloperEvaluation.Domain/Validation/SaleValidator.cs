using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleValidator : AbstractValidator<Sale>
{
    public SaleValidator()
    {
        RuleFor(s => s.SaleNumber)
            .NotEmpty().WithMessage("SaleNumber is required.");

        RuleFor(s => s.SaleDate)
            .NotEmpty().WithMessage("SaleDate is required.");

        RuleFor(s => s.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");

        RuleFor(s => s.CustomerName)
            .NotEmpty().WithMessage("CustomerName is required.");

        RuleFor(s => s.Branch)
            .NotEmpty().WithMessage("Branch is required.");

        RuleFor(s => s.Items)
            .NotEmpty().WithMessage("Sale must have at least one item.");

        RuleForEach(s => s.Items)
            .SetValidator(new SaleItemValidator());
    }
}
