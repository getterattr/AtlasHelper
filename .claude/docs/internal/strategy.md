# League-Start Strategy

Tactical business rules the plugin encodes. Distilled from definitive/Cyclone's 3.29 "How to get to Endgame FAST" guide. Describes *what the plugin advises*, not *how it renders it*.

## Voidstone acquisition order

Fixed, independent of build:

1. **Eldritch** (bottom-left) - Exarch + Eater.
2. **Originator** (top-left) - Eagon's chain, run alongside bonus completion.
3. **Decayed** (top-right) - Shaper + Elder, self-farmed via Guardians.
4. **Ceremonial** (bottom-right) - Maven's Writ, self-farmed via splinters.

## Phases

Aligned with the ordered voidstone list above. See [glossary.md#phase](glossary.md).

### Phase 1 - First voidstone (Eldritch)

Goal: reach T16 in the bottom-left corner, complete Exarch and Eater chains, socket Eldritch.

Two sequential steps:
1. **Rush T1 -> T11.** Highest tier available each map, taking bonus completion along the way. The two Phase-1 pinnacle-fight icons live in the bottom-left (Eldritch) quadrant of the atlas: **Polaric Void** (the Black Star arena, entered via the Polaric Invitation dropped from any T11+ Exarch-influenced map) and **Seething Chyme** (the Infinite Hunger arena, entered via the Writhing Invitation). Both are `map_not_on_atlas` off-atlas instances, but their entry icons on the atlas (internal ids `BlackStarBoss` and `InfiniteHungerBoss`) are pathable and are the two intermediate stops the plugin routes through.
2. **T11 -> T16 questline sprint.** Skip second-pass yellows. Run one map per red tier (T11, T12, T13, T14, T15, T16) *magic-rolled* to progress the Exarch and Eater chains. Bonus is intentionally deferred - Phase 3's tier-boost re-runs pay it back.

### Phase 2 - Ten-way Maven and second voidstone (Originator)

Begins *before* Phase 1 completes. Overlap is expected.

**Eagon steering.** Eagon first spawns on the player's first T14+ run when no chain is active, and respawns whenever the previous chain finishes. To exploit this, open a T14+ map item onto an *unfinished white atlas node* (T5 or lower) in another quadrant and run it magic-rolled. Eagon's memory tear spawns on that white node, which credits its bonus completion (whites only need magic) while progressing his chain. Repeat across unfinished whites in every quadrant.

**Maven parallel farm.** Keep Maven's Beacon toggled on the Map Device whenever running an eligible-tier map with an un-witnessed boss. Feeds the 3-way -> 4-way -> 5-way -> 6-way -> 10-way ladder. The 10-way is the repeating splinter source and the target for this phase.

Phase 2 ends when both:
- Originator voidstone dropped from Incarnation of Dread.
- 10-way Maven completed (unlocks the fourth scarab slot on the Map Device).

### Phase 3 - 100% bonus completion

Complete bonus on every remaining non-unique atlas node. Run Priority rule 4 (tier-boost exception) applies throughout.

Order:
1. Unfinished white maps.
2. Unfinished yellow maps.
3. Unfinished red maps.

Unique maps: 10-completion cap, any 10 uniques satisfy it.

### Phase 4 - Final voidstones (Decayed, Ceremonial)

Self-farm both:
- **Decayed**: Shaper and Elder fragments via Guardian maps (Chimera/Hydra/Minotaur/Phoenix for Shaper; Enslaver/Constrictor/Purifier/Eradicator for Elder). Fight Shaper, then Elder.
- **Ceremonial**: 10 Crescent Splinters combined into The Maven's Writ. Splinters come from the repeatable 10-way and from themed invitations (see [questline.md](questline.md#themed-mavens-invitations)).

## Run priority

Rule set for "what map do I run next":

1. **Highest-tier-available.** Default rule. Always run the highest atlas tier the player has unlocked.
2. **Chain-gating override.** If Exarch, Eater, or Maven chain progression requires a specific lower tier *right now*, run that tier instead. Example: needing Polaric Void (T11 Exarch boss) means running a T11 with Exarch influence even if T13s are unlocked.
3. **Eagon override.** If Eagon is spawned and his memory maps are outstanding, slot a T14+ map item onto an unfinished *white* atlas node in another quadrant (see Phase 2 - Eagon steering).
4. **Tier-boost exception in Phase 3.** After voidstones 1-2 are socketed, prefer *unbonused lower-tier* maps that can now be run at higher effective tier for bonus completion.

## In-map loop

Fixed tactical loop the plugin surfaces via HUD once the player enters a map:

1. **Rush to the map boss.** Boss kill is the highest map-drop-probability event. Loot en route only if opportunistic.
2. **Check for map drop.** If a map dropped, portal out. Move to the next map.
3. **One more drop chance.** If no map dropped, run one nearby drop-friendly event - shrine pack, league mechanic - then re-check.
4. **Leave.** No further mechanics.

The HUD renders these four steps as a static reminder while the player is in a map. Dynamic "bother yes/no" reasoning is not attempted: map-item drop detection is out of scope ([decisions/scope.md](decisions/scope.md)), and a nag telling the player they can leave is post-hoc lint territory.

## Route planning

Reaching a voidstone corner slot to socket the voidstone requires unlocking a path of adjacent maps. The plugin routes on the atlas graph **from the corner back toward the completed-frontier** — the nearest node the player has already completed. This reveals the shortest sequence of maps that must be run to unlock the corner.

- **Start**: the voidstone corner slot (Eldritch bottom-left, Originator top-left, Decayed top-right, Ceremonial bottom-right).
- **Destination**: nearest node with `Completed = true`. Fallback when no maps are completed yet is TBD.
- **Play order**: reverse the returned path - from the completed frontier walking outward to the corner.

For Phase 1 specifically, the two intermediate Eldritch-chain pinnacle-fight icons in the bottom-left quadrant are required stops on the route from the Eldritch corner outward: **Polaric Void** (Black Star arena, atlas icon `BlackStarBoss`) and **Seething Chyme** (Infinite Hunger arena, atlas icon `InfiniteHungerBoss`). Both fights must be completed to progress the Exarch and Eater chains toward the T16 final fights that drop the Eldritch voidstone.

## Rarity rules

Bonus completion requires the map be run at or above the rarity printed on its atlas node:

- **White maps (T1-T5)**: magic sufficient.
- **Yellow maps (T6-T10)**: rare required.
- **Red maps (T11-T16)**: rare and corrupted required.

