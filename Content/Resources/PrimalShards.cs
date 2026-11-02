using DestroyerTest.Common;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Resources
{
    public class PrimalShards : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 100;
            Item.alpha = 0;
            Item.rare = ModContent.RarityType<CrimsonSpecialRarity>();
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, ColorLib.Ichor.ToVector3() * 0.55f * Main.essScale);
        }
    }
}
