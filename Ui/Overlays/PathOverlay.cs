using System.Collections.Generic;
using System.Numerics;
using AtlasHelper.GameState;
using AtlasHelper.GameState.Atlas;
using AtlasHelper.Services;
using ExileCore.PoEMemory.Elements;
using ExileCore.Shared.Enums;
using Color = SharpDX.Color;
using Graphics = ExileCore.Graphics;

namespace AtlasHelper.Ui.Overlays;

// Phase-1 and Phase-2 render surface. For every objective the phase
// requires (Eldritch: corner + Black Star + Infinite Hunger; Originator:
// corner alone) the plugin independently BFS-searches from every already-
// completed node in parallel to the shortest chain of incomplete maps
// that reaches that objective. The union of those chains is what gets
// rendered - a Steiner-tree-shaped highlight that lets already-run high-
// tier maps act as jumping-off points instead of forcing a walk back to
// T1. See strategy.md#route-planning.
//
// Phase 3 is handled by AtlasOverlay (uncompleted bonus rings). Phase
// 4 has no path overlay: by Phase 3's end the atlas is fully unlocked,
// so Decayed and Ceremonial acquisition is Guardian farming + pinnacle
// fights with no atlas-routing question left to answer.
internal static class PathOverlay
{
    // World-unit size for the ring around a path node. Atlas node icons
    // render at ~22 world units per bridge inspection; 14 puts the ring
    // just inside the icon boundary so the label centered on the node
    // stays legible without the ring competing for visual space.
    private const float NodeRingWorldSize = 14f;
    private const float RingThickness = 2f;
    private const float LineThickness = 2f;
    private const int RingSegments = 32;

    // Path color used for rings, connecting lines, and the label text.
    private static readonly Color PathColor = new(255, 200, 80, 235);

    // Label background - near-opaque dark warm tone so the gold text
    // reads cleanly over any atlas backdrop.
    private static readonly Color LabelBackground = new(20, 15, 10, 220);

    // Slightly warmer off-white for the label text; higher contrast
    // than the ring color while keeping the gold-plated feel.
    private static readonly Color LabelText = new(255, 240, 210, 255);

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

        var paths = phase == PhaseId.One
            ? ComputeEldritchPaths(snapshot.Tree, byId, objectives)
            : ComputeOriginatorPaths(snapshot.Tree, byId, objectives);

        if (paths.Count == 0) return;

        RenderPaths(graphics, atlas, paths, snapshot);
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

    private static List<AtlasPath> ComputeEldritchPaths(
        AtlasTree tree,
        Dictionary<string, AtlasMapNode> byId,
        AtlasObjectives objectives)
    {
        var targets = new List<string?>
        {
            objectives.EldritchCornerId,
            objectives.EldritchWaypointA,
            objectives.EldritchWaypointB,
        };
        return PathsToTargets(tree, byId, targets);
    }

    private static List<AtlasPath> ComputeOriginatorPaths(
        AtlasTree tree,
        Dictionary<string, AtlasMapNode> byId,
        AtlasObjectives objectives)
    {
        return PathsToTargets(tree, byId, new List<string?> { objectives.OriginatorCornerId });
    }

    // For each objective id, BFS from every already-completed node (plus
    // T1s as a fresh-atlas fallback) to that objective, returning the
    // shortest chain of incomplete maps to reach it. Empty results are
    // dropped. Each returned path is [nearest_source, ..., objective] -
    // the source is either a completed node or an initial T1, either
    // way it renders as a de-emphasised jumping-off point after the
    // completed-node filter runs.
    private static List<AtlasPath> PathsToTargets(
        AtlasTree tree,
        Dictionary<string, AtlasMapNode> byId,
        IReadOnlyList<string?> targetIds)
    {
        var result = new List<AtlasPath>(targetIds.Count);
        foreach (var raw in targetIds)
        {
            if (string.IsNullOrEmpty(raw)) continue;
            if (!byId.ContainsKey(raw)) continue;

            var target = raw;
            var path = Pathfinding.FindPathFromSources(
                tree,
                isSource: n => n.Completed || n.BaseTier == 1,
                isDestination: n => n.AreaId == target);

            if (path.Length > 0) result.Add(path);
        }
        return result;
    }

