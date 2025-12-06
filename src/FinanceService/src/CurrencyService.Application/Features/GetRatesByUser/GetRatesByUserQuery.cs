using MediatR;

namespace CurrencyService.Application.Features.GetRatesByUser;

public record GetRatesByUserQuery(Guid UserId, string AccessToken) : IRequest<IReadOnlyCollection<UserCurrencyRateDto>>;
