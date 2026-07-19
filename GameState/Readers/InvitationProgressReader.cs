using ExileCore;

namespace AtlasHelper.GameState.Readers;

internal static class InvitationProgressReader
{
    private static readonly string[] StageCompleteFlags =
    {
        "MavensCrucibleStage1Complete",
        "MavensCrucibleStage2Complete",
        "MavensCrucibleStage3Complete",
        "MavensCrucibleStage4Complete",
        "MavensCrucibleStage5Complete",
    };

    public static AtlasInvitationProgress Read(GameController gc, QuestFlagLookup flags)
    {
        int? completedStage = null;
        for (int i = 0; i < StageCompleteFlags.Length; i++)
        {
            if (flags.Get(StageCompleteFlags[i]) == true)
                completedStage = i + 1;
        }

        var witnessed = gc.IngameState.Data.ServerData.MavenWitnessedAreas?.Count ?? 0;
        return new AtlasInvitationProgress(completedStage, witnessed);
    }
}
