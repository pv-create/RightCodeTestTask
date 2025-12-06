using MediatR;

namespace UserService.Application.Features.Auth.Queries.Login;

public record LoginQuery(string Name, string Password) : IRequest<string>;
