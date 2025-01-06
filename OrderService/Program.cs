using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OrderService;
using OrderService.Data;
using OrderService.Messaging;
using OrderService.Model;
using OrderService.Service;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<ServiceRegistrarHostedService>();
builder.Services.AddScoped<IDBService, DBService>();
builder.Services.AddSingleton<OrderMessageManager>();

var configuration = builder.Configuration;
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();