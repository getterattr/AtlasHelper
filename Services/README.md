# Services

Stateless synthesis of `AtlasSnapshot` into higher-order domain values. Sits between `GameState/` (facts) and `Ui/` (rendering).

## Pattern

- **Static classes, static methods.** No lifecycle, no DI.
- **Input**: an `AtlasSnapshot`. Extra inputs are allowed only when the fact isn't on the snapshot; add it to the snapshot instead if it's stable state.
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
- `Pathfinding.cs` - BFS across the atlas graph. Three entry points, same algorithm:
  - `FindPath(start, isDestination)` - predicate-driven single-target BFS (nearest T1, specific chain target, nearest unbonused, ...).
  - `FindOrderedPath(start, orderedTargets)` - chains BFS through targets in the exact order given. Use when the ordering is domain-required (Ceremonial voidstone must be last, memory maps in sequence, ...).
  - `FindMultiTargetPath(start, targets)` - enumerates every ordering, returns the route with fewest total hops, tie-broken by most unbonused nodes traversed. Fits Phase 1's "hit both Polaric Void and Seething Chime with max bonus completion along the shortest route".

More arrive as workstream 3 lands - see [roadmap](../.claude/docs/internal/roadmap.md#3-business-synthesis-advisory-blocked-only-by-phase-inmapadvice-blocked-by-2).
