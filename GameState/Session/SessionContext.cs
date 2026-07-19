namespace AtlasHelper.GameState.Session;

// Transient session context that is not owned by any atlas-domain module.
// Drives HUD in-town vs in-map variant switch and gates the InMapAdvice
// static rendering.
public sealed record SessionContext(bool IsInMap)
{
    public static SessionContext Empty { get; } = new(false);
}
