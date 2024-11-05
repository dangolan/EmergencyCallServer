using EmergencyCallServer.Models; // Ensure you have the correct namespaces
using Microsoft.EntityFrameworkCore; // Add this namespace for EF Core
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Register VolunteerModel as scoped
builder.Services.AddScoped<VolunteerModel>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register ApplicationDbContext with the connection string
builder.Services.AddDbContext<VolunteersDB>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register RouteModel with HttpClient for dependency injection
builder.Services.AddHttpClient<RouteModel>();

// Register EventVolunteersModel
builder.Services.AddTransient<EventVolunteersModel>();

// Add CORS policy to allow requests from any origin (or specific origins)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddSingleton<GeocodingModel>(provider =>
{
    var apiKey = "AIzaSyAOO3UGtE0Dq35gR_oQJj4ViJmw96tSe4w"; // Ensure your API key is in appsettings.json
    return new GeocodingModel(apiKey);
});

// Register GoogleCloudStorageService
builder.Services.AddSingleton(sp =>
{
    var bucketName = "volunteerphotos";
    return new GoogleCloudStorageModel(bucketName);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply CORS policy
app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
