using AtlasHelper.GameState.Atlas;
using AtlasHelper.GameState.Voidstones;

namespace AtlasHelper.GameState.Readers;

internal static class DecayedReader
{
    // Guardian atlas node ids per AtlasNodeDump.tsv. Completion lives
    // in ServerData.CompletedNodes (surfaced via AtlasMapNode.Completed);
    // no per-guardian quest flags exist.
    private const string ChimeraId = "MapWorldsChimera";
    private const string HydraId = "MapWorldsHydra";
    private const string MinotaurId = "MapWorldsMinotaur";
    private const string PhoenixId = "MapWorldsPhoenix";
    private const string EnslaverId = "MapWorldsEnslaver";
    private const string ConstrictorId = "MapWorldsConstrictor";
    private const string PurifierId = "MapWorldsPurifier";
    private const string EradicatorId = "MapWorldsEradicator";

    public static Decayed Read(AtlasTree tree) => new(
        ChimeraDefeated: IsCompleted(tree, ChimeraId),
        HydraDefeated: IsCompleted(tree, HydraId),
        MinotaurDefeated: IsCompleted(tree, MinotaurId),
        PhoenixDefeated: IsCompleted(tree, PhoenixId),
        EnslaverDefeated: IsCompleted(tree, EnslaverId),
        ConstrictorDefeated: IsCompleted(tree, ConstrictorId),
        PurifierDefeated: IsCompleted(tree, PurifierId),
        EradicatorDefeated: IsCompleted(tree, EradicatorId));

    private static bool? IsCompleted(AtlasTree tree, string areaId)
    {
        foreach (var node in tree.Nodes)
            if (node.AreaId == areaId) return node.Completed;
        return null;
    }
}
