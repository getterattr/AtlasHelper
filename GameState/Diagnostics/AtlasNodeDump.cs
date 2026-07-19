using System;
using System.IO;
using System.Text;
using ExileCore;

namespace AtlasHelper.GameState.Diagnostics;

// Startup-once dump of every atlas node from Files.AtlasNodes to a tsv.
// Provides the source of truth for verifying AtlasObjectives.Resolve
// found the right nodes - grep the tsv for area_name or the corner
// slot id pattern.
//
// Also cross-references the resolved AtlasObjectives against expected
// coverage and surfaces unresolved ids loudly per
// decisions/read-pattern.md.
internal sealed class AtlasNodeDump
{
    private readonly string _dumpDirectory;
    private readonly Action<string> _logInfo;
    private readonly Action<string> _logError;

    private bool _ran;

    public AtlasNodeDump(string dumpDirectory, Action<string> logInfo, Action<string> logError)
    {
        _dumpDirectory = dumpDirectory;
        _logInfo = logInfo;
        _logError = logError;
    }

    public void RunOnce(GameController gc)
    {
        if (_ran) return;

        var atlasNodes = gc?.Files?.AtlasNodes?.EntriesList;
        if (atlasNodes == null || atlasNodes.Count == 0) return;

        var sb = new StringBuilder();
        sb.AppendLine("id\tarea_id\tarea_name\ttier\tx\ty\tis_unique\tconnections");
        foreach (var entry in atlasNodes)
        {
            if (entry == null) continue;
            var id = entry.Id ?? string.Empty;
            var areaId = entry.Area?.Id ?? string.Empty;
            var areaName = entry.Area?.Name ?? string.Empty;
            var tier = entry.BaseTier;
            var x = entry.PosX;
            var y = entry.PosY;
            var isUnique = entry.IsUniqueMap;
            var connCount = entry.Connections?.Count ?? 0;

            sb.Append(id).Append('\t')
              .Append(areaId).Append('\t')
              .Append(areaName).Append('\t')
              .Append(tier).Append('\t')
              .Append(x).Append('\t')
              .Append(y).Append('\t')
              .Append(isUnique).Append('\t')
              .Append(connCount).Append('\n');
        }

        var path = Path.Combine(_dumpDirectory, "AtlasNodeDump.tsv");
        File.WriteAllText(path, sb.ToString());
        _logInfo($"[AtlasHelper] Dumped {atlasNodes.Count} atlas nodes to {path}");

        var objectives = AtlasObjectives.Resolve(gc);
        var unresolved = new System.Collections.Generic.List<string>();
        var totalCount = 0;
        foreach (var (label, resolvedId) in objectives.All())
        {
            totalCount++;
            if (string.IsNullOrEmpty(resolvedId)) unresolved.Add(label);
        }
        if (unresolved.Count == 0)
            _logInfo($"[AtlasHelper] Atlas objectives: {totalCount}/{totalCount} resolved.");
        else
            _logError($"[AtlasHelper] Atlas objectives: {unresolved.Count} unresolved of {totalCount}. Missing: {string.Join(", ", unresolved)}");

        _ran = true;
    }
}
