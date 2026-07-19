using System.Collections.Generic;

namespace AtlasHelper.GameState;

public sealed record AtlasPassive(int PassiveId, string InternalName, string DisplayName);

public sealed record AtlasPassives(IReadOnlyList<AtlasPassive> Allocated)
{
    public int Count => Allocated.Count;

    public static AtlasPassives Empty { get; } = new(new List<AtlasPassive>());
}
