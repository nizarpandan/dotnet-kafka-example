using Confluent.Kafka;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Text.Json;

namespace MyApp.Consumer.Worker;

public class OrderConsumerWorker : BackgroundService
{
    private readonly ILogger<OrderConsumerWorker> _logger;
    private readonly IConsumer<string, string> _consumer;
    private readonly IMongoCollection<BsonDocument> _collection;

    public OrderConsumerWorker(
        ILogger<OrderConsumerWorker> logger,
        IConsumer<string, string> consumer,
        IMongoClient mongoClient)
    {
        _logger = logger;
        _consumer = consumer;

        var database = mongoClient.GetDatabase("eventsdb");
        _collection = database.GetCollection<BsonDocument>("orders");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe("order-events");
        _logger.LogInformation("Started consuming from order-events topic");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);

                    if (consumeResult?.Message?.Value != null)
                    {
                        _logger.LogInformation(
                            "Received message at {Partition}:{Offset}",
                            consumeResult.Partition.Value,
                            consumeResult.Offset.Value);

                        // Deserialize the event
                        var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(
                            consumeResult.Message.Value);

                        if (orderEvent != null)
                        {
                            // Save to MongoDB
                            var document = new BsonDocument
                            {
                                { "orderId", orderEvent.OrderId },
                                { "customerName", orderEvent.CustomerName },
                                { "amount", (double)orderEvent.Amount },
                                { "createdAt", orderEvent.CreatedAt },
                                { "processedAt", DateTime.UtcNow }
                            };

                            await _collection.InsertOneAsync(document, null, stoppingToken);

                            _logger.LogInformation(
                                "Saved order {OrderId} to MongoDB",
                                orderEvent.OrderId);

                            // Commit the offset
                            _consumer.Commit(consumeResult);
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                }
            }
        }
        finally
        {
            _consumer.Close();
        }
    }
}