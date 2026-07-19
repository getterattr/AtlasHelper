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

1. Open the AtlasHelper settings panel in the ExileApi menu.
2. Under **Progression**, pick a **Strategy** (`Destructive Play` or `Exarch Altars`). See `Help` for what each phase and strategy means.
3. Under **HUD Overlay**, tune colors and text scale to taste.
4. Toggle the HUD in-game with `F3` (rebindable).
