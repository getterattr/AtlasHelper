# State model: derived, not persisted

Live game memory is the source of truth. Every HUD line and overlay hint is derived from `GameController` reads on each tick. There is no per-character JSON, no session log, no analytics store.

## Persistence

The only persisted data is the standard `BaseSettingsPlugin` settings file. It is plugin-global (not per-character, not per-league) and holds:

- Phase override (dropdown; default: `Auto`).
- Show/hide toggles per surface (HUD, atlas overlay, in-map boss arrow).
- Cosmetics (colors, text scale).

## Why

- Switching characters mid-league requires no plugin action; state redraws from the new character's memory.
- No file format to version, no migration code, no persistence bugs.
- Historical metrics (time-to-first-voidstone, maps run this league) are impossible by design - out of scope per [scope.md](scope.md).

## Phase inference vs override

Phase is inferred from game state by default. A manual override exists because Phase 2 (Maven farming) legitimately overlaps Phase 1; a pure state machine will pick one and be wrong for some playstyles. If a specific derived value turns out to be unreadable via ExileApi, it collapses to a user-set setting instead.
