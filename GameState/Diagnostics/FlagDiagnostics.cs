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
        // has ever seen it.
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

        // Files.QuestStates - richer catalog of per-quest progression
        // states. Each entry carries QuestStateId + Quest.Id + Quest.Name
        // + ProgressText. Where QuestFlags is a flat name list,
        // QuestStates lets us find the enumerated stages of a specific
        // quest (e.g. Maven's Crucible stages 1..5).
        var questStates = gc.Files?.QuestStates?.EntriesList;
        if (questStates != null)
        {
            var qsSb = new StringBuilder();
            qsSb.AppendLine("state_id\tquest_id\tact\tquest_name");
            foreach (var entry in questStates)
            {
                if (entry == null) continue;
                var stateId = entry.QuestStateId;
                var questId = entry.Quest?.Id ?? string.Empty;
                var act = entry.Quest?.Act ?? 0;
                var questName = entry.Quest?.Name ?? string.Empty;
                qsSb.Append(stateId).Append('\t')
                    .Append(questId).Append('\t')
                    .Append(act).Append('\t')
                    .Append(questName)
                    .Append('\n');
            }
            var qsPath = Path.Combine(_dumpDirectory, "QuestStates.tsv");
            File.WriteAllText(qsPath, qsSb.ToString());
            _logInfo($"[AtlasHelper] Dumped {questStates.Count} Files.QuestStates to {qsPath}");
        }

        try
        {
            var qr = gc.Files?.QuestRewards?.EntriesList;
            if (qr != null)
            {
                var qrSb = new StringBuilder();
                qrSb.AppendLine("quest_id\tact\treward_class\treward_name\treward_level");
                foreach (var entry in qr)
                {
                    if (entry?.Offer?.Quest == null) continue;
                    var qid = entry.Offer.Quest.Id ?? string.Empty;
                    var act = entry.Offer.Quest.Act;
                    var cls = entry.Reward?.ClassName ?? string.Empty;
                    var name = entry.Reward?.BaseName ?? string.Empty;
                    var lvl = entry.RewardLevel;
                    qrSb.Append(qid).Append('\t').Append(act).Append('\t')
                        .Append(cls).Append('\t').Append(name).Append('\t')
                        .Append(lvl).Append('\n');
                }
                var qrPath = Path.Combine(_dumpDirectory, "QuestRewards.tsv");
                File.WriteAllText(qrPath, qrSb.ToString());
                _logInfo($"[AtlasHelper] Dumped {qr.Count} QuestRewards to {qrPath}");
            }
        }
        catch (Exception ex)
        {
            _logError($"[AtlasHelper] QuestRewards dump failed: {ex.Message}");
        }

        // IngameUi.GetQuests / GetCompletedQuests: character's current
        // quest-state cursor per tracked quest. This is where maven_atlas
        // (17-state Crucible ladder) reports the player's current stage.
        try
        {
            var qtSb = new StringBuilder();
            qtSb.AppendLine("bucket\tstate_id\tquest_id\tact\tquest_name");
            var all = gc.IngameState?.IngameUi?.GetQuests;
            if (all != null)
            {
                foreach (var tuple in all)
                {
                    var q = tuple.Item1;
                    if (q == null) continue;
                    qtSb.Append("all\t").Append(tuple.Item2).Append('\t')
                        .Append(q.Id ?? string.Empty).Append('\t')
                        .Append(q.Act).Append('\t')
                        .Append(q.Name ?? string.Empty).Append('\n');
                }
            }
            var done = gc.IngameState?.IngameUi?.GetCompletedQuests;
            if (done != null)
            {
                foreach (var tuple in done)
                {
                    var q = tuple.Item1;
                    if (q == null) continue;
                    qtSb.Append("done\t").Append(tuple.Item2).Append('\t')
                        .Append(q.Id ?? string.Empty).Append('\t')
                        .Append(q.Act).Append('\t')
                        .Append(q.Name ?? string.Empty).Append('\n');
                }
            }
            var qtPath = Path.Combine(_dumpDirectory, "QuestTracker.tsv");
            File.WriteAllText(qtPath, qtSb.ToString());
            _logInfo($"[AtlasHelper] Dumped quest tracker to {qtPath}");
        }
        catch (Exception ex)
        {
            _logError($"[AtlasHelper] QuestTracker dump failed: {ex.Message}");
        }

        var result = AtlasQuestFlags.Validate(gc);
        if (result.Unresolved.Count == 0)
            _logInfo($"[AtlasHelper] Quest flag catalog: {result.Total}/{result.Total} resolved.");
        else
            _logError($"[AtlasHelper] Quest flag catalog: {result.Unresolved.Count} unresolved of {result.Total}. Missing: {string.Join(", ", result.Unresolved)}");

        _ran = true;
    }
}
