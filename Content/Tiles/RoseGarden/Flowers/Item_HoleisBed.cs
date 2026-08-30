using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles.RoseGarden.Flowers
{
    public class Item_HoleisBed : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 0;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 13;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tile_HoleisBed>());
            Item.width = 26;
            Item.height = 32;
            Item.value = 3000;
        }
    }
}