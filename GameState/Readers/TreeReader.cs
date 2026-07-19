using System.Collections.Generic;
using System.Numerics;
using AtlasHelper.GameState.Atlas;
using ExileCore;

namespace AtlasHelper.GameState.Readers;

internal static class TreeReader
{
    // Regular map atlas nodes flagged as non-bonus even though they carry a
    // tier and Area. Shaper Guardians appear as normal map nodes but do not
    // contribute to the 100-normal bonus pool.
    private static readonly HashSet<string> NonBonusAreaIds = new()
    {
        // Shaper Guardians
        "MapWorldsChimera",
        "MapWorldsHydra",
        "MapWorldsMinotaur",
        "MapWorldsPhoenix",
        // Eagon's memory maps (map items, not atlas icons; live at world (0,0))
        "MapWorldsCourtyardOfWasting",
        "MapWorldsChambersOfImpurity",
        "MapWorldsTheatreOfLies",
    };

    // Synthesis Cortex and Eagon's memory guardians use a distinct Area.Id
    // prefix (Synthesis_MapGuardianN) instead of MapWorlds*, so a prefix
    // check catches the whole family.
    private const string SynthesisAreaPrefix = "Synthesis_";

    public static AtlasTree Read(GameController gc)
    {
        var catalog = gc.Files?.AtlasNodes?.EntriesList;
        if (catalog == null || catalog.Count == 0)
            return AtlasTree.Empty;

        var server = gc.IngameState.Data.ServerData;
        var completedIds = new HashSet<string>();
        var bonusIds = new HashSet<string>();

        if (server?.CompletedNodes != null)
        {
            foreach (var node in server.CompletedNodes)
                if (node?.Area?.Id is { } id) completedIds.Add(id);
        }
        if (server?.BonusCompletedNodes != null)
        {
            foreach (var node in server.BonusCompletedNodes)
                if (node?.Area?.Id is { } id) bonusIds.Add(id);
        }

        var nodes = new List<AtlasMapNode>(catalog.Count);
        foreach (var entry in catalog)
        {
            if (entry == null) continue;

            // Pinnacle boss nodes and Corner voidstone slots have no Area and
            // BaseTier=0. They render as atlas icons but must not be treated as
            // bonus candidates. Emit them anyway with GrantsBonus=false so the
            // overlay's nearest-neighbour lookup returns the correct node
            // (and then skips it) instead of matching the icon to an adjacent
            // regular map.
            var areaId = entry.Area?.Id ?? entry.Id ?? string.Empty;
            if (areaId.Length == 0) continue;

            var connectionIds = new List<string>();
            if (entry.Connections != null)
            {
                foreach (var neighbour in entry.Connections)
                    if (neighbour?.Area?.Id is { } neighbourId)
                        connectionIds.Add(neighbourId);
            }

            var hasArea = entry.Area != null;
            var isUnique = entry.IsUniqueMap;
            // Uniques are indexed at tier=0 in the catalog but still grant a
            // bonus point (up to the 10-cap). Regular maps are tier>0. Nodes
            // without an Area (corner slots, pinnacle boss encounters, league
            // mechanic entrances) never grant a bonus and are skipped here.
            var grantsBonus = hasArea
                              && !NonBonusAreaIds.Contains(areaId)
                              && !areaId.StartsWith(SynthesisAreaPrefix);

            nodes.Add(new AtlasMapNode(
                areaId,
                entry.Area?.Name ?? string.Empty,
                entry.BaseTier,
                new Vector2(entry.PosX, entry.PosY),
                isUnique,
                grantsBonus,
                connectionIds,
                completedIds.Contains(areaId),
                bonusIds.Contains(areaId)));
        }

        return new AtlasTree(nodes);
    }
}
