using Terraria.Enums;
using Terraria.ModLoader;
using DestroyerTest.Content.Tiles;

namespace DestroyerTest.Content.Tiles
{
	/// <summary>
	/// The coupled item that places the Example Pylon tile. For more information on said tile,
	/// see <seealso cref="ExamplePylonTile"/>.
	/// </summary>
	public class Item_RiftPylon : ModItem
	{
		public override void SetDefaults() {
			// Basically, this a just a shorthand method that will set all default values necessary to place
			// the passed in tile type; in this case, the Example Pylon tile.
			Item.DefaultToPlaceableTile(ModContent.TileType<Tile_RiftPylon>());

			// Another shorthand method that will set the rarity and how much the item is worth.
			Item.SetShopValues(ItemRarityColor.Blue1, Terraria.Item.buyPrice(gold: 10));
		}
	}
}