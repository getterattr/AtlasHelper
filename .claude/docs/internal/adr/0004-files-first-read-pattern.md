# ADR 0004 - Files-First Read Pattern

## Status
Accepted - 2026-07-19

## Context
[[0003-gamestate-facade]] established that all `GameController` traversal is contained in the `GameState/` facade. Left unaddressed: *how* each reader inside the facade sources its data.

A reader audit against the current codebase found no reader that consistently follows the pattern:

1. Load the base catalog from `Files.*` (or a raw `Data/*.dat` table): "what exists in the game" - every atlas node, every world area, every quest flag definition.
2. Overlay `ServerData.*` state: "what has this specific character done" - which nodes completed, which flags set.

Readers today seed their base data from `ServerData` (e.g., `TreeReader` walking from `ServerData.CompletedNodes[0]`) or hardcode it as constants (e.g., `CompletionReader`'s `100` and `10` targets). Both shortcuts produce correctness gaps:

- A character with no completed atlas nodes yields an empty tree.
- Hardcoded targets bit-rot silently across league reworks.
- Placeholder QuestFlag names return `null` with no way to distinguish "flag is false" from "we spelled the flag wrong".

## Decision
1. **Every reader whose section has a base catalog reads it from `Files.*` first.** The catalog defines the denominator - the set of things that exist. Cross-reference `ServerData.*` supplies the numerator - the set of things done. Readers without a meaningful base catalog (e.g., `MavenReader` - witness count is intrinsic state) are exempt.
2. **Failure to find a base-catalog entry is a hard error the reader must surface, not swallow.** A QuestFlag name that doesn't exist in `Files.QuestFlags.EntriesList` must fail loudly (e.g., log or throw at debug time) rather than resolve to `null` alongside legitimate "flag is false" reads.
3. **Base-catalog reads are cached per plugin session** where possible (Files data is immutable within a session).
4. **`TreeReader` becomes the reference implementation once retrofitted per TBD C.** Its target shape: pull the atlas-node catalog from `Data/AtlasNode.dat`, overlay completion and bonus flags from `ServerData.CompletedNodes` and `ServerData.BonusCompletedNodes`. Until that retrofit lands, no reader in the codebase demonstrates the pattern.

## Consequences
- **Fresh characters render correctly.** No more "empty tree on a character with zero completed nodes" bugs.
- **Rework resilience.** GGG changes atlas node counts, adds unique maps, renames pinnacles - the Files layer absorbs the change; readers stay correct.
- **Loud failures on rename.** If GGG renames a `QuestFlag` enum value the plugin depends on, we see the failure at plugin startup instead of silently reporting `null`.
- **More code per reader.** Cross-referencing a Files catalog is slightly more work than iterating a ServerData collection - accepted cost.
- **Retrofit debt is real.** Six of seven current readers do not follow this pattern; see the audit table in [context.md](../context.md) and TBDs A-F for the remediation work.

## Alternatives considered
- **ServerData-only reads.** Rejected: this is what most readers do today, and it's the cause of the correctness gaps identified in the audit.
- **Hardcode catalog constants.** Rejected: silently rots across league reworks (e.g., "100 normal maps" would need editing every time GGG changes atlas node counts).
- **Cache a hybrid snapshot in one reader, share across others.** Rejected as premature: sharing base-data caches across readers introduces ordering coupling that ADR 0003's per-section reader model was designed to avoid.
