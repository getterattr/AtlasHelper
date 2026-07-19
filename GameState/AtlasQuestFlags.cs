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
            // MetEnvoyMaven is the moment Kirac/Envoy hands the Beacon
            // over; closest real flag to "have the Beacon".
            public const string BeaconAcquired = "MetEnvoyMaven";

            // Semi-confident: MavenFirstInvitation matches Stage 1
            // ("first invitation seen/accepted").
            public const string Stage1 = "MavenFirstInvitation";
            // TODO(fabricated): Files.QuestFlags catalog confirms
            // MavensCrucibleStage{2..5}Complete are NOT real POE flags.
            // The 5-stage ladder progression is not exposed as a
            // per-stage flag; likely derived from splinter/witness
            // counters in ServerData. Left as null-returning constants
            // for API stability; consumers must handle null.
            public const string Stage2 = "MavensCrucibleStage2Complete";
            public const string Stage3 = "MavensCrucibleStage3Complete";
            public const string Stage4 = "MavensCrucibleStage4Complete";
            public const string Stage5 = "MavensCrucibleStage5Complete";
        }

        // TODO(fabricated): Files.QuestFlags catalog confirms no
        // MavensInvitationThe{X}Complete flags exist. POE tracks themed
        // invitation completion via inventory/item state (invitation
        // items are consumed on run). If completion state is ever needed
        // it must come from a different source; catalog constants here
        // are placeholders that never resolve.
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
                public const string IncandescentInvitationDropped = "HaveCleansingFireKey2";
            }

            public static class Eater
            {
                // FleshCompassReceived semantically = Envoy handed the
                // compass; stricter "compass in inventory" candidate is
                // PickUpTangleMapDeviceAlteration.
                public const string FleshCompassReceived = "MetEnvoyEater";
                public const string InfluenceUnlocked = "HaveTangleMapDeviceAlteration";
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
            // Incarnations - two mid-tier, one final (Dread in Pinnacles).
            // Ignorance = Dread, Benevolence = Neglect, Fear = Fear.
            public const string IncarnationOfNeglectDefeated = "BenevolenceBossNonQuestDefeated";
            // TODO(fabricated): Files.QuestFlags catalog confirms no
            // FearBossNonQuestDefeated. Fear is fought only inside the
            // Threads-of-the-Originator quest chain - no non-quest
            // defeat mechanism exists. Fall back to derived state:
            // Fear is defeated when all three memory maps are Completed.
            public const string IncarnationOfFearDefeated = "FearBossNonQuestDefeated";
        }

        // Decayed: Guardian defeats live in AtlasMapNode.Completed;
        // DecayedReader reads the tree directly.
    }

    // Every constant we expect Files.QuestFlags to resolve. Flags marked
    // TODO(fabricated) above (Maven Crucible stages 2-5, themed
    // invitations, FearBossNonQuestDefeated) are intentionally excluded -
    // they do not exist in POE's flag definitions and would just add
    // noise to the startup validation log.
    private static readonly string[] All =
    {
        Pinnacles.Maven, Pinnacles.Shaper, Pinnacles.Elder,
        Pinnacles.SearingExarch, Pinnacles.EaterOfWorlds,
        Pinnacles.IncarnationOfDread, Pinnacles.Sirus,

        Maven.AtlasLadder.BeaconAcquired,
        Maven.AtlasLadder.Stage1,

        Voidstones.Eldritch.Exarch.EnvoyMet, Voidstones.Eldritch.Exarch.InfluenceUnlocked,
        Voidstones.Eldritch.Exarch.PolaricInvitationDropped, Voidstones.Eldritch.Exarch.BlackStarDefeated,
        Voidstones.Eldritch.Exarch.IncandescentInvitationDropped,
        Voidstones.Eldritch.Eater.FleshCompassReceived, Voidstones.Eldritch.Eater.InfluenceUnlocked,
        Voidstones.Eldritch.Eater.ScreamingInvitationDropped,
        Voidstones.Eldritch.Eater.InfiniteHungerDefeated,

        Voidstones.Originator.EagonMet, Voidstones.Originator.EagonIntroductionSeen,
        Voidstones.Originator.IncarnationOfNeglectDefeated,
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
