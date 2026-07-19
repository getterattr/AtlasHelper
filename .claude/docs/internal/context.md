# AtlasHelper - Design Context

Narrative record of the design grill that shaped the plugin. Complements the [glossary](glossary.md) (vocabulary only) and the topical [decisions](decisions/) (current-state design docs) by preserving what was asked, what was chosen, what was rejected, and why.

## Origin

The plugin was scoped from a single reference: definitive/Cyclone's 3.29 "How to get to Endgame FAST" YouTube guide. The guide decomposes league-start atlas progression into four ordered phases:

1. First voidstone (bottom-left, Exarch + Eater)
2. Ten-way Maven + second voidstone (top-left, Originator)
3. 100% bonus completion
4. Final voidstones (Shaper/Elder, top-right and bottom-right)

Everything in the plugin is downstream of this strategy. See [glossary](glossary.md) for the canonical terms and [questline](questline.md) for the wiki-sourced facts about voidstones, Maven progression tiers, and endgame invitations.

## Current-state decisions

Live design docs under [decisions/](decisions/):

- [scope.md](decisions/scope.md) - league-start-only value, in-scope surfaces, rejected surfaces, advisory (not prescriptive) style.
- [state-model.md](decisions/state-model.md) - live game memory is the source of truth; only settings persist. Phase inference vs manual override.
- [gamestate.md](decisions/gamestate.md) - facade pattern, module structure (`Atlas/`, `Voidstones/`, `Maven/`, `Pinnacles/`, `Diagnostics/`), single-fact ownership.
- [read-pattern.md](decisions/read-pattern.md) - `Files.*` catalog + `ServerData.*` overlay, loud failure on rename.

## Reader audit vs Files-first rule

Snapshot of correctness as of the current commit. "Files-first?" column marks readers that establish their base data from `Files.*` before overlaying `ServerData.*`.

| Reader | Files-first? | Gap |
|---|---|---|
| `TreeReader` | Yes | Reads `Files.AtlasNodes.EntriesList` as the catalog, overlays `ServerData.CompletedNodes` / `BonusCompletedNodes`. Reference implementation. |
| `VoidstoneReader` | No | Reads UI (`IngameUi.Atlas`), null when panel closed. Slot -> corner -> kind mapping hardcoded. |
| `CompletionReader` | No | Total denominators (100 normal, 10 unique) are hardcoded constants; no Files enumeration. |
| `WitnessesReader` | N/A | Reads `ServerData.MavenWitnessedAreas`. Witness list is intrinsic state; no meaningful Files base. |
| `AtlasInvitationReader` | Uses catalog | Reads `AtlasQuestFlags.Maven.AtlasLadder.*` via `QuestFlagLookup`. Catalog names are TODO placeholders (see TBDs). |
| `ThemedInvitationsReader` | Uses catalog | Flag catalog wiring only. |
| `PinnacleBossesReader` | Uses catalog | Same. |
| `EldritchReader`, `OriginatorReader`, `DecayedReader` | Uses catalog | Same - flag-only readers. |
| `QuestFlagLookup` | No | Enumerates `ServerData.QuestFlags`; unknown flag names resolve to `null`, but the catalog validator (`AtlasQuestFlags.Validate`) surfaces them loudly at startup. |

`AtlasQuestFlags` (top-level `GameState/` catalog) enumerates every declared flag name in an `All` array. `FlagDiagnostics` runs the validator once when `ServerData.QuestFlags` hydrates and logs any unresolved names.

## Diagnostics

Two modules under `GameState/Diagnostics/`:

- **`FlagDiagnostics`** - startup-once. Dumps every true `QuestFlag` to `QuestFlagDump.tsv` (the source of truth for pinning canonical names), then validates the `AtlasQuestFlags` catalog against the runtime set.
- **`SnapshotHealth`** - after a 30-second warm-up, reports which snapshot sections are still `Empty`. Distinguishes a genuinely-blank character from a broken reader after a patch shifts memory offsets. Only checks sections that should hydrate as soon as the player is in-game with the atlas panel opened once (`Tree`, `Voidstones`).

