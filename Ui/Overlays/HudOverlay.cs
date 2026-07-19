using AtlasHelper.GameState;
using AtlasHelper.GameState.Atlas;
using AtlasHelper.GameState.Maven;
using AtlasHelper.Services;
using ImGuiNET;
using System.Numerics;

namespace AtlasHelper.Ui.Overlays;

internal static class HudOverlay
{
    private const string WindowId = "AtlasHelper##AtlasHelperHud";
    private const float Padding = 8f;
    private const string AdvisoryPrefix = "> ";
    private const string AdvisoryIndent = "  ";

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
        var advisory = Advisory.From(settings, state);
        DrawAdvisory(advisory);
        ImGui.Separator();
        DrawSummary(settings, state);
    }

    private static void DrawAdvisory(AdvisoryLine advisory)
    {
        var lines = advisory.Text.Split('\n');
        for (var i = 0; i < lines.Length; i++)
        {
            var prefix = i == 0 ? AdvisoryPrefix : AdvisoryIndent;
            ImGui.Text($"{prefix}{lines[i]}");
        }
    }

    private static void DrawSummary(AtlasHelperSettings settings, AtlasSnapshot state)
    {
        ImGui.Text($"Phase:       {FormatPhaseLine(settings, state)}");
        ImGui.Text($"Voidstones:  {state.Voidstones.SocketedCount} / 4");
        ImGui.Text($"Normal maps: {state.Completion.NormalBonusCount} / {AtlasCompletion.NormalBonusTarget}");
        ImGui.Text($"Unique maps: {state.Completion.UniqueBonusCount} / {AtlasCompletion.UniqueBonusTarget}");
        ImGui.Text($"Maven:       {FormatMavenLine(state.AtlasInvitation)}");
    }

    private static string FormatPhaseLine(AtlasHelperSettings settings, AtlasSnapshot state)
    {
        var selection = settings.Progression.PhaseOverride.Value;
        if (selection != "Auto") return selection;

        return $"Auto ({FormatPhaseId(Phase.From(state).Id)})";
    }

    private static string FormatPhaseId(PhaseId id) => id switch
    {
        PhaseId.One => "Phase 1",
        PhaseId.Two => "Phase 2",
        PhaseId.Three => "Phase 3",
        PhaseId.Four => "Phase 4",
        PhaseId.Complete => "Complete",
        _ => id.ToString(),
    };

    private static string FormatMavenLine(AtlasInvitation invitation)
    {
        if (invitation.QuestlineComplete)
            return "Complete";

        if (invitation.NextStageWitnessTarget is int target && invitation.NextStage is int stage)
            return $"Stage {stage}/{AtlasInvitation.FinalStage}  {invitation.WitnessProgressCapped} / {target}";

        return "-";
    }

    private static void PopStyles()
    {
        ImGui.PopStyleVar(2);
        ImGui.PopStyleColor(2);
    }
}
