using FluentValidation;
using Inventory.API.Kafka.Configuration;
using Inventory.API.Kafka.Consumers;
using Inventory.API.Kafka.Producers;
using Inventory.API.Middleware;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("InventoryDatabase")));

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));

builder.Services.AddSingleton<InventoryEventProducer>();

builder.Services.AddHostedService<OrderEventConsumer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

    try
    {
        app.Logger.LogInformation("Начала миграции");
        dbContext.Database.Migrate();
        app.Logger.LogInformation("Миграции применены успешно");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Ошибка применения миграции");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
