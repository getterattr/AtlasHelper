using System.Collections.Generic;
using ExileCore;

namespace AtlasHelper.GameState.Readers;

internal static class PassivesReader
{
    public static AtlasPassives Read(GameController gc)
    {
        var ids = gc.IngameState.Data.ServerData.AtlasPassiveSkillIds;
        if (ids == null || ids.Count == 0)
            return AtlasPassives.Empty;

        var lookup = gc.Files.PassiveSkills;
        var allocated = new List<AtlasPassive>(ids.Count);
        foreach (var id in ids)
        {
            var skill = lookup.GetPassiveSkillByPassiveId(id);
            if (skill == null) continue;
            allocated.Add(new AtlasPassive(id, skill.Id ?? string.Empty, skill.Name ?? string.Empty));
        }

        return new AtlasPassives(allocated);
    }
}
