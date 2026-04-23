using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesHandler : IRequestHandler<ListSalesQuery, ListSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<ListSalesResult> Handle(ListSalesQuery query, CancellationToken cancellationToken)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var size = query.Size is < 1 or > 100 ? 10 : query.Size;

        var (sales, totalCount) = await _saleRepository.GetPagedAsync(page, size, query.OrderBy, cancellationToken);

        return new ListSalesResult
        {
            Sales = _mapper.Map<IEnumerable<ListSalesItemResult>>(sales),
            TotalCount = totalCount,
            Page = page,
            Size = size
        };
    }
}
