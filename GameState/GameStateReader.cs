using System;
using AtlasHelper.GameState.Readers;
using ExileCore;

namespace AtlasHelper.GameState;

public sealed class GameStateReader
{
    private static readonly TimeSpan MaxSnapshotAge = TimeSpan.FromSeconds(2);

    private AtlasSnapshot _current = AtlasSnapshot.Empty;

    public AtlasSnapshot Current => _current;

    public AtlasSnapshot RefreshIfStale(GameController gc)
    {
        if (DateTime.UtcNow - _current.CapturedAt < MaxSnapshotAge)
            return _current;
        return Refresh(gc);
    }

    public AtlasSnapshot Refresh(GameController gc)
    {
        if (!gc.InGame || gc.Player == null)
            return _current;

        _current = new AtlasSnapshot(
            DateTime.UtcNow,
            VoidstoneReader.Read(gc),
            CompletionReader.Read(gc),
            MavenReader.Read(gc),
            PassivesReader.Read(gc),
            TreeReader.Read(gc),
            PinnacleCompletion.Empty);

        return _current;
    }
}
