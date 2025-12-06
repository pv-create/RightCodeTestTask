using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Controllers.Requests;
using UserService.Application.Features.Auth.Commands.Register;
using UserService.Application.Features.Auth.Queries.Login;

namespace UserService.Api.Controllers;

[ApiController]
[Route("")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<Guid>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var id = await _mediator.Send(new RegisterUserCommand(request.Name, request.Password), ct);
        return Ok(id);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        try
        {
            var token = await _mediator.Send(new LoginQuery(request.Name, request.Password), ct);
            return Ok(token);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // JWT is stateless; the client should discard the token.
        return Ok(new { message = "Logged out" });
    }
}
