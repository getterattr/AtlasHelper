# Files-first read pattern

Every reader whose section has a base catalog reads it from `Files.*` first, then overlays `ServerData.*`. The catalog defines the denominator - the set of things that exist. `ServerData.*` supplies the numerator - the set of things done.

## Rules

1. **Base catalog from `Files.*`.** Atlas node catalog from `Files.AtlasNodes`, passives from `Files.PassiveSkills`, quest flag names from the runtime `ServerData.QuestFlags` key set (there is no separate `Files.QuestFlags` catalog; validation cross-references the two).
2. **Overlay `ServerData.*`** for the character-specific state - which nodes are completed, which flags set.
3. **Failure to find a base-catalog entry is loud, not silent.** A `QuestFlag` name that doesn't exist at runtime is surfaced by `AtlasQuestFlags.Validate` at plugin startup and logged loudly. Placeholder names in the catalog fail this check until pinned via `QuestFlagDump.tsv`.
4. **Base-catalog reads are cached per plugin session.** `Files.*` is immutable within a session.

## Reference implementation

`TreeReader` is the reference. It pulls the atlas node catalog from `Files.AtlasNodes.EntriesList` and overlays completion and bonus flags from `ServerData.CompletedNodes` and `ServerData.BonusCompletedNodes`. A character with zero completed nodes still renders the full atlas graph correctly.

## Readers without a Files base

Some sections have no meaningful catalog (`WitnessesReader` reads `ServerData.MavenWitnessedAreas` directly - the witness list is intrinsic state). These are exempt from the catalog rule but still return their section's `Empty` value on null.

## Consequences

- **Fresh characters render correctly.** No more "empty tree on a character with zero completed nodes" bugs.
- **Rework resilience.** GGG changes atlas node counts, adds unique maps, renames pinnacles - the Files layer absorbs the change; readers stay correct.
- **Loud failures on rename.** If GGG renames a `QuestFlag` value the plugin depends on, the catalog validator logs it at plugin startup instead of silently reporting `null`.

## Rejected alternatives

- **ServerData-only reads.** Rejected: causes correctness gaps (e.g. empty tree on a fresh character).
- **Hardcode catalog constants.** Rejected: silently rots across league reworks.
- **Share a hybrid snapshot across readers.** Rejected as premature: introduces ordering coupling that the per-section reader model was designed to avoid.
