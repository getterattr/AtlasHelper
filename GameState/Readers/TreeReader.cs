using System.Collections.Generic;
using System.Numerics;
using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;

namespace AtlasHelper.GameState.Readers;

internal static class TreeReader
{
    public static AtlasTree Read(GameController gc)
    {
        var completed = gc.IngameState.Data.ServerData.CompletedNodes;
        var bonusCompleted = gc.IngameState.Data.ServerData.BonusCompletedNodes;
        if (completed == null)
            return AtlasTree.Empty;

        var bonusIds = new HashSet<string>();
        if (bonusCompleted != null)
        {
            foreach (var node in bonusCompleted)
                if (node.Area?.Id is { } id) bonusIds.Add(id);
        }

        var nodes = new Dictionary<string, AtlasMapNode>();
        foreach (var node in completed)
            AddNode(node, nodes, bonusIds);

        return new AtlasTree(nodes);
    }

    private static void AddNode(AtlasNode node, Dictionary<string, AtlasMapNode> sink, HashSet<string> bonusIds)
    {
        if (node?.Area?.Id is not { } areaId || sink.ContainsKey(areaId))
            return;

        var connectionIds = new List<string>();
        if (node.Connections != null)
        {
            foreach (var neighbour in node.Connections)
                if (neighbour?.Area?.Id is { } neighbourId)
                    connectionIds.Add(neighbourId);
        }

        sink[areaId] = new AtlasMapNode(
            areaId,
            node.Area.Name ?? string.Empty,
            node.BaseTier,
            new Vector2(node.PosX, node.PosY),
            node.IsUniqueMap,
            connectionIds,
            bonusIds.Contains(areaId));
    }
}
