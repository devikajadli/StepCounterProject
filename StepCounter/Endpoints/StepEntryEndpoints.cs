using StepCounter.Models;
using StepCounter.Services;

namespace StepCounter.Endpoints;

public static class StepEntryEndpoints
{
    public static WebApplication MapStepEntryEndpoints(this WebApplication app)
    {
        app.MapGet("/api/step-entries", async (
            IStepEntryService service,
            CancellationToken cancellationToken) =>
        {
            var entries = await service.GetAllAsync(cancellationToken);
            return Results.Ok(entries);
        })
        .WithName("GetStepEntries");

        app.MapPost("/api/step-entries", async (
            CreateStepEntryRequest request,
            IStepEntryService service,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.EntryDate))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(request.EntryDate)] = ["EntryDate is required."]
                });
            }

            if (request.Steps < 0)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(request.Steps)] = ["Steps must be zero or greater."]
                });
            }

            if (request.GoalSteps <= 0)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(request.GoalSteps)] = ["GoalSteps must be greater than zero."]
                });
            }

            var newEntry = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/api/step-entries/{newEntry.Id}", newEntry);
        })
        .WithName("CreateStepEntry");

        app.MapPut("/api/step-entries/{id:long}", async (
            long id,
            UpdateStepEntryRequest request,
            IStepEntryService service,
            CancellationToken cancellationToken) =>
        {
            if (request.Steps < 0)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(request.Steps)] = ["Steps must be zero or greater."]
                });
            }

            if (request.GoalSteps <= 0)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(request.GoalSteps)] = ["GoalSteps must be greater than zero."]
                });
            }

            var updatedEntry = await service.UpdateAsync(id, request, cancellationToken);
            if (updatedEntry is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(updatedEntry);
        })
        .WithName("UpdateStepEntry");

        app.MapDelete("/api/step-entries/{id:long}", async (
            long id,
            IStepEntryService service,
            CancellationToken cancellationToken) =>
        {
            var deletedEntry = await service.DeleteAsync(id, cancellationToken);
            if (deletedEntry is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(deletedEntry);
        })
        .WithName("DeleteStepEntry");

        return app;
    }
}
