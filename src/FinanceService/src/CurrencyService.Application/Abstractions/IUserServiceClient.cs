using Refit;

namespace CurrencyService.Application.Abstractions;

public interface IUserServiceClient
{
    [Get("/favorites")]
    Task<IReadOnlyCollection<string>> GetFavoritesAsync([Header("Authorization")] string token, CancellationToken ct);
}
