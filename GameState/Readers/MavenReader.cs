using System.Collections.Generic;
using ExileCore;

namespace AtlasHelper.GameState.Readers;

internal static class MavenReader
{
    public static MavenState Read(GameController gc)
    {
        var witnessed = gc.IngameState.Data.ServerData.MavenWitnessedAreas;
        if (witnessed == null)
            return MavenState.Empty;

        var names = new List<string>(witnessed.Count);
        foreach (var area in witnessed)
            names.Add(area.Name);

        return new MavenState(names);
    }
}
