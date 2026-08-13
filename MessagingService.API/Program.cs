using EmailGateway.Models;
using EmailGateway.Services;
using MassTransit;
using MessagingService.Application.Consumers;
using MessagingService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSignalR();
builder.Services.AddScoped<MessagingService.Domain.Common.Interfaces.INotificationRepository, MessagingService.Infrastructure.Persistence.Repositories.NotificationRepository>();
builder.Services.AddTransient<System.Data.IDbConnection>(sp => 
    new Npgsql.NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "MessagingService_";
});
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MessagingService.Application.Consumers.CreateUserNotificationConsumer).Assembly));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CreateUserNotificationConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"));
        cfg.ReceiveEndpoint("user-notifications", e =>
        {
            e.ConfigureConsumer<CreateUserNotificationConsumer>(context);
        });
    });
});

// ──────────────────────────────────────────────────────────────
// Email Configuration
// ──────────────────────────────────────────────────────────────
var emailConfig = builder.Configuration
                      .GetSection("EmailConfiguration")
                      .Get<EmailConfiguration>()
                  ?? throw new InvalidOperationException(
                      "EmailConfiguration section is missing from appsettings.");

builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map controllers (this is where your EmailController lives)
app.MapControllers();

app.Run();