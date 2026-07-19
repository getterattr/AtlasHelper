# Scope: league-start only

The plugin's value is the transcript-derived Phase machine that guides a fresh character from empty atlas to four socketed voidstones. It is not a general-purpose atlas helper.

## In scope

- The four ordered phases from definitive/Cyclone's league-start guide.
- Advisory HUD: current Phase, voidstones (0-4), bonus completion %, Exarch/Eater chain progress, Maven witness count, static in-map loop reminder (rush boss, check drop, one drop-friendly event, leave).
- Atlas map-tree overlay highlighting maps that satisfy current-Phase rules.
- In-map boss-path arrow, using ExileApi `Areas` plus the `Repositories/Stranded/Pathfinder/` pattern.

## Out of scope

- Late-league mechanics (deep delve, aspirational farming strategies, endgame currency).
- Map inventory highlighting - requires modelling held-map items and detecting inventory panel state.
- Atlas passive-tree overlay - high effort for a 4-times-per-league decision; guide-author images already solve it. The plugin names specific atlas passives when they gate a Phase step (e.g. Polaric Void, Seething Chime) but does not ship a preset UI.
- Kirac steering suggestions - not in [strategy.md](../strategy.md).
- Per-character persistence, session logs, analytics.
- Automation - rejected on TOS-risk and complexity grounds. Value at league start is cognitive offload, not mechanical speed.

## Style: advisory, not prescriptive

Highlight the *set* of maps satisfying current-Phase rules; the user picks. Prescriptive ("run this exact map") requires modelling held-map inventory and was rejected. Pure informational ("here is the strategy") was rejected as wasting the captured guide.

## No post-hoc lints, no static-rule readers

Two categories of feature that superficially look useful were rejected because they add readers or services without earning them:

- **Post-hoc "you did it wrong" nags.** Reading the current map's rarity + corruption to warn "this run won't credit bonus" was rejected: the choice is already committed, the player already opened the map, and the warning is prescriptive-shaped rather than advisory. Same for "you forgot to toggle Maven's Beacon" style warnings.
- **Readers for constants.** The rarity-per-tier-band rule (whites need magic, yellows rare, reds rare+corrupted) is a deterministic function of tier - a value the plugin already has from `AtlasTree`. No `Rarity.cs` service, no current-map rarity reader. The rule is a static reminder rendered inline. Same for map-device toggle state: the advice ("keep Beacon toggled in Phase 2") is derivable from the Phase and is a static reminder; reading the toggle to confirm the player did it is a post-hoc lint.

The load-bearing test for any new reader or `Services/*.cs` file: does it inform an *upcoming* decision from state the plugin doesn't already have? If not, cut it.
