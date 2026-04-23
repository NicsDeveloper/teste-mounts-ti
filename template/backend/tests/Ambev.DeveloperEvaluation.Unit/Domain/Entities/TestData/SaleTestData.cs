using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleTestData
{
    private static readonly Faker<SaleItem> ItemFaker = new Faker<SaleItem>()
        .RuleFor(i => i.Id, _ => Guid.NewGuid())
        .RuleFor(i => i.ProductId, f => f.Commerce.Ean8())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 3))
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(1, 500))
        .RuleFor(i => i.IsCancelled, _ => false);

    private static readonly Faker<Sale> SaleFaker = new Faker<Sale>()
        .RuleFor(s => s.Id, _ => Guid.NewGuid())
        .RuleFor(s => s.SaleNumber, f => f.Random.AlphaNumeric(8).ToUpper())
        .RuleFor(s => s.SaleDate, f => f.Date.Recent())
        .RuleFor(s => s.CustomerId, f => Guid.NewGuid().ToString())
        .RuleFor(s => s.CustomerName, f => f.Name.FullName())
        .RuleFor(s => s.Branch, f => f.Address.City())
        .RuleFor(s => s.Status, _ => SaleStatus.Active);

    public static Sale GenerateValidSale(int itemCount = 1)
    {
        var sale = SaleFaker.Generate();
        for (var i = 0; i < itemCount; i++)
        {
            var item = ItemFaker.Generate();
            sale.AddItem(item);
        }
        return sale;
    }

    public static SaleItem GenerateValidItem(int quantity = 1)
    {
        return ItemFaker.Clone()
            .RuleFor(i => i.Quantity, _ => quantity)
            .Generate();
    }
}
