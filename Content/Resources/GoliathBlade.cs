using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;

namespace DestroyerTest.Content.Resources
{
	public class GoliathBlade : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 1;
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 25; // Influences the inventory sort order. 59 is PlatinumBar, higher is more valuable.
            
		}

		public override void SetDefaults() {
			// ModContent.TileType returns the ID of the tile that this item should place when used. ModContent.TileType<T>() method returns an integer ID of the tile provided to it through its generic type argument (the type in angle brackets)
			Item.width = 32;
			Item.height = 32;
			Item.value = 120; // The cost of the item in copper coins. (1 = 1 copper, 100 = 1 silver, 1000 = 1 gold, 10000 = 1 platinum)
			Item.maxStack = 1;
            Item.rare = ModContent.RarityType<GoliathRarity>();
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.Obsidian, 25)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}