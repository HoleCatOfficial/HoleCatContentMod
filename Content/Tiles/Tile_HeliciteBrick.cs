using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using DestroyerTest.Content.Dusts;

namespace DestroyerTest.Content.Tiles
{
	public class Tile_HeliciteBrick : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileLighted[Type] = true;
			TileID.Sets.ChecksForMerge[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileShine2[Type] = true; // Modifies the draw color slightly.
			Main.tileShine[Type] = 975; // How often tiny dust appear off this tile. Larger is less frequently
			DustType = ModContent.DustType<HeliciteCrystalDust>();
			HitSound = new SoundStyle("DestroyerTest/Assets/Audio/Scholar/ShieldHit", 3) with { PitchVariance = 0.5f };
			MineResist = 1.5f;
			MinPick = 200;

			AddMapEntry(new Color(255, 155, 0));
		}

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
			makeDust = true;
			dustType = ModContent.DustType<RiftDust>();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameX == 0) {
				// We can support different light colors for different styles here: switch (tile.frameY / 54)
				r = 255f / 255f;
				g = 155f / 255f;
				b = 0f / 255f;
			}
		}

		public override void NumDust(int i, int j, bool fail, ref int num) {
			num = fail ? 1 : 3;
		}

	}
}