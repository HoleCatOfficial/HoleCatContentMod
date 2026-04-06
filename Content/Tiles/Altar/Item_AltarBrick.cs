using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles.Altar
{
    public class Item_AltarBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 100;

        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tile_AltarBrick>());
            Item.width = 16;
            Item.height = 16;
            Item.value = 1;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.White;
        }
    }
}