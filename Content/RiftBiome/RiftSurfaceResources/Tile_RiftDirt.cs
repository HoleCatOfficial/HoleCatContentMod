using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.RiftBiome.RiftDesertResources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using GlowmaskHelper.Content;

namespace DestroyerTest.Content.RiftBiome.RiftSurfaceResources
{
	[AutoloadGlowmask]
	public class Tile_RiftDirt : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			TileID.Sets.ChecksForMerge[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBlendAll[Type] = true;
			

			Main.tileBlockLight[Type] = true;

			DustType = DustID.Wraith;

			AddMapEntry(new Color(15, 15, 15));
		}

		public override void NumDust(int i, int j, bool fail, ref int num) {
			num = fail ? 1 : 3;
		}

 
		public override void ChangeWaterfallStyle(ref int style) {
			style = ModContent.GetInstance<RiftWaterfallStyle>().Slot;
		}
	}
}