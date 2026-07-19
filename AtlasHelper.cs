using System;
using AtlasHelper.GameState;
using AtlasHelper.GameState.Diagnostics;
using AtlasHelper.Ui.Overlays;
using AtlasHelper.Ui.Panels;
using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;

namespace AtlasHelper;

public class AtlasHelper : BaseSettingsPlugin<AtlasHelperSettings>
{
    private readonly GameStateReader _state = new();
    private FlagDiagnostics _flagDiagnostics;
    private SnapshotHealth _snapshotHealth;
    private AtlasNodeDump _atlasNodeDump;

    public AtlasHelper()
    {
        Name = "AtlasHelper";
    }

    public AtlasSnapshot State => _state.Current;

    public override void OnLoad()
    {
        Settings.Overview.DrawDelegate = () => OverviewPanel.Draw(Settings);
        Settings.ConfigurationHeader.DrawDelegate = OverviewPanel.DrawConfigurationHeader;
        Settings.Progression.Reference.DrawDelegate = ProgressionReferencePanel.Draw;

        _flagDiagnostics = new FlagDiagnostics(
            DirectoryFullName,
            msg => LogMessage(msg, 10f),
            msg => LogError(msg, 30f));

        _snapshotHealth = new SnapshotHealth(
            TimeSpan.FromSeconds(30),
            msg => LogMessage(msg, 10f),
            msg => LogError(msg, 30f));

        _atlasNodeDump = new AtlasNodeDump(
            DirectoryFullName,
            msg => LogMessage(msg, 10f),
            msg => LogError(msg, 30f));
    }

    public override void OnClose()
    {
        base.OnClose();
        _flagDiagnostics = null;
        _snapshotHealth = null;
        _atlasNodeDump = null;
    }

    public override void AreaChange(AreaInstance area)
    {
        _state.Refresh(GameController);
    }

    public override Job Tick()
    {
        _state.RefreshIfStale(GameController);
        _flagDiagnostics?.RunOnce(GameController);
        _snapshotHealth?.CheckOnce(_state.Current);
        _atlasNodeDump?.RunOnce(GameController);
        return null;
    }

    public override void Render()
    {
        if (Settings.Hud.ToggleHotkey.PressedOnce())
            Settings.Hud.Show.Value = !Settings.Hud.Show.Value;

        var atlas = GameController?.IngameState?.IngameUi?.Atlas;
        PathOverlay.Draw(Graphics, Settings, atlas, _state.Current);
        AtlasOverlay.Draw(Graphics, Settings, atlas, _state.Current);

        if (!Settings.Hud.Show.Value)
            return;

        HudOverlay.Draw(Settings, _state.Current);
    }

    public override void EntityAdded(Entity entity)
    {
    }

    internal void LogDebug(string message)
    {
        if (Settings?.DebugLogging?.Value != true) return;
        try { DebugWindow.LogMsg($"[AtlasHelper] {message}"); } catch { }
    }
}
