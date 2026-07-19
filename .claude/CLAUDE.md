# Claude Code Configuration

> **Note**: For implementation overview, read `README.md` in this directory.

## Quick Start

1. Read `README.md` for what this plugin does and how it's wired.
2. Read `.claude/docs/internal/context.md` for the narrative design record (what was decided, what was rejected, what the blocking spike is).
3. Read `.claude/docs/internal/glossary.md` before using any domain term (Phase, Voidstone, Corner, Quest Chain, Bonus Completion, Run Priority).
4. Check `.claude/docs/internal/adr/` for architectural decisions before proposing changes to scope or persistence:
   - `0001-scope-league-start-only.md` - plugin is league-start only, do not add late-league features.
   - `0002-derived-state-primary.md` - state is derived from game memory; only the standard `BaseSettingsPlugin` settings file persists user config.
5. Load relevant skills based on your task (skills live in `~/.claude/skills/` and load on demand):
   - `exileapi` - ExileApi API reference, lifecycle, performance rules
   - `bridge` - Query live game state via the MCP bridge
   - `dotnet` - C# / .NET tooling
   - `commits` - Conventional commit messages
   - `publish` - Publish this plugin to GitHub
6. Plugins are compiled by `Loader.exe` on HUD startup. Do not run `dotnet build` to produce runtime DLLs.

## Settings

Pre-allowed permissions are in `.claude/settings.json`. Local overrides go in `.claude/settings.local.json` (gitignored).
