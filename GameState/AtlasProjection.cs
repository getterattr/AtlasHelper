using System.Collections.Generic;
using System.Numerics;
using ExileCore.PoEMemory.Elements;
using RectangleF = SharpDX.RectangleF;
using Vector2 = System.Numerics.Vector2;

namespace AtlasHelper.GameState;

/// Projects atlas map-tree nodes from world coordinates to screen coordinates
/// so an overlay can highlight arbitrary nodes without walking the panel's
/// Element tree every frame.
///
/// The panel exposes:
///   - Atlas.Children[0]           the canvas container
///   - Atlas.Children[0].X, Y      screen origin (accounts for pan)
///   - Atlas.Children[0].Scale     live zoom factor
///   - Atlas.Children[0].Children  ~175 per-node Elements at canvas-local coords
///
/// On first-open we solve a linear fit  local = k * world + offset  from the
/// visible Element positions and the catalog's AtlasNode.PosX/PosY. Per-frame
/// draw reads only the three canvas fields plus the cached local coords.
public sealed class AtlasProjection
{
    private const float NodeIconSize = 53f;
    private const int MinCorrespondences = 6;

    private float _k;
    private Vector2 _offset;
    private readonly Dictionary<string, Vector2> _localByAreaId = new();
    private bool _calibrated;

    public bool IsCalibrated => _calibrated;

    /// Solves K, Cx, Cy from the catalog spans and the canvas texture dimensions.
    /// The atlas canvas is designed so its full texture (canvas.Width × canvas.Height)
    /// encompasses the entire atlas world, so K = canvas.Width / worldSpan.
    /// Idempotent: safe to call every frame; only fits on the first successful pass.
    public void CalibrateIfNeeded(AtlasPanel? atlas, AtlasTree catalog)
    {
        if (_calibrated || atlas == null || !atlas.IsVisible) return;

        var canvas = atlas.GetChildAtIndex(0);
        if (canvas == null || canvas.Width <= 0f || canvas.Height <= 0f) return;
        if (catalog.NodesByAreaId.Count < MinCorrespondences) return;

        var worldMin = new Vector2(float.MaxValue, float.MaxValue);
        var worldMax = new Vector2(float.MinValue, float.MinValue);
        foreach (var node in catalog.NodesByAreaId.Values)
        {
            if (node.Position.X < worldMin.X) worldMin.X = node.Position.X;
            if (node.Position.Y < worldMin.Y) worldMin.Y = node.Position.Y;
            if (node.Position.X > worldMax.X) worldMax.X = node.Position.X;
            if (node.Position.Y > worldMax.Y) worldMax.Y = node.Position.Y;
        }

        var worldSpanX = worldMax.X - worldMin.X;
        var worldSpanY = worldMax.Y - worldMin.Y;
        if (worldSpanX <= 0f || worldSpanY <= 0f) return;

        // Both axes share the same K because the atlas is drawn without axis-independent scaling.
        // Use whichever axis exercises more of the canvas as the more reliable estimator.
        var kx = canvas.Width / worldSpanX;
        var ky = canvas.Height / worldSpanY;
        _k = kx < ky ? kx : ky;

        // Anchor world (min) at canvas-local (0, 0), matching the origin marker
        // observed at Atlas.Children[0].Children[0] which sits at local (0, 0).
        _offset = new Vector2(-worldMin.X * _k, -worldMin.Y * _k);

        _localByAreaId.Clear();
        foreach (var (areaId, node) in catalog.NodesByAreaId)
            _localByAreaId[areaId] = node.Position * _k + _offset;

        _calibrated = true;
    }

    /// Projects a catalog node onto the atlas canvas at its current pan/zoom.
    /// Returns false when the panel is closed or the projection has not been calibrated.
    public bool TryGetScreenRect(AtlasPanel? atlas, string areaId, out RectangleF rect)
    {
        rect = default;
        if (!_calibrated || atlas == null || !atlas.IsVisible) return false;
        if (!_localByAreaId.TryGetValue(areaId, out var local)) return false;

        var canvas = atlas.GetChildAtIndex(0);
        if (canvas == null || canvas.Width <= 0f) return false;

        // canvas.GetClientRect() returns the on-screen rect after the panel's
        // pan and zoom transforms are applied. Its width divided by canvas.Width
        // (the untransformed texture dimension) gives the effective zoom factor
        // for converting canvas-local coordinates into on-screen pixels.
        var canvasRect = canvas.GetClientRect();
        var visualScale = canvasRect.Width / canvas.Width;

        // element.X/Y in memory is the top-left of the node icon in canvas space,
        // matching how ExileApi exposes raw child rects. Apply visualScale and
        // translate by the canvas's on-screen top-left.
        var screenX = canvasRect.X + local.X * visualScale;
        var screenY = canvasRect.Y + local.Y * visualScale;
        var iconOnScreen = NodeIconSize * visualScale;
        rect = new RectangleF(screenX, screenY, iconOnScreen, iconOnScreen);
        return true;
    }

    public void Invalidate()
    {
        _calibrated = false;
        _localByAreaId.Clear();
    }
}

