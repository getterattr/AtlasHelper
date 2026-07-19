# ADR 0002 - Derived State Primary, Minimal Settings Only

## Status
Accepted - 2026-07-19

## Context
The plugin needs to know: current Phase, voidstone count, atlas bonus completion %, Exarch/Eater Quest Chain progress, Maven witness count, current map's bonus/chain relevance. Most of this is available from live game memory via ExileApi.

A tempting alternative is per-character JSON persistence: track Phase overrides, Kirac steering history, session logs, etc., keyed by character name and league.

The user's directive is explicit: this plugin stays lightweight. It should not accumulate complexity beyond what the transcript-derived strategy actually requires.

## Decision
1. **Game state is the source of truth.** Every HUD line and overlay hint is derived from `GameController` reads on each tick. No caching layer, no per-character files, no session logs.
2. **User configuration lives in the standard `BaseSettingsPlugin` settings file** and nowhere else. Settings are plugin-global (not per-character, not per-league) and are limited to:
   - Phase override (dropdown; default: inferred)
   - Show/hide toggles per surface (HUD, atlas overlay, in-map boss arrow)
3. **A blocking spike precedes implementation** to confirm each derived value is actually readable via ExileApi: voidstone count, Maven witnesses, per-map bonus completion, current Quest Chain step, atlas map-tree node data. Any value that isn't readable collapses to a user-set setting.

## Consequences
- Switching characters mid-league requires no plugin action - state redraws itself from the new character's memory.
- No file format to version, no migration code, no persistence bugs.
- We cannot show historical metrics (time-to-first-voidstone, maps run this league). By design; see [[0001-scope-league-start-only]].
- The spike outcome directly shapes the settings surface. If, say, Maven witness count is not readable, the settings file grows a "Maven witnesses X/10" manual counter.

## Alternatives considered
- **Per-character JSON persistence.** Rejected: violates the lightweight directive, adds a whole failure mode (stale/corrupt files) for information the game already tracks.
- **Zero settings at all**, everything inferred. Rejected: Phase inference has genuine overlaps (Phase 2 Maven farming starts during Phase 1), so a manual Phase override is required.
