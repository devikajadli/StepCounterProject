using Microsoft.Data.Sqlite;
using StepCounter.Models;

namespace StepCounter.Repositories;

internal sealed class SqliteStepEntryRepository(
    string readOnlyConnectionString,
    string readWriteConnectionString) : IStepEntryRepository
{
    public async Task<IReadOnlyList<StepEntry>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection(readOnlyConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, EntryDate, Steps, GoalSteps
            FROM StepEntries
            ORDER BY EntryDate;
            """;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var entries = new List<StepEntry>();
        while (await reader.ReadAsync(cancellationToken))
        {
            entries.Add(new StepEntry(
                reader.GetInt64(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.GetInt32(3)));
        }

        return entries;
    }

    public async Task<StepEntry> CreateAsync(
        string entryDate,
        int steps,
        int goalSteps,
        CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection(readWriteConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO StepEntries (EntryDate, Steps, GoalSteps)
            VALUES ($EntryDate, $Steps, $GoalSteps);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("$EntryDate", entryDate);
        command.Parameters.AddWithValue("$Steps", steps);
        command.Parameters.AddWithValue("$GoalSteps", goalSteps);

        var createdId = await command.ExecuteScalarAsync(cancellationToken);
        return new StepEntry(Convert.ToInt64(createdId), entryDate, steps, goalSteps);
    }

    public async Task<StepEntryUpdateResult?> UpdateAsync(
        long id,
        int steps,
        int goalSteps,
        CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection(readWriteConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using var findCommand = connection.CreateCommand();
        findCommand.CommandText = """
            SELECT EntryDate, Steps, GoalSteps
            FROM StepEntries
            WHERE Id = $Id;
            """;
        findCommand.Parameters.AddWithValue("$Id", id);

        string entryDate;
        int previousSteps;
        int previousGoalSteps;
        await using (var reader = await findCommand.ExecuteReaderAsync(cancellationToken))
        {
            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            entryDate = reader.GetString(0);
            previousSteps = reader.GetInt32(1);
            previousGoalSteps = reader.GetInt32(2);
        }

        await using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE StepEntries
            SET Steps = $Steps, GoalSteps = $GoalSteps
            WHERE Id = $Id;
            """;
        command.Parameters.AddWithValue("$Id", id);
        command.Parameters.AddWithValue("$Steps", steps);
        command.Parameters.AddWithValue("$GoalSteps", goalSteps);

        var rowsUpdated = await command.ExecuteNonQueryAsync(cancellationToken);
        if (rowsUpdated == 0)
        {
            return null;
        }

        return new StepEntryUpdateResult(
            new StepEntry(id, entryDate, steps, goalSteps),
            previousSteps != steps,
            previousGoalSteps != goalSteps);
    }

    public async Task<string?> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection(readWriteConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using var findCommand = connection.CreateCommand();
        findCommand.CommandText = """
            SELECT EntryDate
            FROM StepEntries
            WHERE Id = $Id;
            """;
        findCommand.Parameters.AddWithValue("$Id", id);

        var entryDate = await findCommand.ExecuteScalarAsync(cancellationToken) as string;
        if (entryDate is null)
        {
            return null;
        }

        await using var command = connection.CreateCommand();
        command.CommandText = """
            DELETE FROM StepEntries
            WHERE Id = $Id;
            """;
        command.Parameters.AddWithValue("$Id", id);

        var rowsDeleted = await command.ExecuteNonQueryAsync(cancellationToken);
        return rowsDeleted == 0 ? null : entryDate;
    }
}
