namespace AtlasHelper.GameState.Readers;

internal static class BeaconReader
{
    // Placeholder flag names. TODO: pin canonical QuestFlag names once identified
    // via QuestFlagLookup.Keys diagnostic (see AtlasHelper.LogFlagCandidates).
    private const string MavenBeaconFlag = "GotMavensBeacon";
    private const string SearingExarchBeaconFlag = "SearingExarchIntroduced";
    private const string EaterOfWorldsBeaconFlag = "EaterOfWorldsIntroduced";

    public static AtlasBeacons Read(QuestFlagLookup flags) => new(
        flags.Get(MavenBeaconFlag),
        flags.Get(SearingExarchBeaconFlag),
        flags.Get(EaterOfWorldsBeaconFlag));
}
