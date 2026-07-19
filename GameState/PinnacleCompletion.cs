namespace AtlasHelper.GameState;

public sealed record PinnacleCompletion(
    bool? Formed,
    bool? Feared,
    bool? Twisted,
    bool? UberElder,
    bool? Elderslayers,
    bool? Remembered,
    bool? Forgotten)
{
    public static PinnacleCompletion Empty { get; } = new(null, null, null, null, null, null, null);
}
