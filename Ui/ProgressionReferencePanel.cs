using ImGuiNET;

namespace AtlasHelper.Ui;

internal static class ProgressionReferencePanel
{
    private static readonly (string, string, string)[] Phases =
    {
        ("Phase 1", "First voidstone",  "Rush T16 bottom-left. Exarch + Eater."),
        ("Phase 2", "Ten-Way Maven",    "Farm Maven maps. Second voidstone top-left."),
        ("Phase 3", "Full completion",  "Sweep uncompleted maps at magic."),
        ("Phase 4", "Final voidstones", "Self-farm or buy a carry (see Strategy)."),
    };

    private static readonly (string, string, string)[] Strategies =
    {
        ("Destructive Play", "Self-farm",     "Kill Shaper, Elder, and Maven yourself. Bossing prep from Phase 3."),
        ("Exarch Altars",    "Currency farm", "Skip pinnacle fights. Farm altars, buy a carry for the final voidstones."),
    };

    public static void Draw()
    {
        ImGuiHelpers.SectionLabel("Phases", "What each phase means and when it starts.");
        ImGuiHelpers.ReferenceTable(
            "##AtlasHelperReferencePhases",
            ("Phase", 0.14f), ("Focus", 0.22f), ("Details", 0.64f),
            Phases);

        ImGui.Spacing();
        ImGuiHelpers.SectionLabel("Strategies", "How you plan to earn the final two voidstones.");
        ImGuiHelpers.ReferenceTable(
            "##AtlasHelperReferenceStrategies",
            ("Strategy", 0.22f), ("Focus", 0.18f), ("Details", 0.60f),
            Strategies);
    }
}
