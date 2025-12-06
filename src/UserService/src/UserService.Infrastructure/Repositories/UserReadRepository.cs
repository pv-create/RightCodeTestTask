using Dapper;
using Npgsql;
using UserService.Application.Abstractions;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Repositories;

public class UserReadRepository : IUserReadRepository
{
    private readonly string _connectionString;

    public UserReadRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<User?> GetByNameAsync(string name, CancellationToken ct)
    {
        const string sql = "SELECT id, name, passwordhash FROM users WHERE name = @name LIMIT 1";
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<User>(new CommandDefinition(sql, new { name }, cancellationToken: ct));
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        const string sql = "SELECT id, name, passwordhash FROM users WHERE id = @id LIMIT 1";
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<User>(new CommandDefinition(sql, new { id }, cancellationToken: ct));
    }

    public async Task<IReadOnlyCollection<UserFavoriteCurrency>> GetFavoritesAsync(Guid userId, CancellationToken ct)
    {
        const string sql = "SELECT id, userid, currencycode FROM user_favorites WHERE userid = @userId";
        await using var connection = new NpgsqlConnection(_connectionString);
        var items = await connection.QueryAsync<UserFavoriteCurrency>(new CommandDefinition(sql, new { userId }, cancellationToken: ct));
        return items.ToList();
    }
}
