using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Options;
using UserService.Infrastructure.Persistence;
using CurrencyService.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, builder) =>
    {
        builder.AddJsonFile("appsettings.json", optional: true);
        builder.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<DatabaseOptions>(context.Configuration.GetSection(DatabaseOptions.SectionName));

        services.AddDbContext<UserDbContext>((sp, options) =>
        {
            var dbOptions = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<DatabaseOptions>>().Value;
            options.UseNpgsql(dbOptions.ToConnectionString());
        });

        services.AddDbContext<CurrencyDbContext>((sp, options) =>
        {
            var dbOptions = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<DatabaseOptions>>().Value;
            options.UseNpgsql(dbOptions.ToConnectionString());
        });
    })
    .Build();

using var scope = host.Services.CreateScope();
var userDb = scope.ServiceProvider.GetRequiredService<UserDbContext>();
var currencyDb = scope.ServiceProvider.GetRequiredService<CurrencyDbContext>();

Console.WriteLine("Applying migrations...");
await userDb.Database.MigrateAsync();
await currencyDb.Database.MigrateAsync();
Console.WriteLine("Migrations applied");
