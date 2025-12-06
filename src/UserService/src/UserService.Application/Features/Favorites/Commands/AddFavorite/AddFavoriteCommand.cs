using MediatR;

namespace UserService.Application.Features.Favorites.Commands.AddFavorite;

public record AddFavoriteCommand(Guid UserId, string CurrencyCode) : IRequest;
