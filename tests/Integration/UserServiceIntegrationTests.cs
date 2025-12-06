using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserService.Api;
using UserService.Api.Controllers.Requests;
using UserService.Application.Abstractions;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Repositories;

namespace IntegrationTests;

public class UserServiceIntegrationTests : IClassFixture<UserServiceFactory>
{
    private readonly UserServiceFactory _factory;

    public UserServiceIntegrationTests(UserServiceFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RegisterAndLogin_ReturnsToken()
    {
        var client = _factory.CreateClient();

        var registerResponse = await client.PostAsJsonAsync("/register", new RegisterRequest("integration", "pwd"));
        registerResponse.EnsureSuccessStatusCode();

        var loginResponse = await client.PostAsJsonAsync("/login", new LoginRequest("integration", "pwd"));
        loginResponse.EnsureSuccessStatusCode();

        var token = await loginResponse.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(token));
    }
}

public class UserServiceFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<UserDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<UserDbContext>(options => options.UseInMemoryDatabase("users-integration"));
            services.AddScoped<IUserWriteRepository, UserWriteRepository>();
            services.AddScoped<IUserReadRepository, InMemoryReadRepository>();
        });
    }
}

public class InMemoryReadRepository : IUserReadRepository
{
    private readonly UserDbContext _context;

    public InMemoryReadRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct) => await _context.Users.FindAsync(new object[] { id }, ct);

    public async Task<User?> GetByNameAsync(string name, CancellationToken ct) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Name == name, ct);

    public async Task<IReadOnlyCollection<UserFavoriteCurrency>> GetFavoritesAsync(Guid userId, CancellationToken ct)
    {
        var favs = await _context.Favorites.Where(f => f.UserId == userId).ToListAsync(ct);
        return favs;
    }
}
