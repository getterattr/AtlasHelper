using System;
using AtlasHelper.GameState.Atlas;
using AtlasHelper.GameState.Maven;
using AtlasHelper.GameState.Pinnacles;
using AtlasHelper.GameState.Voidstones;

namespace AtlasHelper.GameState;

public sealed record AtlasSnapshot(
    DateTime CapturedAt,
    // Atlas surface
    AtlasTree Tree,
    AtlasCompletion Completion,
    // Voidstone slot state + per-corner chain progression
    VoidstoneState Voidstones,
    Eldritch Eldritch,
    Originator Originator,
    Decayed Decayed,
    // Individual pinnacle boss kills (sole owner)
    PinnacleBosses PinnacleBosses,
    // Maven-owned state
    Witnesses Witnesses,
    AtlasInvitation AtlasInvitation,
    ThemedInvitations ThemedInvitations)
{
    public static AtlasSnapshot Empty { get; } = new(
        DateTime.MinValue,
        AtlasTree.Empty,
        AtlasCompletion.Empty,
        VoidstoneState.Empty,
        Eldritch.Empty,
        Originator.Empty,
        Decayed.Empty,
        PinnacleBosses.Empty,
        Witnesses.Empty,
        AtlasInvitation.Empty,
        ThemedInvitations.Empty);
}
