using ImGuiNET;
using Color = SharpDX.Color;
using ImGuiVector4 = System.Numerics.Vector4;

namespace AtlasHelper.Ui;

internal static class Theme
{
    public static readonly ImGuiVector4 Accent = new(0.95f, 0.74f, 0.26f, 1f);
    public static readonly ImGuiVector4 Ok = new(0.47f, 0.90f, 0.56f, 1f);
    public static readonly ImGuiVector4 Muted = new(0.63f, 0.66f, 0.72f, 1f);

    public const ImGuiTableFlags SummaryTableFlags =
        ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.RowBg;

    public const ImGuiTableFlags ReferenceTableFlags =
        ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.BordersOuter |
        ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.BordersInnerH | ImGuiTableFlags.RowBg;

    public static ImGuiVector4 ToVector4(Color color) =>
        new(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
}
