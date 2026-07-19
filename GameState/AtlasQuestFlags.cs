using System.Collections.Generic;
using ExileCore;

namespace AtlasHelper.GameState;

// Central catalog of every QuestFlag name the plugin reads, grouped to
// mirror the GameState modules. Names marked TODO are placeholders until
// identified via QuestFlagDump.tsv (see AtlasHelper.DumpFlagCandidatesOnce).
// Unresolved names are surfaced at startup by Validate() and read back as
// null at runtime - distinguishable from a known-false flag by consumers.
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
            // TODO: pin canonical name via QuestFlagDump.tsv - placeholder
            public const string BeaconAcquired = "GotMavensBeacon";
            public const string Stage1 = "MavensCrucibleStage1Complete";
            public const string Stage2 = "MavensCrucibleStage2Complete";
            public const string Stage3 = "MavensCrucibleStage3Complete";
            public const string Stage4 = "MavensCrucibleStage4Complete";
            public const string Stage5 = "MavensCrucibleStage5Complete";
        }

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
                // TODO: pin canonical names via QuestFlagDump.tsv - placeholders
                public const string EnvoyMet = "EnvoyMet";
                public const string InfluenceUnlocked = "SearingExarchInfluenceUnlocked";
                public const string PolaricInvitationDropped = "PolaricInvitationDropped";
                // Confirmed via QuestFlagDump.tsv - Black Star = Exarch mini-boss.
                public const string BlackStarDefeated = "CleansingMiniBossDefeated";
                public const string T12Cleared = "SearingExarchT12Complete";
                public const string T13Cleared = "SearingExarchT13Complete";
                public const string T14Cleared = "SearingExarchT14Complete";
                public const string T15Cleared = "SearingExarchT15Complete";
                public const string IncandescentInvitationDropped = "IncandescentInvitationDropped";
            }

            public static class Eater
            {
                // TODO: pin canonical names via QuestFlagDump.tsv - placeholders
                public const string FleshCompassReceived = "FleshCompassReceived";
                public const string InfluenceUnlocked = "EaterOfWorldsInfluenceUnlocked";
                public const string T9Cleared = "EaterOfWorldsT9Complete";
                public const string T10Cleared = "EaterOfWorldsT10Complete";
                public const string T12Cleared = "EaterOfWorldsT12Complete";
                public const string T14Cleared = "EaterOfWorldsT14Complete";
                public const string ScreamingInvitationDropped = "ScreamingInvitationDropped";
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
            // TODO: individual memory-map completion tracking - dump
            // shows only aggregate "NonQuestMemoryBossesDefeated" plus
            // FirstMemoryBossSeen / SecondMemoryBossSeen /
            // ThirdMemoryBossSeen. Split-per-map completion flags may
            // exist under other names; leaving placeholders.
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
            // TODO: pin canonical names via QuestFlagDump.tsv - placeholders
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
