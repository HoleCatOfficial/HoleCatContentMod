using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles.Altar
{
    public class Item_BlessingAltar : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 100;

        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tile_BlessingAltar>());
            Item.width = 24;
            Item.height = 10;
            Item.value = 1;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.White;
        }
    }
}