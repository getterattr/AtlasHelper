namespace AtlasHelper.GameState.Maven;

// Maven's Invitation: The Atlas (3/4/5/6/10-way Crucible ladder).
// StateId is the character's current position in the maven_atlas quest,
// read from IngameUi.GetQuests. State ids run BACKWARD in POE: 16 is
// the earliest step (pre-beacon) and 0 is quest complete.
public sealed record AtlasInvitation(
    bool? BeaconAcquired,
    int? StateId,
    int WitnessedBossCount)
{
    public const int FinalStage = 5;

    private static readonly int[] StageWitnessTargets = { 3, 4, 5, 6, 10 };

    // Highest ladder stage fully completed (0..5), derived from the
    // maven_atlas quest StateId.
    //
    // Verified from Files.QuestStates.QuestStateText:
    //   state 0        = "Quest Complete" (all 5 stages done)
    //   state 16       = "Maven wants to witness..." (pre-beacon)
    //   state 15       = "Maven has gifted you her Beacon"
    //   state 14       = "Beacon appears to be compatible..."
    //   states 13,11,9,7,5 = "Call the Maven to Tier {0}+..." (witnessing)
    //   states 12,10,8,6,4 = "Talk to Kirac..." (invitation ready)
    //   state 3        = "Enter the Maven's Crucible..."
    //   state 2        = "Use the device to begin..."
    //   state 1        = "Defeat all of the Maven's creations."
    //
    // TODO(unverified): the mapping of witness/invite-ready state pairs
    // to specific N-way stages (13->3-way, 11->4-way, 9->5-way,
    // 7->6-way, 5->10-way) is inferred from ordering and the wiki's
    // ladder progression, NOT proved. The state templates share the
    // string "Tier {0}+ ... out of {2}" - tier and target are runtime
    // placeholders, so Files.QuestStates alone cannot disambiguate. To
    // ground this, watch a character's StateId transition across a
    // stage completion, or cross-reference Files.QuestRewards to see
    // which state grants the "1 atlas passive skill point" reward
    // (each stage completion grants one).
    //
    // Similarly, states 3/2/1 are treated as "stage 5 in progress"
    // here, but they may actually apply to any in-Crucible run (all
    // five stages share the enter-portals -> use-device -> fight flow).
    // If so, StateId in 1..3 tells us the player is IN a Crucible run
    // but not which one - so CompletedStage would be undefined at
    // those points.
    public int? CompletedStage => StateId switch
    {
        null => null,
        0 => FinalStage,
        >= 1 and <= 5 => 4,
        6 or 7 => 3,
        8 or 9 => 2,
        10 or 11 => 1,
        12 or 13 => 0,
        _ => 0,
    };

    public bool QuestlineComplete => StateId == 0;

    public int? NextStage => CompletedStage switch
    {
        null => null,
        >= FinalStage => null,
        var s => s + 1,
    };

    public int? NextStageWitnessTarget =>
        NextStage is int stage ? StageWitnessTargets[stage - 1] : null;

    public int WitnessProgressCapped =>
        NextStageWitnessTarget is int target
            ? (WitnessedBossCount > target ? target : WitnessedBossCount)
            : 0;

    public static AtlasInvitation Empty { get; } = new(null, null, 0);
}
