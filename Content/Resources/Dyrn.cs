using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Resources
{
	public class Dyrn : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 50;
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 2; // Influences the inventory sort order.
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 6));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;
			ItemID.Sets.ItemNoGravity[Item.type] = true;
		}

		public override void SetDefaults() {
			Item.width = 24;
			Item.height = 20;
			Item.value = 20;
			Item.maxStack = 9999;
            Item.rare = ItemRarityID.White;
		}

		public override void AddRecipes() {
			CreateRecipe(3)
				.AddIngredient(ItemID.VileMushroom, 1)
                .AddIngredient<Dyrn>(1)
				.AddTile(TileID.DemonAltar)
				.Register();
		}
	}
}