using AtlasHelper.Ui;
using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;

namespace AtlasHelper;

public class AtlasHelper : BaseSettingsPlugin<AtlasHelperSettings>
{
    public override bool Initialise()
    {
        Settings.Overview.DrawDelegate = () => OverviewPanel.Draw(Settings);
        Settings.Progression.Help.Reference.DrawDelegate = ProgressionReferencePanel.Draw;
        return true;
    }

    public override void AreaChange(AreaInstance area)
    {
    }

    public override Job Tick() => null;

    public override void Render()
    {
        if (Settings.Hud.ToggleHotkey.PressedOnce())
            Settings.Hud.Show.Value = !Settings.Hud.Show.Value;

        if (!Settings.Hud.Show.Value)
            return;

        HudOverlay.Draw(Settings);
    }

    public override void EntityAdded(Entity entity)
    {
    }
}
