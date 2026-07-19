using AtlasHelper.GameState.Voidstones;

namespace AtlasHelper.GameState.Readers;

internal static class OriginatorReader
{
    public static Originator Read(QuestFlagLookup flags) => new(
        EagonIntroduced: flags.Get(AtlasQuestFlags.Voidstones.Originator.EagonIntroduced),
        CourtyardCleared: flags.Get(AtlasQuestFlags.Voidstones.Originator.CourtyardCleared),
        ChambersCleared: flags.Get(AtlasQuestFlags.Voidstones.Originator.ChambersCleared),
        TheatreCleared: flags.Get(AtlasQuestFlags.Voidstones.Originator.TheatreCleared));
}
