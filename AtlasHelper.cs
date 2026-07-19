using AtlasHelper.GameState;
using AtlasHelper.GameState.Readers;
using AtlasHelper.Ui;
using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;

namespace AtlasHelper;

public class AtlasHelper : BaseSettingsPlugin<AtlasHelperSettings>
{
    private static readonly string[] FlagKeywords =
    {
        "Maven", "Beacon", "Exarch", "Eater", "Sirus", "Shaper", "Elder",
        "Incarnation", "Dread", "Feared", "Formed", "Twisted", "Elderslayer",
        "Forgotten", "Remembered", "Crucible", "Voidstone", "Writ",
    };

    private readonly GameStateReader _state = new();
    private bool _flagsDumped;

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
        DumpFlagCandidatesOnce();
        return null;
    }

    private void DumpFlagCandidatesOnce()
    {
        if (_flagsDumped || GameController?.IngameState?.Data?.ServerData?.QuestFlags == null)
            return;

        var flags = QuestFlagLookup.Build(GameController);
        foreach (var kvp in flags.WhereNameContains(FlagKeywords))
            LogMessage($"[AtlasHelper.QuestFlagCandidate] {kvp.Key}={kvp.Value}", 15f);

        _flagsDumped = true;
    }

    public override void Render()
    {
        if (Settings.Hud.ToggleHotkey.PressedOnce())
            Settings.Hud.Show.Value = !Settings.Hud.Show.Value;

        var atlas = GameController?.IngameState?.IngameUi?.Atlas;
        AtlasOverlay.Draw(Graphics, Settings, atlas);

        if (!Settings.Hud.Show.Value)
            return;

        HudOverlay.Draw(Settings, _state.Current);
    }

    public override void EntityAdded(Entity entity)
    {
    }
}
