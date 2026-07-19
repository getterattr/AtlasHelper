using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;
using ImGuiNET;
using System.Numerics;
using Color = SharpDX.Color;
using ImGuiVector4 = System.Numerics.Vector4;

namespace AtlasHelper;

public class AtlasHelper : BaseSettingsPlugin<AtlasHelperSettings>
{
    public override bool Initialise() => true;

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
        ImGui.Text($"Branch:      {Settings.Branch.Value}");
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
