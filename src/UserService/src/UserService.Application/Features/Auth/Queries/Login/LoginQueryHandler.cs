using BCrypt.Net;
using MediatR;
using UserService.Application.Abstractions;

namespace UserService.Application.Features.Auth.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, string>
{
    private readonly IUserReadRepository _readRepository;
    private readonly IJwtProvider _jwtProvider;

    public LoginQueryHandler(IUserReadRepository readRepository, IJwtProvider jwtProvider)
    {
        _readRepository = readRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task<string> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _readRepository.GetByNameAsync(request.Name, cancellationToken);
        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var valid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!valid)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        return _jwtProvider.GenerateToken(user.Id, user.Name);
    }
}
