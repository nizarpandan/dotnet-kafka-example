using Confluent.Kafka;
using MyApp.Producer.Api;
using Scalar.AspNetCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations
builder.AddServiceDefaults();

// Add Swagger/OpenAPI
builder.Services.AddOpenApi();

// Add Kafka producer
builder.AddKafkaProducer<string, string>("kafka");

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Use OpenAPI and Scalar
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.WithOpenApiRoutePattern("/openapi/{documentName}.json");
});

app.UseHttpsRedirection();

app.MapPost("/orders", async (
    IProducer<string, string> producer,
    OrderCreatedEvent orderEvent) =>
{
    try
    {
        var message = new Message<string, string>
        {
            Key = orderEvent.OrderId,
            Value = JsonSerializer.Serialize(orderEvent)
        };

        var result = await producer.ProduceAsync("order-events", message);

        return Results.Ok(new
        {
            Success = true,
            Partition = result.Partition.Value,
            Offset = result.Offset.Value
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Failed to publish event: {ex.Message}");
    }
})
.WithName("CreateOrder")
.WithDescription("Creates a new order and publishes an OrderCreatedEvent to Kafka");

app.MapDefaultEndpoints();
app.Run();