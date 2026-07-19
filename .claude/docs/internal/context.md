# AtlasHelper - Design Context

Narrative record of the design grill that shaped the plugin. Complements the glossary (vocabulary only) and the ADRs (individual decisions in isolation) by preserving the flow of the conversation: what was asked, what was chosen, what was rejected, and why.

## Origin

The plugin was scoped from a single reference: definitive/Cyclone's 3.29 "How to get to Endgame FAST" YouTube guide. The guide decomposes league-start atlas progression into four ordered phases:

1. First voidstone (bottom-left, Exarch + Eater)
2. Ten-way Maven + second voidstone (top-left, Originator)
3. 100% bonus completion
4. Final voidstones (Shaper/Elder, top-right and bottom-right)

Everything in the plugin is downstream of this strategy. See [glossary](glossary.md) for the canonical terms and [questline](questline.md) for the wiki-sourced facts about voidstones, Maven progression tiers, and endgame invitations.

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
Captured in [ADR 0002](adr/0002-derived-state-primary.md). Persistence is limited to the standard `BaseSettingsPlugin` settings file, plugin-global (not per-character, not per-league). Settings surface: Phase override, show/hide toggles.

### Read pattern: Files for base data, ServerData for state
Emerging rule from the readers audit. `Files.*` holds the invariant catalog (all atlas nodes, all passive skills, all quest flag definitions, all world areas). `ServerData.*` holds this character's runtime state. Every reader should read the catalog from Files to establish "what exists" and then overlay ServerData to determine "what the player has done". The `PassivesReader` is the reference implementation; the other readers pre-date this rule and need remediation - see the audit table below.

## Reader audit vs Files-first rule

Snapshot of correctness as of the current commit. "Files-first?" column marks readers that establish their base data from `Files.*` before overlaying `ServerData.*`.

| Reader | Files-first? | Gap |
|---|---|---|
| VoidstoneReader | No | Reads UI (`IngameUi.Atlas`), null when panel closed. Slot -> corner -> kind mapping hardcoded. |
| CompletionReader | No | Total denominators (100 normal, 10 unique) are hardcoded constants; no Files enumeration. `UniqueBonusCount` currently returns 0 - source unknown. |
| MavenReader | N/A | Witness count is intrinsic state; no meaningful Files base. |
| PassivesReader | **Yes** | Reference pattern. `Files.PassiveSkills` supplies name/id lookup; `ServerData.AtlasPassiveSkillIds` supplies allocated set. |
| TreeReader | Partial | Walks the full graph via `.Connections` BFS, but the seed is `ServerData.CompletedNodes[0]`. A character with no completed maps yields an empty tree. Real base is `Data/AtlasNode.dat`. |
| QuestFlagLookup | No | Enumerates `ServerData.QuestFlags` only; never validates against `Files.QuestFlags.EntriesList`, so unknown flag names silently return null. |
| BeaconReader / InvitationProgressReader / PinnacleCompletionReader | No | All use hardcoded placeholder flag names that resolve to null. Real names TBD until log dump. |

## Open TBDs

Investigation items that block Files-first correctness. To be converted to local issues.

| # | Question | Blocking |
|---|---|---|
| A | Where does unique-map bonus-completion state live? Not in `BonusCompletedNodes` per empirical test. | `CompletionReader.UniqueBonusCount` stuck at 0. |
| B | Canonical `QuestFlag` names for each pinnacle kill, beacon acquisition, and Maven's Crucible stage completion. | Beacons, invitation stage, pinnacle completion snapshots are null. |
| C | Does ExileCore expose a typed wrapper for `Data/AtlasNode.dat`? If not, write a minimal reader. | TreeReader cannot bootstrap without a completed node seed. |
| D | Voidstone slot-index -> corner mapping stability across leagues. | Currently hardcoded from 3.28 empirical observation; could break next league. |
| E | Are 100 normal / 10 unique targets defined in-engine? Where? | Constants are magic numbers; would prefer a Files-sourced lookup for future-proofing. |

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
