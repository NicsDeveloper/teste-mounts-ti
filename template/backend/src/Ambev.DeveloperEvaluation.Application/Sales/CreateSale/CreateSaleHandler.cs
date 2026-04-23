using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;

    public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<CreateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existing = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
        if (existing != null)
            throw new InvalidOperationException($"Sale with number {command.SaleNumber} already exists.");

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = command.SaleNumber,
            SaleDate = command.SaleDate,
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            Branch = command.Branch
        };

        foreach (var itemCommand in command.Items)
        {
            var item = _mapper.Map<SaleItem>(itemCommand);
            item.Id = Guid.NewGuid();
            sale.AddItem(item);
        }

        var created = await _saleRepository.CreateAsync(sale, cancellationToken);

        _logger.LogInformation("SaleCreatedEvent: SaleId={SaleId}, Number={SaleNumber}", created.Id, created.SaleNumber);

        return _mapper.Map<CreateSaleResult>(created);
    }
}
