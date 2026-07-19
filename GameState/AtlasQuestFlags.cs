using System.Collections.Generic;
using ExileCore;

namespace AtlasHelper.GameState;

// Central catalog of every QuestFlag name the plugin reads, grouped to
// mirror the GameState modules.
//
// Every constant sits in one of three states:
//   1. Confirmed - the flag string appears in QuestFlagDump.tsv and its
//      semantics have been verified by dump-vs-game-state correlation.
//   2. Placeholder marked with a `TODO(unresolved):` block that lists
//      candidate flag names from the dump plus a one-line note on which
//      candidate is most likely. These stay as the plausible-guess string
//      until pinned; the catalog validator (Validate()) will call them
//      out at startup if they do not exist in ServerData.QuestFlags.
//   3. Placeholder with no known candidate - preserved so the API shape
//      is stable while the reader migrates to a different data source
//      (e.g. Guardian defeats live in CompletedNodes, not QuestFlags).
//
// Unresolved names surface loudly at startup via Validate() and read
// back as null at runtime - distinguishable from a known-false flag by
// consumers.
internal static class AtlasQuestFlags
{
    // Individual pinnacle boss kills. Sole owner in the catalog.
    //
    // Naming pattern from empirical dump (QuestFlagDump.tsv):
    //   "Cleansing" family = Searing Exarch chain (fire theme)
    //   "Consume" family   = Eater of Worlds chain (void theme)
    //   "Boss"             = final pinnacle fight
    //   "MiniBoss"         = mid-tier chain fight (Black Star, Infinite Hunger)
    // Placeholders remain for pinnacles not yet defeated on any test
    // character - they will surface as unresolved at plugin startup
    // once someone runs the flag catalog against a fresh dump.
    public static class Pinnacles
    {
        public const string Maven = "MavenEnragedDefeated";
        public const string Shaper = "ShaperDefeated";
        public const string Elder = "ElderDefeated";
        public const string SearingExarch = "CleansingBossDefeated";
        public const string EaterOfWorlds = "ConsumeBossDefeated";
        // Incarnation of Dread = final Threads-of-the-Originator boss.
        // Internal atlas icon is IgnoranceBoss (top-left, directly
        // adjacent to CleansingFireWatchstoneSlotNode / the Originator
        // corner). Empirical evidence: IgnoranceBossNonQuestDefeated is
        // the only non-quest-defeat flag among the top-left Incarnation
        // trio (Benevolence, Fear, Ignorance) that fires from the
        // corner-adjacent boss. Wiki verification blocked by anti-bot.
        public const string IncarnationOfDread = "IgnoranceBossNonQuestDefeated";
        public const string Sirus = "SirusDefeated";
    }

    // Maven-owned state: witness ladder and themed invitations.
    public static class Maven
    {
        public static class AtlasLadder
        {
            // TODO(unresolved): Maven's Beacon acquisition. Candidates
            // from dump:
            //   MetEnvoyMaven         - Envoy handed over the Beacon
            //   HaveMavenMapDeviceAlteration - Beacon in inventory
            //   HaveSkillBookMaven    - possibly the Beacon-adjacent
            //                           skill book from Kirac
            // Most likely: MetEnvoyMaven (the moment Beacon is granted).
            public const string BeaconAcquired = "GotMavensBeacon";
            // TODO(unresolved): per-stage completion of Maven's
            // Invitation: The Atlas. Dump has no MavensCrucibleStageN
            // flags. Candidates:
            //   MavenFirstMapBossCapture   - Stage 1 (3-way) related?
            //   MavenFirstVoidBossCapture  - void variant of Stage 1?
            //   HaveMavenMapAtlas{1..5}    - has invitation item for
            //                                stage N (not completion)
            //   MavenFirstInvitation       - TRUE on this character;
            //                                might correspond to Stage 1
            //                                completion (first invitation
            //                                'accepted').
            // Note: HaveMavenMapVoid1..6 are all TRUE on this character
            // - these are likely themed-invitation splinter tracking or
            //   Void 10-way stages, not the 5-stage Atlas ladder.
            public const string Stage1 = "MavensCrucibleStage1Complete";
            public const string Stage2 = "MavensCrucibleStage2Complete";
            public const string Stage3 = "MavensCrucibleStage3Complete";
            public const string Stage4 = "MavensCrucibleStage4Complete";
            public const string Stage5 = "MavensCrucibleStage5Complete";
        }

