using DestroyerTest.Common;
using DestroyerTest.Content.RiftBiome;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace DestroyerTest.Content.RiftBiome.RiftSurfaceResources
{
	public class Tile_RiftStoneLighted : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			TileID.Sets.ChecksForMerge[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileBlendAll[Type] = true;
            Main.tileLighted[Type] = true;

			DustType = DustID.Wraith;

			TileObjectData.addTile(Type);

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

        public override void RandomUpdate(int i, int j)
        {
            if (Main.rand.NextBool(24))
			{
				WorldGen.PlaceTile(i, j, ModContent.TileType<Tile_RiftStone>(), true, true);
			}
        }
	}
}