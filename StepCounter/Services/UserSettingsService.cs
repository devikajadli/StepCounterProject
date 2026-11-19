using StepCounter.Models;
using StepCounter.Repositories;

namespace StepCounter.Services;

internal sealed class UserSettingsService(IUserSettingsRepository repository)
    : IUserSettingsService
{
    public Task<UserSettings?> GetAsync(CancellationToken cancellationToken) =>
        repository.GetAsync(cancellationToken);
}
