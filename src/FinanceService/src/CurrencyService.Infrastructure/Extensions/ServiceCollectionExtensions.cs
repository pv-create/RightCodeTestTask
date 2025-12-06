using CurrencyService.Application.Abstractions;
using CurrencyService.Infrastructure.Persistence;
using CurrencyService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Options;

namespace CurrencyService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCurrencyInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var dbOptions = new DatabaseOptions();
        configuration.GetSection(DatabaseOptions.SectionName).Bind(dbOptions);

        services.AddDbContext<CurrencyDbContext>(options =>
        {
            var assembly = typeof(CurrencyDbContext).Assembly.GetName().Name;
            options.UseNpgsql(dbOptions.ToConnectionString(), b =>
            {
                b.MigrationsAssembly(assembly);
                b.MigrationsHistoryTable("__EFMigrationsHistory_Currency");
            });
        });

        services.AddScoped<ICurrencyWriteRepository, CurrencyWriteRepository>();
        services.AddScoped<ICurrencyReadRepository>(_ => new CurrencyReadRepository(dbOptions.ToConnectionString()));

        return services;
    }
}
