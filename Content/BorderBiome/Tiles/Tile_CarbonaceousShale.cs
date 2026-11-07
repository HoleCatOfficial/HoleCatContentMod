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
    public class Tile_CarbonaceousShale : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlockOverride[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

            AddMapEntry(new Color(42, 54, 53));

            DustType = ModContent.DustType<CarbonaceousShaleDust>();

            HitSound = new SoundStyle(DTAssetLib.AudioPath + "/Border/ShaleMine", 3) { PitchVariance = 0.5f, MaxInstances = 0};
            MineResist = 4f;
            MinPick = 12;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {


        }

    }
}