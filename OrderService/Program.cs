using System.Text;
using System.Text.Json;
using OrderService;
using OrderService.Messaging;
using OrderService.Model;
using OrderService.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHostedService<ServiceRegistrarHostedService>();
builder.Services.AddSingleton<OrderMessagePublisher>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();