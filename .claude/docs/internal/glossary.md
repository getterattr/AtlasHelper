# Glossary

Canonical domain vocabulary for AtlasHelper. Devoid of implementation detail and business rules; those live in [strategy.md](strategy.md).

## Phase
One of the four ordered stages of league-start atlas progression:
1. **Phase 1 - First Voidstone**: reach T16 in the bottom-left corner and complete both the Exarch and Eater chains to socket the **Eldritch** voidstone.
2. **Phase 2 - Second Voidstone and Ten-Way Maven**: two parallel goals. Complete Eagon's *Threads of the Originator* memory questline to defeat the Incarnation of Dread and socket the **Originator** voidstone (top-left corner), and complete the 10-way stage of Maven's Invitation: The Atlas to unlock the fourth scarab slot on the Map Device.
3. **Phase 3 - Full Completion**: reach the atlas bonus completion targets - 100 normal-map bonus points plus up to 10 unique-map bonus points (any 10 uniques satisfy the cap).
4. **Phase 4 - Final Voidstones**: self-farm both remaining voidstones - **Decayed** via Shaper + Elder in the top-right corner, and **Ceremonial** via the Maven's Writ in the bottom-right corner.

## Bonus Completion
The one-time "+1 atlas passive point" awarded per map for killing its boss at or above the map's native tier and at or above the rarity printed on the atlas node: **magic** for whites (T1-T5), **rare** for yellows (T6-T10), **rare + corrupted** for reds (T11-T16).

## Corner
One of the four extreme regions of the atlas. Each holds one Voidstone slot; the fight that drops each Voidstone varies.

## Voidstone
An atlas-level upgrade item socketed into a Corner. Each socketed Voidstone raises all map tiers by four. Four total: **Eldritch**, **Originator**, **Decayed**, **Ceremonial**.

## Quest Chain
A pre-voidstone questline whose stages gate the appearance of a pinnacle-boss invitation. In scope: the **Exarch chain**, the **Eater chain**, **Eagon's memory chain** (Threads of the Originator), and the five-stage **Maven's Invitation: The Atlas** chain. See [questline.md](questline.md) for stage-by-stage facts.

## Eagon
NPC Eagon Caeserius. Spawns a memory tear on the player's first T14+ map when no Eagon chain is already active, and respawns on the next T14+ after the previous chain finishes. Each chain hands the player the three memory maps that make up the *Threads of the Originator* questline.

## Envoy
NPC who introduces the Exarch and Eater questlines in a yellow (T6+) map, handing over the **Luminous Astrolabe** and **Flesh Compass** used to add Exarch and Eater influence at the Map Device.

## Maven's Beacon
A Map Device toggle awarded after first meeting the Envoy. Enabling it invites the Maven to witness the map boss; killing that boss registers a witness toward the Maven's Invitation ladder.

## Run Priority
The rule set the plugin uses to advise the next map to run. Rules live in [strategy.md#run-priority](strategy.md#run-priority).
