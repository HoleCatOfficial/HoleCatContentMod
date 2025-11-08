using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

public class PitySystem : ModSystem
{
    // Dictionary: (npcType or bagItemID) -> itemID -> pity count
    public static Dictionary<int, Dictionary<int, int>> PityCounters = new();

    public static int GetPity(int sourceID, int itemID)
    {
        if (PityCounters.TryGetValue(sourceID, out var itemDict))
            if (itemDict.TryGetValue(itemID, out int value))
                return value;

        return 0;
    }

    public static void IncrementPity(int sourceID, int itemID)
    {
        if (!PityCounters.ContainsKey(sourceID))
            PityCounters[sourceID] = new Dictionary<int, int>();

        if (!PityCounters[sourceID].ContainsKey(itemID))
            PityCounters[sourceID][itemID] = 0;

        PityCounters[sourceID][itemID]++;
    }

    public static void ResetPity(int sourceID, int itemID)
    {
        if (PityCounters.ContainsKey(sourceID))
            PityCounters[sourceID].Remove(itemID);
    }
}
