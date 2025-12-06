namespace CurrencyService.Domain.Entities;

public class Currency
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public DateTime UpdatedAt { get; set; }
}
