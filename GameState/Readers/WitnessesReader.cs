using System.Collections.Generic;
using AtlasHelper.GameState.Maven;
using ExileCore;

namespace AtlasHelper.GameState.Readers;

internal static class WitnessesReader
{
    public static Witnesses Read(GameController gc)
    {
        var witnessed = gc.IngameState.Data.ServerData.MavenWitnessedAreas;
        if (witnessed == null)
            return Witnesses.Empty;

        var names = new List<string>(witnessed.Count);
        foreach (var area in witnessed)
            names.Add(area.Name);

        return new Witnesses(names);
    }
}
