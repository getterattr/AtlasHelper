using AtlasHelper.GameState;
using AtlasHelper.GameState.Atlas;

namespace AtlasHelper.Services;

// The Advisory is the plugin's per-phase "what to do next" line. It is
// the single source of truth for both HUD text and phase-scoped overlay
// filtering: the HUD renders Text, the AtlasOverlay pattern-matches on
// the case record to derive which nodes to highlight.
//
// See .claude/docs/internal/glossary.md#advisory and strategy.md for the
// rules this file encodes.

public enum Rarity
{
    Magic,           // whites
    Rare,            // yellows
    RareCorrupted,   // reds
}

public abstract record AdvisoryLine(string Text);

public sealed record Phase1Advisory(string Text) : AdvisoryLine(Text)
{
    public static Phase1Advisory Default { get; } =
        new("Path bottom-left; complete Exarch + Eater");
}

public sealed record Phase2Advisory(string Text) : AdvisoryLine(Text)
{
    public static Phase2Advisory Default { get; } =
        new("Originator corner + 10-way Maven\nKeep Maven's Beacon toggled on");
}

// Phase 3 has two shapes: the band sweep (whites -> yellows -> reds) and
// the uniques-only endgame that follows once every non-unique bonus is
// completed. ActiveBand and RequiredRarity are meaningless when
// UniquesOnly is true; consumers must branch on UniquesOnly first.
public sealed record Phase3Advisory(
    string Text,
    TierBand ActiveBand,
    Rarity RequiredRarity,
    bool UniquesOnly) : AdvisoryLine(Text);

public sealed record Phase4Advisory(string Text) : AdvisoryLine(Text)
{
    public static Phase4Advisory Default { get; } =
        new("Guardians for Shaper/Elder; splinters for Writ");
}

public sealed record CompleteAdvisory(string Text) : AdvisoryLine(Text)
{
    public static CompleteAdvisory Default { get; } =
        new("Atlas progression complete");
}

public static class Advisory
{
    public static AdvisoryLine From(AtlasHelperSettings settings, AtlasSnapshot snapshot)
    {
        var phase = Phase.Resolve(settings, snapshot);
        return phase switch
        {
            PhaseId.One => Phase1Advisory.Default,
            PhaseId.Two => Phase2Advisory.Default,
            PhaseId.Three => ForPhase3(snapshot),
            PhaseId.Four => Phase4Advisory.Default,
            PhaseId.Complete => CompleteAdvisory.Default,
            _ => Phase1Advisory.Default,
        };
    }

    private static Phase3Advisory ForPhase3(AtlasSnapshot snapshot)
    {
        var band = LowestUnbonusedBand(snapshot.Tree);
        if (band is null)
        {
            var missing = AtlasCompletion.UniqueBonusTarget - snapshot.Completion.UniqueBonusCount;
            var text = missing > 0
                ? $"Grab {missing} more unique-map bonuses"
                : "Phase 3 complete";
            return new Phase3Advisory(text, TierBand.Red, Rarity.RareCorrupted, UniquesOnly: true);
        }

        var remaining = CountUnbonusedInBand(snapshot.Tree, band.Value);
        var rarity = RarityFor(band.Value);
        var sweepText =
            $"Sweep {BandLabel(band.Value)}, {RarityLabel(rarity)}\n" +
            $"{remaining} unbonused remaining";
        return new Phase3Advisory(sweepText, band.Value, rarity, UniquesOnly: false);
    }

    // Whites, then yellows, then reds. Strict flip: as long as any node
    // in the earlier band is unbonused, that band is active. Returns null
    // when nothing non-unique is left to bonus (the endgame trigger).
    private static TierBand? LowestUnbonusedBand(AtlasTree tree)
    {
        var whites = false;
        var yellows = false;
        var reds = false;
        foreach (var node in tree.Nodes)
        {
            if (!node.GrantsBonus) continue;
            if (node.IsUnique) continue;
            if (node.BonusCompleted) continue;
            var band = TierBands.For(node.BaseTier);
            switch (band)
            {
                case TierBand.White: whites = true; break;
                case TierBand.Yellow: yellows = true; break;
                case TierBand.Red: reds = true; break;
            }
        }
        if (whites) return TierBand.White;
        if (yellows) return TierBand.Yellow;
        if (reds) return TierBand.Red;
        return null;
    }

    private static int CountUnbonusedInBand(AtlasTree tree, TierBand band)
    {
        var count = 0;
        foreach (var node in tree.Nodes)
        {
            if (!node.GrantsBonus) continue;
            if (node.IsUnique) continue;
            if (node.BonusCompleted) continue;
            if (TierBands.For(node.BaseTier) != band) continue;
            count++;
        }
        return count;
    }

    private static Rarity RarityFor(TierBand band) => band switch
    {
        TierBand.White => Rarity.Magic,
        TierBand.Yellow => Rarity.Rare,
        TierBand.Red => Rarity.RareCorrupted,
        _ => Rarity.Magic,
    };

    private static string BandLabel(TierBand band) => band switch
    {
        TierBand.White => "whites",
        TierBand.Yellow => "yellows",
        TierBand.Red => "reds",
        _ => "-",
    };

    private static string RarityLabel(Rarity rarity) => rarity switch
    {
        Rarity.Magic => "magic-rolled",
        Rarity.Rare => "rare-rolled",
        Rarity.RareCorrupted => "rare + corrupted",
        _ => "-",
    };
}
