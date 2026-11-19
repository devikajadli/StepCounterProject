using StepCounter.Services;

namespace StepCounter.Endpoints;

public static class UserSettingsEndpoints
{
    public static WebApplication MapUserSettingsEndpoints(this WebApplication app)
    {
        app.MapGet("/api/user-settings", async (
            IUserSettingsService service,
            CancellationToken cancellationToken) =>
        {
            var settings = await service.GetAsync(cancellationToken);
            if (settings is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(settings);
        })
        .WithName("GetUserSettings");

        return app;
    }
}
