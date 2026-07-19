namespace AtlasHelper.GameState.Atlas;

public sealed record AtlasCompletion(
    int NormalBonusCount,
    int UniqueBonusCount,
    int TotalCompletedCount)
{
    public const int NormalBonusTarget = 100;
    public const int UniqueBonusTarget = 10;

    public bool NormalBonusComplete => NormalBonusCount >= NormalBonusTarget;
    public bool UniqueBonusComplete => UniqueBonusCount >= UniqueBonusTarget;

    public static AtlasCompletion Empty { get; } = new(0, 0, 0);
}
