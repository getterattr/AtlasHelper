# League-Start Strategy

Source-of-truth for the tactical business rules the plugin encodes. Distilled from definitive/Cyclone's 3.29 "How to get to Endgame FAST" guide.

Complements the other internal docs:
- [glossary.md](glossary.md) - canonical vocabulary
- [questline.md](questline.md) - wiki-sourced game facts (voidstone sources, Maven stages, invitations)
- [context.md](context.md) - design decisions and scope
- [adr/](adr/) - individual architectural decisions

This document describes *what the plugin advises*, not *how it renders it*.

## Voidstone acquisition order

Fixed, independent of build:

1. **Eldritch** (bottom-left) - Exarch + Eater. Easiest and unlocks Exarch altars for early currency.
2. **Originator** (top-left) - Eagon's chain. Naturally combines with bonus completion via Eagon's maps.
3. **Decayed** (top-right) - Shaper + Elder. Buy carry (mapping build) or self-farm (bossing build).
4. **Ceremonial** (bottom-right) - Maven's Writ. Buy carry or self-farm.

Voidstones 3 and 4 are typically purchased. Self-farming is only advised for a build that has explicitly picked Destructive Play.

## Phases

Aligned with the ordered voidstone list above. See [glossary.md#phase](glossary.md).

### Phase 1 - First voidstone (Eldritch)

Goal: reach T16 in the bottom-left corner, complete Exarch and Eater chains, socket Eldritch.

Two sub-passes through the atlas:

**Pass A - Straight-line rush to T11.** Run maps T1 through T11 in the bottom-left, hitting the highest available tier each map. Unlock two T11 nodes so both the Polaric Invitation (Exarch chain) and the equivalent Eater chain step are runnable. Rare/magic rarity is fine; do *not* skip bonus completion on this pass.

**Pass B - Magic red maps for questline only.** Once yellow is cleared, jump straight to red maps. Run the first ~6 red maps (T11-T16) *magic-rolled* to make the Exarch/Eater chain progress cheaper and safer. Bonus completion is *sacrificed* on these because Pass B in Phase 3 will re-complete them via the higher-tier-on-lower-node rule (see below).

Envoy handoff (Luminous Astrolabe, Flesh Compass) triggers naturally in yellow maps. Chain-required tiers override the "run highest tier" rule while a chain step is gating progress.

### Phase 2 - Ten-way Maven and second voidstone (Originator)

Begins *before* Phase 1 completes. Overlap is expected.

**Eagon steering.** Eagon spawns on the player's first T14+ run when no chain is active. His three memory maps (Courtyard of Wasting, Chambers of Impurity, Theatre of Lies) are run *magic-rolled* with map-drop rolls stacked. This yields bonus completion + questline progression + T14-T16 map sustain simultaneously. Eagon respawns after each chain finishes; player can steer *which* T14 to run second to influence where he next appears.

**Maven parallel farm.** Keep Maven's Beacon toggled on the Map Device whenever running an eligible-tier map with an un-witnessed boss. Feeds the 3-way -> 4-way -> 5-way -> 6-way -> 10-way ladder. The 10-way is the repeating splinter source and the target for this phase.

Phase 2 ends when both:
- Originator voidstone dropped from Incarnation of Dread.
- 10-way Maven is unlocked (fourth scarab slot).

### Phase 3 - 100% bonus completion

After voidstones 1 and 2 are socketed, tiers are boosted by 8. Any remaining unbonused map can now be re-run at a *higher* effective tier through the "run higher tier on lower node" rule (post-3.28), which repays the bonus deficit created in Phase 1 Pass B.

Order within Phase 3:
1. Unfinished white maps, magic-rolled.
2. Unfinished yellow maps, rare-rolled.
3. Unfinished red maps.

Unique maps: 10-completion cap. Any 10 uniques satisfy the cap. Player may skip disliked uniques.

### Phase 4 - Final voidstones (Decayed, Ceremonial)

Two variants:

**Mapping build (default).** Buy Shaper, Elder, and Maven carries. Plugin surfaces the shopping list.

**Bossing build (Destructive Play allocated).** Self-farm:
- Shaper and Elder fragments via Guardian maps.
- Maven's Writ via 10 Crescent Splinters (from 10-way and themed invitations).
- Themed invitations - The Formed, The Twisted, The Elderslayers, The Forgotten - are all repeatable splinter sources.

## Run priority

Rule set for "what map do I run next":

1. **Highest-tier-available.** Default rule. Always run the highest atlas tier the player has unlocked.
2. **Chain-gating override.** If Exarch, Eater, or Maven chain progression requires a specific lower tier *right now*, run that tier instead. Example: needing Polaric (T11 Exarch boss) means running a T11 with Exarch influence even if T13s are unlocked.
3. **Egon-steering override.** If Egon is spawned and his maps are outstanding, run his maps at the current highest tier they can be pushed to.
4. **Tier-boost exception in Phase 3.** After voidstones 1-2 are socketed, prefer *unbonused lower-tier* maps that can now be run at higher effective tier for bonus completion.

## In-map loop

Fixed tactical loop the plugin surfaces via HUD once the player enters a map:

1. **Rush to the map boss.** Boss kill is the highest map-drop-probability event. Loot en route only if opportunistic.
2. **Check for map drop.** If a map dropped, portal out. Move to the next map.
3. **Shrine (if allocated).** If no map dropped, path to the map's shrine, kill the pack it spawns with. Hope for a drop.
4. **League mechanic.** Path to the league mechanic (3.29: Mirage cache; 3.30+: whatever cache the current league provides). Not for reward - for the chance of a map drop.
5. **Leave.** No further mechanics. Random loot does not compensate for time cost when the primary goal is tier progression.

The HUD's "bother with this map: yes/no" line summarises steps 2-4 for the current map based on drop state.

## Rarity rules

Bonus completion requires the map be run at or above the rarity printed on its atlas node:

- **White maps (T1-T5)**: magic sufficient.
- **Yellow maps (T6-T10)**: rare required.
- **Red maps (T11-T16)**: rare and corrupted required.

Questline-only runs (Phase 1 Pass B red maps, Eagon's memory maps when he is spawned) may be run *magic* to save time and risk when bonus completion is not the goal on that run - it will be repaid in Phase 3 via the higher-tier-on-lower-node rule.

Phase 3 re-runs match the rarity rule above (magic whites, rare yellows, rare-corrupted reds).

## Guaranteed spawns and steerable events

- **Envoy**: yellow map, T6+. Hands over Luminous Astrolabe and Flesh Compass on first meeting.
- **Eagon**: first T14+ map run when no chain is active. Steerable by choosing which T14 to run.
- **Maven witness**: any eligible-tier map with Beacon toggled on and boss un-witnessed for the current stage.
- **Kirac mission**: opens invitations as prerequisites are met.

