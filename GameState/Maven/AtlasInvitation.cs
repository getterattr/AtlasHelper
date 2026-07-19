namespace AtlasHelper.GameState.Maven;

// Maven's Invitation: The Atlas - the 5-stage witness ladder.
//   Stage 1: 3-way  (3 bosses at T6+)
//   Stage 2: 4-way  (4 bosses at T8+)
//   Stage 3: 5-way  (5 bosses at T10+)
//   Stage 4: 6-way  (6 bosses at T12+)
//   Stage 5: 10-way (10 bosses at T14+, repeatable)
public sealed record AtlasInvitation(
    int? CompletedStage,
    int WitnessedBossCount)
{
    public const int FinalStage = 5;

    private static readonly int[] StageWitnessTargets = { 3, 4, 5, 6, 10 };

    public bool QuestlineComplete => CompletedStage is >= FinalStage;

    public int? NextStage => CompletedStage switch
    {
        >= FinalStage => null,
        null => 1,
        var s => s + 1,
    };

    public int? NextStageWitnessTarget =>
        NextStage is int stage ? StageWitnessTargets[stage - 1] : null;

    public int WitnessProgressCapped =>
        NextStageWitnessTarget is int target
            ? (WitnessedBossCount > target ? target : WitnessedBossCount)
            : 0;

    public static AtlasInvitation Empty { get; } = new(null, 0);
}
