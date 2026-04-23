using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given existing sale id When getting Then returns sale result")]
    public async Task Handle_ExistingId_ReturnsSaleResult()
    {
        // Given
        var sale = SaleTestData.GenerateValidSale(1);
        var expectedResult = new GetSaleResult { Id = sale.Id, SaleNumber = sale.SaleNumber };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(expectedResult);

        // When
        var result = await _handler.Handle(new GetSaleQuery(sale.Id), CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
    }

    [Fact(DisplayName = "Given non-existing id When getting Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingId_ThrowsKeyNotFoundException()
    {
        // Given
        var id = Guid.NewGuid();
        _saleRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Domain.Entities.Sale?)null);

        // When
        var act = () => _handler.Handle(new GetSaleQuery(id), CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given empty id When getting Then throws ValidationException")]
    public async Task Handle_EmptyId_ThrowsValidationException()
    {
        var act = () => _handler.Handle(new GetSaleQuery(Guid.Empty), CancellationToken.None);
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
