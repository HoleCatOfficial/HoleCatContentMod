using DestroyerTest.Content.RiftBiome;
using Microsoft.Xna.Framework;
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
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);

			DustType = DustID.Wraith;

			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LinkedAlternates = true;
			TileObjectData.addSubTile(1);

			TileObjectData.addTile(Type);

			AddMapEntry(new Color(0, 0, 0));
		}

		public override void NumDust(int i, int j, bool fail, ref int num) {
			num = fail ? 1 : 3;
		}

		public override void ChangeWaterfallStyle(ref int style) {
			style = ModContent.GetInstance<RiftWaterfallStyle>().Slot;
		}
	}
}