namespace AtlasHelper.GameState.Pinnacles;

// Individual pinnacle boss kill state. Sole owner of these facts - the
// Voidstones module tracks chain progression toward each boss but does not
// duplicate the terminal kill.
public sealed record PinnacleBosses(
    bool? Maven,
    bool? Shaper,
    bool? Elder,
    bool? SearingExarch,
    bool? EaterOfWorlds,
    bool? IncarnationOfDread,
    bool? Sirus)
{
    public static PinnacleBosses Empty { get; } = new(
        null, null, null, null, null, null, null);
}
