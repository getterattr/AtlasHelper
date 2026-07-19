# Roadmap

Current-state view of what's built, what's left, and the order that respects dependencies. Living doc - update as workstreams land. Fine-grained items belong in GitHub issues, not here.

## Built

- **Domain model.** Glossary, questline facts, strategy rules, phase definitions. See [glossary](glossary.md), [questline](questline.md), [strategy](strategy.md).
- **State model.** `AtlasSnapshot` immutable record, modules (`Atlas/`, `Voidstones/`, `Maven/`, `Pinnacles/`, `Session/`, `Diagnostics/`), single-fact ownership, per-section readers, Files-first pattern. See [decisions/gamestate.md](decisions/gamestate.md) and [decisions/read-pattern.md](decisions/read-pattern.md).
- **Diagnostics.** `AtlasQuestFlags` catalog + validator, `FlagDiagnostics` (startup dump), `SnapshotHealth` (post-patch offset breakage). Debug logging toggle in settings.
- **UI surfaces (rudimentary).** Split under `Ui/Overlays/` (in-game surfaces: HUD showing raw counters, atlas overlay circling uncompleted nodes) and `Ui/Panels/` (settings-panel content: overview, progression reference).
- **Plugin lifecycle scaffold.** `AtlasHelperRuntime` facade owns state + diagnostics; `Core.Initialize`/`Shutdown` bookend session lifetime.

## Open workstreams

### 1. Data unblocks (parallel; human, in-game)
`QuestFlag` name discovery via `QuestFlagDump.tsv` diffs. Placeholders in `AtlasQuestFlags` fail catalog validation loudly at startup - work through them one milestone at a time.

- TBD B - pinnacle kill / beacon / ladder stage flag names.
- TBD F - chain-step flag names (Envoy met, influence unlocked, mid-tier clears, invitation-item drops, Eagon memory maps, guardian kills).
- TBD A - unique-map bonus source (empirically not `BonusCompletedNodes`).
- TBD D - voidstone slot -> corner mapping stability across leagues.
- TBD E - Files-sourced `NormalBonusTarget` / `UniqueBonusTarget`.

See [context.md](context.md#open-tbds) for the full TBD table.

### 2. Session context (blocks HUD variant)
One fact: is the player currently in a map? Drives the HUD in-town vs in-map variant switch. Lives under a new `GameState/Session/` module (`SessionContext.IsInMap`) with a `SessionReader` in `GameState/Readers/`. No further "current map" facts are read - identity, rarity, and corruption were all cut under the anti-junk lens ([decisions/scope.md](decisions/scope.md#no-post-hoc-lints-no-static-rule-readers)). Map device toggle state was cut for the same reason.

### 3. Business synthesis (Advisory blocked only by Phase; InMapAdvice blocked by 2)
Lives under `Services/` - stateless synthesis of `AtlasSnapshot` into higher-order domain values. See [`Services/README.md`](../../../Services/README.md) for the pattern. `Services/Phase.cs` is the reference implementation (currently a placeholder returning the default).

- **Phase inference** (`Services/Phase.cs`). `Phase.From(AtlasSnapshot) -> PhaseInference` (PhaseId + reason). Voidstones + bonus + chain state resolve to Phase 1-4. Reason is the trigger (e.g. "Eldritch not socketed").
- **Advisory composer** (`Services/Advisory.cs`, new). `Advisory.From(AtlasSnapshot) -> AdvisoryLine`. Composes run-priority rules ([strategy.md#run-priority](strategy.md#run-priority)) into a single "▶ Run next" sentence. Falls back to "highest tier" when chain state is null. Consumes `Services/Pathfinding` (predicate-driven BFS - "nearest T1", "specific chain-target node", "nearest unbonused", etc.) for whichever pathing question the phase is asking. The rarity reminder per tier band ([strategy.md#rarity-rules](strategy.md#rarity-rules)) is emitted inline from `AtlasTree` tier data - no `Rarity.cs` service.
- **In-map advice** (`Services/InMapAdvice.cs`, new). Static rendering of the four-step in-map loop ([strategy.md#in-map-loop](strategy.md#in-map-loop)) when `SessionContext.IsInMap`. No drop-state read, no per-map identity - the loop is the same regardless of map.

### 4. Surfaces (consumes 3)
UI lives under `Ui/`, split by audience: `Ui/Overlays/` for in-game surfaces the player sees while playing, `Ui/Panels/` for settings-panel content in the ExileApi menu. `Theme.cs` and `ImGuiHelpers.cs` sit at the `Ui/` root as shared utilities.

- **HUD redesign** (`Ui/Overlays/HudOverlay`). Lead with the Advisory line, chains second, counters last. In-town vs in-map variants.
- **Atlas overlay phase-awareness** (`Ui/Overlays/AtlasOverlay`). Ring color by tier band, solid for phase-relevant nodes, chain-gating accent, Eagon-eligible white markers during Phase 2.
- **Boss-path arrow** (`Ui/Overlays/BossPathArrow`, new). Uses ExileApi `Areas` plus the `Repositories/Stranded/Pathfinder/` pattern. Independent track - can slot in any time after readers.

### 5. Release
- Publishing pipeline via the `publish` skill.

## Order

Data unblocks run in the background whenever the player triggers a milestone. Code order:

1. Session context (workstream 2) - one boolean; unblocks HUD variant and InMapAdvice.
2. Phase inference + Advisory (workstream 3) - can ship even while chain state is null; degrades to the default rule.
3. HUD redesign - the visible win, consumes Advisory.
4. Atlas overlay phase-awareness - extends existing overlay.
5. Boss-path arrow - parallel any time after readers.
6. Publish.

Jumping straight to HUD skips business synthesis. Don't.

## Out of scope

Recorded here to close the loop when questions arise:

- Map inventory highlighting - requires modelling held-map items.
- Kirac steering suggestions - not in [strategy.md](strategy.md).
- Late-league content (deep delve, aspirational farming).
- Automation of any kind.
- Per-character persistence, session logs, analytics.
- Atlas passive-tree overlay - guide-author images already solve it.

See [decisions/scope.md](decisions/scope.md).
