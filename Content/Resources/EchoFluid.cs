using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Resources
{
	public class EchoFluid : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 25;
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 2; 
		}

		public override void SetDefaults() {
			Item.width = 26;
			Item.height = 28;
			Item.value = 20;
			Item.maxStack = 9999;
            Item.rare = ItemRarityID.White;
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater, 1)
                .AddIngredient<LifeEcho>(5)
				.Register();
		}
	}
}