namespace UserService.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public List<UserFavoriteCurrency> Favorites { get; set; } = new();
}

public class UserFavoriteCurrency
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
}
