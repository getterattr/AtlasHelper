using ImGuiNET;
using System.Numerics;

namespace AtlasHelper.Ui;

internal static class HudOverlay
{
    private const string WindowId = "AtlasHelper##AtlasHelperHud";
    private const float Padding = 8f;

    private const ImGuiWindowFlags WindowFlags =
        ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize;

    public static void Draw(AtlasHelperSettings settings)
    {
        var hud = settings.Hud;

        ImGui.SetNextWindowPos(new Vector2(20, 120), ImGuiCond.FirstUseEver);
        ImGui.PushStyleColor(ImGuiCol.WindowBg, Theme.ToVector4(hud.BackgroundColor.Value));
        ImGui.PushStyleColor(ImGuiCol.Text, Theme.ToVector4(hud.TextColor.Value));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(Padding, Padding));

        if (!ImGui.Begin(WindowId, WindowFlags))
        {
            ImGui.End();
            PopStyles();
            return;
        }

        ImGui.SetWindowFontScale(hud.TextScale.Value);
        DrawBody(settings);
        ImGui.SetWindowFontScale(1f);
        ImGui.End();

        PopStyles();
    }

    private static void DrawBody(AtlasHelperSettings settings)
    {
        ImGui.Text($"Phase:       {settings.Progression.PhaseOverride.Value}");
        ImGui.Text($"Strategy:    {settings.Progression.Strategy.Value}");
        ImGui.Separator();

        ImGui.Text("Voidstones:  0 / 4");
        ImGui.Text("Completion:  0 / 117");
        ImGui.Text("Maven:       0 / 10");
        ImGui.Separator();

        ImGui.Text("Exarch chain:  pending");
        ImGui.Text("Eater chain:   pending");
    }

    private static void PopStyles()
    {
        ImGui.PopStyleVar(2);
        ImGui.PopStyleColor(2);
    }
}
