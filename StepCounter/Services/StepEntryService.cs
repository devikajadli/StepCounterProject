using StepCounter.Models;
using StepCounter.Repositories;

namespace StepCounter.Services;

internal sealed class StepEntryService(IStepEntryRepository repository) : IStepEntryService
{
    public Task<IReadOnlyList<StepEntry>> GetAllAsync(CancellationToken cancellationToken) =>
        repository.GetAllAsync(cancellationToken);

    public Task<StepEntry> CreateAsync(
        CreateStepEntryRequest request,
        CancellationToken cancellationToken) =>
        repository.CreateAsync(
            request.EntryDate,
            request.Steps,
            request.GoalSteps,
            cancellationToken);

    public async Task<UpdateStepEntryResponse?> UpdateAsync(
        long id,
        UpdateStepEntryRequest request,
        CancellationToken cancellationToken)
    {
        var updateResult = await repository.UpdateAsync(
            id,
            request.Steps,
            request.GoalSteps,
            cancellationToken);

        if (updateResult is null)
        {
            return null;
        }

        var message = (updateResult.StepsChanged, updateResult.GoalStepsChanged) switch
        {
            (true, true) => "Steps and goal steps were changed successfully.",
            (true, false) => "Steps were changed successfully.",
            (false, true) => "Goal steps were changed successfully.",
            _ => "No changes were made."
        };

        return new UpdateStepEntryResponse(message, updateResult.Entry);
    }

    public async Task<DeleteStepEntryResponse?> DeleteAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var entryDate = await repository.DeleteAsync(id, cancellationToken);
        return entryDate is null
            ? null
            : new DeleteStepEntryResponse(
                $"Step entry for {entryDate} was deleted successfully.");
    }
}
