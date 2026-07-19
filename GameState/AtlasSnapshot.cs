using System;

namespace AtlasHelper.GameState;

public sealed record AtlasSnapshot(
    DateTime CapturedAt,
    VoidstoneState Voidstones,
    AtlasCompletion Completion,
    MavenState Maven,
    AtlasTree Tree,
    AtlasBeacons Beacons,
    AtlasInvitationProgress InvitationProgress,
    PinnacleCompletion Pinnacles)
{
    public static AtlasSnapshot Empty { get; } = new(
        DateTime.MinValue,
        VoidstoneState.Empty,
        AtlasCompletion.Empty,
        MavenState.Empty,
        AtlasTree.Empty,
        AtlasBeacons.Empty,
        AtlasInvitationProgress.Empty,
        PinnacleCompletion.Empty);
}
