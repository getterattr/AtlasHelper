using AtlasHelper.GameState;
using AtlasHelper.GameState.Voidstones;

namespace AtlasHelper.Services;

// Phase inference derives the player's current league-start phase from
// snapshot state. Phase definitions live in
// .claude/docs/internal/glossary.md and the acquisition order in
// .claude/docs/internal/strategy.md#voidstone-acquisition-order.
//
// This is the reference implementation of the Services/ pattern: input is
// AtlasSnapshot, output is a domain record living in the same file, no
// state, no lifecycle. Correlation across snapshot modules happens here.

public enum PhaseId
{
    One,      // First voidstone: Eldritch (Exarch + Eater)
    Two,      // Second voidstone: Originator (Eagon) + 10-way Maven
    Three,    // 100% bonus completion (100 normal + 10 unique)
    Four,     // Final voidstones: Decayed (Shaper + Elder), Ceremonial (Maven)
    Complete, // All 4 voidstones socketed - atlas progression done.
}

public sealed record PhaseInference(PhaseId Id, string Reason)
{
    public static PhaseInference Default { get; } =
        new(PhaseId.One, "default - Eldritch not socketed");
}

public static class Phase
{
    // Resolves the phase the UI should render for, honouring the user's
    // manual override. Shared by every surface that renders phase-scoped
    // output (HUD, atlas overlays, path overlay, advisory) so the override
    // is respected in exactly one place.
    public static PhaseId Resolve(AtlasHelperSettings settings, AtlasSnapshot snapshot)
    {
        return settings.Progression.PhaseOverride.Value switch
        {
            "Phase 1" => PhaseId.One,
            "Phase 2" => PhaseId.Two,
            "Phase 3" => PhaseId.Three,
            "Phase 4" => PhaseId.Four,
            _ => From(snapshot).Id,
        };
    }

    public static PhaseInference From(AtlasSnapshot snapshot)
    {
        if (snapshot.Voidstones.SocketedCount >= 4)
            return new PhaseInference(PhaseId.Complete, "all four voidstones socketed");

        var eldritchSocketed = IsSocketed(snapshot.Voidstones, VoidstoneKind.Eldritch);
        if (!eldritchSocketed)
            return new PhaseInference(PhaseId.One, "Eldritch not socketed");

        var originatorSocketed = IsSocketed(snapshot.Voidstones, VoidstoneKind.Originator);
        if (!originatorSocketed)
            return new PhaseInference(PhaseId.Two, "Eldritch socketed, Originator pending");

        // Phase 3 = both first voidstones socketed, bonus not yet 100/100 + 10/10.
        // Phase 4 = both first voidstones socketed, bonus complete, working on Decayed/Ceremonial.
        var bonusComplete = snapshot.Completion.NormalBonusComplete
                         && snapshot.Completion.UniqueBonusComplete;
        if (!bonusComplete)
            return new PhaseInference(PhaseId.Three, "voidstones 1-2 socketed, bonus incomplete");

        return new PhaseInference(PhaseId.Four, "bonus complete, Decayed/Ceremonial pending");
    }

    private static bool IsSocketed(VoidstoneState voidstones, VoidstoneKind kind)
    {
        foreach (var slot in voidstones.Slots)
            if (slot.Kind == kind && slot.Socketed) return true;
        return false;
    }
}
