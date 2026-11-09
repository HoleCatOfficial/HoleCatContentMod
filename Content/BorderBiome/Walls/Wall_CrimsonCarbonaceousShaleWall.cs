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
    public class Wall_CrimsonCarbonaceousShaleWall : ModWall
    {
        public override void SetStaticDefaults()
        {

            DustType = ModContent.DustType<CrimsonCarbonaceousShaleDust>();
            VanillaFallbackOnModDeletion = WallID.Stone;

            AddMapEntry(new Color(20, 16, 16));

            //WallID.Sets.SpreadsCrimson[Type] = true;
            WallID.Sets.Crimson[Type] = true;
            WallLoader.RegisterSimpleConversion(ModContent.WallType<Wall_CarbonaceousShaleWall>(), BiomeConversionID.Crimson, Type);
            WallLoader.RegisterConversion(Type, BiomeConversionID.Purity, ModContent.WallType<Wall_CarbonaceousShaleWall>());
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
