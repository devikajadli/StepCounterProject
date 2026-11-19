using StepCounter.Models;

namespace StepCounter.Services;

internal interface IStepEntryService
{
    Task<IReadOnlyList<StepEntry>> GetAllAsync(CancellationToken cancellationToken);

    Task<StepEntry> CreateAsync(
        CreateStepEntryRequest request,
        CancellationToken cancellationToken);

    Task<UpdateStepEntryResponse?> UpdateAsync(
        long id,
        UpdateStepEntryRequest request,
        CancellationToken cancellationToken);

    Task<DeleteStepEntryResponse?> DeleteAsync(long id, CancellationToken cancellationToken);
}
