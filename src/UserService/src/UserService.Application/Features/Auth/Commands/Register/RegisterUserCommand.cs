using MediatR;

namespace UserService.Application.Features.Auth.Commands.Register;

public record RegisterUserCommand(string Name, string Password) : IRequest<Guid>;
