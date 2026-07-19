using System.Collections.Generic;

namespace AtlasHelper.GameState;

public sealed record MavenState(IReadOnlyList<string> WitnessedAreaNames)
{
    public const int InvitationTarget = 10;

    public int WitnessCount => WitnessedAreaNames.Count;
    public bool InvitationReady => WitnessCount >= InvitationTarget;

    public static MavenState Empty { get; } = new(new List<string>());
}