        // TODO(unresolved): themed-invitation completion flags. Dump
        // shows no MavensInvitationThe{X}Complete flags. Candidates:
        //   HaveMavenMapAtlas{1..5}  - single-invitation items (not
        //                              completion); atlas variant.
        //   Individual boss capture flags (MavenFirstMapBossCapture,
        //   MavenFirstVoidBossCapture, ...) may correspond to specific
        //   themed invitations.
        // Kirac dialog flags exist (KiracOnTheElderslayersSeen,
        // HelenaOnTheConquerorsSeen) but only mark exposure, not
        // completion.
        public static class ThemedInvitations
        {
            public const string Formed = "MavensInvitationTheFormedComplete";
            public const string Twisted = "MavensInvitationTheTwistedComplete";
            public const string Elderslayers = "MavensInvitationTheElderslayersComplete";
            public const string Forgotten = "MavensInvitationTheForgottenComplete";
            public const string Remembered = "MavensInvitationTheRememberedComplete";
            public const string Feared = "MavensInvitationTheFearedComplete";
        }
    }

    // Chain progression toward each voidstone. Excludes terminal boss
    // kills (owned by Pinnacles above).
    public static class Voidstones
    {
        public static class Eldritch
        {
            public static class Exarch
            {
                // Confirmed via QuestFlagDump.tsv - Envoy met specifically
                // for the Exarch chain (introducing Cleansing Fire).
                public const string EnvoyMet = "MetEnvoyExarch";
                // Confirmed - "MapDeviceAlteration" is the Astrolabe
                // consumed at the Map Device; HaveX = influence toggle
                // is available.
                public const string InfluenceUnlocked = "HaveCleansingFireMapDeviceAlteration";
                // Confirmed - CleansingFireKey1 is the Polaric Invitation
                // (Black Star arena key).
                public const string PolaricInvitationDropped = "HaveCleansingFireKey1";
                // Confirmed via QuestFlagDump.tsv - Black Star = Exarch mini-boss.
                public const string BlackStarDefeated = "CleansingMiniBossDefeated";
                // TODO(unresolved): per-tier Exarch clear flags. Dump has no
                // dedicated SearingExarchT12/13/14/15 style flags. Candidates
                // to check on a further-progressed character:
                //   CleansingFireTier12Complete (guess)
                //   HaveCleansingFireTier{N}Progress (guess)
                // Or these may be tracked only implicitly via CompletedNodes.
                // Leaving placeholders; validator will flag.
                public const string T12Cleared = "SearingExarchT12Complete";
                public const string T13Cleared = "SearingExarchT13Complete";
                public const string T14Cleared = "SearingExarchT14Complete";
                public const string T15Cleared = "SearingExarchT15Complete";
                // Confirmed - CleansingFireKey2 is the Incandescent
                // Invitation (Searing Exarch arena key).
                public const string IncandescentInvitationDropped = "HaveCleansingFireKey2";
            }

