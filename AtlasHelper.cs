using System.IO;
using System.Linq;
using System.Text;
using AtlasHelper.GameState;
using AtlasHelper.GameState.Readers;
using AtlasHelper.Ui;
using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;

namespace AtlasHelper;

public class AtlasHelper : BaseSettingsPlugin<AtlasHelperSettings>
{
    private readonly GameStateReader _state = new();
    private bool _flagsDumped;
    private bool _catalogValidated;

    public AtlasSnapshot State => _state.Current;

    public override bool Initialise()
    {
        Settings.Overview.DrawDelegate = () => OverviewPanel.Draw(Settings);
        Settings.ConfigurationHeader.DrawDelegate = OverviewPanel.DrawConfigurationHeader;
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
        ValidateCatalogOnce();
        return null;
    }

    private void DumpFlagCandidatesOnce()
    {
        if (_flagsDumped || GameController?.IngameState?.Data?.ServerData?.QuestFlags == null)
            return;

        var flags = QuestFlagLookup.Build(GameController);
        var trueFlags = flags.Keys
            .Where(k => flags.Get(k) == true)
            .OrderBy(k => k)
            .ToList();

        var sb = new StringBuilder();
        sb.AppendLine("value\tname");
        foreach (var name in trueFlags)
            sb.Append("True").Append('\t').AppendLine(name);

        var path = Path.Combine(DirectoryFullName, "QuestFlagDump.tsv");
        File.WriteAllText(path, sb.ToString());
        LogMessage($"[AtlasHelper] Dumped {trueFlags.Count} true quest flags to {path}", 10f);

        _flagsDumped = true;
    }

    private void ValidateCatalogOnce()
    {
        if (_catalogValidated || GameController?.IngameState?.Data?.ServerData?.QuestFlags == null)
            return;

        var result = AtlasQuestFlags.Validate(GameController);
        if (result.Unresolved.Count == 0)
        {
            LogMessage($"[AtlasHelper] Quest flag catalog: {result.Total}/{result.Total} resolved.", 10f);
        }
        else
        {
            LogError($"[AtlasHelper] Quest flag catalog: {result.Unresolved.Count} unresolved of {result.Total}. Missing: {string.Join(", ", result.Unresolved)}", 30f);
        }

        _catalogValidated = true;
    }

    public override void Render()
    {
        if (Settings.Hud.ToggleHotkey.PressedOnce())
            Settings.Hud.Show.Value = !Settings.Hud.Show.Value;

        var atlas = GameController?.IngameState?.IngameUi?.Atlas;
        AtlasOverlay.Draw(Graphics, Settings, atlas, _state.Current);

        if (!Settings.Hud.Show.Value)
            return;

        HudOverlay.Draw(Settings, _state.Current);
    }

    public override void EntityAdded(Entity entity)
    {
    }
}
