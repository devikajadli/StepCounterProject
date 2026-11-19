using Microsoft.Data.Sqlite;
using StepCounter.Models;

namespace StepCounter.Repositories;

internal sealed class SqliteUserSettingsRepository(string readOnlyConnectionString)
    : IUserSettingsRepository
{
    public async Task<UserSettings?> GetAsync(CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection(readOnlyConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, DailyGoal, WeightKg, HeightCm
            FROM UserSettings
            ORDER BY Id
            LIMIT 1;
            """;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new UserSettings(
            reader.GetInt64(0),
            reader.GetInt32(1),
            reader.GetDouble(2),
            reader.GetDouble(3));
    }
}
