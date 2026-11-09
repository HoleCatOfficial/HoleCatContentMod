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
    public class Wall_HallowCarbonaceousShaleWall : ModWall
    {
        public override void SetStaticDefaults()
        {

            DustType = ModContent.DustType<HallowCarbonaceousShaleDust>();
            VanillaFallbackOnModDeletion = WallID.Stone;

            AddMapEntry(new Color(32, 27, 32));

            //WallID.Sets.SpreadsHallow[Type] = true;
            WallID.Sets.Hallow[Type] = true;
            WallLoader.RegisterSimpleConversion(ModContent.WallType<Wall_CarbonaceousShaleWall>(), BiomeConversionID.Hallow, Type);
            WallLoader.RegisterConversion(Type, BiomeConversionID.Purity, ModContent.WallType<Wall_CarbonaceousShaleWall>());
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
