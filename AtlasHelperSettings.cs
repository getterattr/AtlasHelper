using System.Collections.Generic;
using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using Color = SharpDX.Color;

namespace AtlasHelper;

public class AtlasHelperSettings : ISettings
{
    [Menu("Enabled", "Enable or disable the AtlasHelper plugin.")]
    public ToggleNode Enable { get; set; } = new(false);

    [Menu("Phase Override", "Force the current progression phase. Auto derives it from live game state.")]
    public ListNode PhaseOverride { get; set; } = new()
    {
        Values = new List<string> { "Auto", "Phase 1", "Phase 2", "Phase 3", "Phase 4" },
        Value = "Auto",
    };

    [Menu("Branch", "Which endgame branch to bias recommendations toward once Phase 3 starts.")]
    public ListNode Branch { get; set; } = new()
    {
        Values = new List<string> { "Exarch Altars", "Destructive Play" },
        Value = "Exarch Altars",
    };

    [Menu("HUD Overlay", "Configure the always-on HUD panel showing Phase, voidstones, completion, and chain progress.")]
    public HudOverlaySettings Hud { get; set; } = new();
}

[Submenu(CollapsedByDefault = false)]
public class HudOverlaySettings
{
    [Menu("Show", "Show or hide the HUD panel.")]
    public ToggleNode Show { get; set; } = new(true);

    [Menu("Title", "Text shown in the HUD window title bar.")]
    public TextNode Title { get; set; } = new("AtlasHelper");

    [Menu("Opacity", "Global opacity for the HUD. 1.0 = fully opaque, lower values fade the entire overlay.")]
    public RangeNode<float> Opacity { get; set; } = new(1f, 0.1f, 1f);

    [Menu("Text Scale", "Size multiplier for the HUD text. 1.0 = normal size.")]
    public RangeNode<float> TextScale { get; set; } = new(1f, 0.5f, 3f);

    [Menu("Padding", "Inner spacing in pixels between the HUD text and window border.")]
    public RangeNode<float> Padding { get; set; } = new(8, 0, 40);

    [Menu("Text Color", "HUD text color.")]
    public ColorNode TextColor { get; set; } = new(new Color(230, 230, 230, 255));

    [Menu("Background Color", "HUD background color.")]
    public ColorNode BackgroundColor { get; set; } = new(new Color(0, 0, 0, 200));
}