    private static void RenderPaths(
        Graphics graphics,
        AtlasPanel atlas,
        List<AtlasPath> paths,
        AtlasSnapshot snapshot)
    {
        // Union bookkeeping - a node that survives filtering on ONE
        // path (rings, labels) should not be re-drawn if another path
        // also passes through it. Line segments dedupe by endpoint pair.
        var nodeCenters = new Dictionary<string, (Vector2 Center, float Radius, AtlasMapNode Node)>();
        var drawnEdges = new HashSet<(string, string)>();

        // First pass: project each path independently and stage its
        // incomplete points in walk order. Completed maps and defeated
        // pinnacle icons drop out here so lines below hop over their
        // positions.
        var perPathPoints = new List<List<(Vector2 Center, float Radius, AtlasMapNode Node)>>(paths.Count);
        foreach (var path in paths)
        {
            var points = new List<(Vector2, float, AtlasMapNode)>(path.Nodes.Count);
            foreach (var node in path.Nodes)
            {
                if (IsNodeDone(node, snapshot)) continue;
                if (!AtlasProjection.TryGetIconRect(atlas, node.Position, NodeRingWorldSize, out var rect))
                    continue;

                var center = new Vector2(rect.X + rect.Width * 0.5f, rect.Y + rect.Height * 0.5f);
                var radius = (rect.Width < rect.Height ? rect.Width : rect.Height) * 0.5f;
                points.Add((center, radius, node));

                nodeCenters[node.AreaId] = (center, radius, node);
            }
            perPathPoints.Add(points);
        }

        // Lines edge-to-edge, deduped across paths.
        foreach (var points in perPathPoints)
        {
            for (var i = 1; i < points.Count; i++)
            {
                var (aCenter, aRadius, aNode) = points[i - 1];
                var (bCenter, bRadius, bNode) = points[i];

                var key = string.CompareOrdinal(aNode.AreaId, bNode.AreaId) < 0
                    ? (aNode.AreaId, bNode.AreaId)
                    : (bNode.AreaId, aNode.AreaId);
                if (!drawnEdges.Add(key)) continue;

                var delta = bCenter - aCenter;
                var dist = delta.Length();
                if (dist < 1f) continue;
                var direction = delta / dist;
                var lineStart = aCenter + direction * aRadius;
                var lineEnd = bCenter - direction * bRadius;
                graphics.DrawLine(lineStart, lineEnd, LineThickness, PathColor);
            }
        }

        // Rings + labels per unique node.
        foreach (var (center, radius, node) in nodeCenters.Values)
        {
            graphics.DrawCircle(center, radius, PathColor, RingThickness, RingSegments);

            var text = LabelFor(node);
            if (text.Length == 0) continue;

            var size = graphics.MeasureText(text);
            var pos = new Vector2(
                center.X - size.X * 0.5f,
                center.Y - size.Y * 0.5f);
            graphics.DrawTextWithBackground(text, pos, LabelText, FontAlign.Left, LabelBackground);
        }
    }

    // Bridge between AtlasMapNode.Completed (which comes from
    // ServerData.CompletedNodes) and quest-flag driven completion for
    // pinnacle boss atlas icons, whose defeats live in QuestFlags
    // (surfaced through snapshot state) rather than the completion
    // node list.
    private static bool IsNodeDone(AtlasMapNode node, AtlasSnapshot snapshot)
    {
        if (node.Completed) return true;

        if (node.Kind != AtlasNodeKind.PinnacleBoss) return false;

        return node.AreaId switch
        {
            "BlackStarBoss" => snapshot.Eldritch.Exarch.BlackStarDefeated == true,
            "InfiniteHungerBoss" => snapshot.Eldritch.Eater.InfiniteHungerDefeated == true,
            "SearingExarchBoss" => snapshot.PinnacleBosses.SearingExarch == true,
            "EaterOfWorldsBoss" => snapshot.PinnacleBosses.EaterOfWorlds == true,
            "ShaperBoss" => snapshot.PinnacleBosses.Shaper == true,
            "ElderBoss" => snapshot.PinnacleBosses.Elder == true,
            "MavenBoss" => snapshot.PinnacleBosses.Maven == true,
            "SirusBoss" => snapshot.PinnacleBosses.Sirus == true,
            // Threads-of-the-Originator Incarnations. Atlas-icon mapping:
            //   IgnoranceBoss   = Incarnation of Dread   (final; Pinnacles)
            //   BenevolenceBoss = Incarnation of Neglect (Originator chain)
            //   FearBoss        = Incarnation of Fear    (Originator chain)
            "IgnoranceBoss" => snapshot.PinnacleBosses.IncarnationOfDread == true,
            "BenevolenceBoss" => snapshot.Originator.IncarnationOfNeglectDefeated == true,
            "FearBoss" => snapshot.Originator.IncarnationOfFearDefeated == true,
            _ => false,
        };
    }

    private static string LabelFor(AtlasMapNode node) => node.Kind switch
    {
        AtlasNodeKind.NormalMap => $"{node.AreaName} ({node.BaseTier})",
        AtlasNodeKind.UniqueMap => node.AreaName,
        AtlasNodeKind.MemoryMap => node.AreaName,
        AtlasNodeKind.PinnacleBoss => PinnacleLabel(node.AreaId),
        AtlasNodeKind.VoidstoneSlot => VoidstoneLabel(node.AreaId),
        _ => node.AreaName.Length > 0 ? node.AreaName : node.AreaId,
    };

    // Wiki-facing name for the pinnacle boss icons. Handles the two
    // families where the internal id does not match the player-facing
    // name (Threads-of-the-Originator Incarnations - Ignorance is
    // Dread, Benevolence is Neglect, Fear is Fear). Everything else
    // falls through to the generic Prettify pattern.
    private static string PinnacleLabel(string id) => id switch
    {
        "IgnoranceBoss" => "Incarnation of Dread",
        "BenevolenceBoss" => "Incarnation of Neglect",
        "FearBoss" => "Incarnation of Fear",
        _ => Prettify(id),
    };

    // Strip trailing "Boss" and split PascalCase for readability.
    private static string Prettify(string id)
    {
        if (id.EndsWith("Boss")) id = id.Substring(0, id.Length - 4);
        var sb = new System.Text.StringBuilder(id.Length + 4);
        for (var i = 0; i < id.Length; i++)
        {
            if (i > 0 && char.IsUpper(id[i]) && !char.IsUpper(id[i - 1]))
                sb.Append(' ');
            sb.Append(id[i]);
        }
        return sb.ToString();
    }

    private static string VoidstoneLabel(string id) => id switch
    {
        "TangledWatchstoneSlotNode" => "Eldritch",
        "CleansingFireWatchstoneSlotNode" => "Originator",
        "ElderWatchstoneSlotNode" => "Decayed",
        "MavenWatchstoneSlotNode" => "Ceremonial",
        _ => "Voidstone",
    };
}
