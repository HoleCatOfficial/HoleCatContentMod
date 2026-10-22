using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Content.Tiles.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles.RoseGarden.Walls
{
    public class Item_SanctifiedBrickWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<Wall_SanctifiedBrickWall>());
        }
        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<Item_SanctifiedBrick>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
