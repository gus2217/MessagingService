using EmailGateway.Models;
using EmailGateway.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

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