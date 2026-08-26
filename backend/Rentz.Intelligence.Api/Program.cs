using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using Rentz.Intelligence.Application.Services;
using Rentz.Intelligence.Infrastructure.Persistence;
using Rentz.Intelligence.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// SERVICES
// =========================================================

builder.Services.AddControllers();


// =========================================================
// DATABASE
// =========================================================

builder.Services.AddDbContext<RentzDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.UseVector();
        }
    )
);


// =========================================================
// HTTP CLIENT
// =========================================================

builder.Services.AddHttpClient();


// =========================================================
// GEMINI API KEY
// =========================================================

var geminiApiKey = builder.Configuration["Gemini:ApiKey"]
    ?? throw new InvalidOperationException(
        "Gemini API key is not configured."
    );


// =========================================================
// EMBEDDING SERVICE
// =========================================================

builder.Services.AddScoped<EmbeddingService>(serviceProvider =>
{
    var httpClientFactory =
        serviceProvider.GetRequiredService<IHttpClientFactory>();

    var httpClient =
        httpClientFactory.CreateClient();

    return new EmbeddingService(
        httpClient,
        geminiApiKey
    );
});


// =========================================================
// PROPERTY EMBEDDING SERVICE
// =========================================================

builder.Services.AddScoped<PropertyEmbeddingService>();


// =========================================================
// PROPERTY VECTOR SEARCH SERVICE
// =========================================================

builder.Services.AddScoped<PropertyVectorSearchService>();


// =========================================================
// PROPERTY SERVICE
// =========================================================

builder.Services.AddScoped<
    IPropertyService,
    PropertyService
>();


// =========================================================
// QUERY UNDERSTANDING
// =========================================================

// Gemini AI service
builder.Services.AddScoped<
    GeminiQueryUnderstandingService
>(
    _ => new GeminiQueryUnderstandingService(
        geminiApiKey
    )
);


// Regex / rule-based fallback service
builder.Services.AddScoped<
    QueryUnderstandingService
>();


// Hybrid AI + fallback service
builder.Services.AddScoped<
    IQueryUnderstandingService,
    HybridQueryUnderstandingService
>();


// =========================================================
// PROPERTY RANKING
// =========================================================

builder.Services.AddScoped<
    IPropertyRankingService,
    PropertyRankingService
>();


// =========================================================
// HYBRID PROPERTY SEARCH
// =========================================================

builder.Services.AddScoped<
    IHybridPropertySearchService,
    HybridPropertySearchService
>();


// =========================================================
// OPENAPI
// =========================================================

builder.Services.AddOpenApi();


// =========================================================
// BUILD APPLICATION
// =========================================================

var app = builder.Build();


// =========================================================
// HTTP REQUEST PIPELINE
// =========================================================

// HTTPS disabled temporarily for Render/local API testing
// app.UseHttpsRedirection();


// =========================================================
// OPENAPI
// =========================================================

// Expose OpenAPI in both Development and Production.
// This allows us to verify the live Render API.
app.MapOpenApi();


// =========================================================
// CONTROLLERS
// =========================================================

app.MapControllers();


// =========================================================
// RUN
// =========================================================

app.Run();