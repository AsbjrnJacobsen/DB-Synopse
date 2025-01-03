using System.Text;
using System.Text.Json;
using InventoryService;
using InventoryService.Messaging;
using InventoryService.Model;
using InventoryService.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<ServiceRegistrarHostedService>();
builder.Services.AddHostedService<InventoryMessageConsumer>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();