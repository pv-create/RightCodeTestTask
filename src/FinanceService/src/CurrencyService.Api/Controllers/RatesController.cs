using System.Security.Claims;
using CurrencyService.Application.Features.GetRatesByUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyService.Api.Controllers;

[ApiController]
[Route("rates")]
public class RatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IReadOnlyCollection<UserCurrencyRateDto>>> GetRates(CancellationToken ct)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            return Unauthorized();
        }

        var token = Request.Headers.Authorization.ToString().Replace("Bearer ", string.Empty, StringComparison.OrdinalIgnoreCase);
        var result = await _mediator.Send(new GetRatesByUserQuery(Guid.Parse(userIdClaim), token), ct);
        return Ok(result);
    }
}
