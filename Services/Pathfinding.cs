using System;
using System.Collections.Generic;
using AtlasHelper.GameState.Atlas;

namespace AtlasHelper.Services;

// Pathfinding across the atlas graph. Consumers pass a predicate that
// identifies the destination node; the search returns the shortest hop
// sequence from `startAreaId` to the first matching node.
//
// The predicate shape lets one algorithm serve every atlas pathing
// question the roadmap turns up:
//
//   nearest T1 for Phase 1 unlock planning:
//       n => n.BaseTier == 1
//
//   specific target for a chain step (Polaric Void, Seething Chime):
//       n => n.AreaId == "MapWorldsPolaricVoid"
//
//   nearest unfinished T1 during Phase 3 tier-boost sweep:
//       n => n.BaseTier == 1 && !n.BonusCompleted
//
//   nearest bonus-eligible unique map:
//       n => n.IsUnique && !n.Completed
//
// The algorithm itself never changes - only what "destination" means.

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
    // BFS from `startAreaId` to the first node matching `isDestination`.
    // Returns the path in start-to-destination order: [start, ..., dest].
    // Consumers that want play order (destination first, then walking
    // back toward the start position) reverse the returned list.
    //
    // If `startAreaId` is not in the tree, or no node satisfies the
    // predicate, returns AtlasPath.Empty.
    public static AtlasPath FindPath(
        AtlasTree tree,
        string startAreaId,
        Func<AtlasMapNode, bool> isDestination)
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

            if (isDestination(current))
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
