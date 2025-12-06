using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Options;
using UserService.Application.Abstractions;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Security;

namespace UserService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var dbOptions = new DatabaseOptions();
        configuration.GetSection(DatabaseOptions.SectionName).Bind(dbOptions);

        services.AddDbContext<UserDbContext>(options =>
        {
            var assembly = typeof(UserDbContext).Assembly.GetName().Name;
            options.UseNpgsql(dbOptions.ToConnectionString(), b =>
            {
                b.MigrationsAssembly(assembly);
                b.MigrationsHistoryTable("__EFMigrationsHistory_User");
            });
        });

        services.AddScoped<IUserWriteRepository, UserWriteRepository>();
        services.AddScoped<IUserReadRepository>(_ => new UserReadRepository(dbOptions.ToConnectionString()));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped<IJwtProvider, JwtProvider>();

        return services;
    }
}
