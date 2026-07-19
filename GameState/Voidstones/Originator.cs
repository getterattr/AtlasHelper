namespace AtlasHelper.GameState.Voidstones;

// Top-left corner. Eagon's Threads of the Originator memory questline gates
// the voidstone. The terminal Incarnation of Dread kill lives in
// Pinnacles.PinnacleBosses; this module owns intermediate chain progression:
// Eagon met + three memory maps cleared + the two mid-tier Incarnations
// (Fear and Neglect - Dread is the final and lives in Pinnacles).
public sealed record Originator(
    bool? EagonMet,
    bool? EagonIntroductionSeen,
    bool? CourtyardCleared,
    bool? ChambersCleared,
    bool? TheatreCleared,
    bool? IncarnationOfNeglectDefeated,
    bool? IncarnationOfFearDefeated)
{
    public static Originator Empty { get; } =
        new(null, null, null, null, null, null, null);
}
