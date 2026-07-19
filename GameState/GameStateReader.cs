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

        var flags = QuestFlagLookup.Build(gc);
        var tree = TreeReader.Read(gc);

        _current = new AtlasSnapshot(
            DateTime.UtcNow,
            tree,
            CompletionReader.Read(gc),
            VoidstoneReader.Read(gc),
            EldritchReader.Read(flags),
            OriginatorReader.Read(flags, tree),
            DecayedReader.Read(tree),
            PinnacleBossesReader.Read(flags),
            WitnessesReader.Read(gc),
            AtlasInvitationReader.Read(gc, flags),
            ThemedInvitationsReader.Read(flags),
            SessionReader.Read(gc));

        return _current;
    }
}
