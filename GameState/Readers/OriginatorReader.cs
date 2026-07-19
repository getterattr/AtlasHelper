using AtlasHelper.GameState.Atlas;
using AtlasHelper.GameState.Voidstones;

namespace AtlasHelper.GameState.Readers;

internal static class OriginatorReader
{
    // Memory-map area ids per AtlasNodeDump.tsv. Completion lives in
    // ServerData.CompletedNodes (surfaced via AtlasMapNode.Completed);
    // no per-map quest flags exist for these three.
    private const string CourtyardId = "MapWorldsCourtyardOfWasting";
    private const string ChambersId = "MapWorldsChambersOfImpurity";
    private const string TheatreId = "MapWorldsTheatreOfLies";

    public static Originator Read(QuestFlagLookup flags, AtlasTree tree) => new(
        EagonMet: flags.Get(AtlasQuestFlags.Voidstones.Originator.EagonMet),
        EagonIntroductionSeen: flags.Get(AtlasQuestFlags.Voidstones.Originator.EagonIntroductionSeen),
        CourtyardCleared: IsCompleted(tree, CourtyardId),
        ChambersCleared: IsCompleted(tree, ChambersId),
        TheatreCleared: IsCompleted(tree, TheatreId),
        IncarnationOfNeglectDefeated: flags.Get(AtlasQuestFlags.Voidstones.Originator.IncarnationOfNeglectDefeated),
        IncarnationOfFearDefeated: flags.Get(AtlasQuestFlags.Voidstones.Originator.IncarnationOfFearDefeated));

    private static bool? IsCompleted(AtlasTree tree, string areaId)
    {
        foreach (var node in tree.Nodes)
            if (node.AreaId == areaId) return node.Completed;
        return null;
    }
}
