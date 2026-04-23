using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }

    public Sale Sale { get; set; } = null!;

    public void ApplyDiscount()
    {
        Discount = Quantity switch
        {
            >= 10 and <= 20 => 0.20m,
            >= 4             => 0.10m,
            _                => 0m
        };

        TotalAmount = Quantity * UnitPrice * (1 - Discount);
    }

    public void Cancel()
    {
        IsCancelled = true;
        TotalAmount = 0;
    }
}
