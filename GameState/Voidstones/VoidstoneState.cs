using System.Collections.Generic;

namespace AtlasHelper.GameState.Voidstones;

public enum AtlasCorner
{
    Unknown,
    TopLeft,
    BottomLeft,
    TopRight,
    BottomRight,
}

public enum VoidstoneKind
{
    Unknown,
    Originator,
    Eldritch,
    Decayed,
    Ceremonial,
}

public sealed record VoidstoneSlot(int Index, AtlasCorner Corner, VoidstoneKind Kind, bool Socketed);

public sealed record VoidstoneState(IReadOnlyList<VoidstoneSlot> Slots)
{
    public int SocketedCount
    {
        get
        {
            var count = 0;
            foreach (var slot in Slots)
                if (slot.Socketed) count++;
            return count;
        }
    }

    public static VoidstoneState Empty { get; } = new(new List<VoidstoneSlot>());
}
