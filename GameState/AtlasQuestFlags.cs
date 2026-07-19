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
    public static class Pinnacles
    {
        public const string Maven = "TheMavenComplete";
        public const string Shaper = "ShaperComplete";
        public const string Elder = "ElderComplete";
        public const string SearingExarch = "SearingExarchComplete";
        public const string EaterOfWorlds = "EaterOfWorldsComplete";
        public const string IncarnationOfDread = "IncarnationOfDreadComplete";
        public const string Sirus = "SirusDefeated";
    }

    // Maven-owned state: witness ladder and themed invitations.
    public static class Maven
    {
        public static class AtlasLadder
        {
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
                public const string BlackStarDefeated = "BlackStarDefeated";
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
            }
        }

        public static class Originator
        {
            // TODO: pin canonical names via QuestFlagDump.tsv - placeholders
            public const string EagonIntroduced = "EagonIntroduced";
            public const string CourtyardCleared = "CourtyardOfWastingComplete";
            public const string ChambersCleared = "ChambersOfImpurityComplete";
            public const string TheatreCleared = "TheatreOfLiesComplete";
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

    // Map Device toggles. Cross-cutting - feeds several voidstone chains.
    public static class Beacons
    {
        // TODO: pin canonical names via QuestFlagDump.tsv
        public const string Maven = "GotMavensBeacon";
        public const string SearingExarch = "SearingExarchIntroduced";
        public const string EaterOfWorlds = "EaterOfWorldsIntroduced";
    }

    // Every constant declared above must appear in this list.
    private static readonly string[] All =
    {
        Pinnacles.Maven, Pinnacles.Shaper, Pinnacles.Elder,
        Pinnacles.SearingExarch, Pinnacles.EaterOfWorlds,
        Pinnacles.IncarnationOfDread, Pinnacles.Sirus,

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

        Voidstones.Originator.EagonIntroduced, Voidstones.Originator.CourtyardCleared,
        Voidstones.Originator.ChambersCleared, Voidstones.Originator.TheatreCleared,

        Voidstones.Decayed.ChimeraDefeated, Voidstones.Decayed.HydraDefeated,
        Voidstones.Decayed.MinotaurDefeated, Voidstones.Decayed.PhoenixDefeated,
        Voidstones.Decayed.EnslaverDefeated, Voidstones.Decayed.ConstrictorDefeated,
        Voidstones.Decayed.PurifierDefeated, Voidstones.Decayed.EradicatorDefeated,

        Beacons.Maven, Beacons.SearingExarch, Beacons.EaterOfWorlds,
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
