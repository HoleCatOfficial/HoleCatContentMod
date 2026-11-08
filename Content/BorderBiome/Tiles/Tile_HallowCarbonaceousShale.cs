using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.BorderBiome.Tiles
{
    public class Tile_HallowCarbonaceousShale : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlockOverride[Type] = true;

            TileID.Sets.Hallow[Type] = true;
            TileID.Sets.HallowBiome[Type] = 1;
            TileID.Sets.HallowBiomeSight[Type] = true;
            TileID.Sets.HallowCountCollection.Add(Type);

            Main.tileBlendAll[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

            AddMapEntry(new Color(78, 66, 79));

            DustType = ModContent.DustType<HallowCarbonaceousShaleDust>();

            HitSound = new SoundStyle(DTAssetLib.AudioPath + "/Border/ShaleMine", 3) { PitchVariance = 0.5f, MaxInstances = 0 };
            MineResist = 4f;
            MinPick = 12;

            //To
            TileLoader.RegisterSimpleConversion(ModContent.TileType<Tile_CarbonaceousShale>(), BiomeConversionID.Hallow, Type);
            //And fro
            TileLoader.RegisterConversion(Type, BiomeConversionID.Purity, ModContent.TileType<Tile_CarbonaceousShale>());
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {


        }

    }
}