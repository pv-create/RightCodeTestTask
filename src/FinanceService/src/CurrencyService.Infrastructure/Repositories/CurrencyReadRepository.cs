using CurrencyService.Application.Abstractions;
using CurrencyService.Domain.Entities;
using Dapper;
using Npgsql;

namespace CurrencyService.Infrastructure.Repositories;

public class CurrencyReadRepository : ICurrencyReadRepository
{
    private readonly string _connectionString;

    public CurrencyReadRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IReadOnlyCollection<Currency>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken ct)
    {
        const string sql = "SELECT id, code, name, rate, updatedat FROM currency WHERE code = ANY(@codes)";
        await using var connection = new NpgsqlConnection(_connectionString);
        var items = await connection.QueryAsync<Currency>(new CommandDefinition(sql, new { codes = codes.ToArray() }, cancellationToken: ct));
        return items.ToList();
    }
}
