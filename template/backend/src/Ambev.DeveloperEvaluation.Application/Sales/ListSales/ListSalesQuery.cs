using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public record ListSalesQuery(int Page = 1, int Size = 10, string? OrderBy = null) : IRequest<ListSalesResult>;