            public static class Eater
            {
                // Confirmed via QuestFlagDump.tsv - Envoy met for Eater chain.
                // Historically named after the Astrolabe/Compass "Received"
                // event; the actual flag fires on the Envoy meeting itself.
                // Candidates for a stricter "compass in inventory" state if
                // ever needed: PickUpTangleMapDeviceAlteration.
                public const string FleshCompassReceived = "MetEnvoyEater";
                // Confirmed - Tangle = Eater/void theme.
                public const string InfluenceUnlocked = "HaveTangleMapDeviceAlteration";
                // TODO(unresolved): per-tier Eater clear flags. Same story
                // as Exarch above - no dedicated per-tier flags in the
                // dump. Candidates:
                //   HaveTangleTier{N}Progress (guess)
                //   TangleTierNComplete (guess)
                public const string T9Cleared = "EaterOfWorldsT9Complete";
                public const string T10Cleared = "EaterOfWorldsT10Complete";
                public const string T12Cleared = "EaterOfWorldsT12Complete";
                public const string T14Cleared = "EaterOfWorldsT14Complete";
                // Semi-confident - Tangle has two keys. Key1 fires earlier,
                // matching the Writhing Invitation (Infinite Hunger arena);
                // Key2 fires later, so it matches the Screaming Invitation
                // (Eater of Worlds arena). Candidates if the mapping is
                // reversed on real progression:
                //   HaveTangleKey1 (reversed)
                public const string ScreamingInvitationDropped = "HaveTangleKey2";
                // Confirmed via QuestFlagDump.tsv - Infinite Hunger = Eater mini-boss.
                public const string InfiniteHungerDefeated = "ConsumeMiniBossDefeated";
            }
        }

        public static class Originator
        {
            // Confirmed via dump:
            //   MetEagon                 = player has met Eagon
            //   EagonIntroductionSeen    = introduction cutscene seen
            //   EagonMemoryQuestStartSeen = quest start acknowledged
            public const string EagonMet = "MetEagon";
            public const string EagonIntroductionSeen = "EagonIntroductionSeen";
            // TODO(unresolved): individual memory-map completion tracking.
            // Dump has no CourtyardOfWasting/ChambersOfImpurity/TheatreOfLies
            // named flags. Candidates:
            //   FirstMemoryBossSeen / SecondMemoryBossSeen / ThirdMemoryBossSeen
            //     - "seen" not "cleared"; probably tracks arena entry.
            //   FirstMemoryBossPinnacleSeen / Second... / Third...
            //     - saw the pinnacle version.
            //   NonQuestMemoryBossesDefeated - aggregate, fires when all
            //     three memory bosses are defeated outside quest chain.
            //   HaveSkillBookFirstMemoryBoss / Second / Third
            //     - has the "skill book" item associated with each.
            // Individual per-map completion may only exist via
            // CompletedNodes on the atlas; the memory maps DO appear
            // there (MapWorldsCourtyardOfWasting etc. per AtlasNodeDump.tsv).
            // Consider dropping these flag-based placeholders and reading
            // completion via AtlasMapNode.Completed instead.
            public const string CourtyardCleared = "CourtyardOfWastingComplete";
            public const string ChambersCleared = "ChambersOfImpurityComplete";
            public const string TheatreCleared = "TheatreOfLiesComplete";
            // Incarnations of Neglect and Fear - the two mid-tier
            // Originator-chain memory bosses (the "final" one is
            // Incarnation of Dread, tracked as Pinnacles.IncarnationOfDread).
            // Mappings from atlas-position + POE naming conventions:
            //   BenevolenceBoss -> Incarnation of Neglect
            //     (Neglect = absence of Benevolence)
            //   FearBoss        -> Incarnation of Fear
            //     (direct name match)
            // Wiki verification blocked by anti-bot. FearBossNonQuestDefeated
            // is a plausible-guess placeholder: no defeat flag surfaced in
            // the dump (only KeyHeldBefore + HaveQuestKey), so the catalog
            // validator will flag this as unresolved if it is wrong.
            public const string IncarnationOfNeglectDefeated = "BenevolenceBossNonQuestDefeated";
            public const string IncarnationOfFearDefeated = "FearBossNonQuestDefeated";
        }

        public static class Decayed
        {
            // TODO(unresolved): Guardian defeats. Dump has ZERO flags
            // matching Chimera / Hydra / Minotaur / Phoenix / Enslaver /
            // Constrictor / Purifier / Eradicator - the game tracks these
            // only via CompletedNodes (the Guardian maps ARE atlas nodes -
            // MapWorldsChimera etc. per AtlasNodeDump.tsv).
            //
            // Recommended follow-up: drop these placeholder flag lookups
            // and read Guardian completion from AtlasMapNode.Completed
            // for the four Shaper Guardian + four Elder Guardian nodes.
            // The catalog entries stay for now to preserve the API shape
            // while the readers migrate.
            public const string ChimeraDefeated = "ChimeraDefeated";
            public const string HydraDefeated = "HydraDefeated";
            public const string MinotaurDefeated = "MinotaurDefeated";
            public const string PhoenixDefeated = "PhoenixDefeated";
            public const string EnslaverDefeated = "EnslaverDefeated";
            public const string ConstrictorDefeated = "ConstrictorDefeated";
            public const string PurifierDefeated = "PurifierDefeated";
            public const string EradicatorDefeated = "EradicatorDefeated";
        }
    }

