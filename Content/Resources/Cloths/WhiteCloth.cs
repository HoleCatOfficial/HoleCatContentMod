using DestroyerTest.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Resources.Cloths
{
	public class WhiteCloth : ModItem
	{
		public override void SetStaticDefaults() 
		{
			Item.StaticDefaultToCloth();
		}

		public override void SetDefaults() 
		{
			Item.DefaultToCloth();
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.TatteredCloth, 2)
                .AddIngredient<EchoFluid>(1)
				.Register();
            CreateRecipe()
				.AddIngredient(ItemID.Silk, 2)
                .AddIngredient<EchoFluid>(1)
				.Register();
			foreach (int itemType in ContentSamples.ItemsByType.Keys)
        {
            Item item = new Item();
            item.SetDefaults(itemType);

            if (item.TryGetGlobalItem(out NonWhiteCloth globalItem) && globalItem.isNonWhiteCloth)
            {
                Recipe recipe = CreateRecipe();
                recipe.AddIngredient(itemType); // Uses the non-white cloth item
                recipe.AddIngredient<EchoFluid>(2); // Your special dye remover
                recipe.AddTile(TileID.DyeVat); // Example crafting station
                recipe.Register();
            }
        }
				
		}
	}
}