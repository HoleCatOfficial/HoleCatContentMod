using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.BorderBiome.Walls
{
    public class Wall_CarbonaceousShaleWall : ModWall
    {
        public override void SetStaticDefaults()
        {

            DustType = ModContent.DustType<CarbonaceousShaleDust>();
            VanillaFallbackOnModDeletion = WallID.Stone;

            AddMapEntry(new Color(16, 20, 19));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
