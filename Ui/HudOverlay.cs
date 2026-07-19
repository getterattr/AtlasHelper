using AtlasHelper.GameState;
using ImGuiNET;
using System.Numerics;

namespace AtlasHelper.Ui;

internal static class HudOverlay
{
    private const string WindowId = "AtlasHelper##AtlasHelperHud";
    private const float Padding = 8f;

    private const ImGuiWindowFlags WindowFlags =
        ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize;

    public static void Draw(AtlasHelperSettings settings, AtlasSnapshot state)
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
        DrawBody(settings, state);
        ImGui.SetWindowFontScale(1f);
        ImGui.End();

        PopStyles();
    }

    private static void DrawBody(AtlasHelperSettings settings, AtlasSnapshot state)
    {
        ImGui.Text($"Phase:       {settings.Progression.PhaseOverride.Value}");
        ImGui.Separator();

        ImGui.Text($"Voidstones:  {state.Voidstones.SocketedCount} / 4");
        ImGui.Text($"Normal maps: {state.Completion.NormalBonusCount} / {AtlasCompletion.NormalBonusTarget}");
        ImGui.Text($"Unique maps: {state.Completion.UniqueBonusCount} / {AtlasCompletion.UniqueBonusTarget}");
        ImGui.Text($"Maven:       {state.Maven.WitnessCount} / {MavenState.InvitationTarget}");
    }

    private static void PopStyles()
    {
        ImGui.PopStyleVar(2);
        ImGui.PopStyleColor(2);
    }
}
