using System.Collections.Generic;
using System.Numerics;
using AtlasHelper.GameState;
using AtlasHelper.GameState.Atlas;
using AtlasHelper.Services;
using ExileCore.PoEMemory.Elements;
using Color = SharpDX.Color;
using Graphics = ExileCore.Graphics;

namespace AtlasHelper.Ui.Overlays;

// Phase-1 and Phase-2 render surface. Draws the shortest unlock path
// from the current phase's voidstone corner outward to the completed
// frontier (or nearest T1 if nothing is completed yet), routing through
// strategy-required waypoints (both T11 bottom-left maps for Eldritch).
// Rings on path nodes plus lines between consecutive nodes, one color
// for now.
//
// Direction: corner is the start, frontier is the destination. Player
// plays the path in reverse (from their completed frontier walking out
// toward the corner where the voidstone gets socketed). See
// strategy.md#route-planning.
//
// Phase 3 is handled by AtlasOverlay (uncompleted bonus rings). Phase
// 4 render is out of scope for the demo.
internal static class PathOverlay
{
    private const float NodeIconWorldSize = 60f;
    private const float RingThickness = 3f;
    private const float LineThickness = 3f;
    private const int RingSegments = 32;

    // Placeholder single color for the demo. Configurable later if needed.
    private static readonly Color PathColor = new(255, 200, 80, 220);

    public static void Draw(
        Graphics graphics,
        AtlasHelperSettings settings,
        AtlasPanel? atlas,
        AtlasSnapshot snapshot,
        AtlasObjectives objectives)
    {
        if (!settings.AtlasOverlay.Show.Value) return;
        if (atlas == null || !atlas.IsVisible) return;

        var phase = ResolvePhase(settings, snapshot);
        if (phase != PhaseId.One && phase != PhaseId.Two) return;

        var byId = BuildLookup(snapshot.Tree);

        var path = phase == PhaseId.One
            ? ComputeEldritchPath(snapshot.Tree, byId, objectives)
            : ComputeOriginatorPath(snapshot.Tree, byId, objectives);

        if (path.Length == 0) return;

        RenderPath(graphics, atlas, path);
    }

    private static PhaseId ResolvePhase(AtlasHelperSettings settings, AtlasSnapshot snapshot)
    {
        return settings.Progression.PhaseOverride.Value switch
        {
            "Phase 1" => PhaseId.One,
            "Phase 2" => PhaseId.Two,
            "Phase 3" => PhaseId.Three,
            "Phase 4" => PhaseId.Four,
            _ => Phase.From(snapshot).Id,
        };
    }

    private static Dictionary<string, AtlasMapNode> BuildLookup(AtlasTree tree)
    {
        var d = new Dictionary<string, AtlasMapNode>(tree.Nodes.Count);
        foreach (var node in tree.Nodes)
            d[node.AreaId] = node;
        return d;
    }

    private static AtlasPath ComputeEldritchPath(
        AtlasTree tree,
        Dictionary<string, AtlasMapNode> byId,
        AtlasObjectives objectives)
    {
        var cornerId = objectives.EldritchCornerId;
        var waypointA = objectives.EldritchWaypointA;
        var waypointB = objectives.EldritchWaypointB;
        if (string.IsNullOrEmpty(cornerId)) return AtlasPath.Empty;
        if (string.IsNullOrEmpty(waypointA)) return AtlasPath.Empty;
        if (string.IsNullOrEmpty(waypointB)) return AtlasPath.Empty;
        if (!byId.ContainsKey(cornerId)) return AtlasPath.Empty;
        if (!byId.ContainsKey(waypointA)) return AtlasPath.Empty;
        if (!byId.ContainsKey(waypointB)) return AtlasPath.Empty;

        var routeThroughWaypoints = Pathfinding.FindMultiTargetPath(
            tree, cornerId, new[] { waypointA, waypointB });
        if (routeThroughWaypoints.Length == 0) return AtlasPath.Empty;

        var tailStart = routeThroughWaypoints.Destination!.AreaId;
        var tail = Pathfinding.FindPath(tree, tailStart, n => n.Completed || n.BaseTier == 1);

        return Concatenate(routeThroughWaypoints, tail);
    }

    private static AtlasPath ComputeOriginatorPath(
        AtlasTree tree,
        Dictionary<string, AtlasMapNode> byId,
        AtlasObjectives objectives)
    {
        var cornerId = objectives.OriginatorCornerId;
        if (string.IsNullOrEmpty(cornerId)) return AtlasPath.Empty;
        if (!byId.ContainsKey(cornerId)) return AtlasPath.Empty;

        return Pathfinding.FindPath(tree, cornerId, n => n.Completed || n.BaseTier == 1);
    }

    private static AtlasPath Concatenate(AtlasPath head, AtlasPath tail)
    {
        if (tail.Length == 0) return head;
        if (head.Length == 0) return tail;

        var combined = new List<AtlasMapNode>(head.Nodes.Count + tail.Nodes.Count - 1);
        combined.AddRange(head.Nodes);
        for (var i = 1; i < tail.Nodes.Count; i++)
            combined.Add(tail.Nodes[i]);
        return new AtlasPath(combined);
    }

    private static void RenderPath(Graphics graphics, AtlasPanel atlas, AtlasPath path)
    {
        Vector2? previousCenter = null;

        foreach (var node in path.Nodes)
        {
            if (!AtlasProjection.TryGetIconRect(atlas, node.Position, NodeIconWorldSize, out var rect))
            {
                previousCenter = null;
                continue;
            }

            var center = new Vector2(rect.X + rect.Width * 0.5f, rect.Y + rect.Height * 0.5f);
            var radius = (rect.Width < rect.Height ? rect.Width : rect.Height) * 0.5f;

            graphics.DrawCircle(center, radius, PathColor, RingThickness, RingSegments);

            if (previousCenter.HasValue)
                graphics.DrawLine(previousCenter.Value, center, LineThickness, PathColor);

            previousCenter = center;
        }
    }
}
