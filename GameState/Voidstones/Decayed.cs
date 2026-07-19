namespace AtlasHelper.GameState.Voidstones;

// Top-right corner. Shaper and Elder fragments come from their respective
// guardian sets. Terminal Shaper and Elder kills live in
// Pinnacles.PinnacleBosses; this module owns only the guardian kill state.
public sealed record Decayed(
    bool? ChimeraDefeated,
    bool? HydraDefeated,
    bool? MinotaurDefeated,
    bool? PhoenixDefeated,
    bool? EnslaverDefeated,
    bool? ConstrictorDefeated,
    bool? PurifierDefeated,
    bool? EradicatorDefeated)
{
    public int ShaperGuardiansDefeated =>
        CountTrue(ChimeraDefeated, HydraDefeated, MinotaurDefeated, PhoenixDefeated);

    public int ElderGuardiansDefeated =>
        CountTrue(EnslaverDefeated, ConstrictorDefeated, PurifierDefeated, EradicatorDefeated);

    public bool ShaperFragmentsReady => ShaperGuardiansDefeated == 4;
    public bool ElderFragmentsReady => ElderGuardiansDefeated == 4;

    private static int CountTrue(params bool?[] values)
    {
        var count = 0;
        foreach (var v in values)
            if (v == true) count++;
        return count;
    }

    public static Decayed Empty { get; } =
        new(null, null, null, null, null, null, null, null);
}
