using System.Numerics;
using ImGuiNET;

namespace AtlasHelper.Ui;

internal static class OverviewPanel
{
    public static void Draw(AtlasHelperSettings settings)
    {
        var enabled = settings.Enable.Value;
        var phase = settings.Progression.PhaseOverride.Value;
        var hud = settings.Hud.Show.Value;
        var atlasOverlay = settings.AtlasOverlay.Show.Value;

        ImGui.Dummy(new Vector2(0, 12));
        ImGui.TextColored(Theme.Accent, "Overview");
        ImGui.Separator();

        if (!ImGui.BeginTable("##AtlasHelperOverviewConfig", 2, Theme.SummaryTableFlags))
            return;

        ImGuiHelpers.SummaryRow("Plugin state", enabled ? "Live" : "Offline", enabled ? Theme.Ok : Theme.Muted);
        ImGuiHelpers.SummaryRow("Phase", phase, Theme.Accent);
        ImGuiHelpers.SummaryRow("HUD overlay", hud ? "Visible" : "Hidden", hud ? Theme.Ok : Theme.Muted);
        ImGuiHelpers.SummaryRow("Atlas overlay", atlasOverlay ? "Visible" : "Hidden", atlasOverlay ? Theme.Ok : Theme.Muted);
        ImGui.EndTable();
    }

    public static void DrawConfigurationHeader()
    {
        ImGui.Dummy(new Vector2(0, 12));
        ImGui.TextColored(Theme.Accent, "Configuration");
        ImGui.Separator();
    }
}
