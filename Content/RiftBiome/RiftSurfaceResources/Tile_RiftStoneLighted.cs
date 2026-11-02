using DestroyerTest.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RiftBiome.RiftSurfaceResources
{
    public class Tile_RiftStoneLighted : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileLighted[Type] = true;

            DustType = DustID.Wraith;

            AddMapEntry(ColorLib.Rift);

        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = ColorLib.DarkRift2.R;
            g = ColorLib.DarkRift2.G;
            b = ColorLib.DarkRift2.B;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ChangeWaterfallStyle(ref int style)
        {
            style = ModContent.GetInstance<RiftWaterfallStyle>().Slot;
        }
    }
}