using DestroyerTest.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Resources.Cloths
{
    public class LimeCloth : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.StaticDefaultToCloth();
        }

        public override void SetDefaults()
        {
            Item.DefaultToCloth();
        }

        public override void AddRecipes()
        {
            Item.DefaultRecipe(ItemID.JungleGrassSeeds, ItemID.Blinkroot);
        }
    }
}