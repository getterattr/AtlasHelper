using AtlasHelper.GameState.Pinnacles;

namespace AtlasHelper.GameState.Readers;

internal static class PinnacleBossesReader
{
    public static PinnacleBosses Read(QuestFlagLookup flags) => new(
        Maven: flags.Get(AtlasQuestFlags.Pinnacles.Maven),
        Shaper: flags.Get(AtlasQuestFlags.Pinnacles.Shaper),
        Elder: flags.Get(AtlasQuestFlags.Pinnacles.Elder),
        SearingExarch: flags.Get(AtlasQuestFlags.Pinnacles.SearingExarch),
        EaterOfWorlds: flags.Get(AtlasQuestFlags.Pinnacles.EaterOfWorlds),
        IncarnationOfDread: flags.Get(AtlasQuestFlags.Pinnacles.IncarnationOfDread),
        Sirus: flags.Get(AtlasQuestFlags.Pinnacles.Sirus));
}
