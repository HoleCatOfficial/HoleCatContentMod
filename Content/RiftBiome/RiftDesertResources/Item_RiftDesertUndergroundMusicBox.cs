using DestroyerTest.Content.Tiles.RiftConfigurator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RiftBiome.RiftDesertResources
{
    public class Item_RiftDesertUndergroundMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Assets/Music/RiftDesertUnderground"), ModContent.ItemType<Item_RiftDesertUndergroundMusicBox>(), ModContent.TileType<Tile_RiftDesertUndergroundMusicBox>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<Tile_RiftDesertUndergroundMusicBox>(), 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MusicBox)
                .AddIngredient<Item_HardenedRiftSilt>(10)
                .AddTile<Tile_RiftConfiguratorCore>()
                .Register();
        }
    }
}
