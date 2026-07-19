using System.Collections.Generic;
using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using Newtonsoft.Json;
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

    [Menu("Phase Guide", "What each phase means and when it starts. Read this before overriding.")]
    [JsonIgnore]
    public CustomNode PhaseGuide { get; set; } = new();

    [Menu(
        "Strategy",
        "How you plan to acquire the 3rd and 4th voidstones.\n\n" +
        "Destructive Play (default): you kill the Shaper, Elder, and Maven yourself. Recommendations from Phase 3 onward favour bossing prep - farming their invitations and running the fights.\n\n" +
        "Exarch Altars: you don't want to fight the pinnacle bosses. Recommendations favour currency farming (Exarch altars) so you can buy a carry for the last two voidstones instead.")]
    public ListNode Strategy { get; set; } = new()
    {
        Values = new List<string> { "Destructive Play", "Exarch Altars" },
        Value = "Destructive Play",
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
