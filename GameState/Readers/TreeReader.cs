using System.Collections.Generic;
using System.Numerics;
using ExileCore;

namespace AtlasHelper.GameState.Readers;

internal static class TreeReader
{
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

        var nodes = new Dictionary<string, AtlasMapNode>(catalog.Count);
        foreach (var entry in catalog)
        {
            if (entry?.Area?.Id is not { } areaId) continue;
            if (nodes.ContainsKey(areaId)) continue;

            var connectionIds = new List<string>();
            if (entry.Connections != null)
            {
                foreach (var neighbour in entry.Connections)
                    if (neighbour?.Area?.Id is { } neighbourId)
                        connectionIds.Add(neighbourId);
            }

            nodes[areaId] = new AtlasMapNode(
                areaId,
                entry.Area.Name ?? string.Empty,
                entry.BaseTier,
                new Vector2(entry.PosX, entry.PosY),
                entry.IsUniqueMap,
                connectionIds,
                completedIds.Contains(areaId),
                bonusIds.Contains(areaId));
        }

        return new AtlasTree(nodes);
    }
}
