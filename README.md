# AtlasHelper

An ExileApi overlay plugin for Path of Exile. Guides league-start atlas progression from empty atlas to four voidstones: tracks the current phase, highlights which map to run next, and points at the boss room while you're in a map.

## Install

### Requirements

- Path of Exile should be running in Windowed or Windowed Fullscreen mode.
- [ExileApi](https://github.com/exApiTools/ExileApi-Compiled).
- .NET 10 SDK.

### Install With PluginUpdater

1. Open [ExileApi](https://github.com/exApiTools/ExileApi-Compiled).
2. Open the `PluginUpdater` plugin.
3. Click the Add tab.
4. Paste `https://github.com/getterattr/AtlasHelper` into Repository URL.
5. Click Clone.
6. Either restart ExileApi, or open ExileApi Core settings, scroll down, and press Reload Plugins.

### Install From Source Folder

1. Download or clone this repository.
2. Place the `AtlasHelper` folder inside your `Plugins/Source/` directory.
3. Launch ExileApi.
4. Let the host compile the plugin.
5. Enable AtlasHelper in the plugin settings.

## Setup

1. Open the AtlasHelper settings panel in the ExileApi menu and expand **Overview** to confirm plugin state, current phase, strategy, and HUD visibility.
2. Expand **Progression** and pick a **Strategy** - `Destructive Play` (self-farm the final voidstones) or `Exarch Altars` (currency-farm to buy a carry). Leave **Phase Override** on `Auto` unless you want to preview a later phase.
3. Open **Progression > Help > Reference** for a compact table of what each phase and strategy means.
4. Expand **HUD Overlay** to tune the always-on panel: `Show`, `Text Scale`, `Text Color`, `Background Color`. Use the alpha channel on the color wheel for translucent overlays.
5. Toggle the HUD in-game with `F3` (rebindable under **HUD Overlay > Toggle Hotkey**).
