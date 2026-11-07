using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Content.BorderBiome.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.BorderBiome
{
    public class MidnightMossWand : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GrassSeeds[Type] = true;
            ItemID.Sets.GrassSeedDirtTiles[Type] = [ModContent.TileType<Tile_CarbonaceousShale>()];

        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MidnightMoss_CarbonaceousShale>());
            Item.consumable = false;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.buyPrice(copper: 20);
        }
    }
}
