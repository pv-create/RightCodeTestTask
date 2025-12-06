using CurrencyService.Application.Abstractions;
using MediatR;

namespace CurrencyService.Application.Features.GetRatesByUser;

public class GetRatesByUserQueryHandler : IRequestHandler<GetRatesByUserQuery, IReadOnlyCollection<UserCurrencyRateDto>>
{
    private readonly ICurrencyReadRepository _currencyReadRepository;
    private readonly IUserServiceClient _userServiceClient;

    public GetRatesByUserQueryHandler(
        ICurrencyReadRepository currencyReadRepository,
        IUserServiceClient userServiceClient)
    {
        _currencyReadRepository = currencyReadRepository;
        _userServiceClient = userServiceClient;
    }

    public async Task<IReadOnlyCollection<UserCurrencyRateDto>> Handle(GetRatesByUserQuery request, CancellationToken cancellationToken)
    {
        var favorites = await _userServiceClient.GetFavoritesAsync($"Bearer {request.AccessToken}", cancellationToken);
        if (favorites.Count == 0)
        {
            return Array.Empty<UserCurrencyRateDto>();
        }

        var currencies = await _currencyReadRepository.GetByCodesAsync(favorites, cancellationToken);
        return currencies
            .Select(c => new UserCurrencyRateDto(c.Code, c.Name, c.Rate))
            .ToList();
    }
}
