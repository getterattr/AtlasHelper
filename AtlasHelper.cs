using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;
using ImGuiNET;
using System.Numerics;
using Color = SharpDX.Color;
using ImGuiVector4 = System.Numerics.Vector4;

namespace AtlasHelper;

public class AtlasHelper : BaseSettingsPlugin<AtlasHelperSettings>
{
    private static readonly ImGuiVector4 SummaryAccentColor = new(0.95f, 0.74f, 0.26f, 1f);
    private static readonly ImGuiVector4 SummaryOkColor = new(0.47f, 0.90f, 0.56f, 1f);
    private static readonly ImGuiVector4 SummaryMutedColor = new(0.63f, 0.66f, 0.72f, 1f);

    private const ImGuiTableFlags SummaryTableFlags =
        ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.RowBg;

    private const ImGuiTableFlags PhaseTableFlags =
        ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.BordersOuter |
        ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.BordersInnerH | ImGuiTableFlags.RowBg;

    private static readonly (string Phase, string Focus, string Details)[] PhaseRows =
    {
        ("Phase 1", "First voidstone", "Rush T16 bottom-left. Exarch + Eater."),
        ("Phase 2", "Ten-Way Maven",   "Farm Maven maps. Second voidstone top-left."),
        ("Phase 3", "Full completion", "Sweep uncompleted maps at magic."),
        ("Phase 4", "Final voidstones","Self-farm or buy a carry (see Strategy)."),
    };

    private static readonly (string Name, string Focus, string Details)[] StrategyRows =
    {
        ("Destructive Play", "Self-farm",     "Kill Shaper, Elder, and Maven yourself. Bossing prep from Phase 3."),
        ("Exarch Altars",    "Currency farm", "Skip pinnacle fights. Farm altars, buy a carry for the final voidstones."),
    };

    public override bool Initialise()
    {
        Settings.Overview.DrawDelegate = DrawOverviewPanel;
        Settings.Progression.Reference.DrawDelegate = DrawProgressionReference;
        return true;
    }

    public override void AreaChange(AreaInstance area)
    {
    }

    public override Job Tick() => null;

    public override void Render()
    {
        if (!Settings.Hud.Show.Value)
            return;

        DrawHudPanel();
    }

    public override void EntityAdded(Entity entity)
    {
    }

    private void DrawOverviewPanel()
    {
        var enabled = Settings.Enable.Value;
        var phase = Settings.Progression.PhaseOverride.Value;
        var strategy = Settings.Progression.Strategy.Value;
        var hud = Settings.Hud.Show.Value;

        if (ImGui.BeginTable("##AtlasHelperOverviewConfig", 2, SummaryTableFlags))
        {
            DrawSummaryRow("Plugin state", enabled ? "Live" : "Offline", enabled ? SummaryOkColor : SummaryMutedColor);
            DrawSummaryRow("Phase", phase, SummaryAccentColor);
            DrawSummaryRow("Strategy", strategy, SummaryAccentColor);
            DrawSummaryRow("HUD overlay", hud ? "Visible" : "Hidden", hud ? SummaryOkColor : SummaryMutedColor);
            ImGui.EndTable();
        }
    }

    private static void DrawProgressionReference()
    {
        DrawSectionLabel("Phases", "What each phase means and when it starts.");
        if (ImGui.BeginTable("##AtlasHelperReferencePhases", 3, PhaseTableFlags))
        {
            ImGui.TableSetupColumn("Phase", ImGuiTableColumnFlags.WidthStretch, 0.14f);
            ImGui.TableSetupColumn("Focus", ImGuiTableColumnFlags.WidthStretch, 0.22f);
            ImGui.TableSetupColumn("Details", ImGuiTableColumnFlags.WidthStretch, 0.64f);
            ImGui.TableHeadersRow();

            foreach (var (p, focus, details) in PhaseRows)
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.TextColored(SummaryAccentColor, p);
                ImGui.TableNextColumn();
                ImGui.TextUnformatted(focus);
                ImGui.TableNextColumn();
                ImGui.TextWrapped(details);
            }

            ImGui.EndTable();
        }

        ImGui.Spacing();
        DrawSectionLabel("Strategies", "How you plan to earn the final two voidstones.");
        if (ImGui.BeginTable("##AtlasHelperReferenceStrategies", 3, PhaseTableFlags))
        {
            ImGui.TableSetupColumn("Strategy", ImGuiTableColumnFlags.WidthStretch, 0.22f);
            ImGui.TableSetupColumn("Focus", ImGuiTableColumnFlags.WidthStretch, 0.18f);
            ImGui.TableSetupColumn("Details", ImGuiTableColumnFlags.WidthStretch, 0.60f);
            ImGui.TableHeadersRow();

            foreach (var (name, focus, details) in StrategyRows)
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.TextColored(SummaryAccentColor, name);
                ImGui.TableNextColumn();
                ImGui.TextUnformatted(focus);
                ImGui.TableNextColumn();
                ImGui.TextWrapped(details);
            }

            ImGui.EndTable();
        }
    }

    private static void DrawSectionLabel(string title, string description)
    {
        ImGui.TextColored(SummaryAccentColor, title);
        if (!string.IsNullOrWhiteSpace(description))
        {
            ImGui.SameLine();
            ImGui.TextDisabled(description);
        }
        ImGui.Separator();
        ImGui.Dummy(new Vector2(0f, 2f));
    }

    private static void DrawSummaryRow(string label, string value, ImGuiVector4? color = null)
    {
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        ImGui.TextDisabled(label);
        ImGui.TableNextColumn();
        if (color.HasValue)
            ImGui.TextColored(color.Value, value);
        else
            ImGui.TextUnformatted(value);
    }

    private void DrawHudPanel()
    {
        var hud = Settings.Hud;

        ImGui.SetNextWindowPos(new Vector2(20, 120), ImGuiCond.FirstUseEver);

        ImGui.PushStyleColor(ImGuiCol.WindowBg, ToImGuiColor(hud.BackgroundColor.Value));
        ImGui.PushStyleColor(ImGuiCol.Text, ToImGuiColor(hud.TextColor.Value));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(hud.Padding.Value, hud.Padding.Value));
        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, hud.Opacity.Value);

        const ImGuiWindowFlags flags =
            ImGuiWindowFlags.NoCollapse |
            ImGuiWindowFlags.AlwaysAutoResize;

        var title = string.IsNullOrWhiteSpace(hud.Title.Value) ? " " : hud.Title.Value;
        if (!ImGui.Begin($"{title}##AtlasHelperHud", flags))
        {
            ImGui.End();
            ImGui.PopStyleVar(3);
            ImGui.PopStyleColor(2);
            return;
        }

        ImGui.SetWindowFontScale(hud.TextScale.Value);

        ImGui.Text($"Phase:       {Settings.Progression.PhaseOverride.Value}");
        ImGui.Text($"Strategy:    {Settings.Progression.Strategy.Value}");
        ImGui.Separator();

        ImGui.Text("Voidstones:  0 / 4");
        ImGui.Text("Completion:  0 / 117");
        ImGui.Text("Maven:       0 / 10");
        ImGui.Separator();

        ImGui.Text("Exarch chain:  pending");
        ImGui.Text("Eater chain:   pending");

        ImGui.SetWindowFontScale(1f);
        ImGui.End();

        ImGui.PopStyleVar(3);
        ImGui.PopStyleColor(2);
    }

    private static ImGuiVector4 ToImGuiColor(Color color) =>
        new(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
}
