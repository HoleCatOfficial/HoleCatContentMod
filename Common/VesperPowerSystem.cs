using System;
using System.Collections.Generic;
using DestroyerTest.Content.Equips;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class VesperPowerSystem : ModSystem
    {

        public bool PowerUp = false;
        public bool PowerDown = false;

        public List<NPC> HappyNPCs = new List<NPC>();
        public List<NPC> UnhappyNPCs = new List<NPC>();

        public void DetectMassNPCHappiness()
        {
            
            foreach(NPC npc in Main.npc)
            {
                if (!npc.townNPC)
                {
                    continue;
                }

                if (npc.townNPC)
                {
                    ShoppingSettings settings = Main.ShopHelper.GetShoppingSettings(Main.LocalPlayer, npc);
                    if (settings.PriceAdjustment <= 0.94f || settings.PriceAdjustment <= 0.88f)
                    {
                        HappyNPCs.Add(npc);
                    }
                    else
                    {
                        UnhappyNPCs.Add(npc);
                    }
                    // Refer to https://terraria.wiki.gg/wiki/NPCs#Happiness's table of four faces
                    /*
                    if (however we check that an NPC's happiness is in the upper two faces)
                    {
                        HappyNPCs.Add(npc);
                    }
                    else //Bottom two faces
                    {
                        UnhappyNPCs.Add(npc)
                    }
                    */
                }
            }
        }

        public override void PostUpdateNPCs()
        {
            DetectMassNPCHappiness();

            if (HappyNPCs.Count > 10)
            {
                PowerUp = true;
            }

            if (UnhappyNPCs.Count > 10)
            {
                PowerDown = true;
            }
        }
    }

    public class VesperPowerItem : GlobalItem
    {
        public override bool InstancePerEntity => true; 
        public bool powerup = false;
        public bool powerdown = false;
        public static List<int> VesperItems = new List<int>();

        public override void UpdateInventory(Item item, Player player)
        {
            var system = ModContent.GetInstance<VesperPowerSystem>();
            powerup = system.PowerUp;
            powerdown = system.PowerDown;
        }
        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            if (VesperItems.Contains(item.type))
            {
                if (powerup)
                {
                    crit += 10;
                }
                if (powerdown)
                {
                    crit -= 10;
                }
            }
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (VesperItems.Contains(item.type))
            {
                if (powerup)
                {
                    damage *= 1.1f;
                }
                if (powerdown)
                {
                    damage *= 0.9f;
                }
            }
        }
    }
}