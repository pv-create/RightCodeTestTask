using CurrencyService.Domain.Entities;

namespace CurrencyService.Application.Abstractions;

public interface ICurrencyWriteRepository
{
    Task UpsertAsync(IEnumerable<Currency> currencies, CancellationToken ct);
}
