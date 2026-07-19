namespace AtlasHelper.GameState;

public sealed record AtlasInvitationProgress(
    int? CompletedStage,
    int WitnessedBossCount)
{
    public const int FinalStage = 5;

    public bool QuestlineComplete => CompletedStage is >= FinalStage;

    public int? NextStage => CompletedStage switch
    {
        null => null,
        >= FinalStage => null,
        var s => s + 1,
    };

    public static AtlasInvitationProgress Empty { get; } = new(null, 0);
}
