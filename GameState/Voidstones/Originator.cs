namespace AtlasHelper.GameState.Voidstones;

// Top-left corner. Eagon's Threads of the Originator memory questline gates
// the voidstone. The terminal Incarnation of Dread kill lives in
// Pinnacles.PinnacleBosses; this module owns only the intermediate chain
// progression (Eagon met, three memory maps cleared).
public sealed record Originator(
    bool? EagonIntroduced,
    bool? CourtyardCleared,
    bool? ChambersCleared,
    bool? TheatreCleared)
{
    public static Originator Empty { get; } = new(null, null, null, null);
}
