using AtlasHelper.GameState.Maven;
using ExileCore;

namespace AtlasHelper.GameState.Readers;

internal static class AtlasInvitationReader
{
    private const string MavenAtlasQuestId = "maven_atlas";

    public static AtlasInvitation Read(GameController gc, QuestFlagLookup flags)
    {
        int? stateId = null;
        var quests = gc.IngameState?.IngameUi?.GetQuests;
        if (quests != null)
        {
            foreach (var tuple in quests)
            {
                if (tuple.Item1?.Id == MavenAtlasQuestId)
                {
                    stateId = tuple.Item2;
                    break;
                }
            }
        }

        return new AtlasInvitation(
            BeaconAcquired: flags.Get(AtlasQuestFlags.Maven.AtlasLadder.BeaconAcquired),
            StateId: stateId,
            WitnessedBossCount: gc.IngameState.Data.ServerData.MavenWitnessedAreas?.Count ?? 0);
    }
}
