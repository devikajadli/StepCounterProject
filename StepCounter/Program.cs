using Microsoft.Data.Sqlite;
using StepCounter.Endpoints;
using StepCounter.Repositories;
using StepCounter.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var configuredDatabasePath = builder.Configuration.GetConnectionString("StepCounter")
    ?? throw new InvalidOperationException("Connection string 'StepCounter' is not configured.");
var databasePath = Path.GetFullPath(
    Path.Combine(builder.Environment.ContentRootPath, configuredDatabasePath));
var readOnlyConnectionString = new SqliteConnectionStringBuilder
{
    DataSource = databasePath,
    Mode = SqliteOpenMode.ReadOnly
}.ToString();
var readWriteConnectionString = new SqliteConnectionStringBuilder
{
    DataSource = databasePath,
    Mode = SqliteOpenMode.ReadWrite
}.ToString();

builder.Services.AddSingleton<IStepEntryRepository>(
    _ => new SqliteStepEntryRepository(readOnlyConnectionString, readWriteConnectionString));
builder.Services.AddSingleton<IUserSettingsRepository>(
    _ => new SqliteUserSettingsRepository(readOnlyConnectionString));
builder.Services.AddScoped<IStepEntryService, StepEntryService>();
builder.Services.AddScoped<IUserSettingsService, UserSettingsService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapStepEntryEndpoints();
app.MapUserSettingsEndpoints();

app.Run();
