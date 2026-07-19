using ImGuiNET;

namespace AtlasHelper.Ui.Panels;

internal static class ProgressionReferencePanel
{
    private static readonly (string, string, string)[] Phases =
    {
        ("Phase 1", "First voidstone",  "Rush T16 bottom-left. Exarch + Eater."),
        ("Phase 2", "Ten-Way Maven",    "Farm Maven maps. Second voidstone top-left."),
        ("Phase 3", "Full completion",  "Sweep uncompleted maps at required rarity."),
        ("Phase 4", "Final voidstones", "Self-farm Shaper + Elder, then the Maven."),
    };

    public static void Draw()
    {
        ImGuiHelpers.SectionLabel("Phases", "What each phase means and when it starts.");
        ImGuiHelpers.ReferenceTable(
            "##AtlasHelperReferencePhases",
            ("Phase", 0.14f), ("Focus", 0.22f), ("Details", 0.64f),
            Phases);
    }
}
