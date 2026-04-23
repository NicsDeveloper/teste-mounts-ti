using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    public UpdateSaleRequestValidator()
    {
        RuleFor(s => s.SaleNumber).NotEmpty();
        RuleFor(s => s.SaleDate).NotEmpty();
        RuleFor(s => s.CustomerId).NotEmpty();
        RuleFor(s => s.CustomerName).NotEmpty();
        RuleFor(s => s.Branch).NotEmpty();
        RuleFor(s => s.Items).NotEmpty().WithMessage("Sale must contain at least one item.");

        RuleForEach(s => s.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.ProductName).NotEmpty();
            item.RuleFor(i => i.Quantity)
                .GreaterThan(0)
                .LessThanOrEqualTo(20).WithMessage("Cannot sell more than 20 identical items.");
            item.RuleFor(i => i.UnitPrice).GreaterThan(0);
        });
    }
}
