using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Features.Favorites.Commands.AddFavorite;
using UserService.Application.Features.Favorites.Queries.GetFavorites;

namespace UserService.Api.Controllers;

[ApiController]
[Route("favorites")]
public class FavoritesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FavoritesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Add([FromQuery] string code, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        await _mediator.Send(new AddFavoriteCommand(userId.Value, code), ct);
        return Ok();
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<string>>> Get(CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var favorites = await _mediator.Send(new GetFavoritesQuery(userId.Value), ct);
        return Ok(favorites);
    }

    private Guid? GetUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out var guid) ? guid : null;
    }
}
