using DestroyerTest.Content.Tiles.RiftConfigurator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RiftBiome.RiftSurfaceResources
{
    public class Item_RiftUndergroundMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Assets/Music/RiftUnderground"), ModContent.ItemType<Item_RiftUndergroundMusicBox>(), ModContent.TileType<Tile_RiftUndergroundMusicBox>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<Tile_RiftUndergroundMusicBox>(), 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.MusicBox)
            .AddIngredient<Item_RiftDirt>(10)
            .AddTile<Tile_RiftConfigurator>()
            .Register();
        }
    }
}
