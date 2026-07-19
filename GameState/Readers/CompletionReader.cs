using ExileCore;

namespace AtlasHelper.GameState.Readers;

internal static class CompletionReader
{
    public static AtlasCompletion Read(GameController gc)
    {
        var server = gc.IngameState.Data.ServerData;
        var bonusNodes = server.BonusCompletedNodes;
        var completedNodes = server.CompletedNodes;

        if (bonusNodes == null || completedNodes == null)
            return AtlasCompletion.Empty;

        int normal = 0, unique = 0;
        foreach (var node in bonusNodes)
        {
            if (node.IsUniqueMap) unique++;
            else normal++;
        }

        return new AtlasCompletion(normal, unique, completedNodes.Count);
    }
}
