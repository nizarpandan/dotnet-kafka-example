var builder = DistributedApplication.CreateBuilder(args);

// Add Kafka
var kafka = builder.AddKafka("kafka")
    .WithDataVolume()
    .WithKafkaUI();

// Add MongoDB
var mongodb = builder.AddMongoDB("mongodb")
    .WithDataVolume()
    .WithMongoExpress();

var database = mongodb.AddDatabase("eventsdb");

// Add Producer API
var producer = builder.AddProject<Projects.MyApp_Producer_Api>("producer-api")
    .WithReference(kafka);

// Add Consumer Worker
builder.AddProject<Projects.MyApp_Consumer_Worker>("consumer-worker")
    .WithReference(kafka)
    .WithReference(mongodb)
    .WaitFor(kafka)
    .WaitFor(mongodb); // Ensure both are ready before starting

builder.Build().Run();