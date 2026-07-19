using System.Collections.Generic;
using System.Numerics;

namespace AtlasHelper.GameState.Atlas;

public sealed record AtlasMapNode(
    string AreaId,
    string AreaName,
    int BaseTier,
    Vector2 Position,
    bool IsUnique,
    bool GrantsBonus,
    IReadOnlyList<string> ConnectedAreaIds,
    bool Completed,
    bool BonusCompleted);

public sealed record AtlasTree(IReadOnlyList<AtlasMapNode> Nodes)
{
    public int NodeCount => Nodes.Count;

    public static AtlasTree Empty { get; } = new(new List<AtlasMapNode>());
}
