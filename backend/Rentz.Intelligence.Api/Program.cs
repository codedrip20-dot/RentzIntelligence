using Microsoft.EntityFrameworkCore;
using Rentz.Intelligence.Application.Services;
using Rentz.Intelligence.Infrastructure.Persistence;
using Rentz.Intelligence.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Database
builder.Services.AddDbContext<RentzDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Application services
builder.Services.AddScoped<IPropertyService, PropertyService>();

builder.Services.AddScoped<
    IQueryUnderstandingService,
    QueryUnderstandingService
>();

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// HTTPS disabled temporarily for local API testing
// app.UseHttpsRedirection();

app.MapControllers();

app.Run();