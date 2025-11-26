using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Orders.API.Kafka.Configuration;
using Orders.API.Kafka.Consumers;
using Orders.API.Kafka.Producers;
using Orders.API.Middleware;
using Orders.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrdersDatabase")));

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));

builder.Services.AddSingleton<OrderEventProducer>();

builder.Services.AddHostedService<InventoryEventConsumer>();

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

    try
    {
        app.Logger.LogInformation("Начала миграции");
        dbContext.Database.Migrate();
        app.Logger.LogInformation("Миграции применены успешно");
    }
    catch(Exception ex)
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
