using AtlasHelper.GameState.Maven;
using ExileCore;

namespace AtlasHelper.GameState.Readers;

internal static class AtlasInvitationReader
{
    private static readonly string[] StageCompleteFlags =
    {
        AtlasQuestFlags.Maven.AtlasLadder.Stage1,
        AtlasQuestFlags.Maven.AtlasLadder.Stage2,
        AtlasQuestFlags.Maven.AtlasLadder.Stage3,
        AtlasQuestFlags.Maven.AtlasLadder.Stage4,
        AtlasQuestFlags.Maven.AtlasLadder.Stage5,
    };

    public static AtlasInvitation Read(GameController gc, QuestFlagLookup flags)
    {
        int? completedStage = null;
        for (int i = 0; i < StageCompleteFlags.Length; i++)
        {
            if (flags.Get(StageCompleteFlags[i]) == true)
                completedStage = i + 1;
        }

        var witnessed = gc.IngameState.Data.ServerData.MavenWitnessedAreas?.Count ?? 0;
        return new AtlasInvitation(completedStage, witnessed);
    }
}
