using AtlasHelper.GameState.Maven;

namespace AtlasHelper.GameState.Readers;

internal static class ThemedInvitationsReader
{
    public static ThemedInvitations Read(QuestFlagLookup flags) => new(
        Formed: flags.Get(AtlasQuestFlags.Maven.ThemedInvitations.FormedUnlocked),
        Twisted: flags.Get(AtlasQuestFlags.Maven.ThemedInvitations.TwistedUnlocked),
        Elderslayers: flags.Get(AtlasQuestFlags.Maven.ThemedInvitations.ElderslayersUnlocked),
        Forgotten: flags.Get(AtlasQuestFlags.Maven.ThemedInvitations.ForgottenUnlocked),
        Remembered: flags.Get(AtlasQuestFlags.Maven.ThemedInvitations.RememberedUnlocked),
        Feared: flags.Get(AtlasQuestFlags.Maven.ThemedInvitations.FearedUnlocked));
}
