# Scope: league-start only

The plugin's value is the transcript-derived Phase machine that guides a fresh character from empty atlas to four socketed voidstones. It is not a general-purpose atlas helper.

## In scope

- The four ordered phases from definitive/Cyclone's league-start guide.
- Advisory HUD: current Phase, voidstones (0-4), bonus completion %, Exarch/Eater chain progress, Maven witness count, in-map "bother yes/no" line.
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
