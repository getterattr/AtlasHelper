using AtlasHelper.GameState;

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
    One,   // First voidstone: Eldritch (Exarch + Eater)
    Two,   // Second voidstone: Originator (Eagon) + 10-way Maven
    Three, // 100% bonus completion (100 normal + 10 unique)
    Four,  // Final voidstones: Decayed (Shaper + Elder), Ceremonial (Maven)
}

public sealed record PhaseInference(PhaseId Id, string Reason)
{
    public static PhaseInference Default { get; } =
        new(PhaseId.One, "default - Eldritch not socketed");
}

public static class Phase
{
    public static PhaseInference From(AtlasSnapshot snapshot)
    {
        // Placeholder implementation - returns default until workstream 3
        // (see roadmap.md) fleshes out phase inference against voidstones,
        // completion counts, and chain state.
        return PhaseInference.Default;
    }
}
