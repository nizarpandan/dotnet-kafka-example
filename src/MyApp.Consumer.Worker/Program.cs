using MyApp.Consumer.Worker;
using MongoDB.Driver;

var builder = Host.CreateApplicationBuilder(args);

// Add service defaults & Aspire client integrations
builder.AddServiceDefaults();

// Add Kafka consumer
builder.AddKafkaConsumer<string, string>("kafka", settings =>
{
    settings.Config.GroupId = "order-consumer-group";
    settings.Config.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
});

// Add MongoDB
builder.AddMongoDBClient("mongodb");

builder.Services.AddHostedService<OrderConsumerWorker>();

var host = builder.Build();
host.Run();