using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Content.BorderBiome.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.BorderBiome
{
    public class MidnightMossWand : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GrassSeeds[Type] = true;
            ItemID.Sets.GrassSeedDirtTiles[Type] = [ModContent.TileType<Tile_CarbonaceousShale>(), ModContent.TileType<Tile_CorruptCarbonaceousShale>(), ModContent.TileType<Tile_CrimsonCarbonaceousShale>(), ModContent.TileType<Tile_HallowCarbonaceousShale>()];

        }

        public override void SetDefaults()
        {
            Item.useStyle = 1;
            Item.useTurn = true;
            Item.useAnimation = 25;
            Item.useTime = 13;
            Item.autoReuse = true;
            Item.width = 24;
            Item.height = 28;
            Item.damage = 7;
            Item.createTile = ModContent.TileType<MidnightMoss_CarbonaceousShale>();
            Item.UseSound = SoundID.Item1;
            Item.knockBack = 3f;
            Item.rare = ItemRarityID.Orange;
            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(0, 0, 50));
            Item.maxStack = 1;
            Item.consumable = false;
            Item.value = Item.buyPrice(copper: 20);
        }
    }
}
