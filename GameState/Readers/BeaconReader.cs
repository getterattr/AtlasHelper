namespace AtlasHelper.GameState.Readers;

internal static class BeaconReader
{
    public static AtlasBeacons Read(QuestFlagLookup flags) => new(
        flags.Get(AtlasQuestFlags.Beacons.Maven),
        flags.Get(AtlasQuestFlags.Beacons.SearingExarch),
        flags.Get(AtlasQuestFlags.Beacons.EaterOfWorlds));
}
