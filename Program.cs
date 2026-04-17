using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TheDigitalCookbook.Data;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

string? connectionString;
string? dbHost = null;
int? dbPort = null;
string? dbUser = null;
string? dbName = null;

var useAwsSecretsManager = builder.Configuration.GetValue<bool>("ConnectionSources:UseAwsSecretsManager");

if (useAwsSecretsManager)
{
    var secretName = builder.Configuration["AWS:SecretName"];
    var regionName = builder.Configuration["AWS:Region"];

    if (string.IsNullOrWhiteSpace(secretName))
        throw new InvalidOperationException("AWS secret name is not configured.");

    if (string.IsNullOrWhiteSpace(regionName))
        throw new InvalidOperationException("AWS region is not configured.");

    var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

    using var secretsManagerClient = new AmazonSecretsManagerClient(regionEndpoint);
    var secretResponse = await secretsManagerClient.GetSecretValueAsync(new GetSecretValueRequest
    {
        SecretId = secretName
    });

    if (string.IsNullOrWhiteSpace(secretResponse.SecretString))
        throw new InvalidOperationException("AWS secret value is empty.");

    var secretData = JsonSerializer.Deserialize<AwsDbSecret>(
         secretResponse.SecretString,
         new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new InvalidOperationException("AWS secret could not be parsed.");

    dbHost = secretData.Host;
    dbPort = secretData.Port;
    dbUser = secretData.Username;
    dbName = "cookbook";

    connectionString =
        $"Server={secretData.Host};Port={secretData.Port};Database=cookbook;User Id={secretData.Username};Password={secretData.Password};";

    Console.WriteLine($"AWS DB host: {dbHost}");
    Console.WriteLine($"AWS DB port: {dbPort}");
    Console.WriteLine($"AWS DB user: {dbUser}");
    Console.WriteLine($"AWS DB name: {dbName}");
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    Console.WriteLine("Using local connection string from configuration.");
}

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Database connection string is missing.");
}

Console.WriteLine("Database connection string is set.");

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSession();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4, 8)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseCors("AllowFrontend");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();

public sealed class AwsDbSecret
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Engine { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string DbInstanceIdentifier { get; set; } = string.Empty;
    public string? Database { get; set; }
}