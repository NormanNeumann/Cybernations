using System.Collections.Generic;

namespace Cybernations.Application.ViewModels;

public sealed class EnvisionPhaseVm
{
    public bool IsVisible { get; init; } = true;
    public string PhaseTitle { get; init; } = "Envision Phase";
    public string ActivePlayerText { get; init; } = "Current Player: P1";
    public string PhaseHintText { get; init; } = "Players take actions until everyone passes.";
    public string ActionCostText { get; init; } = "Current extra cost: none";
    public string StatusMessage { get; init; } = "Waiting for player action...";

    public IReadOnlyList<string> FeedbackSlots { get; init; } = new[]
    {
        "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-"
    };

    public bool CanShiftPower { get; init; } = true;
    public bool CanComeTogether { get; init; } = true;
    public bool CanConnect { get; init; } = true;
    public bool CanSetCourse { get; init; } = true;
    public bool CanPrepare { get; init; } = true;
    public bool CanSteer { get; init; } = true;
    public bool CanPass { get; init; } = true;
}