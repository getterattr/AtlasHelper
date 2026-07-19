using System.Collections.Generic;

namespace AtlasHelper.GameState.Maven;

// Names of map bosses the Maven has witnessed. Feeds the AtlasInvitation
// ladder (each ladder stage requires N distinct witnessed bosses at a
// minimum tier).
public sealed record Witnesses(IReadOnlyList<string> AreaNames)
{
    public int Count => AreaNames.Count;

    public static Witnesses Empty { get; } = new(new List<string>());
}
