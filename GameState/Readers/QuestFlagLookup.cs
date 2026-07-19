using System.Collections.Generic;
using ExileCore;

namespace AtlasHelper.GameState.Readers;

internal sealed class QuestFlagLookup
{
    private readonly Dictionary<string, bool> _values;

    private QuestFlagLookup(Dictionary<string, bool> values)
    {
        _values = values;
    }

    public bool? Get(string flagName) =>
        _values.TryGetValue(flagName, out var value) ? value : null;

    public IEnumerable<string> Keys => _values.Keys;

    public IEnumerable<KeyValuePair<string, bool>> WhereNameContains(params string[] substrings)
    {
        foreach (var kvp in _values)
        {
            foreach (var needle in substrings)
            {
                if (kvp.Key.Contains(needle, System.StringComparison.OrdinalIgnoreCase))
                {
                    yield return kvp;
                    break;
                }
            }
        }
    }

    public static QuestFlagLookup Build(GameController gc)
    {
        var flags = gc.IngameState.Data.ServerData.QuestFlags;
        var dict = new Dictionary<string, bool>(flags?.Count ?? 0);
        if (flags == null) return new QuestFlagLookup(dict);

        foreach (var kvp in flags)
        {
            var name = kvp.Key.ToString();
            if (!string.IsNullOrEmpty(name))
                dict[name] = kvp.Value;
        }

        return new QuestFlagLookup(dict);
    }
}
