namespace AtlasHelper.GameState;

public sealed record PinnacleCompletion(
    bool? Maven,
    bool? Shaper,
    bool? Elder,
    bool? SearingExarch,
    bool? EaterOfWorlds,
    bool? IncarnationOfDread,
    bool? Sirus,
    bool? Formed,
    bool? Twisted,
    bool? Elderslayers,
    bool? Feared,
    bool? Forgotten,
    bool? Remembered)
{
    public static PinnacleCompletion Empty { get; } = new(
        null, null, null, null, null, null, null,
        null, null, null, null, null, null);
}
