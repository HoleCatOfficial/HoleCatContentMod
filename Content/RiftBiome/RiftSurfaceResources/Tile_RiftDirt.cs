using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.RiftBiome.RiftDesertResources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using GlowmaskHelper.Content;
using System.Linq;
using DestroyerTest.Content.Particles;
using DestroyerTest.Common;
using BreadLibrary.Core.Graphics.Particles;
using Terraria.WorldBuilding;
using Terraria.GameContent.Drawing;

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
			Main.tileLighted[Type] = true;

			Main.tileBlockLight[Type] = true;

			DustType = DustID.Wraith;

			AddMapEntry(new Color(15, 15, 15));
		}

		public override void NumDust(int i, int j, bool fail, ref int num) {
			num = fail ? 1 : 3;
		}

		bool Inner = false;
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile t = Framing.GetTileSafely(i, j);

            Tile Left = Main.tile[i - 1, j];
            Tile Top = Main.tile[i, j - 1];
            Tile Right = Main.tile[i + 1, j];
            Tile Bottom = Main.tile[i, j + 1];

            if ((Left.HasTile && Top.HasTile && Right.HasTile && Bottom.HasTile))
			{
				Inner = true;
			}

            return base.TileFrame(i, j, ref resetFrame, ref noBreak);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
			Tile t = Framing.GetTileSafely(i, j);

            Tile Left = Main.tile[i - 1, j];
            Tile Top = Main.tile[i, j - 1];
            Tile Right = Main.tile[i + 1, j];
            Tile Bottom = Main.tile[i, j + 1];

            if ((Left.HasTile && Top.HasTile && Right.HasTile && Bottom.HasTile))
            {
                Inner = true;
            }
			else
			{
				Inner = false;
			}

		

            if (!Inner)
			{
				if (Main.rand.NextBool(20) && !Main.gameInactive && !Main.gamePaused)
				{
					PixelParticle pixel = new();
					pixel.Initialize(new Point(i, j).ToWorldCoordinates(), Main.rand.NextVector2Circular(0.5f, 0.5f), ColorLib.Rift, 2f);
					ParticleEngine.Particles.Add(pixel);
				}
			}
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile t = Framing.GetTileSafely(i, j);


            Tile Left = Main.tile[i - 1, j];
            Tile Top = Main.tile[i, j - 1];
            Tile Right = Main.tile[i + 1, j];
            Tile Bottom = Main.tile[i, j + 1];

            if ((Left.HasTile && Top.HasTile && Right.HasTile && Bottom.HasTile))
            {
                Inner = true;
            }
            else
            {
                Inner = false;
            }


            if (Inner)
			{
				r = g = b = 0;
			}
			else
			{
				r = 2.55f * 0.5f;
				g = 1.55f * 0.5f;
				b = 0 * 0.4f;
			}
			
        }
 
		public override void ChangeWaterfallStyle(ref int style) 
		{
			style = ModContent.GetInstance<RiftWaterfallStyle>().Slot;
		}
	}
}