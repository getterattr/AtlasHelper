using AtlasHelper.GameState.Voidstones;

namespace AtlasHelper.GameState.Readers;

internal static class DecayedReader
{
    public static Decayed Read(QuestFlagLookup flags) => new(
        ChimeraDefeated: flags.Get(AtlasQuestFlags.Voidstones.Decayed.ChimeraDefeated),
        HydraDefeated: flags.Get(AtlasQuestFlags.Voidstones.Decayed.HydraDefeated),
        MinotaurDefeated: flags.Get(AtlasQuestFlags.Voidstones.Decayed.MinotaurDefeated),
        PhoenixDefeated: flags.Get(AtlasQuestFlags.Voidstones.Decayed.PhoenixDefeated),
        EnslaverDefeated: flags.Get(AtlasQuestFlags.Voidstones.Decayed.EnslaverDefeated),
        ConstrictorDefeated: flags.Get(AtlasQuestFlags.Voidstones.Decayed.ConstrictorDefeated),
        PurifierDefeated: flags.Get(AtlasQuestFlags.Voidstones.Decayed.PurifierDefeated),
        EradicatorDefeated: flags.Get(AtlasQuestFlags.Voidstones.Decayed.EradicatorDefeated));
}
