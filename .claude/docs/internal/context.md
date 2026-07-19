# AtlasHelper - Design Context

Narrative record of the design grill that shaped the plugin. Complements the glossary (vocabulary only) and the ADRs (individual decisions in isolation) by preserving the flow of the conversation: what was asked, what was chosen, what was rejected, and why.

## Origin

The plugin was scoped from a single reference: definitive/Cyclone's 3.29 "How to get to Endgame FAST" YouTube guide. The guide decomposes league-start atlas progression into four ordered phases:

1. First voidstone (bottom-left, Exarch + Eater)
2. Ten-way Maven + second voidstone (top-left, Originator)
3. 100% bonus completion
4. Final voidstones (Shaper/Elder, top-right and bottom-right)

Everything in the plugin is downstream of this strategy. See [glossary](glossary.md) for the canonical terms.

## Decisions

### Purpose: guide + tracker, no automation
The value at league start is cognitive offload ("what map next, which bonus am I missing, am I on pace"), not mechanical speed. Automation was rejected on both TOS-risk and complexity grounds.

### Scope: league-start only
Captured in [ADR 0001](adr/0001-scope-league-start-only.md). Rejected the "general-purpose atlas helper" alternative - the transcript-derived Phase machine is the plugin's entire value; generalising dilutes it.

### Surfaces (in scope)
1. **HUD panel** - always-on ImGui window: Phase, voidstones (0-4), bonus completion %, Exarch/Eater chain progress, Maven witness count, "bother with this map: yes/no" line while in-map.
2. **Atlas map-tree overlay** - draws on the atlas screen. Highlights advisory set of maps satisfying current-Phase rules, marks target Corner, per-node annotations (tier, bonus done, chain relevance).
3. **In-map boss-path arrow** - uses ExileApi `Areas` plus the Stranded plugin's Pathfinder pattern (`Repositories/Stranded/Pathfinder/`).

### Surfaces (out of scope)
- **Map inventory highlighting** - would require modelling held map items and detecting inventory panel state. Not worth the complexity.
- **Atlas passive-tree overlay** - high effort (zoomable/pannable node coordinates) for a 4-times-per-league decision. Static images on the guide author's site already solve it.
- **Atlas passive preset guidance** (Rush / Exarch Altars / Destructive Play / Essence Fallback text checklists) - rots every league when GGG rebalances; orthogonal to map-run decisions.
- **Kirac steering suggestions** - deferred.
- **Per-character persistence, session logs, analytics** - see next decision.

### Recommendation style: advisory, not prescriptive
Highlight the set of maps satisfying current-Phase rules; user picks. Prescriptive ("run this exact map") was rejected because it requires modelling held-map inventory - a whole extra data source and failure mode. Informational-only was rejected as wasting the captured strategy.

### Phase detection: inferred, with manual override
Default is derived from game state. Override exists because Phase 2 (Maven farming) legitimately overlaps Phase 1; a pure state machine will pick one and be wrong for some playstyles.

### In-map behaviour: HUD stays visible, per-map annotation active
The HUD is not suppressed outside town/atlas. In-map, it surfaces "bother yes/no" plus the boss-path arrow. The transcript's tactical loop ("boss -> shrine -> league mechanic -> leave") depends on knowing whether the current map is worth farming; the plugin already knows.

### Boss pathing: use ExileApi Areas + Stranded pattern
User specified. Reference implementation is `Repositories/Stranded/Pathfinder/`.

### State model: derived from game memory, minimal settings
Captured in [ADR 0002](adr/0002-derived-state-primary.md). Persistence is limited to the standard `BaseSettingsPlugin` settings file, plugin-global (not per-character, not per-league). Settings surface: Phase override, Strategy (Destructive Play vs Exarch Altars), show/hide toggles.

## Blocking spike before implementation

Verify via ExileApi MCP bridge that each of the following is readable from game memory. Any value that isn't collapses into a manual setting:

- Voidstone count (0-4)
- Maven witness count (X/10)
- Per-map bonus-completion state
- Current Exarch and Eater Quest Chain step
- Atlas map-tree node data (map name, tier, connections, current position)

## Implementation order (post-spike)

HUD panel first, then atlas map-tree overlay, then in-map boss-path arrow. Each is independently useful; if the schedule slips, the earlier surfaces still ship value.

## Reference plugins

- `Repositories/BeastsV2/` - atlas map-device integration precedent (Automation/MapDevice.Atlas.cs).
- `Repositories/Stranded/` - pathfinding reference.
