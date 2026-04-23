using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleItemValidator : AbstractValidator<SaleItem>
{
    public SaleItemValidator()
    {
        RuleFor(i => i.ProductId)
            .NotEmpty().WithMessage("ProductId is required.");

        RuleFor(i => i.ProductName)
            .NotEmpty().WithMessage("ProductName is required.");

        RuleFor(i => i.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.")
            .LessThanOrEqualTo(20).WithMessage("Cannot sell more than 20 identical items.");

        RuleFor(i => i.UnitPrice)
            .GreaterThan(0).WithMessage("UnitPrice must be greater than zero.");
    }
}
