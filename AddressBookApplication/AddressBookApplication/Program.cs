using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
var builder = WebApplication.CreateBuilder(args);

// Ensure Configuration is available
var configuration = builder.Configuration;

// Configure Dependency Injection for Database Services BEFORE building the app
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
