using ImGuiNET;

namespace AtlasHelper.Ui;

internal static class OverviewPanel
{
    public static void Draw(AtlasHelperSettings settings)
    {
        var enabled = settings.Enable.Value;
        var phase = settings.Progression.PhaseOverride.Value;
        var strategy = settings.Progression.Strategy.Value;
        var hud = settings.Hud.Show.Value;

        if (!ImGui.BeginTable("##AtlasHelperOverviewConfig", 2, Theme.SummaryTableFlags))
            return;

        ImGuiHelpers.SummaryRow("Plugin state", enabled ? "Live" : "Offline", enabled ? Theme.Ok : Theme.Muted);
        ImGuiHelpers.SummaryRow("Phase", phase, Theme.Accent);
        ImGuiHelpers.SummaryRow("Strategy", strategy, Theme.Accent);
        ImGuiHelpers.SummaryRow("HUD overlay", hud ? "Visible" : "Hidden", hud ? Theme.Ok : Theme.Muted);
        ImGui.EndTable();
    }
}
