using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles.RoseGarden
{
    public class Item_TallBookcase : ModItem
    {
        public int[] Variants = new int[3]
        {
            ModContent.TileType<Tile_TallBookcase1>(),
            ModContent.TileType<Tile_TallBookcase2>(),
            ModContent.TileType<Tile_TallBookcase3>(),
        };
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 13;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tile_TallBookcase1>());
            Item.width = 16;
            Item.height = 16;
            Item.value = 3000;
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