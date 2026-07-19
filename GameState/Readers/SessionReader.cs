using AtlasHelper.GameState.Session;
using ExileCore;

namespace AtlasHelper.GameState.Readers;

internal static class SessionReader
{
    public static SessionContext Read(GameController gc)
    {
        var area = gc.Area?.CurrentArea;
        if (area == null)
            return SessionContext.Empty;

        // ExileCore's AreaInstance exposes no IsMap boolean; derive from the
        // negative predicate against IsTown / IsHideout / IsPeaceful. The
        // three flags together cover hideouts, town instances, and other
        // safe zones (rogue harbour, guild hall). Campaign zones satisfy the
        // predicate too, but the plugin is league-start-atlas-oriented and
        // the HUD carries no meaningful campaign content anyway.
        var isInMap = !area.IsTown && !area.IsHideout && !area.IsPeaceful;
        return new SessionContext(isInMap);
    }
}
