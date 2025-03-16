using BusinessLayer.Interface;
using BusinessLayer.Service;
using BusinessLayer.Mapping;
using BusinessLayer.Validator;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ModelLayer.DTO;
using RepositoryLayer.Context;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using Middleware.JWT_Token;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Middleware.RabbitMQ;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders(); 
    logging.AddConsole();      
});


// Ensure Configuration is available
var configuration = builder.Configuration;

// Configure Dependency Injection for Database Services BEFORE building the app
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Configure JWT Secret Key
string secretKey = builder.Configuration["Jwt:SecretKey"];
builder.Services.AddSingleton(new JwtService(secretKey));

// Register Auth Service
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<IAddressBookBL, AddressBookBL>();
builder.Services.AddScoped<IAddressBookRL, AddressBookRL>();
builder.Services.AddScoped<IValidator<AddressBookDTO>, AddressBookValidator>();
builder.Services.AddTransient<IEmailService, EmailService>();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<AddressBookValidator>();

// Add Redis cache service
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
});

// Enable Session Middleware
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<IRabbitMQProducer, RabbitMQProducer>();
builder.Services.AddHostedService<RabbitMQConsumer>();

// Add services to the container.
builder.Services.AddControllers();

// Register Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Address Book API",
        Version = "v1",
        Description = "API for managing contacts in the Address Book",
    });
    c.EnableAnnotations();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Address Book API v1");
    });
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseSession(); 

app.UseAuthorization();
app.MapControllers();

app.Run();
