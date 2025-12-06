using MediatR;

namespace UserService.Application.Features.Favorites.Queries.GetFavorites;

public record GetFavoritesQuery(Guid UserId) : IRequest<IReadOnlyCollection<string>>;
