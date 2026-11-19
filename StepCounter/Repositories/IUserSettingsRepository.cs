using StepCounter.Models;

namespace StepCounter.Repositories;

internal interface IUserSettingsRepository
{
    Task<UserSettings?> GetAsync(CancellationToken cancellationToken);
}
