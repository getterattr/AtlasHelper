using AtlasHelper.GameState.Maven;

namespace AtlasHelper.GameState.Readers;

internal static class ThemedInvitationsReader
{
    public static ThemedInvitations Read(QuestFlagLookup flags) => new(
        Formed: flags.Get(AtlasQuestFlags.Maven.ThemedInvitations.Formed),
        Twisted: flags.Get(AtlasQuestFlags.Maven.ThemedInvitations.Twisted),
        Elderslayers: flags.Get(AtlasQuestFlags.Maven.ThemedInvitations.Elderslayers),
        Forgotten: flags.Get(AtlasQuestFlags.Maven.ThemedInvitations.Forgotten),
        Remembered: flags.Get(AtlasQuestFlags.Maven.ThemedInvitations.Remembered),
        Feared: flags.Get(AtlasQuestFlags.Maven.ThemedInvitations.Feared));
}
