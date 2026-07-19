using System.Numerics;
using ExileCore.PoEMemory.Elements;
using RectangleF = SharpDX.RectangleF;
using Vector2 = System.Numerics.Vector2;

namespace AtlasHelper.GameState;

/// Projects an atlas node's world coordinates (AtlasNode.PosX/PosY, kept in
/// AtlasMapNode.Position) to on-screen pixels, tracking the atlas panel's pan
/// and zoom.
///
/// The atlas world coord system is anchored at (0, 0) and spans `WorldSpan`
/// units per axis. The panel's inner canvas is Atlas.Children[0]; its
/// GetClientRect() gives the on-screen top-left plus current-zoom width, so a
/// single ratio (screenWidth / WorldSpan) converts world units to pixels.
///
/// Used for the "off-viewport" projection cases (e.g. drawing a target-Corner
/// arrow when the destination node isn't currently rendered). Visible-node
/// highlighting bypasses this by walking Atlas.Children[0].Children directly
/// and calling GetClientRect() per Element.
public static class AtlasProjection
{
    public const float WorldSpan = 1000f;

    public static bool TryProject(AtlasPanel? atlas, Vector2 worldPos, out Vector2 screen)
    {
        screen = default;
        if (atlas == null || !atlas.IsVisible) return false;

        var canvas = atlas.GetChildAtIndex(0);
        if (canvas == null) return false;

        var canvasRect = canvas.GetClientRect();
        if (canvasRect.Width <= 0f) return false;

        var scale = canvasRect.Width / WorldSpan;
        screen = new Vector2(canvasRect.X, canvasRect.Y) + worldPos * scale;
        return true;
    }

    public static bool TryGetIconRect(AtlasPanel? atlas, Vector2 worldPos, float iconWorldSize, out RectangleF rect)
    {
        rect = default;
        if (!TryProject(atlas, worldPos, out var center)) return false;

        var canvas = atlas!.GetChildAtIndex(0);
        var canvasRect = canvas!.GetClientRect();
        var scale = canvasRect.Width / WorldSpan;
        var half = iconWorldSize * scale * 0.5f;
        rect = new RectangleF(center.X - half, center.Y - half, iconWorldSize * scale, iconWorldSize * scale);
        return true;
    }
}
