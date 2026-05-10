using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.PotionFlowers
{
    public class EphemeralSolvent : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 106;
            Item.maxStack = 1;
            Item.value = 1000;
            Item.accessory = true;
            Item.rare = ItemRarityID.Cyan;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<PotionFlowerPlayer>(out PotionFlowerPlayer flower))
            {
                flower.EphemeralSolvent = true;
            }
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RadiantRose>(1)
                .AddIngredient<LifeTalisman>(1)
                .AddIngredient<LifeEcho>(8)
                .AddIngredient<StellarMatter>(12)
                .Register();
        }
	}
}