# ADR 0003 - GameState Facade

## Status
Accepted - 2026-07-19

## Context
[[0002-derived-state-primary]] established that live game memory is the source of truth. Left unaddressed: how memory reads are organised.

If every HUD panel, overlay, and pathing helper calls `GameController.IngameState...` directly, five things go wrong:
1. Reads scatter across files, so any offset or API change ripples through every consumer.
2. Every consumer has to re-implement null-guards for closed panels and unhydrated collections.
3. Reads happen at unbounded cadence, some per-Render at 60 Hz, over data that changes at map-completion frequency.
4. Consumers cannot be unit tested - they need a live `GameController`.
5. There is no single place to snapshot state for debugging or recording.

## Decision
1. **All `GameController` traversal lives under `GameState/`.** HUD, overlay, and pathfinder code consume the immutable `AtlasSnapshot` record and never touch `GameController` directly.
2. **`GameStateReader` is the single entry point.** It owns the current `AtlasSnapshot` and exposes two methods:
   - `Refresh(GameController)` - always re-reads. Called from `AreaChange`.
   - `RefreshIfStale(GameController)` - re-reads only if the snapshot is older than 2 seconds. Called from `Tick`.
3. **Snapshots are immutable records.** Each section (`VoidstoneState`, `AtlasCompletion`, `MavenState`, `AtlasPassives`, `AtlasTree`, `PinnacleCompletion`) is a discrete record with an `Empty` static instance for degenerate states.
4. **Failure returns `Empty`, never throws.** Any reader that hits a null collection, closed panel, or unexpected shape returns its section's `Empty` value. The snapshot as a whole is always valid.
5. **Per-section readers live in `GameState/Readers/`.** One static class per section - `VoidstoneReader`, `CompletionReader`, `MavenReader`, `PassivesReader`, `TreeReader`. Each takes `GameController` and returns its section record.

## Consequences
- Adding a new HUD field: add a snapshot section + reader, wire into `GameStateReader.Refresh`, consumers bind to the new field. No touching of existing readers.
- Offset changes in ExileApi are localised: exactly one reader breaks.
- The 2-second staleness bound is a first cut. Hot fields (player position for the boss-path arrow) will read every Render and bypass the snapshot; cold fields (atlas tree graph) will move behind a hash-based invalidation later. Both are additive changes.
- Consumers become testable with fixture snapshots without needing a running game.

## Alternatives considered
- **Per-Tick full refresh, no staleness bound.** Rejected: wastes cycles on atlas-tree and passives data that only change on map complete or panel interactions.
- **Event-driven refresh via ExileApi hooks.** Rejected as first step: ExileApi only emits `AreaChange` reliably. No "map completed" or "voidstone socketed" hook exists, so we would still need staleness fallback for correctness.
- **Lazy per-field with staleness tags.** Rejected as premature: the section granularity is coarse enough that we can refine to per-section cadence later if profiling demands it.
