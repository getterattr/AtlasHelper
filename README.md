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
2. Under **HUD Overlay**, tune colors and text scale to taste.
3. Toggle the HUD in-game with `F3` (rebindable).

## Development

Design docs live in `.claude/docs/internal/`:

- `context.md` - narrative design record, reader audit, blocking spikes.
- `roadmap.md` - current-state view of what's built, what's left, workstream order.
- `glossary.md` - canonical domain vocabulary (Phase, Voidstone, Corner, Quest Chain, ...).
- `questline.md` - wiki-sourced facts about voidstones, Maven ladder, endgame invitations.
- `strategy.md` - tactical business rules distilled from the league-start guide.
- `decisions/` - current-state design docs (`scope`, `state-model`, `gamestate`, `read-pattern`).

Runtime state is split into modules under `GameState/`:

- `Atlas/` - map graph, bonus completion, coord projection.
- `Voidstones/` - physical slot state and per-corner chain progression.
- `Maven/` - witness list, 5-stage ladder, themed invitations.
- `Pinnacles/` - individual boss kills.
- `Diagnostics/` - startup flag dump + snapshot health check for detecting broken readers after patch offset shifts.

UI surfaces live under `Ui/`, split by audience:

- `Ui/Overlays/` - in-game surfaces rendered while the player is playing (`HudOverlay`, `AtlasOverlay`, future `BossPathArrow`).
- `Ui/Panels/` - settings-panel content rendered in the ExileApi menu (`OverviewPanel`, `ProgressionReferencePanel`).
- `Ui/Theme.cs`, `Ui/ImGuiHelpers.cs` - shared styling and drawing utilities.

Plugins are compiled by `Loader.exe` on HUD startup; do not run `dotnet build`. To pin canonical `QuestFlag` names, trigger a milestone in-game and diff `QuestFlagDump.tsv` (written once per session by `FlagDiagnostics`).
