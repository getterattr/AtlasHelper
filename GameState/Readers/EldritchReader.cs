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
            IncandescentInvitationDropped: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Exarch.IncandescentInvitationDropped));

        var eater = new EaterChain(
            FleshCompassReceived: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Eater.FleshCompassReceived),
            InfluenceUnlocked: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Eater.InfluenceUnlocked),
            ScreamingInvitationDropped: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Eater.ScreamingInvitationDropped),
            InfiniteHungerDefeated: flags.Get(AtlasQuestFlags.Voidstones.Eldritch.Eater.InfiniteHungerDefeated));

        return new Eldritch(exarch, eater);
    }
}
