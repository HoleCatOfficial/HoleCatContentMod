
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Particles;
using DestroyerTest.Rarity;
 
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tools
{
    public class WretchedIdol : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 32;
            Item.value = 100;
            Item.rare = ModContent.RarityType<WretchedRarity>();
            Item.useAnimation = 60;
            Item.useTime = 60;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item92;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.TryGetGlobalNPC<CFNGlobal>(out var cf))
                {
                    if (cf.IsNodeSpawned)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        
        public override bool? UseItem(Player player)
        {
            return null;
        }

        public override bool ConsumeItem(Player player)
        {
            return true;
        }

        public override void OnConsumeItem(Player player)
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.TryGetGlobalNPC<CFNGlobal>(out var cf))
                {
                    if (cf.IsNodeSpawned || cf.Node != null)
                    {
                        npc.StrikeInstantKill();
                    }
                }
            }
        }
    }
}