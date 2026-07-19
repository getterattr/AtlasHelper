using System;
using System.IO;
using System.Linq;
using System.Text;
using AtlasHelper.GameState.Readers;
using ExileCore;

namespace AtlasHelper.GameState.Diagnostics;

// Startup-once quest flag discovery workflow. Runs the first time
// ServerData.QuestFlags hydrates: writes every true flag to
// QuestFlagDump.tsv (the pin-canonical-names source of truth) and
// cross-references AtlasQuestFlags against the runtime set to surface
// unresolved names per ADR 0004.
internal sealed class FlagDiagnostics
{
    private readonly string _dumpDirectory;
    private readonly Action<string> _logInfo;
    private readonly Action<string> _logError;

    private bool _dumped;
    private bool _validated;

    public FlagDiagnostics(string dumpDirectory, Action<string> logInfo, Action<string> logError)
    {
        _dumpDirectory = dumpDirectory;
        _logInfo = logInfo;
        _logError = logError;
    }

    public void RunOnce(GameController gc)
    {
        if (gc?.IngameState?.Data?.ServerData?.QuestFlags == null)
            return;

        DumpOnce(gc);
        ValidateOnce(gc);
    }

    private void DumpOnce(GameController gc)
    {
        if (_dumped) return;

        var flags = QuestFlagLookup.Build(gc);
        var trueFlags = flags.Keys
            .Where(k => flags.Get(k) == true)
            .OrderBy(k => k)
            .ToList();

        var sb = new StringBuilder();
        sb.AppendLine("value\tname");
        foreach (var name in trueFlags)
            sb.Append("True").Append('\t').AppendLine(name);

        var path = Path.Combine(_dumpDirectory, "QuestFlagDump.tsv");
        File.WriteAllText(path, sb.ToString());
        _logInfo($"[AtlasHelper] Dumped {trueFlags.Count} true quest flags to {path}");

        _dumped = true;
    }

    private void ValidateOnce(GameController gc)
    {
        if (_validated) return;

        var result = AtlasQuestFlags.Validate(gc);
        if (result.Unresolved.Count == 0)
            _logInfo($"[AtlasHelper] Quest flag catalog: {result.Total}/{result.Total} resolved.");
        else
            _logError($"[AtlasHelper] Quest flag catalog: {result.Unresolved.Count} unresolved of {result.Total}. Missing: {string.Join(", ", result.Unresolved)}");

        _validated = true;
    }
}
