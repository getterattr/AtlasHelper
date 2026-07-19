using System.Numerics;
using AtlasHelper.GameState;
using AtlasHelper.GameState.Atlas;
using AtlasHelper.Services;
using ExileCore.PoEMemory.Elements;
using Graphics = ExileCore.Graphics;

namespace AtlasHelper.Ui.Overlays;

// Phase-3 render surface. Highlights every uncompleted bonus-eligible
// node so the player can sweep them. Phase 1 and Phase 2 paths are
// drawn by PathOverlay instead.
internal static class AtlasOverlay
{
    private const float NodeIconSize = 53f;
    private const float SizeTolerance = 8f;
    private const int CircleSegments = 48;

    // Match tolerance in world coordinates. Nodes are ~5+ units apart, so 3
    // is comfortably below the nearest-neighbour distance.
    private const float MatchToleranceSq = 9f;

    public static void Draw(
        Graphics graphics,
        AtlasHelperSettings settings,
        AtlasPanel? atlas,
        AtlasSnapshot state)
    {
        var overlay = settings.AtlasOverlay;
        if (!overlay.Show.Value) return;
        if (atlas == null || !atlas.IsVisible) return;

        if (ResolvePhase(settings, state) != PhaseId.Three) return;

        var canvas = atlas.GetChildAtIndex(0);
        var canvasChildren = canvas?.Children;
        if (canvasChildren == null || canvasChildren.Count == 0) return;
        if (canvas!.Width <= 0f) return;

        // canvas.Width is the untransformed texture dimension. Dividing by the
        // world-coord span gives the ratio between element canvas-local units
        // and AtlasNode world units.
        var localPerWorld = canvas.Width / AtlasProjection.WorldSpan;
        if (localPerWorld <= 0f) return;

        var halfIcon = NodeIconSize * 0.5f;
        var includeUniques = overlay.IncludeUniques.Value;
        var hideUniquesPastCap = overlay.HideUniquesPastCap.Value;
        var uniqueCapHit = state.Completion.UniqueBonusComplete;
        var color = overlay.HighlightColor.Value;
        var thickness = (int)overlay.BorderThickness.Value;

        foreach (var element in canvasChildren)
        {
            if (element == null) continue;
            var w = element.Width;
            if (w < NodeIconSize - SizeTolerance || w > NodeIconSize + SizeTolerance) continue;

            // Element.X/Y is the icon's top-left in canvas-local space; add
            // half the icon size to get the icon centre, then divide by the
            // scale to recover world coords for catalog lookup.
            var worldX = (element.X + halfIcon) / localPerWorld;
            var worldY = (element.Y + halfIcon) / localPerWorld;

            var node = FindNearest(state.Tree, worldX, worldY);
            if (node is null) continue;

            // Guardians, Synthesis Cortex and memory areas render on the atlas
            // but are outside the bonus-completion pool. Skip regardless of
            // any completion flags.
            if (!node.GrantsBonus) continue;

            if (node.IsUnique)
            {
                // Uniques never appear in BonusCompletedNodes; use the Completed
                // flag (sourced from CompletedNodes) as the equivalent signal.
                if (!includeUniques) continue;
                if (node.Completed) continue;
                if (hideUniquesPastCap && uniqueCapHit) continue;
            }
            else if (node.BonusCompleted)
            {
                continue;
            }

            var rect = element.GetClientRect();
            if (rect.Width <= 0f || rect.Height <= 0f) continue;

            var centre = new Vector2(rect.X + rect.Width * 0.5f, rect.Y + rect.Height * 0.5f);
            var radius = (rect.Width < rect.Height ? rect.Width : rect.Height) * 0.5f;
            graphics.DrawCircle(centre, radius, color, thickness, CircleSegments);
        }
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

    private static AtlasMapNode? FindNearest(AtlasTree tree, float worldX, float worldY)
    {
        AtlasMapNode? best = null;
        var bestDistSq = MatchToleranceSq;
        foreach (var node in tree.Nodes)
        {
            var dx = node.Position.X - worldX;
            var dy = node.Position.Y - worldY;
            var distSq = dx * dx + dy * dy;
            if (distSq < bestDistSq)
            {
                bestDistSq = distSq;
                best = node;
            }
        }
        return best;
    }
}
