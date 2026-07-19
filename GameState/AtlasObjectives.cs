using System.Collections.Generic;
using ExileCore;

namespace AtlasHelper.GameState;

// Resolves the identifiers of atlas nodes referenced by strategy - the
// four voidstone corner slots and the Phase-1 T11 waypoints - from
// Files.AtlasNodes at plugin startup. Uses stable-signal patterns
// rather than raw entry.Id constants:
//
//   - Corner slots: entry.Id.EndsWith("WatchstoneSlotNode") plus
//     position-quadrant classification. Two-signal match survives
//     either signal rot alone; both signals rotting is a full atlas
//     rework and warrants human review.
//
//   - Phase-1 waypoints: display-name lookup (Area.Name). Names are
//     the most patch-stable identifier POE exposes; "Siege" does not
//     rename to "Fortress Siege" without breaking every guide ever
//     written.
//
// See strategy.md#route-planning for how the resolved objectives feed
// PathOverlay. Unresolved objectives at plugin startup surface loudly
// via the diagnostics log (mirrors AtlasQuestFlags.Validate).
internal sealed record AtlasObjectives(
    string? EldritchCornerId,
    string? OriginatorCornerId,
    string? DecayedCornerId,
    string? CeremonialCornerId,
    string? EldritchWaypointA,
    string? EldritchWaypointB)
{
    // Phase-1 Eldritch chain waypoints. These are the atlas icons for
    // The Black Star and The Infinite Hunger arenas - the mid-tier
    // Eldritch pinnacle fights entered via Polaric Invitation (Black
    // Star / Polaric Void) and Writhing Invitation (Infinite Hunger /
    // Seething Chyme). Both live in the Eldritch (bottom-left)
    // quadrant. Wiki-verified 3.29.
    //
    // The arenas themselves (MapWorldsPrimordialBoss1/2, area level 83)
    // are off-atlas via `map_not_on_atlas`, but their entry icons are
    // pathable atlas nodes.
    private const string BlackStarAtlasId = "BlackStarBoss";
    private const string InfiniteHungerAtlasId = "InfiniteHungerBoss";

    // Corner-slot id-suffix signal. Legacy Watchstone naming from
    // Conquerors of the Atlas; GGG never renamed on the voidstone
    // migration, so 5+ years of stability. If they do rename, position
    // quadrant is still resolved.
    private const string CornerSlotIdSuffix = "WatchstoneSlotNode";

    public static AtlasObjectives Empty { get; } =
        new(null, null, null, null, null, null);

    public IEnumerable<(string Label, string? Id)> All()
    {
        yield return ("Eldritch corner", EldritchCornerId);
        yield return ("Originator corner", OriginatorCornerId);
        yield return ("Decayed corner", DecayedCornerId);
        yield return ("Ceremonial corner", CeremonialCornerId);
        yield return ($"Black Star icon '{BlackStarAtlasId}'", EldritchWaypointA);
        yield return ($"Infinite Hunger icon '{InfiniteHungerAtlasId}'", EldritchWaypointB);
    }

    public static AtlasObjectives Resolve(GameController gc)
    {
        var atlasNodes = gc?.Files?.AtlasNodes?.EntriesList;
        if (atlasNodes == null || atlasNodes.Count == 0) return Empty;

        // Find atlas centre for quadrant classification. Extremes give
        // a stable midpoint even if a few nodes shift.
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;
        foreach (var entry in atlasNodes)
        {
            if (entry == null) continue;
            if (entry.PosX < minX) minX = entry.PosX;
            if (entry.PosX > maxX) maxX = entry.PosX;
            if (entry.PosY < minY) minY = entry.PosY;
            if (entry.PosY > maxY) maxY = entry.PosY;
        }
        var midX = (minX + maxX) * 0.5f;
        var midY = (minY + maxY) * 0.5f;

        string? eldritchCorner = null;
        string? originatorCorner = null;
        string? decayedCorner = null;
        string? ceremonialCorner = null;
        string? waypointA = null;
        string? waypointB = null;

        foreach (var entry in atlasNodes)
        {
            if (entry == null) continue;

            var id = entry.Id;
            var areaName = entry.Area?.Name;

            // Corner slot resolution: id suffix + quadrant. In POE's
            // coord system, y increases downward - top has smaller y.
            if (!string.IsNullOrEmpty(id) && id.EndsWith(CornerSlotIdSuffix))
            {
                var isLeft = entry.PosX < midX;
                var isTop = entry.PosY < midY;

                if (isTop && isLeft) originatorCorner = id;
                else if (!isTop && isLeft) eldritchCorner = id;
                else if (isTop && !isLeft) decayedCorner = id;
                else ceremonialCorner = id;

                continue;
            }

            // Boss-arena entry icons on the atlas have no Area (like
            // corner slots); resolve by exact entry.Id. Wiki-verified
            // stable ids as of 3.29. If GGG renames, fall back to
            // position-quadrant + no-area filter (not implemented
            // until we see it break in practice).
            if (id == BlackStarAtlasId) waypointA = id;
            else if (id == InfiniteHungerAtlasId) waypointB = id;
        }

        return new AtlasObjectives(
            eldritchCorner, originatorCorner, decayedCorner, ceremonialCorner,
            waypointA, waypointB);
    }
}
