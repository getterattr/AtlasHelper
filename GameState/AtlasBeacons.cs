namespace AtlasHelper.GameState;

public sealed record AtlasBeacons(
    bool? MavenBeacon,
    bool? SearingExarchBeacon,
    bool? EaterOfWorldsBeacon)
{
    public static AtlasBeacons Empty { get; } = new(null, null, null);
}
