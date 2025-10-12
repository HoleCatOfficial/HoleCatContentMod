using System.Collections.Generic;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Equips.PetrifiedSet;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class ShieldManager : ModSystem
    {
        // Singleton-style access
        public static ShieldManager Instance;

        // The ID of the active shield (or -1 if none)
        public int ActiveShieldID = -1;

        // Track who owns it (player index)
        public int Owner = -1;

        // Optional: priority values for shields
        public Dictionary<int, int> ShieldPriorities = new();

        public override void OnWorldLoad()
        {
            Instance = this;
            ShieldPriorities.Clear();
            ShieldRegistry.Clear();

            // Register shield classes and assign known IDs
            int infernalID = ShieldRegistry.Register<InfernalShieldPlayer>();
            int petrifiedID = ShieldRegistry.Register<PetrifiedShieldPlayer>();

            RegisterShield(infernalID, 1);
            RegisterShield(petrifiedID, 2);

            ActiveShieldID = -1;
            Owner = -1;
        }

        public override void OnWorldUnload()
        {
            Instance = null;
        }

        // Registers a new shield type with its priority
        public void RegisterShield(int id, int priority)
        {
            ShieldPriorities[id] = priority;
        }

        // Tries to activate a shield
        public bool TryActivateShield(Player player, int shieldID)
        {
            if (!ShieldPriorities.TryGetValue(shieldID, out int priority))
                return false; // Unknown shield

            // If no active shield or this one is higher priority
            if (ActiveShieldID == -1 || priority > ShieldPriorities[ActiveShieldID])
            {
                DeactivateShield();
                ActiveShieldID = shieldID;
                Owner = player.whoAmI;
                // Optionally spawn visuals, etc
                return true;
            }

            return false; // Denied due to lower priority
        }

        // Deactivates the active shield
        public void DeactivateShield()
        {
            if (ActiveShieldID != -1)
            {
                // Kill shield projectiles or visuals here
                ActiveShieldID = -1;
                Owner = -1;
            }
        }

        // Optional: check if shield belongs to a given player
        public bool IsOwner(Player player)
        {
            return player.whoAmI == Owner;
        }
    }

    public static class ShieldIDs
    {
        public const int Infernal = 0;
        public const int Petrified = 1;
        public static int GetShieldID()
        {
            return -1;
        }
    }

}
