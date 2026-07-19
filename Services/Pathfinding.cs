using System.Collections.Generic;
using AtlasHelper.GameState.Atlas;

namespace AtlasHelper.Services;

// Pathfinding across the atlas graph. Consumers use this to identify
// the shortest sequence of connected maps between two positions - e.g.
// path from a T11 objective node (Polaric Void for the Exarch chain,
// Seething Chime for the Eater chain) back to the nearest already-
// unlocked T1 to determine the play order for Phase 1.
//
// Currently exposes one predicate (nearest T1). More predicates land
// as workstream 3 arrives - nearest unfinished, nearest with rarity
// satisfied, path to arbitrary target, etc.

public sealed record AtlasPath(IReadOnlyList<AtlasMapNode> Nodes)
{
    public int Length => Nodes.Count;

    public AtlasMapNode? Start => Nodes.Count > 0 ? Nodes[0] : null;

    public AtlasMapNode? Destination =>
        Nodes.Count > 0 ? Nodes[Nodes.Count - 1] : null;

    public static AtlasPath Empty { get; } = new(new List<AtlasMapNode>());
}

public static class Pathfinding
{
    // BFS from `startAreaId` to the nearest node with BaseTier == 1.
    // Returns the path in start-to-destination order: [start, ..., T1].
    // Consumers that want play order (T1 first, then walking up to the
    // target) reverse the returned list.
    public static AtlasPath FindPathToNearestTierOne(AtlasTree tree, string startAreaId)
    {
        var byId = new Dictionary<string, AtlasMapNode>(tree.Nodes.Count);
        foreach (var node in tree.Nodes)
            byId[node.AreaId] = node;

        if (!byId.TryGetValue(startAreaId, out var start))
            return AtlasPath.Empty;

        var queue = new Queue<AtlasMapNode>();
        var visited = new HashSet<string> { start.AreaId };
        var previous = new Dictionary<string, AtlasMapNode>();

        queue.Enqueue(start);

        AtlasMapNode? destination = null;
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current.BaseTier == 1)
            {
                destination = current;
                break;
            }

            foreach (var neighborId in current.ConnectedAreaIds)
            {
                if (!byId.TryGetValue(neighborId, out var neighbor)) continue;
                if (!visited.Add(neighborId)) continue;

                previous[neighborId] = current;
                queue.Enqueue(neighbor);
            }
        }

        if (destination == null)
            return AtlasPath.Empty;

        var path = new List<AtlasMapNode>();
        var cursor = destination;
        while (cursor != null)
        {
            path.Add(cursor);
            if (!previous.TryGetValue(cursor.AreaId, out var prev)) break;
            cursor = prev;
        }
        path.Reverse();
        return new AtlasPath(path);
    }
}
