using System.Collections.Generic;
using System.Numerics;

namespace AtlasHelper.GameState.Atlas;

// Structural classification of an atlas node. Lets consumers switch on
// a stable enum instead of re-testing string patterns (has-Area /
// id-ends-with / id-starts-with) at every render call.
public enum AtlasNodeKind
{
    Other,           // catch-all - unrecognised structural node
    NormalMap,       // ordinary tiered map with an Area
    UniqueMap,       // IsUniqueMap = true (Whakawairua Tuahu, etc.)
    PinnacleBoss,    // no Area, tier 0, boss atlas icon (BlackStarBoss, ShaperBoss, ...)
    VoidstoneSlot,   // no Area, tier 0, id ends in "WatchstoneSlotNode"
    MemoryMap,       // Eagon's memory-thread maps (Courtyard, Chambers, Theatre)
    SynthesisNode,   // Cortex and Distant Memories
}

public sealed record AtlasMapNode(
    string AreaId,
    string AreaName,
    int BaseTier,
    Vector2 Position,
    bool IsUnique,
    bool GrantsBonus,
    IReadOnlyList<string> ConnectedAreaIds,
    bool Completed,
    bool BonusCompleted,
    AtlasNodeKind Kind);

public sealed record AtlasTree(IReadOnlyList<AtlasMapNode> Nodes)
{
    public int NodeCount => Nodes.Count;

    public static AtlasTree Empty { get; } = new(new List<AtlasMapNode>());
}
