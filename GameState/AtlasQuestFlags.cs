using System.Collections.Generic;
using ExileCore;

namespace AtlasHelper.GameState;

// Every flag name the plugin reads, grouped to mirror the GameState
// modules. Constants are either confirmed against QuestFlagDump.tsv, or
// marked TODO(unresolved) with candidate names inline. Validate() logs
// unresolved names at plugin startup.
//
// In-game vocabulary shortcuts:
//   Cleansing / CleansingFire = Exarch chain
//   Tangle / Consume          = Eater chain
//   Boss / MiniBoss           = pinnacle / mid-tier fight
internal static class AtlasQuestFlags
{
    public static class Pinnacles
    {
        public const string Maven = "MavenEnragedDefeated";
        public const string Shaper = "ShaperDefeated";
        public const string Elder = "ElderDefeated";
        public const string SearingExarch = "CleansingBossDefeated";
        public const string EaterOfWorlds = "ConsumeBossDefeated";
        // IgnoranceBoss is the only Incarnation atlas icon adjacent to
        // the Originator corner - matches the "final" chain fight.
        public const string IncarnationOfDread = "IgnoranceBossNonQuestDefeated";
        public const string Sirus = "SirusDefeated";
    }

    public static class Maven
    {
        public static class AtlasLadder
        {
            // TODO(unresolved): MetEnvoyMaven, HaveMavenMapDeviceAlteration.
            public const string BeaconAcquired = "GotMavensBeacon";

            // Semi-confident: MavenFirstInvitation matches Stage 1
            // ("first invitation seen/accepted"). Stage 2-5 have no
            // clear candidates in the dump. Also worth checking:
            // Entered{First,Second,Third}PinnacleBossArea.
            public const string Stage1 = "MavenFirstInvitation";
            public const string Stage2 = "MavensCrucibleStage2Complete";
            public const string Stage3 = "MavensCrucibleStage3Complete";
            public const string Stage4 = "MavensCrucibleStage4Complete";
            public const string Stage5 = "MavensCrucibleStage5Complete";
        }

        // TODO(unresolved): the six HaveMavenMapVoid1-6 flags line up
        // with six themed invitations. Ordering unknown - map after
        // completing each theme in a test session.
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

    public static class Voidstones
    {
        public static class Eldritch
        {
            public static class Exarch
            {
                public const string EnvoyMet = "MetEnvoyExarch";
                public const string InfluenceUnlocked = "HaveCleansingFireMapDeviceAlteration";
                public const string PolaricInvitationDropped = "HaveCleansingFireKey1";
                public const string BlackStarDefeated = "CleansingMiniBossDefeated";
                // TODO(migrate): no per-tier flags exist. Only
                // CleansingFireDefeatMapBoss (single, "any tier").
                // Migrate reader to CompletedNodes for per-tier.
                public const string T12Cleared = "SearingExarchT12Complete";
                public const string T13Cleared = "SearingExarchT13Complete";
                public const string T14Cleared = "SearingExarchT14Complete";
                public const string T15Cleared = "SearingExarchT15Complete";
                public const string IncandescentInvitationDropped = "HaveCleansingFireKey2";
            }

            public static class Eater
            {
                // FleshCompassReceived semantically = Envoy handed the
                // compass; stricter "compass in inventory" candidate is
                // PickUpTangleMapDeviceAlteration.
                public const string FleshCompassReceived = "MetEnvoyEater";
                public const string InfluenceUnlocked = "HaveTangleMapDeviceAlteration";
                // TODO(migrate): same story as Exarch above - only
                // TangleDefeatMapBoss (single). Use CompletedNodes.
                public const string T9Cleared = "EaterOfWorldsT9Complete";
                public const string T10Cleared = "EaterOfWorldsT10Complete";
                public const string T12Cleared = "EaterOfWorldsT12Complete";
                public const string T14Cleared = "EaterOfWorldsT14Complete";
                // Semi-confident: Tangle Key1 fires earlier (Writhing /
                // Infinite Hunger), Key2 later (Screaming). Reverse if
                // real progression shows otherwise.
                public const string ScreamingInvitationDropped = "HaveTangleKey2";
                public const string InfiniteHungerDefeated = "ConsumeMiniBossDefeated";
            }
        }

        public static class Originator
        {
            public const string EagonMet = "MetEagon";
            public const string EagonIntroductionSeen = "EagonIntroductionSeen";
            // TODO(migrate): no per-map flags. Only aggregate
            // NonQuestMemoryBossesDefeated + FirstMemoryBossSeen etc.
            // The three maps are atlas nodes - use CompletedNodes.
            public const string CourtyardCleared = "CourtyardOfWastingComplete";
            public const string ChambersCleared = "ChambersOfImpurityComplete";
            public const string TheatreCleared = "TheatreOfLiesComplete";
            // Incarnations - two mid-tier, one final (Dread in Pinnacles).
            // Ignorance = Dread, Benevolence = Neglect, Fear = Fear.
            // Fear flag is a guess (no defeat flag in dump, only KeyHeld
            // + HaveQuestKey); validator will flag if wrong.
            public const string IncarnationOfNeglectDefeated = "BenevolenceBossNonQuestDefeated";
            public const string IncarnationOfFearDefeated = "FearBossNonQuestDefeated";
        }

        // TODO(migrate): no Guardian defeat flags. All eight are
        // atlas nodes (MapWorldsChimera etc.) - use CompletedNodes.
        public static class Decayed
        {
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
