namespace AtlasHelper.GameState.Atlas;

// Grouping of atlas map tiers by required Bonus Completion rarity.
// See .claude/docs/internal/glossary.md#tier-band and strategy.md#rarity-rules.
public enum TierBand
{
    White,   // T1-T5, magic sufficient
    Yellow,  // T6-T10, rare required
    Red,     // T11-T16, rare + corrupted required
}

public static class TierBands
{
    // Returns null for tier 0 / non-map nodes. Callers should filter by
    // AtlasMapNode.GrantsBonus first so this only sees mapped tiers.
    public static TierBand? For(int baseTier) => baseTier switch
    {
        >= 1 and <= 5 => TierBand.White,
        >= 6 and <= 10 => TierBand.Yellow,
        >= 11 and <= 16 => TierBand.Red,
        _ => null,
    };
}
