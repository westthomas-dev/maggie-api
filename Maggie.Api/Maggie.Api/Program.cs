using Asp.Versioning;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1);
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Theme = ScalarTheme.Moon;
        options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

app
    .MapGet("api/v{apiVersion:apiVersion}/maggie", async () =>
    {
        return Results.Ok(new Dog("Maggie", "Labrador", "Golden", "Maggie likes to watch fireworks", DateOnly.FromDateTime(DateTime.Now)));
    })
    .WithApiVersionSet(apiVersionSet)
    .MapToApiVersion(1);

app.Run();

internal record Dog(string Name, string Breed, string Colour, string FunFact, DateOnly DateOfBirth)
{
    public int Age => DateTime.Now.Year - DateOfBirth.Year;
};
