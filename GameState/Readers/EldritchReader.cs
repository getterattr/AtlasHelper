using AtlasHelper.GameState.Voidstones;

namespace AtlasHelper.GameState.Readers;

internal static class EldritchReader
{
    public static Eldritch Read(QuestFlagLookup flags)
    {
        var exarch = new ExarchChain(
            EnvoyMet: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Exarch.EnvoyMet),
            InfluenceUnlocked: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Exarch.InfluenceUnlocked),
            PolaricInvitationDropped: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Exarch.PolaricInvitationDropped),
            BlackStarDefeated: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Exarch.BlackStarDefeated),
            T12Cleared: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Exarch.T12Cleared),
            T13Cleared: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Exarch.T13Cleared),
            T14Cleared: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Exarch.T14Cleared),
            T15Cleared: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Exarch.T15Cleared),
            IncandescentInvitationDropped: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Exarch.IncandescentInvitationDropped));

        var eater = new EaterChain(
            FleshCompassReceived: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Eater.FleshCompassReceived),
            InfluenceUnlocked: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Eater.InfluenceUnlocked),
            T9Cleared: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Eater.T9Cleared),
            T10Cleared: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Eater.T10Cleared),
            T12Cleared: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Eater.T12Cleared),
            T14Cleared: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Eater.T14Cleared),
            ScreamingInvitationDropped: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Eater.ScreamingInvitationDropped));

        return new Eldritch(exarch, eater);
    }
}
