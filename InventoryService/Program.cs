using System.Text;
using System.Text.Json;
using InventoryService;
using InventoryService.Model;
using InventoryService.Service;
using EasyNetQ;
using EasyNetQ.DI.Microsoft;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<ProductService>();

var rabbitMqConnectionString = builder.Configuration.GetConnectionString("RabbitMQ");
if (!string.IsNullOrEmpty(rabbitMqConnectionString))
{
    var bus = RabbitHutch.CreateBus(rabbitMqConnectionString);
    builder.Services.AddSingleton<IBus>(bus);
}
else
{
    builder.Services.AddSingleton<IBus, NullBus>();
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<ServiceRegistrarHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();