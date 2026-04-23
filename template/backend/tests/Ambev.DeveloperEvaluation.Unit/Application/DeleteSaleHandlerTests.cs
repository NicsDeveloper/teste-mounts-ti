using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class DeleteSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<DeleteSaleHandler> _logger;
    private readonly DeleteSaleHandler _handler;

    public DeleteSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _logger = Substitute.For<ILogger<DeleteSaleHandler>>();
        _handler = new DeleteSaleHandler(_saleRepository, _logger);
    }

    [Fact(DisplayName = "Given existing sale id When deleting Then returns success")]
    public async Task Handle_ExistingId_ReturnsSuccess()
    {
        // Given
        var id = Guid.NewGuid();
        _saleRepository.DeleteAsync(id, Arg.Any<CancellationToken>()).Returns(true);

        // When
        var result = await _handler.Handle(new DeleteSaleCommand(id), CancellationToken.None);

        // Then
        result.Success.Should().BeTrue();
        await _saleRepository.Received(1).DeleteAsync(id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existing sale id When deleting Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingId_ThrowsKeyNotFoundException()
    {
        // Given
        var id = Guid.NewGuid();
        _saleRepository.DeleteAsync(id, Arg.Any<CancellationToken>()).Returns(false);

        // When
        var act = () => _handler.Handle(new DeleteSaleCommand(id), CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given empty id When deleting Then throws ValidationException")]
    public async Task Handle_EmptyId_ThrowsValidationException()
    {
        var act = () => _handler.Handle(new DeleteSaleCommand(Guid.Empty), CancellationToken.None);
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
