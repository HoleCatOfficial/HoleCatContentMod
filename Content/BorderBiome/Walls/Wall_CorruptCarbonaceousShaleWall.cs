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
    public class Wall_CorruptCarbonaceousShaleWall : ModWall
    {
        public override void SetStaticDefaults()
        {

            DustType = ModContent.DustType<CorruptCarbonaceousShaleDust>();
            VanillaFallbackOnModDeletion = WallID.Stone;

            AddMapEntry(new Color(16, 16, 20));

            //WallID.Sets.SpreadsCorruption[Type] = true;
            WallID.Sets.Corrupt[Type] = true;
            WallLoader.RegisterSimpleConversion(ModContent.WallType<Wall_CarbonaceousShaleWall>(), BiomeConversionID.Corruption, Type);
            WallLoader.RegisterConversion(Type, BiomeConversionID.Purity, ModContent.WallType<Wall_CarbonaceousShaleWall>());
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
