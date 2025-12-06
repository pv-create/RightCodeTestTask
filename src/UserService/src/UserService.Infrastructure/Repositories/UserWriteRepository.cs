using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstractions;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories;

public class UserWriteRepository : IUserWriteRepository
{
    private readonly UserDbContext _context;

    public UserWriteRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user, CancellationToken ct)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AddFavoriteAsync(UserFavoriteCurrency favorite, CancellationToken ct)
    {
        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync(ct);
    }
}
