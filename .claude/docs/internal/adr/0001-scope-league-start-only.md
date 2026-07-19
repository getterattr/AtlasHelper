# ADR 0001 - Scope: League-Start Only

## Status
Accepted - 2026-07-19

## Context
AtlasHelper is being built to compress the "empty atlas to four voidstones" grind at league start. The reference strategy (definitive's 3.29 rush guide) decomposes into four ordered Phases:

1. First voidstone (bottom-left corner, Exarch + Eater)
2. Ten-way Maven + second voidstone (top-left, Originator)
3. 100% bonus completion
4. Third and fourth voidstones (Shaper/Elder)

The information architecture we've agreed on - a Phase state machine, Corner ordering, Exarch/Eater Quest Chain tracking, "advisory next-map" rule set - is meaningful only while these phases are unfinished. Once the atlas is complete, every user-facing surface degenerates into a trivial identity.

## Decision
AtlasHelper is explicitly scoped to league-start progression. It will not grow features for:
- Late-league farming strategies (Kirac wheels, scarab configurations, endgame juicing)
- Passive-tree preset guidance (see [[0002-derived-state-primary]] for the philosophy)
- Any progression state past Phase 4

## Consequences
- Every surface (HUD, atlas map-tree overlay, in-map boss arrow) can assume "Phase is meaningful" without null-handling for a completed atlas.
- Requests to add late-league features are rejected by pointing here.
- If a future league fundamentally reshapes voidstone acquisition, the plugin may need retirement rather than adaptation.

## Alternatives considered
- **General-purpose atlas helper** covering league-start through endgame. Rejected: the transcript-derived strategy is the plugin's entire value; generalising it dilutes both the HUD and the advisory rule set into "here is some information about the atlas", which no one needs.
