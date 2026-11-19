namespace StepCounter.Models;

/// <summary>Represents a step entry returned by the API.</summary>
internal sealed record StepEntry(long Id, string EntryDate, int Steps, int GoalSteps);

/// <summary>Request body for creating a step entry.</summary>
internal sealed record CreateStepEntryRequest(string EntryDate, int Steps, int GoalSteps);

/// <summary>Request body for updating a step entry.</summary>
internal sealed record UpdateStepEntryRequest(int Steps, int GoalSteps);

/// <summary>Represents the user's step-counting settings.</summary>
internal sealed record UserSettings(long Id, int DailyGoal, double WeightKg, double HeightCm);

/// <summary>Contains an updated step entry and the change message for the response.</summary>
internal sealed record UpdateStepEntryResponse(string Message, StepEntry StepEntry);

/// <summary>Contains the message returned after a step entry is deleted.</summary>
internal sealed record DeleteStepEntryResponse(string Message);

/// <summary>Describes the values that changed while updating a step entry.</summary>
internal sealed record StepEntryUpdateResult(
    StepEntry Entry,
    bool StepsChanged,
    bool GoalStepsChanged);
