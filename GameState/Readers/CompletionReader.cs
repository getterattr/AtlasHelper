using AtlasHelper.GameState.Atlas;
using ExileCore;

namespace AtlasHelper.GameState.Readers;

internal static class CompletionReader
{
    public static AtlasCompletion Read(GameController gc)
    {
        var server = gc.IngameState.Data.ServerData;
        var bonusNodes = server?.BonusCompletedNodes;
        var completedNodes = server?.CompletedNodes;

        if (bonusNodes == null || completedNodes == null)
            return AtlasCompletion.Empty;

        var normalBonus = 0;
        foreach (var node in bonusNodes)
        {
            if (node == null || node.IsUniqueMap) continue;
            normalBonus++;
        }

        var uniqueCompleted = 0;
        foreach (var node in completedNodes)
        {
            if (node == null || !node.IsUniqueMap) continue;
            uniqueCompleted++;
        }

        var uniqueBonus = uniqueCompleted < AtlasCompletion.UniqueBonusTarget
            ? uniqueCompleted
            : AtlasCompletion.UniqueBonusTarget;

        return new AtlasCompletion(normalBonus, uniqueBonus, completedNodes.Count);
    }
}
