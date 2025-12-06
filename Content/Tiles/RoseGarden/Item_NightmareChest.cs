using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles.RoseGarden
{
	public class Item_NightmareChest : ModItem
	{
		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tile_NightmareChest>());
			// Item.placeStyle = 1; // Use this to place the chest in its locked style
			Item.width = 26;
			Item.height = 22;
			Item.value = 500;
		}
	}
}