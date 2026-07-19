using System.Numerics;
using AtlasHelper.GameState;
using ExileCore.PoEMemory.Elements;
using Graphics = ExileCore.Graphics;

namespace AtlasHelper.Ui;

internal static class AtlasOverlay
{
    private const float NodeIconSize = 53f;
    private const float SizeTolerance = 8f;

    public static void Draw(
        Graphics graphics,
        AtlasHelperSettings settings,
        AtlasPanel? atlas,
        AtlasSnapshot state,
        AtlasProjection projection)
    {
        _ = state;
        _ = projection;
        var overlay = settings.AtlasOverlay;
        if (!overlay.Show.Value) return;
        if (atlas == null || !atlas.IsVisible) return;

        var canvas = atlas.GetChildAtIndex(0);
        var canvasChildren = canvas?.Children;
        if (canvasChildren == null || canvasChildren.Count == 0) return;

        var color = overlay.HighlightColor.Value;
        var thickness = (int)overlay.BorderThickness.Value;

        foreach (var element in canvasChildren)
        {
            if (element == null) continue;
            var w = element.Width;
            if (w < NodeIconSize - SizeTolerance || w > NodeIconSize + SizeTolerance) continue;
            var rect = element.GetClientRect();
            if (rect.Width <= 0f || rect.Height <= 0f) continue;
            var center = new Vector2(rect.X + rect.Width * 0.5f, rect.Y + rect.Height * 0.5f);
            var radius = (rect.Width < rect.Height ? rect.Width : rect.Height) * 0.5f;
            graphics.DrawCircle(center, radius, color, thickness, 48);
        }
    }
}
