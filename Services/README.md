# Services

Stateless synthesis of `AtlasSnapshot` into higher-order domain values. Sits between `GameState/` (facts) and `Ui/` (rendering).

## Pattern

- **Static classes, static methods.** No lifecycle, no DI.
- **Input**: an `AtlasSnapshot` (plus small extras where warranted, e.g. current-area details).
- **Output**: a domain record or enum. Lives in the same file as the service that produces it.
- **No `Service` suffix on filenames.** The folder tells you what kind of type is inside. `Services/Phase.cs`, not `Services/PhaseService.cs`.
- **Correlation across `AtlasSnapshot` sections happens here.** Per `.claude/docs/internal/decisions/gamestate.md`, single-fact ownership means "is Eldritch ready?" requires composing `PinnacleBosses` + `Eldritch` chain state - that composition belongs in a service, not in `GameState/`.

## Layering

```
GameState/   facts        (what IS)
Services/    synthesis    (what to DO about it)
Ui/          rendering    (how to SHOW it)
```

## Files

- `Phase.cs` - phase inference (Phase 1-4 based on voidstones + bonus + chain state).
- `Pathfinding.cs` - BFS across the atlas graph. Takes a destination predicate so one algorithm covers every atlas pathing question: nearest T1 for Phase 1 unlock planning, specific target for chain steps (Polaric Void, Seething Chime), nearest unfinished T1 for Phase 3 tier-boost, nearest bonus-eligible unique, etc.

More arrive as workstream 3 lands - see [roadmap](../.claude/docs/internal/roadmap.md#3-business-synthesis-blocked-by-2-degrades-gracefully-without-1).
