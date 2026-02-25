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
		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			TileID.Sets.ChecksForMerge[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileBlendAll[Type] = true;

			DustType = DustID.Wraith;

			AddMapEntry(new Color(0, 0, 0));
			
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