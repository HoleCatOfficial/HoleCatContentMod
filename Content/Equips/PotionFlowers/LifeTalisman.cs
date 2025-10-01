
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.PotionFlowers
{
    public class LifeTalisman : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 38;
            Item.maxStack = 1;
            Item.value = 100;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {

        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LifeCrystal, 1)
                .AddIngredient(ItemID.SunBanner, 1)
                .AddIngredient(ItemID.OmegaBanner, 1)
                .Register();
        }
	}
}