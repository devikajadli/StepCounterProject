using StepCounter.Models;

namespace StepCounter.Services;

internal interface IUserSettingsService
{
    Task<UserSettings?> GetAsync(CancellationToken cancellationToken);
}
