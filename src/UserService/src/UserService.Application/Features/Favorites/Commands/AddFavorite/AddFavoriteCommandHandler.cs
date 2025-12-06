using MediatR;
using UserService.Application.Abstractions;
using UserService.Domain.Entities;

namespace UserService.Application.Features.Favorites.Commands.AddFavorite;

public class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommand>
{
    private readonly IUserWriteRepository _writeRepository;

    public AddFavoriteCommandHandler(IUserWriteRepository writeRepository)
    {
        _writeRepository = writeRepository;
    }

    public async Task Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
    {
        var favorite = new UserFavoriteCurrency
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            CurrencyCode = request.CurrencyCode
        };

        await _writeRepository.AddFavoriteAsync(favorite, cancellationToken);
    }
}
