namespace MyApp.Consumer.Worker;

public record OrderCreatedEvent
{
    public string OrderId { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTime CreatedAt { get; init; }
}
