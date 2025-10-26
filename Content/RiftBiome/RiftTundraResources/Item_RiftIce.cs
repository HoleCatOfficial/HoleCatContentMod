using DestroyerTest.Content.Resources;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RiftBiome.RiftTundraResources
{
	public class Item_RiftIce : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 100;
            Item.material = true;
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tile_RiftIce>());
			Item.width = 12;
			Item.height = 12;
		}

		public override void AddRecipes() {
			CreateRecipe(2)
				.AddIngredient<Living_Shadow>(2)
                .AddIngredient(ItemID.IceBlock, 2)
				.AddTile(TileID.Blendomatic)
				.Register();
		}

	}
}