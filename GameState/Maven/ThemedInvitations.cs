namespace AtlasHelper.GameState.Maven;

// Themed Maven's Invitations - Kirac-opened Crucible fights against
// pre-composed boss sets. All drop Crescent Splinters and feed the same
// Writ economy as the Atlas ladder.
//
//   Formed        - 4 Shaper Guardians: Chimera, Hydra, Minotaur, Phoenix
//   Twisted       - 4 Elder Guardians: Enslaver, Constrictor, Purifier, Eradicator
//   Elderslayers  - 4 Conquerors: Baran, Veritania, Al-Hezmin, Drox
//   Forgotten     - 4 Synthesis bosses (incl. Rewritten Synthete)
//   Remembered    - 3 memory-thread bosses (Courtyard, Chambers, Theatre)
//   Feared        - 5 endgame bosses simultaneously, no phase transitions
public sealed record ThemedInvitations(
    bool? Formed,
    bool? Twisted,
    bool? Elderslayers,
    bool? Forgotten,
    bool? Remembered,
    bool? Feared)
{
    public static ThemedInvitations Empty { get; } = new(null, null, null, null, null, null);
}
