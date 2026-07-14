using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Content.RiftBiome.RiftDesertResources;

namespace DestroyerTest.Content.RiftBiome.RiftDesertResources
{
	public class Item_RiftDesertMusicBox : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.CanGetPrefixes[Type] = false;
			ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Assets/Music/RiftDesert"), ModContent.ItemType<Item_RiftDesertMusicBox>(), ModContent.TileType<Tile_RiftDesertMusicBox>());
		}

		public override void SetDefaults() {
			Item.DefaultToMusicBox(ModContent.TileType<Tile_RiftDesertMusicBox>(), 0);
		}

        public override void AddRecipes()
        {
            CreateRecipe()
				.AddIngredient(ItemID.MusicBox)
				.AddIngredient<Item_RiftSilt>(10)
				.AddTile<Tile_RiftConfigurator>()
				.Register();
        }
    }
}