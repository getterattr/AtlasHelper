using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;
using ImGuiNET;
using System.Numerics;
using Color = SharpDX.Color;
using ImGuiVector4 = System.Numerics.Vector4;

namespace AtlasHelper;

public class AtlasHelper : BaseSettingsPlugin<AtlasHelperSettings>
{
    public override bool Initialise()
    {
        Settings.PhaseGuide.DrawDelegate = DrawPhaseGuide;
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

        ImGui.Text($"Phase:       {Settings.PhaseOverride.Value}");
        ImGui.Text($"Strategy:    {Settings.Strategy.Value}");
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

    private static readonly (string Title, string Body)[] PhaseCards =
    {
        ("Phase 1 - First Voidstone",
         "Push tier by tier to T16 in the bottom-left corner. Kill the Searing Exarch and the Eater of Worlds invitations to earn your first voidstone. Skip bonus completion on this run - you want speed, not coverage."),
        ("Phase 2 - Ten-Way Maven",
         "Runs in parallel with the tail of Phase 1. Stack Maven-influenced maps, then run the Maven's Writ invitation until you have ten witnessed maps. The reward is the second voidstone, socketed in the top-left corner."),
        ("Phase 3 - Full Completion",
         "With two voidstones down, sweep every uncompleted map at magic rarity for the bonus point. White maps first, then yellow, then red - always at or above the map's native tier."),
        ("Phase 4 - Final Voidstones",
         "Acquire the third and fourth voidstones (Shaper/Elder). Either self-farm the pinnacle invitations (see Strategy: Destructive Play) or currency-farm Exarch altars and buy a carry (Strategy: Exarch Altars)."),
    };

    private static void DrawPhaseGuide()
    {
        for (var i = 0; i < PhaseCards.Length; i++)
        {
            var (title, body) = PhaseCards[i];
            DrawPhaseCard($"AtlasHelperPhaseCard{i}", title, body);
            if (i < PhaseCards.Length - 1)
                ImGui.Spacing();
        }
    }

    private static readonly ImGuiVector4 CardBackground = new(0.10f, 0.12f, 0.16f, 1f);
    private static readonly ImGuiVector4 CardBorder = new(0.28f, 0.34f, 0.44f, 1f);
    private static readonly ImGuiVector4 CardTitle = new(1.00f, 0.78f, 0.35f, 1f);
    private static readonly ImGuiVector4 CardBody = new(0.82f, 0.84f, 0.88f, 1f);

    private static void DrawPhaseCard(string scopeId, string title, string body)
    {
        ImGui.PushStyleColor(ImGuiCol.ChildBg, CardBackground);
        ImGui.PushStyleColor(ImGuiCol.Border, CardBorder);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 8f);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 1f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(12f, 10f));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(8f, 6f));

        if (ImGui.BeginChild(
                $"##{scopeId}",
                Vector2.Zero,
                ImGuiChildFlags.Border | ImGuiChildFlags.AutoResizeY,
                ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            ImGui.TextColored(CardTitle, title);
            ImGui.PushStyleColor(ImGuiCol.Text, CardBody);
            ImGui.TextWrapped(body);
            ImGui.PopStyleColor();
        }

        ImGui.EndChild();
        ImGui.PopStyleVar(4);
        ImGui.PopStyleColor(2);
    }
}
