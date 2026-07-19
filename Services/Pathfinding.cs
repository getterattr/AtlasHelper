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

    // Chains BFS through `orderedTargetAreaIds` in the exact order given.
    // Each leg's start is the previous leg's destination; the join node
    // is not duplicated in the returned path.
    //
    // Use when the ordering is domain-required (e.g. the Ceremonial
    // voidstone must be the final leg, Eagon's memory maps must be
    // completed in a specific sequence). If any leg has no path, the
    // whole route returns Empty.
    public static AtlasPath FindOrderedPath(
        AtlasTree tree,
        string startAreaId,
        IReadOnlyList<string> orderedTargetAreaIds)
    {
        if (orderedTargetAreaIds.Count == 0)
            return AtlasPath.Empty;

        var combined = new List<AtlasMapNode>();
        var currentStart = startAreaId;

        for (var i = 0; i < orderedTargetAreaIds.Count; i++)
        {
            var targetId = orderedTargetAreaIds[i];
            var leg = FindPath(tree, currentStart, n => n.AreaId == targetId);
            if (leg.Length == 0)
                return AtlasPath.Empty;

            var skipFirst = combined.Count > 0;
            for (var j = skipFirst ? 1 : 0; j < leg.Nodes.Count; j++)
                combined.Add(leg.Nodes[j]);

            currentStart = targetId;
        }

        return new AtlasPath(combined);
    }

    // Enumerates every ordering of `targetAreaIds` and returns the
    // multi-target route that:
    //   1. hits every target at least once
    //   2. has the fewest total hops
    //   3. tie-broken by the most unbonused (GrantsBonus && !BonusCompleted)
    //      nodes on the route
    //
    // Fits the Phase 1 case: hit both Polaric Void (T11 Exarch) and
    // Seething Chime (T11 Eater) with maximum bonus completion along
    // the shortest route. Caller may append fixed endpoints via
    // FindOrderedPath (e.g. add the voidstone corner as the tail leg).
    //
    // Permutation count is n! - practical up to ~7 targets given the
    // per-permutation cost is (n+1) BFS calls at O(V + E). Phase 1 is
    // n=2 (6 permutation-legs). No memoisation until profiling calls
    // for it.
    public static AtlasPath FindMultiTargetPath(
        AtlasTree tree,
        string startAreaId,
        IReadOnlyList<string> targetAreaIds)
    {
        if (targetAreaIds.Count <= 1)
            return FindOrderedPath(tree, startAreaId, targetAreaIds);

        AtlasPath best = AtlasPath.Empty;
        var bestLength = int.MaxValue;
        var bestBonusCount = -1;

        foreach (var permutation in Permute(targetAreaIds))
        {
            var candidate = FindOrderedPath(tree, startAreaId, permutation);
            if (candidate.Length == 0) continue;
            if (candidate.Length > bestLength) continue;

            var bonusCount = CountBonusOpportunities(candidate);

            if (candidate.Length < bestLength ||
                (candidate.Length == bestLength && bonusCount > bestBonusCount))
            {
                best = candidate;
                bestLength = candidate.Length;
                bestBonusCount = bonusCount;
            }
        }

        return best;
    }

    private static IEnumerable<IReadOnlyList<string>> Permute(IReadOnlyList<string> items)
    {
        if (items.Count <= 1)
        {
            yield return items;
            yield break;
        }

        for (var i = 0; i < items.Count; i++)
        {
            var head = items[i];
            var rest = new List<string>(items.Count - 1);
            for (var j = 0; j < items.Count; j++)
                if (j != i) rest.Add(items[j]);

            foreach (var sub in Permute(rest))
            {
                var perm = new List<string>(items.Count) { head };
                perm.AddRange(sub);
                yield return perm;
            }
        }
    }

    private static int CountBonusOpportunities(AtlasPath path)
    {
        var count = 0;
        foreach (var node in path.Nodes)
            if (node.GrantsBonus && !node.BonusCompleted) count++;
        return count;
    }

    // Multi-source shortest-path search: start from every node matching
    // `isSource` in parallel, return the fewest-hops chain to the first
    // node matching `isDestination`. Path is [source, ..., destination]
    // in walk order.
    //
    // Compound cost = HopMultiplier * hop_count + sum_of_tiers_entered.
    // HopMultiplier (10_000) dominates atlas-scale tier sums (~166 nodes
    // * 16 max tier), so hop count is the primary sort and tier sum is
    // the tiebreaker among equally-short paths. Among two equally-
    // short unlock chains the search prefers the one that walks
    // through lower-tier maps (T11 + T11 wins over T13 + T14).
    //
    // Fits the "shortest unlock chain to an objective" question - pass
    // `n => n.Completed || n.BaseTier == 1` for the source predicate
    // and an objective id for the destination. Any already-completed
    // node becomes a valid jumping-off point, so the algorithm exploits
    // high-tier progress rather than always walking back to T1.
    //
    // Returns AtlasPath.Empty if no source exists in the tree or if no
    // source can reach the destination.
    public static AtlasPath FindPathFromSources(
        AtlasTree tree,
        Func<AtlasMapNode, bool> isSource,
        Func<AtlasMapNode, bool> isDestination)
    {
        const long HopMultiplier = 10_000L;

        var byId = new Dictionary<string, AtlasMapNode>(tree.Nodes.Count);
        foreach (var node in tree.Nodes)
            byId[node.AreaId] = node;

        var cost = new Dictionary<string, long>();
        var previous = new Dictionary<string, AtlasMapNode>();
        var pq = new PriorityQueue<AtlasMapNode, long>();

        foreach (var node in tree.Nodes)
        {
            if (!isSource(node)) continue;
            if (cost.ContainsKey(node.AreaId)) continue;
            cost[node.AreaId] = 0L;
            pq.Enqueue(node, 0L);
        }

        if (pq.Count == 0) return AtlasPath.Empty;

        AtlasMapNode? destination = null;
        while (pq.TryDequeue(out var current, out var currentCost))
        {
            // Stale queue entry from a superseded relaxation.
            if (currentCost > cost[current.AreaId]) continue;

            if (isDestination(current))
            {
                destination = current;
                break;
            }

            foreach (var neighborId in current.ConnectedAreaIds)
            {
                if (!byId.TryGetValue(neighborId, out var neighbor)) continue;

                var newCost = currentCost + HopMultiplier + neighbor.BaseTier;
                if (cost.TryGetValue(neighborId, out var existing) && newCost >= existing) continue;

                cost[neighborId] = newCost;
                previous[neighborId] = current;
                pq.Enqueue(neighbor, newCost);
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
