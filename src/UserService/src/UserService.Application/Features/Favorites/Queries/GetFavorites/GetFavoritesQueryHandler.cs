using MediatR;
using UserService.Application.Abstractions;

namespace UserService.Application.Features.Favorites.Queries.GetFavorites;

public class GetFavoritesQueryHandler : IRequestHandler<GetFavoritesQuery, IReadOnlyCollection<string>>
{
    private readonly IUserReadRepository _readRepository;

    public GetFavoritesQueryHandler(IUserReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<IReadOnlyCollection<string>> Handle(GetFavoritesQuery request, CancellationToken cancellationToken)
    {
        var favorites = await _readRepository.GetFavoritesAsync(request.UserId, cancellationToken);
        return favorites.Select(f => f.CurrencyCode).ToList();
    }
}
