using CurrencyService.Application.Abstractions;
using CurrencyService.Domain.Entities;
using CurrencyService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CurrencyService.Infrastructure.Repositories;

public class CurrencyWriteRepository : ICurrencyWriteRepository
{
    private readonly CurrencyDbContext _context;

    public CurrencyWriteRepository(CurrencyDbContext context)
    {
        _context = context;
    }

    public async Task UpsertAsync(IEnumerable<Currency> currencies, CancellationToken ct)
    {
        foreach (var currency in currencies)
        {
            var existing = await _context.Currencies.FirstOrDefaultAsync(c => c.Code == currency.Code, ct);
            if (existing is null)
            {
                _context.Currencies.Add(currency);
            }
            else
            {
                existing.Name = currency.Name;
                existing.Rate = currency.Rate;
                existing.UpdatedAt = currency.UpdatedAt;
                _context.Currencies.Update(existing);
            }
        }

        await _context.SaveChangesAsync(ct);
    }
}
