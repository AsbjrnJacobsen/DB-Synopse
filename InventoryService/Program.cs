using System.Text;
using System.Text.Json;
using InventoryService;
using InventoryService.Model;
using InventoryService.Service;

using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var mongoDBSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
builder.Services.AddSingleton<ProductService>(sp => new ProductService(mongoDBSettings!.ConnectionString!, mongoDBSettings!.DatabaseName!, mongoDBSettings!.CollectionName!));

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