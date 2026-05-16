using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles.RoseGarden
{
    public class Item_IdriPainting : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 13;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tile_IdriPainting>());
            Item.width = 26;
            Item.height = 32;
            Item.value = 3000;
        }
    }
}