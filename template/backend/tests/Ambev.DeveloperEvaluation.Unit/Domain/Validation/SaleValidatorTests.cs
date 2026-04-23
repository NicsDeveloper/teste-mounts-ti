using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation;

public class SaleValidatorTests
{
    private readonly SaleValidator _validator = new();

    [Fact(DisplayName = "Valid sale should pass all validation rules")]
    public void Given_ValidSale_When_Validated_Then_ShouldNotHaveErrors()
    {
        var sale = SaleTestData.GenerateValidSale(2);
        var result = _validator.TestValidate(sale);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Empty SaleNumber should fail validation")]
    public void Given_EmptySaleNumber_When_Validated_Then_ShouldHaveError()
    {
        var sale = SaleTestData.GenerateValidSale(1);
        sale.SaleNumber = string.Empty;
        var result = _validator.TestValidate(sale);
        result.ShouldHaveValidationErrorFor(s => s.SaleNumber);
    }

    [Fact(DisplayName = "Sale with no items should fail validation")]
    public void Given_SaleWithNoItems_When_Validated_Then_ShouldHaveError()
    {
        var sale = SaleTestData.GenerateValidSale(0);
        var result = _validator.TestValidate(sale);
        result.ShouldHaveValidationErrorFor(s => s.Items);
    }

    [Fact(DisplayName = "Item with quantity over 20 should fail validation")]
    public void Given_ItemWithQuantityOver20_When_Validated_Then_ShouldHaveError()
    {
        var sale = SaleTestData.GenerateValidSale(0);
        var item = SaleTestData.GenerateValidItem(21);
        sale.AddItem(item);

        var itemValidator = new SaleItemValidator();
        var result = itemValidator.TestValidate(item);
        result.ShouldHaveValidationErrorFor(i => i.Quantity);
    }
}
