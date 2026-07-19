# AtlasHelper

A lightweight ExileApi overlay that guides Path of Exile players through the fastest league-start atlas progression: empty atlas to four voidstones.

## What it does

AtlasHelper offloads the bookkeeping and pathing decisions from the standard league-start rush strategy (four ordered phases: first voidstone -> ten-way Maven -> 100% bonus completion -> final voidstones). It is advisory, read-only, and stays out of the way once the atlas is done.

## Surfaces

1. **HUD panel** - always-on ImGui window showing current Phase, voidstones (0-4), atlas bonus completion %, Exarch/Eater Quest Chain progress, Maven witness count (X/10), and a "bother with this map: yes/no" line while in a map.
2. **Atlas map-tree overlay** - draws only on the atlas screen. Highlights the advisory set of maps satisfying the current Phase's rules, marks the target Corner, annotates each node with tier, bonus-done state, and Quest Chain relevance.
3. **In-map boss-path arrow** - uses ExileApi Areas plus the Stranded pathfinder pattern to point at the current map's boss arena. The transcript's tactical loop is "boss -> shrine -> league mechanic -> leave"; getting to the boss fast is 80% of the value.

## Explicitly not included

- Map inventory highlighting (deferred to v2)
- Atlas passive-tree overlay or preset guidance (external guides do this better)
- Kirac steering suggestions
- Per-character persistence or session logs
- Any automation

## Configuration

All user settings live in the standard `BaseSettingsPlugin` file - plugin-global, not per-character:

- Phase override (default: inferred from game state)
- Branch selector: Exarch Altars vs Destructive Play
- Show/hide toggles per surface

Switching characters mid-league requires no plugin action; state redraws itself from the new character's memory.

## Internal docs

- `.claude/docs/internal/glossary.md` - canonical domain vocabulary
- `.claude/docs/internal/adr/` - architectural decision records
  - `0001-scope-league-start-only.md`
  - `0002-derived-state-primary.md`

## Status

Design locked, not yet implemented. Next action is a **blocking spike**: verify via the ExileApi MCP bridge that each of the following is readable from game memory. Any value that isn't readable collapses to a manual setting.

- Voidstone count (0-4)
- Maven witness count (X/10 toward the ten-way invitation)
- Per-map bonus-completion state
- Current Exarch and Eater Quest Chain step
- Atlas map-tree node data (map name, tier, connections, current position)

Once the spike settles, implementation proceeds in this order: HUD panel -> atlas map-tree overlay -> in-map boss-path arrow.

## Reference

Strategy is derived from definitive/Cyclone's 3.29 rush guide. AtlasHelper does not reproduce the atlas passive tree presets; consult the guide author's site for those.
