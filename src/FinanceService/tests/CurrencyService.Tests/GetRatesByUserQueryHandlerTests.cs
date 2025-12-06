using CurrencyService.Application.Abstractions;
using CurrencyService.Application.Features.GetRatesByUser;
using CurrencyService.Domain.Entities;

public class GetRatesByUserQueryHandlerTests
{
    [Fact]
    public async Task ReturnsRates_ForUserFavorites()
    {
        var favorites = new[] { "USD", "EUR" };
        var currencies = new[]
        {
            new Currency { Id = Guid.NewGuid(), Code = "USD", Name = "US Dollar", Rate = 90, UpdatedAt = DateTime.UtcNow },
            new Currency { Id = Guid.NewGuid(), Code = "EUR", Name = "Euro", Rate = 98, UpdatedAt = DateTime.UtcNow }
        };

        var userClient = new FakeUserClient(favorites);
        var readRepository = new FakeCurrencyReadRepository(currencies);
        var handler = new GetRatesByUserQueryHandler(readRepository, userClient);

        var result = await handler.Handle(new GetRatesByUserQuery(Guid.NewGuid(), "token"), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Code == "USD" && r.Rate == 90);
    }

    private class FakeUserClient : IUserServiceClient
    {
        private readonly IReadOnlyCollection<string> _favorites;

        public FakeUserClient(IReadOnlyCollection<string> favorites)
        {
            _favorites = favorites;
        }

        public Task<IReadOnlyCollection<string>> GetFavoritesAsync(string token, CancellationToken ct) =>
            Task.FromResult(_favorites);
    }

    private class FakeCurrencyReadRepository : ICurrencyReadRepository
    {
        private readonly IReadOnlyCollection<Currency> _currencies;

        public FakeCurrencyReadRepository(IReadOnlyCollection<Currency> currencies)
        {
            _currencies = currencies;
        }

        public Task<IReadOnlyCollection<Currency>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken ct)
        {
            var items = _currencies.Where(c => codes.Contains(c.Code)).ToList();
            return Task.FromResult<IReadOnlyCollection<Currency>>(items);
        }
    }
}
