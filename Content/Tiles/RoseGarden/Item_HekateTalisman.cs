using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles.RoseGarden
{
    public class Item_HekateTalisman : ModItem
    {
        public int[] Variants = new int[3]
        {
            ModContent.TileType<Tile_HekateTalisman1>(),
            ModContent.TileType<Tile_HekateTalisman2>(),
            ModContent.TileType<Tile_HekateTalisman3>(),
        };
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(Variants[Main.rand.Next(Variants.Length)]);
            Item.consumable = false;
            Item.maxStack = 1;
            Item.width = 12;
            Item.height = 40;
            Item.value = 100;
        }
        public override void UpdateInventory(Player player)
        {
            if (Main.GameUpdateCount % 60 == 0)
            {
                Item.createTile = Variants[Main.rand.Next(Variants.Length)];
            }
        }
    }
}
