namespace AtlasHelper.GameState.Readers;

internal static class PinnacleCompletionReader
{
    private const string MavenFlag = "TheMavenComplete";
    private const string ShaperFlag = "ShaperComplete";
    private const string ElderFlag = "ElderComplete";
    private const string SearingExarchFlag = "SearingExarchComplete";
    private const string EaterOfWorldsFlag = "EaterOfWorldsComplete";
    private const string IncarnationOfDreadFlag = "IncarnationOfDreadComplete";
    private const string SirusFlag = "A9Q1KilledSirus";
    private const string FormedFlag = "MavensInvitationTheFormedComplete";
    private const string TwistedFlag = "MavensInvitationTheTwistedComplete";
    private const string ElderslayersFlag = "MavensInvitationTheElderslayersComplete";
    private const string FearedFlag = "MavensInvitationTheFearedComplete";
    private const string ForgottenFlag = "MavensInvitationTheForgottenComplete";
    private const string RememberedFlag = "MavensInvitationTheRememberedComplete";

    public static PinnacleCompletion Read(QuestFlagLookup flags) => new(
        Maven: flags.Get(MavenFlag),
        Shaper: flags.Get(ShaperFlag),
        Elder: flags.Get(ElderFlag),
        SearingExarch: flags.Get(SearingExarchFlag),
        EaterOfWorlds: flags.Get(EaterOfWorldsFlag),
        IncarnationOfDread: flags.Get(IncarnationOfDreadFlag),
        Sirus: flags.Get(SirusFlag),
        Formed: flags.Get(FormedFlag),
        Twisted: flags.Get(TwistedFlag),
        Elderslayers: flags.Get(ElderslayersFlag),
        Feared: flags.Get(FearedFlag),
        Forgotten: flags.Get(ForgottenFlag),
        Remembered: flags.Get(RememberedFlag));
}
