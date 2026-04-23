using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact(DisplayName = "Sale with no items should fail validation")]
    public void Given_SaleWithNoItems_When_Validated_Then_ShouldBeInvalid()
    {
        var sale = SaleTestData.GenerateValidSale(0);
        var result = sale.Validate();
        Assert.False(result.IsValid);
    }

    [Fact(DisplayName = "Sale with valid items should pass validation")]
    public void Given_SaleWithValidItems_When_Validated_Then_ShouldBeValid()
    {
        var sale = SaleTestData.GenerateValidSale(2);
        var result = sale.Validate();
        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Cancelling a sale should set status to Cancelled")]
    public void Given_ActiveSale_When_Cancelled_Then_StatusShouldBeCancelled()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();
        Assert.Equal(SaleStatus.Cancelled, sale.Status);
    }

    [Theory(DisplayName = "Item with 4-9 quantity should apply 10% discount")]
    [InlineData(4)]
    [InlineData(9)]
    public void Given_ItemQuantityBetween4And9_When_DiscountApplied_Then_Discount10Percent(int quantity)
    {
        var item = SaleTestData.GenerateValidItem(quantity);
        item.ApplyDiscount();
        Assert.Equal(0.10m, item.Discount);
    }

    [Theory(DisplayName = "Item with 10-20 quantity should apply 20% discount")]
    [InlineData(10)]
    [InlineData(20)]
    public void Given_ItemQuantityBetween10And20_When_DiscountApplied_Then_Discount20Percent(int quantity)
    {
        var item = SaleTestData.GenerateValidItem(quantity);
        item.ApplyDiscount();
        Assert.Equal(0.20m, item.Discount);
    }

    [Theory(DisplayName = "Item with less than 4 quantity should have no discount")]
    [InlineData(1)]
    [InlineData(3)]
    public void Given_ItemQuantityLessThan4_When_DiscountApplied_Then_NoDiscount(int quantity)
    {
        var item = SaleTestData.GenerateValidItem(quantity);
        item.ApplyDiscount();
        Assert.Equal(0m, item.Discount);
    }

    [Fact(DisplayName = "TotalAmount should be correctly calculated after adding items")]
    public void Given_SaleWithItems_When_ItemsAdded_Then_TotalShouldMatchSum()
    {
        var sale = SaleTestData.GenerateValidSale(0);
        var item = SaleTestData.GenerateValidItem(5);
        var expectedTotal = item.Quantity * item.UnitPrice * (1 - 0.10m);

        sale.AddItem(item);

        Assert.Equal(Math.Round(expectedTotal, 10), Math.Round(sale.TotalAmount, 10));
    }

    [Fact(DisplayName = "Cancelling a sale item should remove it from total")]
    public void Given_SaleWithItems_When_ItemCancelled_Then_TotalDecreasesAccordingly()
    {
        var sale = SaleTestData.GenerateValidSale(2);
        var itemToCancel = sale.Items.First();
        var totalBefore = sale.TotalAmount;

        sale.CancelItem(itemToCancel.Id);

        Assert.True(sale.TotalAmount < totalBefore);
        Assert.True(itemToCancel.IsCancelled);
    }
}
