using System;
using System.Collections.Generic;

namespace AtlasHelper.GameState.Diagnostics;

// After a warm-up window, reports which snapshot sections are still
// Empty. Distinguishes a genuinely-blank character from a broken reader
// after a patch that shifted memory offsets.
internal sealed class SnapshotHealth
{
    private readonly TimeSpan _warmUp;
    private readonly Action<string> _logInfo;
    private readonly Action<string> _logWarn;

    private DateTime? _firstSeen;
    private bool _reported;

    public SnapshotHealth(TimeSpan warmUp, Action<string> logInfo, Action<string> logWarn)
    {
        _warmUp = warmUp;
        _logInfo = logInfo;
        _logWarn = logWarn;
    }

    public void CheckOnce(AtlasSnapshot snapshot)
    {
        if (_reported) return;
        if (snapshot.CapturedAt == DateTime.MinValue) return;

        _firstSeen ??= DateTime.UtcNow;
        if (DateTime.UtcNow - _firstSeen.Value < _warmUp) return;

        var empty = new List<string>();
        if (snapshot.Tree.NodeCount == 0) empty.Add(nameof(snapshot.Tree));
        if (snapshot.Voidstones.Slots.Count == 0) empty.Add(nameof(snapshot.Voidstones));

        if (empty.Count == 0)
            _logInfo($"[AtlasHelper] Snapshot health OK ({(int)_warmUp.TotalSeconds}s warm-up).");
        else
            _logWarn($"[AtlasHelper] Snapshot sections still empty after {(int)_warmUp.TotalSeconds}s: {string.Join(", ", empty)}. Likely a broken reader (offset shift after patch?) or the atlas panel has not been opened.");

        _reported = true;
    }
}
