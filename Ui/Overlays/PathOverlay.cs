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

// Phase-1 and Phase-2 render surface. Draws the shortest unlock path
// from the current phase's voidstone corner outward to the completed
// frontier (or nearest T1 if nothing is completed yet), routing through
// strategy-required waypoints (both boss-arena entry icons for
// Eldritch). Rings on path nodes plus lines between consecutive nodes,
// plus a small name+tier label per node.
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

        var path = phase == PhaseId.One
            ? ComputeEldritchPath(snapshot.Tree, byId, objectives)
            : ComputeOriginatorPath(snapshot.Tree, byId, objectives);

        if (path.Length == 0) return;

        RenderPath(graphics, atlas, path, snapshot);
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

    private static void RenderPath(Graphics graphics, AtlasPanel atlas, AtlasPath path, AtlasSnapshot snapshot)
    {
        // First pass: project every INCOMPLETE node on the path.
        // Completed nodes are the frontier (or already-cleared stops
        // en route) - the player has run them, so highlighting is
        // misleading noise. Lines still visually skip over their
        // positions by connecting the surviving incomplete nodes
        // directly.
        //
        // Pinnacle boss icons (Black Star, Infinite Hunger, Searing
        // Exarch, ...) are not tracked in ServerData.CompletedNodes -
        // their completion lives in QuestFlags. IsPinnacleDone reads
        // the matching snapshot state (Eldritch chain, PinnacleBosses)
        // and lets us skip those icons too.
        var points = new List<(Vector2 Center, float Radius, AtlasMapNode Node)>(path.Nodes.Count);
        foreach (var node in path.Nodes)
        {
            if (IsNodeDone(node, snapshot)) continue;
            if (!AtlasProjection.TryGetIconRect(atlas, node.Position, NodeRingWorldSize, out var rect))
                continue;

            var center = new Vector2(rect.X + rect.Width * 0.5f, rect.Y + rect.Height * 0.5f);
            var radius = (rect.Width < rect.Height ? rect.Width : rect.Height) * 0.5f;
            points.Add((center, radius, node));
        }

        if (points.Count == 0) return;

        // Second pass: draw lines edge-to-edge (from ring perimeter to
        // ring perimeter) so they visually connect the circles instead
        // of passing through them.
        for (var i = 1; i < points.Count; i++)
        {
            var (aCenter, aRadius, _) = points[i - 1];
            var (bCenter, bRadius, _) = points[i];
            var delta = bCenter - aCenter;
            var dist = delta.Length();
            if (dist < 1f) continue;
            var direction = delta / dist;
            var lineStart = aCenter + direction * aRadius;
            var lineEnd = bCenter - direction * bRadius;
            graphics.DrawLine(lineStart, lineEnd, LineThickness, PathColor);
        }

        // Third pass: rings under labels.
        foreach (var (center, radius, _) in points)
            graphics.DrawCircle(center, radius, PathColor, RingThickness, RingSegments);

        // Fourth pass: labels centered on each node, with background
        // panel drawn behind them for legibility over the atlas art.
        foreach (var (center, _, node) in points)
        {
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
