using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.EntityFrameworkCore;
using TheDigitalCookbook.Data;

var builder = WebApplication.CreateBuilder(args);

string? connectionString;

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

    connectionString = secretResponse.SecretString;
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Database connection string is missing.");
}

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

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
app.UseCors("AllowFrontend");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();