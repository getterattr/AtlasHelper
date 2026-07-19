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

### 3. Business synthesis
Lives under `Services/` - stateless synthesis of `AtlasSnapshot` into higher-order domain values. See [`Services/README.md`](../../../Services/README.md) for the pattern.

- **Phase inference** (`Services/Phase.cs`) - done. `Phase.From(AtlasSnapshot) -> PhaseInference` and `Phase.Resolve(settings, snapshot) -> PhaseId` (honours the override).
- **Advisory composer** (`Services/Advisory.cs`) - done. Discriminated union: `abstract record AdvisoryLine` with `Phase1Advisory` / `Phase2Advisory` / `Phase3Advisory` / `Phase4Advisory` cases. Phase 3 owns the tier-band sweep (whites -> yellows -> reds via `TierBand`) plus the uniques-only endgame. Phase 1/2/4 ship as minimal static lines; text refines as the TBD F chain-step flags resolve.
- **In-map advice** (`Services/InMapAdvice.cs`, new) - open. Static rendering of the four-step in-map loop ([strategy.md#in-map-loop](strategy.md#in-map-loop)) when `SessionContext.IsInMap`.

### 4. Surfaces (consumes 3)
UI lives under `Ui/`, split by audience: `Ui/Overlays/` for in-game surfaces the player sees while playing, `Ui/Panels/` for settings-panel content in the ExileApi menu. `Theme.cs` and `ImGuiHelpers.cs` sit at the `Ui/` root as shared utilities.

- **HUD** (`Ui/Overlays/HudOverlay`) - Advisory-led. Advisory line on top, Normal + Uniques counters adjacent, Phase / Voidstones / Maven grid below. In-town vs in-map variants remain open (blocked on InMapAdvice).
- **AtlasOverlay** (`Ui/Overlays/AtlasOverlay`) - Phase 3 only. Consumes `Phase3Advisory` to filter unbonused non-uniques by the active `TierBand`; uniques rendered in `UniqueHighlightColor`; endgame flips to unique-only. Phase 1/2 pathing lives in `PathOverlay`; Phase 4 has no overlay.
- **Boss-path arrow** (`Ui/Overlays/BossPathArrow`, new) - open. Uses ExileApi `Areas` plus the `Repositories/Stranded/Pathfinder/` pattern. Independent track - can slot in any time.

### 5. Release
- Publishing pipeline via the `publish` skill.

## Order

Data unblocks run in the background whenever the player triggers a milestone. Code order:

1. ~~Session context~~ - done.
2. ~~Phase inference + Advisory~~ - done. Phase 1/2/4 advisories are minimal static lines pending TBD F.
3. ~~HUD redesign~~ - done for the in-town variant. In-map variant is blocked on InMapAdvice.
4. ~~Atlas overlay phase-awareness~~ - done (Phase 3 band-gated sweep + endgame).
5. InMapAdvice + in-map HUD variant - open.
6. Boss-path arrow - parallel any time.
7. Publish.

## Out of scope

Recorded here to close the loop when questions arise:

- Map inventory highlighting - requires modelling held-map items.
- Kirac steering suggestions - not in [strategy.md](strategy.md).
- Late-league content (deep delve, aspirational farming).
- Automation of any kind.
- Per-character persistence, session logs, analytics.
- Atlas passive-tree overlay - guide-author images already solve it.

See [decisions/scope.md](decisions/scope.md).
