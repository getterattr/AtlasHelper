using System.Collections.Generic;
using ExileCore;

namespace AtlasHelper.GameState.Readers;

internal static class VoidstoneReader
{
    private const string PrimordialPathPrefix = "Metadata/Items/AtlasUpgrades/AtlasUpgradePrimordial";

    // Empirically confirmed on a fully-socketed character (3.29).
    // Slot index maps directly to atlas corner and voidstone identity.
    private static readonly (AtlasCorner Corner, VoidstoneKind Kind)[] SlotLayout =
    {
        (AtlasCorner.TopLeft, VoidstoneKind.Originator),
        (AtlasCorner.BottomLeft, VoidstoneKind.Eldritch),
        (AtlasCorner.TopRight, VoidstoneKind.Decayed),
        (AtlasCorner.BottomRight, VoidstoneKind.Ceremonial),
    };

    public static VoidstoneState Read(GameController gc)
    {
        var atlas = gc.IngameState.IngameUi.Atlas;
        if (atlas == null)
            return VoidstoneState.Empty;

        var slotList = atlas.VoidstoneSlots;
        if (slotList == null)
            return VoidstoneState.Empty;

        var slots = new List<VoidstoneSlot>(slotList.Count);
        for (int i = 0; i < slotList.Count; i++)
        {
            var (corner, kind) = i < SlotLayout.Length
                ? SlotLayout[i]
                : (AtlasCorner.Unknown, VoidstoneKind.Unknown);

            var slot = slotList[i];
            var socketed = slot != null && !slot.IsEmpty && IsPrimordialPath(slot.Voidstone?.Entity?.Path);
            slots.Add(new VoidstoneSlot(i, corner, kind, socketed));
        }

        return new VoidstoneState(slots);
    }

    private static bool IsPrimordialPath(string path) =>
        path != null && path.StartsWith(PrimordialPathPrefix);
}
