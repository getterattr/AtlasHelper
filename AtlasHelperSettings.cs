using System.Collections.Generic;
using System.Windows.Forms;
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

    [Menu("Debug Logs", "Write detailed logs to aid troubleshooting.")]
    public ToggleNode DebugLogging { get; set; } = new(false);

    [Menu("Overview", "Snapshot of your current configuration.")]
    [JsonIgnore]
    public CustomNode Overview { get; set; } = new();

    [Menu("Configuration")]
    [JsonIgnore]
    public CustomNode ConfigurationHeader { get; set; } = new();

    [Menu("Progression", "Which progression phase to display and how you plan to acquire the final voidstones.")]
    public ProgressionSettings Progression { get; set; } = new();

    [Menu("HUD Overlay", "The always-on HUD panel showing Phase, voidstones, completion, and chain progress.")]
    public HudOverlaySettings Hud { get; set; } = new();

    [Menu("Atlas Overlay", "Highlights over uncompleted atlas map nodes when the atlas panel is open.")]
    public AtlasOverlaySettings AtlasOverlay { get; set; } = new();
}

[Submenu(CollapsedByDefault = true)]
public class ProgressionSettings
{
    [Menu("Phase Override", "Force the current progression phase. Auto derives it from live game state.")]
    public ListNode PhaseOverride { get; set; } = new()
    {
        Values = new List<string> { "Auto", "Phase 1", "Phase 2", "Phase 3", "Phase 4" },
        Value = "Auto",
    };

    [Menu("Reference", "Phase definitions.")]
    [JsonIgnore]
    public CustomNode Reference { get; set; } = new();
}

[Submenu(CollapsedByDefault = true)]
public class HudOverlaySettings
{
    [Menu("Show", "Show or hide the HUD panel.")]
    public ToggleNode Show { get; set; } = new(true);

    [Menu("Toggle Hotkey", "Key that toggles the HUD panel on and off.")]
    public HotkeyNodeV2 ToggleHotkey { get; set; } = new(Keys.F3);

    [Menu("Text Scale", "Size multiplier for the HUD text. 1.0 = normal size.")]
    public RangeNode<float> TextScale { get; set; } = new(1f, 0.5f, 3f);

    [Menu("Text Color", "HUD text color. Use the alpha channel on the color wheel to fade text.")]
    public ColorNode TextColor { get; set; } = new(new Color(230, 230, 230, 255));

    [Menu("Background Color", "HUD background color. Use the alpha channel on the color wheel to make the panel translucent.")]
    public ColorNode BackgroundColor { get; set; } = new(new Color(0, 0, 0, 200));
}

[Submenu(CollapsedByDefault = true)]
public class AtlasOverlaySettings
{
    [Menu("Show", "Draw highlights over uncompleted atlas nodes while the atlas panel is open.")]
    public ToggleNode Show { get; set; } = new(true);

    [Menu("Include Unique Maps", "Highlight uncompleted unique atlas nodes (bonus caps at 10 uniques).")]
    public ToggleNode IncludeUniques { get; set; } = new(true);

    [Menu("Hide Uniques Past Cap", "Once 10 unique-map bonus completions are earned, stop highlighting the remaining uncompleted uniques (they no longer grant bonus points).")]
    public ToggleNode HideUniquesPastCap { get; set; } = new(true);

    [Menu("Highlight Color", "Color of the outlined highlight over an uncompleted node.")]
    public ColorNode HighlightColor { get; set; } = new(new Color(255, 200, 60, 220));

    [Menu("Border Thickness", "Stroke width of the highlight border in pixels.")]
    public RangeNode<float> BorderThickness { get; set; } = new(2f, 1f, 6f);
}
