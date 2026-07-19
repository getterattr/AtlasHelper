using System.Collections.Generic;
using System.Numerics;

namespace AtlasHelper.GameState;

public sealed record AtlasMapNode(
    string AreaId,
    string AreaName,
    int BaseTier,
    Vector2 Position,
    bool IsUnique,
    IReadOnlyList<string> ConnectedAreaIds,
    bool BonusCompleted);

public sealed record AtlasTree(IReadOnlyDictionary<string, AtlasMapNode> NodesByAreaId)
{
    public int NodeCount => NodesByAreaId.Count;

    public static AtlasTree Empty { get; } = new(new Dictionary<string, AtlasMapNode>());
}
