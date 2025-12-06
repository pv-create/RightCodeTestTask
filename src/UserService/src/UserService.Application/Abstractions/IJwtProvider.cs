namespace UserService.Application.Abstractions;

public interface IJwtProvider
{
    string GenerateToken(Guid userId, string username, IEnumerable<string>? roles = null);
}
