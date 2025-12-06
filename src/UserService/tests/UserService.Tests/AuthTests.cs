using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstractions;
using UserService.Application.Features.Auth.Commands.Register;
using UserService.Application.Features.Auth.Queries.Login;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Repositories;

public class AuthTests
{
    [Fact]
    public async Task RegisterUser_CreatesUserWithHashedPassword()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new UserDbContext(options);
        var writeRepository = new UserWriteRepository(context);
        var handler = new RegisterUserCommandHandler(writeRepository);

        var id = await handler.Handle(new RegisterUserCommand("alice", "password"), CancellationToken.None);

        var user = await context.Users.FirstAsync();
        Assert.Equal(id, user.Id);
        Assert.NotEqual("password", user.PasswordHash);
    }

    [Fact]
    public async Task Login_ReturnsToken_WhenCredentialsAreValid()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "bob",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("secret")
        };

        var readRepository = new InMemoryUserReadRepository(user);
        var jwtProvider = new FakeJwtProvider();
        var handler = new LoginQueryHandler(readRepository, jwtProvider);

        var token = await handler.Handle(new LoginQuery("bob", "secret"), CancellationToken.None);

        Assert.Equal("token", token);
    }

    private class FakeJwtProvider : IJwtProvider
    {
        public string GenerateToken(Guid userId, string username, IEnumerable<string>? roles = null) => "token";
    }

    private class InMemoryUserReadRepository : IUserReadRepository
    {
        private readonly User _user;

        public InMemoryUserReadRepository(User user)
        {
            _user = user;
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct) => Task.FromResult<User?>(_user);

        public Task<User?> GetByNameAsync(string name, CancellationToken ct) =>
            Task.FromResult<User?>(_user.Name == name ? _user : null);

        public Task<IReadOnlyCollection<UserFavoriteCurrency>> GetFavoritesAsync(Guid userId, CancellationToken ct) =>
            Task.FromResult<IReadOnlyCollection<UserFavoriteCurrency>>(Array.Empty<UserFavoriteCurrency>());
    }
}
