using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.BorderBiome.Tiles
{
    public class Item_HallowCarbonaceousShale : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 5;

        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tile_HallowCarbonaceousShale>());
            Item.width = 12;
            Item.height = 12;
            Item.value = 3000;
        }
    }
}