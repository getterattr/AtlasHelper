using AtlasHelper.GameState;
using AtlasHelper.Ui;
using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;

namespace AtlasHelper;

public class AtlasHelper : BaseSettingsPlugin<AtlasHelperSettings>
{
    private readonly GameStateReader _state = new();

    public AtlasSnapshot State => _state.Current;

    public override bool Initialise()
    {
        Settings.Overview.DrawDelegate = () => OverviewPanel.Draw(Settings);
        Settings.Progression.Help.Reference.DrawDelegate = ProgressionReferencePanel.Draw;
        return true;
    }

    public override void AreaChange(AreaInstance area)
    {
        _state.Refresh(GameController);
    }

    public override Job Tick()
    {
        _state.RefreshIfStale(GameController);
        return null;
    }

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
