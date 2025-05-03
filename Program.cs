using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CanonicalModel;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Build the initial configuration
var configuration = builder.Configuration;

// Load the appsettings.json and environment variables
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Replace the placeholder with the actual environment variable value
var connectionString = configuration.GetConnectionString("DbConnect");
if (connectionString.Contains("${DATABASE_URL}"))
{
    var environmentConnectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrEmpty(environmentConnectionString))
    {
        connectionString = environmentConnectionString;
    }
    configuration["ConnectionStrings:DbConnect"] = connectionString;
}

var app = builder.Build();

// Define the method to register the plugin
async Task RegisterPluginAsync(ILogger<Program> logger)
{
    using var httpClient = new HttpClient();
    var pluginData = new
    {
        PluginName = "sample-plugin-1",
        PageUrl = "http://localhost:5299/sample-plugin.html",
        TabName = "Sample Plugin 1"
    };

    var jsonContent = new StringContent(JsonSerializer.Serialize(pluginData), Encoding.UTF8, "application/json");

    try
    {
        logger.LogInformation("Sending request to register plugin...");
        var response = await httpClient.PostAsync("http://localhost:5150/api/pluginregistry/register", jsonContent);
        response.EnsureSuccessStatusCode(); // Throws an exception if the status code is not successful
        logger.LogInformation("Plugin registered successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to register plugin.");
    }
}

// Define the method to get the list of running plugins
async Task GetRunningPluginsAsync(ILogger<Program> logger)
{
    using var httpClient = new HttpClient();
    
    try
    {
        logger.LogInformation("Sending request to get running plugins...");
        var response = await httpClient.GetAsync("http://localhost:5150/api/pluginstatus/running-plugins");
        
        // Log response status and headers
        logger.LogInformation("Response Status Code: {StatusCode}", response.StatusCode);
        logger.LogInformation("Response Headers: {Headers}", response.Headers.ToString());
        
        response.EnsureSuccessStatusCode(); // Throws an exception if the status code is not successful

        var responseBody = await response.Content.ReadAsStringAsync();
        var plugins = JsonSerializer.Deserialize<List<PluginMetadata>>(responseBody);

        logger.LogInformation("Retrieved list of running plugins: {PluginsData}", JsonSerializer.Serialize(plugins));
    }
    catch (HttpRequestException ex)
    {
        logger.LogError(ex, "Failed to get running plugins. Status Code: {StatusCode}", ex.StatusCode);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An unexpected error occurred while retrieving running plugins.");
    }
}
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// Apply CORS policy
app.UseCors("AllowAllOrigins");

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();

// Get the logger instance from the service provider
var logger = app.Services.GetRequiredService<ILogger<Program>>();

// SAMPLE SHOWING THE USAGE OF CANONICAL MODEL FROM NUGET REPO
var sample = new SampleClass("Alice", 30);
sample.DisplayInfo();

// Call the registration method during startup
await RegisterPluginAsync(logger);

app.Run();


public class PluginMetadata
{
    public string PluginName { get; set; }
    public string PageUrl { get; set; }
    public string TabName { get; set; }
}