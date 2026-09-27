using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles
{
	public class Tile_ShadeParticleBlock : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = false;
            Main.tileLighted[Type] = true;
            
            HitSound = SoundID.Item50;
			DustType = DustID.BubbleBurst_White;
            MineResist = 3.6f;
            

			AddMapEntry(ColorLib.TenebrisGradient);
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
                    pixel.Initialize(new Point(i, j).ToWorldCoordinates(), Main.rand.NextVector2Circular(0.5f, 0.5f), ColorLib.TenebrisGradient, 2f);
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
                r = (ColorLib.TenebrisGradient.R * 0.1f) * 0.1f;
                g = (ColorLib.TenebrisGradient.G * 0.1f) * 0.1f;
                b = (ColorLib.TenebrisGradient.B * 0.1f) * 0.1f;
            }

        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            drawData.colorTint = ColorLib.TenebrisGradient;
            drawData.finalColor = ColorLib.TenebrisGradient;
        }
	}
}