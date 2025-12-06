using UserService.Domain.Entities;

namespace UserService.Application.Abstractions;

public interface IUserReadRepository
{
    Task<User?> GetByNameAsync(string name, CancellationToken ct);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyCollection<UserFavoriteCurrency>> GetFavoritesAsync(Guid userId, CancellationToken ct);
}
