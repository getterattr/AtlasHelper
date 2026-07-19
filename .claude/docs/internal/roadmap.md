# Roadmap

Current-state view of what's built, what's left, and the order that respects dependencies. Living doc - update as workstreams land. Fine-grained items belong in GitHub issues, not here.

## Built

- **Domain model.** Glossary, questline facts, strategy rules, phase definitions. See [glossary](glossary.md), [questline](questline.md), [strategy](strategy.md).
- **State model.** `AtlasSnapshot` immutable record, modules (`Atlas/`, `Voidstones/`, `Maven/`, `Pinnacles/`, `Diagnostics/`), single-fact ownership, per-section readers, Files-first pattern. See [decisions/gamestate.md](decisions/gamestate.md) and [decisions/read-pattern.md](decisions/read-pattern.md).
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

### 2. Missing readers (blocks Advisory)
Small, self-contained additions. Runs sit under `GameState/Readers/`, records under the appropriate module.

- **Current map identity** - which atlas node the player is currently in. Feeds in-map "bother yes/no".
- **Current map rarity + corruption** - required by the rarity rules per tier band ([strategy.md#rarity-rules](strategy.md#rarity-rules)).
- **Map Device toggle state** - Maven Beacon on? Exarch influence enabled? Eater? Feeds Advisory when recommending the next map to open.

### 3. Business synthesis (blocked by 2, degrades gracefully without 1)
Lives under `Services/` - stateless synthesis of `AtlasSnapshot` into higher-order domain values. See [`Services/README.md`](../../../Services/README.md) for the pattern. `Services/Phase.cs` is the reference implementation (currently a placeholder returning the default).

- **Phase inference** (`Services/Phase.cs`). `Phase.From(AtlasSnapshot) -> PhaseInference` (PhaseId + reason). Voidstones + bonus + chain state resolve to Phase 1-4. Reason is the trigger (e.g. "Eldritch not socketed").
- **Advisory composer** (`Services/Advisory.cs`, new). `Advisory.From(AtlasSnapshot) -> AdvisoryLine`. Composes run-priority rules ([strategy.md#run-priority](strategy.md#run-priority)) into a single "▶ Run next" sentence. Falls back to "highest tier" when chain state is null. Consumes `Services/Pathfinding` (predicate-driven BFS - "nearest T1", "specific chain-target node", "nearest unbonused", etc.) for whichever pathing question the phase is asking.
- **In-map advice** (`Services/InMapAdvice.cs`, new). Per-map "bother yes/no" derivation from current-map identity + drop state ([strategy.md#in-map-loop](strategy.md#in-map-loop)).
- **Rarity check** (`Services/Rarity.cs`, new). Given a current-area rarity + tier, does it satisfy the bonus completion rule per tier band ([strategy.md#rarity-rules](strategy.md#rarity-rules))?

### 4. Surfaces (consumes 3)
UI lives under `Ui/`, split by audience: `Ui/Overlays/` for in-game surfaces the player sees while playing, `Ui/Panels/` for settings-panel content in the ExileApi menu. `Theme.cs` and `ImGuiHelpers.cs` sit at the `Ui/` root as shared utilities.

- **HUD redesign** (`Ui/Overlays/HudOverlay`). Lead with the Advisory line, chains second, counters last. In-town vs in-map variants.
- **Atlas overlay phase-awareness** (`Ui/Overlays/AtlasOverlay`). Ring color by tier band, solid for phase-relevant nodes, chain-gating accent, Eagon-eligible white markers during Phase 2.
- **Boss-path arrow** (`Ui/Overlays/BossPathArrow`, new). Uses ExileApi `Areas` plus the `Repositories/Stranded/Pathfinder/` pattern. Independent track - can slot in any time after readers.

### 5. Release
- Publishing pipeline via the `publish` skill.

## Order

Data unblocks run in the background whenever the player triggers a milestone. Code order:

1. Missing readers (workstream 2) - foundation.
2. Phase inference + Advisory (workstream 3) - can ship even while chain state is null; degrades to the default rule.
3. HUD redesign - the visible win, consumes Advisory.
4. Atlas overlay phase-awareness - extends existing overlay.
5. Boss-path arrow - parallel any time after readers.
6. Publish.

Jumping straight to HUD skips readers and business synthesis. Don't.

## Out of scope

Recorded here to close the loop when questions arise:

- Map inventory highlighting - requires modelling held-map items.
- Kirac steering suggestions - not in [strategy.md](strategy.md).
- Late-league content (deep delve, aspirational farming).
- Automation of any kind.
- Per-character persistence, session logs, analytics.
- Atlas passive-tree overlay - guide-author images already solve it.

See [decisions/scope.md](decisions/scope.md).
