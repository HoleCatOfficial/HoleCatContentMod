using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.BorderBiome.Tiles
{
    // This is an example of a vine tile. Vine tiles are fairly straightforward, but vines randomly growing and properly converting to other tiles are a bit more complicated. The ExampleVineGlobalTile class below contains the vine growing and converting code and is necessary for a fully working vine.
    public class MidnightMossVine : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoFail[Type] = true;

            TileID.Sets.TileCutIgnore.Regrowth[Type] = true;
            TileID.Sets.IsVine[Type] = true;
            TileID.Sets.ReplaceTileBreakDown[Type] = true;
            TileID.Sets.VineThreads[Type] = true;

            AddMapEntry(new Color(172, 23, 232));

            DustType = ModContent.DustType<MidnightMossDust>();
            HitSound = SoundID.Grass;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Main.instance.TilesRenderer.CrawlToTopOfVineAndAddSpecialPoint(j, i);
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {

            r = 172 * 0.003f;
            g = 23 * 0.003f;
            b = 232 * 0.003f;

        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (!Main.gameInactive && !Main.gamePaused)
            {
                if (Main.rand.NextBool(40))
                {
                    PointGlowPreMultiplied glow = new();
                    glow.Initialize(new Point(i, j).ToWorldCoordinates(), Main.rand.NextVector2Circular(2f, 2f), new Color(172, 23, 232) * 0.05f, 5f);
                    ParticleEngine.Particles.Add(glow);
                }

                if (Main.rand.NextBool(90))
                {
                    PixelParticle pixel = new();
                    pixel.Initialize(new Point(i, j).ToWorldCoordinates(), Main.rand.NextVector2Circular(0.4f, 0.4f), new Color(172, 23, 232) * 0.5f, 2f);
                    ParticleEngine.Particles.Add(pixel);
                }
            }
        }


        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = -2;
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 0)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }
    }

    // This class handles spawning and growing ExampleVine (RandomUpdate). Vines can either grow from the tip of an existing vine or spawn from the tile it grows from.
    // Because this behavior needs to act on both ExampleVine and ExampleBlock tiles, we put this logic in a GlobalTile rather than in both ModTile classes.
    // This class also handle transforming vines to ExampleVine if the anchor tile changes (TileFrame).
    public class MidnightMossVineGlobalTile : GlobalTile
    {
        private int MidnightVine;
        private int MidnightMoss;

        public override void SetStaticDefaults()
        {
            MidnightVine = ModContent.TileType<MidnightMossVine>();
            MidnightMoss = ModContent.TileType<MidnightMoss>();
        }
        public override void RandomUpdate(int i, int j, int type)
        {
            Tile tile = Main.tile[i, j];
            if (!tile.HasUnactuatedTile)
            {
                return;
            }
            if ((tile.TileType == MidnightVine || tile.TileType == MidnightMoss) && WorldGen.GrowMoreVines(i, j))
            {
                int growChance = 7;

                int below = j + 1;
                Tile tileBelow = Main.tile[i, below];
                if (WorldGen.genRand.NextBool(growChance) && !tileBelow.HasTile && tileBelow.LiquidType != LiquidID.Lava)
                {
                    bool vineIsHangingOffValidTile = true;
                    /*
                    for (int above = j; above > j - 10; above--)
                    {
                        Tile tileAbove = Main.tile[i, above];
                        if (tileAbove.BottomSlope)
                        {
                            return;
                        }

                        if (tileAbove.HasTile && tileAbove.TileType == MidnightMoss && !tileAbove.BottomSlope)
                        {
                            vineIsHangingOffValidTile = true;
                            break;
                        }
                    }
                    */

                    if (vineIsHangingOffValidTile)
                    {
                        tileBelow.TileType = (ushort)MidnightVine;
                        tileBelow.HasTile = true;
                        tileBelow.CopyPaintAndCoating(tile);
                        WorldGen.SquareTileFrame(i, below);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendTileSquare(-1, i, below);
                        }
                    }
                }
            }
        }

        public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
        {
            if (!TileID.Sets.IsVine[type])
            {
                return true;
            }

            Tile tile = Main.tile[i, j];
            Tile tileAbove = Main.tile[i, j - 1];

            int aboveTileType = tileAbove.HasUnactuatedTile && !tileAbove.BottomSlope ? tileAbove.TileType : -1;

            if (type != aboveTileType)
            {
                if ((aboveTileType == MidnightMoss || aboveTileType == MidnightVine) && type != MidnightVine)
                {
                    tile.TileType = (ushort)MidnightVine;
                    WorldGen.SquareTileFrame(i, j);
                    return true;
                }
            }

            return true;
        }
    }

    //With growing or spreading tiles, it can be time consuming to wait for tiles to grow naturally to test their behavior. Debug code like this can help with testing, just be sure to remove it when publishing your mod.
	public class TestVinesSystem : ModSystem
	{
		public override void PostUpdateWorld() 
        {
            /*
			if (Main.keyState.IsKeyDown(Keys.B) && !Main.oldKeyState.IsKeyDown(Keys.B)) {
				// Spawn vines at the cursor location.
				new ActionVines(3, 8, ModContent.TileType<MidnightMossVine>()).Apply(new Point(Player.tileTargetX, Player.tileTargetY), Player.tileTargetX, Player.tileTargetY);

				WorldGen.RangeFrame(Player.tileTargetX - 1, Player.tileTargetY - 1, Player.tileTargetX + 1, Player.tileTargetY + 10);
			}
            */
		}
	}
}
