using LoadBalancer;
using LoadBalancer.Controllers;
using LoadBalancer.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// timeout on requests
builder.Services.AddHttpClient<GWLBController>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
});
builder.Services.AddControllers();

builder.Services.AddSingleton<Instances>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();