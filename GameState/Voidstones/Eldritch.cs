namespace AtlasHelper.GameState.Voidstones;

// Bottom-left corner. Two chains gate the voidstone: Searing Exarch and
// Eater of Worlds. Terminal boss kills live in Pinnacles.PinnacleBosses;
// this module owns only the intermediate chain-progression state.

public sealed record ExarchChain(
    bool? EnvoyMet,
    bool? InfluenceUnlocked,
    bool? PolaricInvitationDropped,
    bool? BlackStarDefeated,
    bool? IncandescentInvitationDropped)
{
    public static ExarchChain Empty { get; } =
        new(null, null, null, null, null);
}

public sealed record EaterChain(
    bool? FleshCompassReceived,
    bool? InfluenceUnlocked,
    bool? ScreamingInvitationDropped,
    bool? InfiniteHungerDefeated)
{
    public static EaterChain Empty { get; } =
        new(null, null, null, null);
}

public sealed record Eldritch(ExarchChain Exarch, EaterChain Eater)
{
    public static Eldritch Empty { get; } = new(ExarchChain.Empty, EaterChain.Empty);
}
