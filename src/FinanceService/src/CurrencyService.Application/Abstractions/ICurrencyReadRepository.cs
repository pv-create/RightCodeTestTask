using CurrencyService.Domain.Entities;

namespace CurrencyService.Application.Abstractions;

public interface ICurrencyReadRepository
{
    Task<IReadOnlyCollection<Currency>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken ct);
}
