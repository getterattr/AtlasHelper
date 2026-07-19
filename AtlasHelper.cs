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
    private static readonly ImGuiVector4 SummaryHeroBackgroundColor = new(0.10f, 0.08f, 0.07f, 0.97f);
    private static readonly ImGuiVector4 SummaryHeroBorderColor = new(0.68f, 0.47f, 0.16f, 1f);
    private static readonly ImGuiVector4 SummaryHeroGlowColor = new(0.36f, 0.24f, 0.08f, 0.90f);
    private static readonly ImGuiVector4 SummaryCardBackgroundColor = new(0.09f, 0.10f, 0.12f, 0.88f);
    private static readonly ImGuiVector4 SummaryCardBorderColor = new(0.45f, 0.34f, 0.17f, 1f);

    private const ImGuiTableFlags SummaryTableFlags =
        ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.RowBg;

    private const ImGuiTableFlags PhaseTableFlags =
        ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.BordersOuter |
        ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.BordersInnerH | ImGuiTableFlags.RowBg;

    private static readonly (string Phase, string Focus, string Details)[] PhaseRows =
    {
        ("Phase 1", "First voidstone",
         "Tier-rush to T16 bottom-left. Kill Exarch and Eater. Skip bonus completion."),
        ("Phase 2", "Ten-Way Maven",
         "Farm Maven-influenced maps in parallel with late Phase 1. Second voidstone (top-left)."),
        ("Phase 3", "Full completion",
         "Sweep every uncompleted map magic at or above native tier. White -> yellow -> red."),
        ("Phase 4", "Final voidstones",
         "Self-farm Shaper/Elder/Maven (Destructive Play) or currency-farm to buy a carry (Exarch Altars)."),
    };

    public override bool Initialise()
    {
        Settings.Overview.DrawDelegate = DrawOverviewPanel;
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

        DrawHeroBanner(
            "AtlasHelperOverviewHero",
            "OVERVIEW",
            "AtlasHelper",
            "A league-start guide from empty atlas to four voidstones. Tracks phase progress, highlights which map to run next, and points at the boss room while you're in a map.");

        DrawSectionLabel("Current configuration", "Snapshot of the toggles that shape the HUD and recommendations.");
        if (ImGui.BeginTable("##AtlasHelperOverviewConfig", 2, SummaryTableFlags))
        {
            DrawSummaryRow("Plugin state", enabled ? "Live" : "Offline", enabled ? SummaryOkColor : SummaryMutedColor);
            DrawSummaryRow("Phase", phase, SummaryAccentColor);
            DrawSummaryRow("Strategy", strategy, SummaryAccentColor);
            DrawSummaryRow("HUD overlay", hud ? "Visible" : "Hidden", hud ? SummaryOkColor : SummaryMutedColor);
            ImGui.EndTable();
        }

        ImGui.Spacing();
        DrawSectionLabel("Progression phases", "What each phase means and when it starts.");
        if (ImGui.BeginTable("##AtlasHelperOverviewPhases", 3, PhaseTableFlags))
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
        DrawHintCallout(
            "AtlasHelperOverviewHint",
            "Tip",
            "Leave Phase on Auto unless you want to preview a later phase. Set Strategy under Progression before Phase 3 - it changes which invitations the plugin recommends.");
    }

    private static void DrawHeroBanner(string scopeId, string eyebrow, string title, string description)
    {
        ImGui.PushStyleColor(ImGuiCol.ChildBg, SummaryHeroBackgroundColor);
        ImGui.PushStyleColor(ImGuiCol.Border, SummaryHeroBorderColor);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 10f);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 1f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(14f, 12f));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(8f, 5f));

        if (ImGui.BeginChild(
                $"##{scopeId}",
                Vector2.Zero,
                ImGuiChildFlags.Border | ImGuiChildFlags.AutoResizeY,
                ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            var drawList = ImGui.GetWindowDrawList();
            var rectMin = ImGui.GetWindowPos();
            var rectMax = rectMin + ImGui.GetWindowSize();
            drawList.AddRectFilled(
                rectMin,
                new Vector2(rectMax.X, rectMin.Y + 4f),
                ImGui.GetColorU32(SummaryHeroGlowColor),
                10f,
                ImDrawFlags.RoundCornersTop);

            ImGui.TextColored(SummaryMutedColor, eyebrow);
            ImGui.TextColored(SummaryAccentColor, title);
            ImGui.PushStyleColor(ImGuiCol.Text, new ImGuiVector4(0.82f, 0.84f, 0.88f, 1f));
            ImGui.TextWrapped(description);
            ImGui.PopStyleColor();
        }

        ImGui.EndChild();
        ImGui.PopStyleVar(4);
        ImGui.PopStyleColor(2);
        ImGui.Spacing();
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

    private static void DrawHintCallout(string scopeId, string title, string body)
    {
        ImGui.PushStyleColor(ImGuiCol.ChildBg, SummaryCardBackgroundColor);
        ImGui.PushStyleColor(ImGuiCol.Border, SummaryCardBorderColor);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 9f);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 1f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(10f, 8f));

        if (ImGui.BeginChild(
                $"##{scopeId}",
                Vector2.Zero,
                ImGuiChildFlags.Border | ImGuiChildFlags.AutoResizeY,
                ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            ImGui.TextColored(SummaryAccentColor, title);
            ImGui.PushStyleColor(ImGuiCol.Text, SummaryMutedColor);
            ImGui.TextWrapped(body);
            ImGui.PopStyleColor();
        }

        ImGui.EndChild();
        ImGui.PopStyleVar(3);
        ImGui.PopStyleColor(2);
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
