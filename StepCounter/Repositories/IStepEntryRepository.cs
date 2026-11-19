using StepCounter.Models;

namespace StepCounter.Repositories;

internal interface IStepEntryRepository
{
    Task<IReadOnlyList<StepEntry>> GetAllAsync(CancellationToken cancellationToken);

    Task<StepEntry> CreateAsync(
        string entryDate,
        int steps,
        int goalSteps,
        CancellationToken cancellationToken);

    Task<StepEntryUpdateResult?> UpdateAsync(
        long id,
        int steps,
        int goalSteps,
        CancellationToken cancellationToken);

    Task<string?> DeleteAsync(long id, CancellationToken cancellationToken);
}
