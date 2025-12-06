using CurrencyService.Application.Abstractions;
using CurrencyService.Domain.Entities;
using CurrencyService.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Refit;
using Shared.Options;
using System.Globalization;
using System.Xml.Linq;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<DatabaseOptions>(context.Configuration.GetSection(DatabaseOptions.SectionName));
        services.AddCurrencyInfrastructure(context.Configuration);

        services
            .AddRefitClient<ICbrClient>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri("http://www.cbr.ru"));

        services.AddHostedService<CurrencySyncWorker>();
    })
    .Build();

await host.RunAsync();

public class CurrencySyncWorker : BackgroundService
{
    private readonly ILogger<CurrencySyncWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ICbrClient _cbrClient;

    public CurrencySyncWorker(
        ILogger<CurrencySyncWorker> logger,
        IServiceScopeFactory scopeFactory,
        ICbrClient cbrClient)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _cbrClient = cbrClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var xml = await _cbrClient.GetDailyAsync(stoppingToken);
                var currencies = Parse(xml);

                using var scope = _scopeFactory.CreateScope();
                var writeRepository = scope.ServiceProvider.GetRequiredService<ICurrencyWriteRepository>();
                await writeRepository.UpsertAsync(currencies, stoppingToken);
                _logger.LogInformation("Currency sync completed. Updated {Count} records", currencies.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync currencies");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private static List<Currency> Parse(string xml)
    {
        var document = XDocument.Parse(xml);
        var list = new List<Currency>();
        foreach (var valute in document.Descendants("Valute"))
        {
            var code = valute.Element("CharCode")?.Value ?? string.Empty;
            var name = valute.Element("Name")?.Value ?? string.Empty;
            var rateString = valute.Element("Value")?.Value?.Replace(",", ".", StringComparison.Ordinal) ?? "0";

            if (!decimal.TryParse(rateString, NumberStyles.Any, CultureInfo.InvariantCulture, out var rate))
            {
                continue;
            }

            list.Add(new Currency
            {
                Id = Guid.NewGuid(),
                Code = code,
                Name = name,
                Rate = rate,
                UpdatedAt = DateTime.UtcNow
            });
        }

        return list;
    }
}

public interface ICbrClient
{
    [Get("/scripts/XML_daily.asp")]
    Task<string> GetDailyAsync(CancellationToken ct);
}