    // Every constant declared above must appear in this list.
    private static readonly string[] All =
    {
        Pinnacles.Maven, Pinnacles.Shaper, Pinnacles.Elder,
        Pinnacles.SearingExarch, Pinnacles.EaterOfWorlds,
        Pinnacles.IncarnationOfDread, Pinnacles.Sirus,

        Maven.AtlasLadder.BeaconAcquired,
        Maven.AtlasLadder.Stage1, Maven.AtlasLadder.Stage2, Maven.AtlasLadder.Stage3,
        Maven.AtlasLadder.Stage4, Maven.AtlasLadder.Stage5,
        Maven.ThemedInvitations.Formed, Maven.ThemedInvitations.Twisted,
        Maven.ThemedInvitations.Elderslayers, Maven.ThemedInvitations.Forgotten,
        Maven.ThemedInvitations.Remembered, Maven.ThemedInvitations.Feared,

        Voidstones.Eldritch.Exarch.EnvoyMet, Voidstones.Eldritch.Exarch.InfluenceUnlocked,
        Voidstones.Eldritch.Exarch.PolaricInvitationDropped, Voidstones.Eldritch.Exarch.BlackStarDefeated,
        Voidstones.Eldritch.Exarch.T12Cleared, Voidstones.Eldritch.Exarch.T13Cleared,
        Voidstones.Eldritch.Exarch.T14Cleared, Voidstones.Eldritch.Exarch.T15Cleared,
        Voidstones.Eldritch.Exarch.IncandescentInvitationDropped,
        Voidstones.Eldritch.Eater.FleshCompassReceived, Voidstones.Eldritch.Eater.InfluenceUnlocked,
        Voidstones.Eldritch.Eater.T9Cleared, Voidstones.Eldritch.Eater.T10Cleared,
        Voidstones.Eldritch.Eater.T12Cleared, Voidstones.Eldritch.Eater.T14Cleared,
        Voidstones.Eldritch.Eater.ScreamingInvitationDropped,
        Voidstones.Eldritch.Eater.InfiniteHungerDefeated,

        Voidstones.Originator.EagonMet, Voidstones.Originator.EagonIntroductionSeen,
        Voidstones.Originator.CourtyardCleared,
        Voidstones.Originator.ChambersCleared, Voidstones.Originator.TheatreCleared,
        Voidstones.Originator.IncarnationOfNeglectDefeated,
        Voidstones.Originator.IncarnationOfFearDefeated,

        Voidstones.Decayed.ChimeraDefeated, Voidstones.Decayed.HydraDefeated,
        Voidstones.Decayed.MinotaurDefeated, Voidstones.Decayed.PhoenixDefeated,
        Voidstones.Decayed.EnslaverDefeated, Voidstones.Decayed.ConstrictorDefeated,
        Voidstones.Decayed.PurifierDefeated, Voidstones.Decayed.EradicatorDefeated,
    };

    public sealed record ValidationResult(int Total, IReadOnlyList<string> Unresolved);

    public static ValidationResult Validate(GameController gc)
    {
        var known = new HashSet<string>();
        var runtime = gc?.IngameState?.Data?.ServerData?.QuestFlags;
        if (runtime != null)
        {
            foreach (var kvp in runtime)
            {
                var name = kvp.Key.ToString();
                if (!string.IsNullOrEmpty(name)) known.Add(name);
            }
        }

        var unresolved = new List<string>();
        foreach (var flag in All)
            if (!known.Contains(flag)) unresolved.Add(flag);
        return new ValidationResult(All.Length, unresolved);
    }
}
