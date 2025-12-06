using UserService.Domain.Entities;

namespace UserService.Application.Abstractions;

public interface IUserWriteRepository
{
    Task AddAsync(User user, CancellationToken ct);
    Task AddFavoriteAsync(UserFavoriteCurrency favorite, CancellationToken ct);
}
