using System.Text;
using System.Text.Json;
using InventoryService;
using InventoryService.Messaging;
using InventoryService.Model;
using InventoryService.Service;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<ProductService>();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<ServiceRegistrarHostedService>();
builder.Services.AddHostedService<InventoryOrderConsumer>();
builder.Services.AddHostedService<DeadLetterQueueManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();