using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using System.Collections.Generic;

namespace AtlasHelper;

public class AtlasHelperSettings : ISettings
{
    public ToggleNode Enable { get; set; } = new ToggleNode(false);

    public ToggleNode ShowHud { get; set; } = new ToggleNode(true);

    public ListNode PhaseOverride { get; set; } = new ListNode
    {
        Values = new List<string> { "Auto", "Phase 1", "Phase 2", "Phase 3", "Phase 4" },
        Value = "Auto",
    };

    public ListNode Branch { get; set; } = new ListNode
    {
        Values = new List<string> { "Exarch Altars", "Destructive Play" },
        Value = "Exarch Altars",
    };
}
