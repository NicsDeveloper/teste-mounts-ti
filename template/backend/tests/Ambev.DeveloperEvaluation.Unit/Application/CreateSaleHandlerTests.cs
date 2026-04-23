using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CreateSaleHandler>>();
        _handler = new CreateSaleHandler(_saleRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid command When creating sale Then returns success result")]
    public async Task Handle_ValidCommand_ReturnsSuccessResult()
    {
        // Given
        var command = new CreateSaleCommand
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid().ToString(),
            CustomerName = "Test Customer",
            Branch = "Branch A",
            Items = new List<CreateSaleItemCommand>
            {
                new() { ProductId = "P1", ProductName = "Product 1", Quantity = 5, UnitPrice = 100m }
            }
        };

        var sale = SaleTestData.GenerateValidSale(1);
        var expectedResult = new CreateSaleResult { Id = sale.Id, SaleNumber = command.SaleNumber };

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<SaleItem>(Arg.Any<CreateSaleItemCommand>())
            .Returns(new SaleItem { Id = Guid.NewGuid(), ProductId = "P1", ProductName = "Product 1", Quantity = 5, UnitPrice = 100m });
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given duplicate sale number When creating Then throws InvalidOperationException")]
    public async Task Handle_DuplicateSaleNumber_ThrowsInvalidOperationException()
    {
        // Given
        var command = new CreateSaleCommand
        {
            SaleNumber = "DUPLICATE",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid().ToString(),
            CustomerName = "Test",
            Branch = "Branch",
            Items = new List<CreateSaleItemCommand>
            {
                new() { ProductId = "P1", ProductName = "P1", Quantity = 1, UnitPrice = 10m }
            }
        };

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(SaleTestData.GenerateValidSale());

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact(DisplayName = "Given empty command When creating Then throws ValidationException")]
    public async Task Handle_EmptyCommand_ThrowsValidationException()
    {
        var command = new CreateSaleCommand();
        var act = () => _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
