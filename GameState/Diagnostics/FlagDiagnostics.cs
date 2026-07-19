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

    private bool _ran;

    public FlagDiagnostics(string dumpDirectory, Action<string> logInfo, Action<string> logError)
    {
        _dumpDirectory = dumpDirectory;
        _logInfo = logInfo;
        _logError = logError;
    }

    public void RunOnce(GameController gc)
    {
        if (_ran) return;
        if (gc?.IngameState?.Data?.ServerData?.QuestFlags == null) return;

        var flags = QuestFlagLookup.Build(gc);
        var allFlags = flags.Keys.OrderBy(k => k).ToList();

        var sb = new StringBuilder();
        sb.AppendLine("value\tname");
        foreach (var name in allFlags)
            sb.Append(flags.Get(name) == true ? "True" : "False").Append('\t').AppendLine(name);

        var path = Path.Combine(_dumpDirectory, "QuestFlagDump.tsv");
        File.WriteAllText(path, sb.ToString());
        var trueCount = allFlags.Count(n => flags.Get(n) == true);
        _logInfo($"[AtlasHelper] Dumped {allFlags.Count} quest flags ({trueCount} true) to {path}");

        // Full Files.QuestFlags catalog - the canonical superset of every
        // flag id the game defines, regardless of whether this character
        // has ever seen it. Useful for pinning names of flags that never
        // surface in ServerData (e.g. Maven Crucible stages 2-5 before
        // the ladder has progressed that far).
        var catalog = gc.Files?.QuestFlags?.EntriesList;
        if (catalog != null)
        {
            var catalogSb = new StringBuilder();
            catalogSb.AppendLine("id");
            var ids = new List<string>(catalog.Count);
            foreach (var entry in catalog)
            {
                if (entry?.Id is { } id && id.Length > 0) ids.Add(id);
            }
            ids.Sort(System.StringComparer.OrdinalIgnoreCase);
            foreach (var id in ids)
                catalogSb.AppendLine(id);
            var catalogPath = Path.Combine(_dumpDirectory, "QuestFlagCatalog.tsv");
            File.WriteAllText(catalogPath, catalogSb.ToString());
            _logInfo($"[AtlasHelper] Dumped {ids.Count} Files.QuestFlags catalog ids to {catalogPath}");
        }

        var result = AtlasQuestFlags.Validate(gc);
        if (result.Unresolved.Count == 0)
            _logInfo($"[AtlasHelper] Quest flag catalog: {result.Total}/{result.Total} resolved.");
        else
            _logError($"[AtlasHelper] Quest flag catalog: {result.Unresolved.Count} unresolved of {result.Total}. Missing: {string.Join(", ", result.Unresolved)}");

        _ran = true;
    }
}
