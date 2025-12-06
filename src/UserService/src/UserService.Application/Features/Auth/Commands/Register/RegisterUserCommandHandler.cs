using BCrypt.Net;
using MediatR;
using UserService.Application.Abstractions;
using UserService.Domain.Entities;

namespace UserService.Application.Features.Auth.Commands.Register;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserWriteRepository _writeRepository;
    

    public RegisterUserCommandHandler(IUserWriteRepository writeRepository)
    {
        _writeRepository = writeRepository;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await _writeRepository.AddAsync(user, cancellationToken);
        return user.Id;
    }
}
