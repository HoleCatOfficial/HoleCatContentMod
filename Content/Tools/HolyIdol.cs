
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
    public class HolyIdol : ModItem
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
            Item.rare = ModContent.RarityType<HallowedSpecialRarity>();
            Item.useAnimation = 60;
            Item.useTime = 60;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item92;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
        }

        public override bool? UseItem(Player player)
        {
            return true;
        }

        public override void OnConsumeItem(Player player)
        {
            foreach(NPC npc in Main.npc)
            {
                if (npc.active && npc.TryGetGlobalNPC<BNGlobal>(out var B))
                {
                    if (B.IsNodeSpawned)
                    {
                        npc.StrikeInstantKill();
                    }
                }
            }
        }
       
    }
}