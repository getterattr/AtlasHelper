using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;
using ImGuiNET;
using System.Numerics;

namespace AtlasHelper;

public class AtlasHelper : BaseSettingsPlugin<AtlasHelperSettings>
{
    public override bool Initialise()
    {
        return true;
    }

    public override void AreaChange(AreaInstance area)
    {
    }

    public override Job Tick()
    {
        return null;
    }

    public override void Render()
    {
        if (!Settings.ShowHud.Value)
            return;

        DrawHudPanel();
    }

    public override void EntityAdded(Entity entity)
    {
    }

    private void DrawHudPanel()
    {
        ImGui.SetNextWindowPos(new Vector2(20, 120), ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowSize(new Vector2(260, 0), ImGuiCond.FirstUseEver);

        const ImGuiWindowFlags flags =
            ImGuiWindowFlags.NoCollapse |
            ImGuiWindowFlags.AlwaysAutoResize;

        if (!ImGui.Begin("AtlasHelper##AtlasHelperHud", flags))
        {
            ImGui.End();
            return;
        }

        var phase = Settings.PhaseOverride.Value;
        ImGui.Text($"Phase:       {phase}");
        ImGui.Text($"Branch:      {Settings.Branch.Value}");
        ImGui.Separator();

        ImGui.Text("Voidstones:  0 / 4");
        ImGui.Text("Completion:  0 / 117");
        ImGui.Text("Maven:       0 / 10");
        ImGui.Separator();

        ImGui.Text("Exarch chain:  pending");
        ImGui.Text("Eater chain:   pending");
        ImGui.Separator();

        ImGui.TextDisabled("(values are placeholders until the spike lands)");

        ImGui.End();
    }
}
