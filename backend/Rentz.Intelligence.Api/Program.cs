using Microsoft.EntityFrameworkCore;
using Rentz.Intelligence.Application.Services;
using Rentz.Intelligence.Infrastructure.Persistence;
using Rentz.Intelligence.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// SERVICES
// =========================================================

// Controllers
builder.Services.AddControllers();

// =========================================================
// DATABASE
// =========================================================

builder.Services.AddDbContext<RentzDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// =========================================================
// APPLICATION SERVICES
// =========================================================

// Property service
builder.Services.AddScoped<
    IPropertyService,
    PropertyService
>();

// Query understanding service
builder.Services.AddScoped<
    IQueryUnderstandingService,
    QueryUnderstandingService
>();

// Property ranking service
builder.Services.AddScoped<
    IPropertyRankingService,
    PropertyRankingService
>();

// =========================================================
// OPENAPI
// =========================================================

builder.Services.AddOpenApi();

var app = builder.Build();

// =========================================================
// HTTP REQUEST PIPELINE
// =========================================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// HTTPS disabled temporarily for local API testing
// app.UseHttpsRedirection();

app.MapControllers();

app.Run();