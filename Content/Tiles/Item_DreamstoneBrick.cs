using DestroyerTest.Content.Tiles.RiftConfigurator;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles
{
    public class Item_DreamstoneBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 40;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 15;

        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tile_DreamstoneBrick>());
            Item.width = 12;
            Item.height = 12;
            Item.value = 3000;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Item_Dreamstone>()
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}