## Open TBDs

Investigation items that block Files-first correctness. To be converted to local issues.

| # | Question | Blocking |
|---|---|---|
| A | Where does unique-map bonus-completion state live? Not in `BonusCompletedNodes` per empirical test. | `CompletionReader.UniqueBonusCount` stuck at 0. |
| B | Canonical `QuestFlag` names for each pinnacle kill, beacon acquisition, and Maven's Crucible stage completion. | Many `AtlasQuestFlags` entries under `Pinnacles`, `Maven`, `Voidstones.*` are placeholders; validator flags them at startup. |
| D | Voidstone slot-index -> corner mapping stability across leagues. | Currently hardcoded in `VoidstoneReader.SlotLayout` from 3.28 empirical observation; could break next league. |
| E | Are 100 normal / 10 unique targets defined in-engine? Where? | Constants are magic numbers; would prefer a Files-sourced lookup for future-proofing. |
| F | Canonical `QuestFlag` names for intermediate chain steps: Envoy met, influence unlocked, mid-tier clears, invitation-item drops, Eagon memory maps cleared, guardian kills. | `AtlasQuestFlags.Voidstones.*` placeholders; chain records read `null` until identified. `QuestFlagDump.tsv` from `FlagDiagnostics` is the workflow. |

TBD C (Files-first bootstrap for `TreeReader`) is closed - the retrofit landed.

## Blocking-spike status

Live tracker for the values the plugin needs to read from game memory.

| Spike | Status | Reader | Notes |
|---|---|---|---|
| Voidstone count (0-4) and per-slot socket state | Resolved | `VoidstoneReader` | Reads `IngameUi.Atlas.VoidstoneSlots`; UI dependency documented in the audit above. |
| Maven witness count (X/10) | Resolved | `WitnessesReader`, `AtlasInvitationReader` | `ServerData.MavenWitnessedAreas.Count`. |
| Per-map bonus-completion state | Partial | `CompletionReader` | Normal count correct via `ServerData.BonusCompletedNodes`. Unique count stuck at 0 - see TBD A. |
| Per-corner chain progression (Exarch, Eater, Eagon, Guardians) | Partial | `EldritchReader`, `OriginatorReader`, `DecayedReader` | Catalog and reader wiring exist; flag names are placeholders (TBD F). Validator surfaces them at startup. |
| Atlas map-tree node data (name, tier, connections, position) | Resolved | `TreeReader` | Reads `Files.AtlasNodes` catalog and overlays `ServerData.CompletedNodes` / `BonusCompletedNodes`. Reference implementation. |
| Post-patch offset breakage detection | Resolved | `SnapshotHealth` | Warm-up + Empty-section report loudly identifies broken readers after a game patch shifts memory offsets. |

## Implementation order

See [roadmap.md](roadmap.md) - workstream breakdown, dependencies, and current position.

## UI layout

Rendering lives under `Ui/`, split by audience:

- `Ui/Overlays/` - in-game surfaces rendered while the player is playing (`HudOverlay`, `AtlasOverlay`, future `BossPathArrow`).
- `Ui/Panels/` - settings-panel content rendered in the ExileApi menu (`OverviewPanel`, `ProgressionReferencePanel`).
- `Ui/Theme.cs`, `Ui/ImGuiHelpers.cs` - shared styling and drawing utilities.

`Ui/` was kept as-is rather than renamed to `Surfaces/`; the docs vocabulary uses "surfaces" both as a noun (rendering targets) and a verb (the validator surfaces flag names), which is mildly ambiguous, and `Ui/` is universally parsed in half a second by any C# reader.

## Reference plugins

- `Repositories/BeastsV2/` - atlas map-device integration precedent (`Automation/MapDevice.Atlas.cs`).
- `Repositories/Stranded/` - pathfinding reference.
