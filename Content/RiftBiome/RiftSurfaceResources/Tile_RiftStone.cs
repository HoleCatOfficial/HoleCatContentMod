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
	public class Tile_RiftStone : ModTile
	{
		bool Alt = false;
		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			TileID.Sets.ChecksForMerge[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileBlendAll[Type] = true;

			DustType = DustID.Wraith;

			TileObjectData.addTile(Type);

			AddMapEntry(new Color(0, 0, 0));
			
		}

		public override void NumDust(int i, int j, bool fail, ref int num) {
			num = fail ? 1 : 3;
		}

		public override void ChangeWaterfallStyle(ref int style) {
			style = ModContent.GetInstance<RiftWaterfallStyle>().Slot;
		}


        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
			Tile t = Main.tile[i, j];
			Alt = Main.rand.NextBool(6);
			if (t.TileType != Type)
			{
				return true;
			}

			if (Alt)
			{
				if (t.TileFrameY < 270)
				{
					t.TileFrameY += 270;
				}
			}
			else
			{
				if (t.TileFrameY > 0)
				{
					t.TileFrameY -= 270;
				}
			}
            return false;
        }
	}
}