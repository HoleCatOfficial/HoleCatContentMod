using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles.Walls
{
    public class Item_DreamstoneWallCraftable : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<Wall_DreamstoneWallCraftable>());
        }
        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<Item_Dreamstone>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
