using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RiftBiome.RiftSurfaceResources
{
	public class Item_RiftSurfaceMusicBox : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.CanGetPrefixes[Type] = false;
			ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Assets/Music/RiftV2"), ModContent.ItemType<Item_RiftSurfaceMusicBox>(), ModContent.TileType<Tile_RiftSurfaceMusicBox>());
		}

		public override void SetDefaults() {
			Item.DefaultToMusicBox(ModContent.TileType<Tile_RiftSurfaceMusicBox>(), 0);
		}

        public override void AddRecipes()
        {
            CreateRecipe()
			.AddIngredient(ItemID.MusicBox)
			.AddIngredient<Item_RiftClay>(10)
			.AddTile<Tile_RiftConfiguratorCore>()
			.Register();
        }
    }
}