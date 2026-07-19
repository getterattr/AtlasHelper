using System.Collections.Generic;
using System.Numerics;
using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;

namespace AtlasHelper.GameState.Readers;

internal static class TreeReader
{
    public static AtlasTree Read(GameController gc)
    {
        var server = gc.IngameState.Data.ServerData;
        var completed = server.CompletedNodes;
        var bonusCompleted = server.BonusCompletedNodes;
        if (completed == null || completed.Count == 0)
            return AtlasTree.Empty;

        var completedIds = new HashSet<string>();
        foreach (var node in completed)
            if (node?.Area?.Id is { } id) completedIds.Add(id);

        var bonusIds = new HashSet<string>();
        if (bonusCompleted != null)
        {
            foreach (var node in bonusCompleted)
                if (node?.Area?.Id is { } id) bonusIds.Add(id);
        }

        var nodes = new Dictionary<string, AtlasMapNode>();
        var queue = new Queue<AtlasNode>();
        foreach (var node in completed)
            queue.Enqueue(node);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current?.Area?.Id is not { } areaId || nodes.ContainsKey(areaId))
                continue;

            var connectionIds = new List<string>();
            if (current.Connections != null)
            {
                foreach (var neighbour in current.Connections)
                {
                    if (neighbour?.Area?.Id is not { } neighbourId) continue;
                    connectionIds.Add(neighbourId);
                    if (!nodes.ContainsKey(neighbourId))
                        queue.Enqueue(neighbour);
                }
            }

            nodes[areaId] = new AtlasMapNode(
                areaId,
                current.Area.Name ?? string.Empty,
                current.BaseTier,
                new Vector2(current.PosX, current.PosY),
                current.IsUniqueMap,
                connectionIds,
                completedIds.Contains(areaId),
                bonusIds.Contains(areaId));
        }

        return new AtlasTree(nodes);
    }
}
