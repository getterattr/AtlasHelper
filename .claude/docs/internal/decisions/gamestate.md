# GameState: facade, modules, and single-fact ownership

All `GameController` traversal is contained. HUD, overlay, and pathfinder code consume the immutable `AtlasSnapshot` record and never touch `GameController` directly.

## Facade

`GameStateReader` is the single entry point. It owns the current `AtlasSnapshot` and exposes:

- `Refresh(GameController)` - always re-reads. Called from `AreaChange`.
- `RefreshIfStale(GameController)` - re-reads only if the snapshot is older than 2 seconds. Called from `Tick`.

Snapshots are immutable records. Each section has an `Empty` static instance for degenerate states. Failure returns `Empty`, never throws - the snapshot as a whole is always valid.

Per-section readers live in `GameState/Readers/`. One static class per section, each takes `GameController` (and where relevant a `QuestFlagLookup`) and returns its section record.

## Modules

`GameState/` splits into domain-grouped modules. Each owns a coherent slice of the atlas questline:

| Module | Owns |
|---|---|
| `Atlas/` | Physical atlas surface: `AtlasTree`, `AtlasCompletion`, `AtlasProjection`. |
| `Voidstones/` | Physical slot state (`VoidstoneState`) plus per-corner **chain progression** records (`Eldritch`, `Originator`, `Decayed`). |
| `Maven/` | Everything Maven-owned: `Witnesses` (list), `AtlasInvitation` (the 5-stage ladder, including `BeaconAcquired`), `ThemedInvitations` (Formed / Twisted / Elderslayers / Forgotten / Remembered / Feared). |
| `Pinnacles/` | Individual pinnacle boss kills - `PinnacleBosses` (Maven, Shaper, Elder, SearingExarch, EaterOfWorlds, IncarnationOfDread, Sirus). |
| `Diagnostics/` | Startup and health diagnostics (`FlagDiagnostics`, `SnapshotHealth`). Not a state section - runs alongside the snapshot pipeline. |

`AtlasQuestFlags` sits at the top of `GameState/` (not under `Readers/`). It is a domain catalog mapping game-flag strings to logical progression events, grouped to mirror the modules.

## Single-fact ownership

Each fact lives in exactly one module. The consumer answering "is Eldritch ready?" reads `snapshot.PinnacleBosses.SearingExarch && snapshot.PinnacleBosses.EaterOfWorlds`. The consumer answering "what Exarch step am I on?" reads `snapshot.Eldritch.Exarch.*`. Duplication is not permitted:

- The voidstone chain records deliberately **do not** carry terminal boss booleans; `Pinnacles/PinnacleBosses` owns them.
- Themed Maven invitations live in `Maven/ThemedInvitations`, not in `Pinnacles/` - they are Maven's Invitations, not standalone bosses.
- There is no `Voidstones/Ceremonial.cs`; all Ceremonial-corner state lives in `Maven/` or `Pinnacles/`, and an empty record for symmetry was rejected as ceremony.

Cross-module correlation is explicit: readiness for a voidstone requires composing state across modules in the caller (a phase inferrer, a HUD renderer) rather than baking a computed property that re-couples the modules.

## Cadence

The 2-second staleness bound is a coarse first cut. Hot fields (player position for the boss-path arrow) will read every Render and bypass the snapshot. Cold fields (atlas tree graph) will move behind hash-based invalidation later. Both are additive changes to `GameStateReader`.

## Rejected alternatives

- **Every consumer calls `GameController.IngameState...` directly.** Rejected: reads scatter, null-guards duplicate, no snapshot point, cadence unbounded, consumers untestable.
- **Keep everything flat under `GameState/`.** Rejected once the folder grew past ten records - domain groupings became implicit.
- **Duplicate terminal boss kills in both voidstone chains and pinnacles.** Rejected: silently allows the two records to disagree if updated on different paths.
- **Add an empty `Voidstones/Ceremonial.cs` for four-corner symmetry.** Rejected: ceremony without content.
