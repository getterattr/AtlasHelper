using System.Collections.Generic;
using ExileCore;

namespace AtlasHelper.GameState;

// Named atlas nodes referenced by strategy - waypoints for routing to
// each voidstone corner, and the corner slots themselves. Mirrors the
// AtlasQuestFlags pattern: hand-listed constants grouped to match the
// Voidstones module, validated at plugin startup against
// Files.AtlasNodes (Files-first per decisions/read-pattern.md), loud
// failure on rename.
//
// The identifiers are AtlasMapNode.AreaId values - i.e. AtlasNode.Area.Id
// for regular map nodes and AtlasNode.Id for corner slots (which have
// no Area). Placeholder values below are best-guess and TODO'd until
// the AtlasNodeDump.tsv (see Diagnostics/AtlasNodeDump.cs) surfaces
// canonical ids from Files.AtlasNodes.
internal static class AtlasObjectives
{
    public static class Voidstones
    {
        public static class Eldritch
        {
            // TODO: pin via AtlasNodeDump.tsv
            public const string CornerSlot = "AtlasSlotBottomLeft";
            public const string PolaricVoid = "MapWorldsPolaricVoid";
            public const string SeethingChime = "MapWorldsSeethingChime";
        }

        public static class Originator
        {
            // TODO: pin via AtlasNodeDump.tsv
            public const string CornerSlot = "AtlasSlotTopLeft";
        }

        public static class Decayed
        {
            // TODO: pin via AtlasNodeDump.tsv
            public const string CornerSlot = "AtlasSlotTopRight";
        }

        public static class Ceremonial
        {
            // TODO: pin via AtlasNodeDump.tsv
            public const string CornerSlot = "AtlasSlotBottomRight";
        }
    }

    // Every constant declared above must appear in this list. Missing an
    // entry means Validate() will not flag its absence when the id no
    // longer exists in Files.AtlasNodes.
    private static readonly string[] All =
    {
        Voidstones.Eldritch.CornerSlot,
        Voidstones.Eldritch.PolaricVoid,
        Voidstones.Eldritch.SeethingChime,
        Voidstones.Originator.CornerSlot,
        Voidstones.Decayed.CornerSlot,
        Voidstones.Ceremonial.CornerSlot,
    };

    public sealed record ValidationResult(int Total, IReadOnlyList<string> Unresolved);

    public static ValidationResult Validate(GameController gc)
    {
        var known = new HashSet<string>();
        var atlasNodes = gc?.Files?.AtlasNodes?.EntriesList;
        if (atlasNodes != null)
        {
            foreach (var entry in atlasNodes)
            {
                var id = entry?.Area?.Id;
                if (!string.IsNullOrEmpty(id)) known.Add(id);
                var rawId = entry?.Id;
                if (!string.IsNullOrEmpty(rawId)) known.Add(rawId);
            }
        }

        var unresolved = new List<string>();
        foreach (var declared in All)
            if (!known.Contains(declared)) unresolved.Add(declared);

        return new ValidationResult(All.Length, unresolved);
    }
}
