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

            // Crucible ladder (3/4/5/6/10-way) has no per-stage flag.
            // Completion is derived purely from
            // ServerData.MavenWitnessedAreas.Count against cumulative
            // thresholds - see AtlasInvitationReader. Quest-item flags
            // are intentionally ignored: writs relate to the pinnacle
            // Maven fight, not to Crucible ladder progression.
        }

        // Themed-invitation unlock state. HaveMavenMapVoid{N} flips true
        // once the requisite bosses have been witnessed and Kirac begins
        // offering the invitation; it stays true regardless of whether
        // the fight has been completed. Not an inventory flag - these
        // invitations are one-shot consumables. Completion state itself
        // has no dedicated flag in Files.QuestFlags; the tooltip's 1/1
        // vs 0/1 count is derived elsewhere (likely a per-quest-reward
        // ledger tied to QuestPassiveSkillPoints grants).
        public static class ThemedInvitations
        {
            public const string FormedUnlocked = "HaveMavenMapVoid1";
            public const string TwistedUnlocked = "HaveMavenMapVoid2";
            public const string ForgottenUnlocked = "HaveMavenMapVoid3";
            public const string RememberedUnlocked = "HaveMavenMapVoid4";
            public const string FearedUnlocked = "HaveMavenMapVoid5";
            public const string ElderslayersUnlocked = "HaveMavenMapVoid6";
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
            // Fear is a quest-only boss - no NonQuestDefeated flag
            // exists. FearBossKeyHeldBefore fires the first time the
            // player picks up the Fear key (crafted by Eagon after all
            // three memory maps clear). It is the closest proxy for
            // "player has reached the Fear fight"; a true "defeated"
            // signal must be derived from the Ceremonial voidstone
            // being socketed in AtlasTree.
            public const string IncarnationOfFearDefeated = "FearBossKeyHeldBefore";
        }

        // Decayed: Guardian defeats live in AtlasMapNode.Completed;
        // DecayedReader reads the tree directly.
    }

    // Every constant we expect Files.QuestFlags to resolve. Themed
    // invitation completions are omitted - POE tracks those via
    // consumed inventory items, not flags.
    private static readonly string[] All =
    {
        Pinnacles.Maven, Pinnacles.Shaper, Pinnacles.Elder,
        Pinnacles.SearingExarch, Pinnacles.EaterOfWorlds,
        Pinnacles.IncarnationOfDread, Pinnacles.Sirus,

        Maven.AtlasLadder.BeaconAcquired,

        Voidstones.Eldritch.Exarch.EnvoyMet, Voidstones.Eldritch.Exarch.InfluenceUnlocked,
        Voidstones.Eldritch.Exarch.PolaricInvitationDropped, Voidstones.Eldritch.Exarch.BlackStarDefeated,
        Voidstones.Eldritch.Exarch.IncandescentInvitationDropped,
        Voidstones.Eldritch.Eater.FleshCompassReceived, Voidstones.Eldritch.Eater.InfluenceUnlocked,
        Voidstones.Eldritch.Eater.ScreamingInvitationDropped,
        Voidstones.Eldritch.Eater.InfiniteHungerDefeated,

        Voidstones.Originator.EagonMet, Voidstones.Originator.EagonIntroductionSeen,
        Voidstones.Originator.IncarnationOfNeglectDefeated,
        Voidstones.Originator.IncarnationOfFearDefeated,

        Maven.ThemedInvitations.FormedUnlocked, Maven.ThemedInvitations.TwistedUnlocked,
        Maven.ThemedInvitations.ForgottenUnlocked, Maven.ThemedInvitations.RememberedUnlocked,
        Maven.ThemedInvitations.FearedUnlocked, Maven.ThemedInvitations.ElderslayersUnlocked,
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
