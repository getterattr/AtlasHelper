using AtlasHelper.GameState.Voidstones;

namespace AtlasHelper.GameState.Readers;

internal static class OriginatorReader
{
    public static Originator Read(QuestFlagLookup flags) => new(
        EagonMet: flags.Get(AtlasQuestFlags.Voidstones.Originator.EagonMet),
        EagonIntroductionSeen: flags.Get(AtlasQuestFlags.Voidstones.Originator.EagonIntroductionSeen),
        CourtyardCleared: flags.Get(AtlasQuestFlags.Voidstones.Originator.CourtyardCleared),
        ChambersCleared: flags.Get(AtlasQuestFlags.Voidstones.Originator.ChambersCleared),
        TheatreCleared: flags.Get(AtlasQuestFlags.Voidstones.Originator.TheatreCleared),
        IncarnationOfNeglectDefeated: flags.Get(AtlasQuestFlags.Voidstones.Originator.IncarnationOfNeglectDefeated),
        IncarnationOfFearDefeated: flags.Get(AtlasQuestFlags.Voidstones.Originator.IncarnationOfFearDefeated));
}
