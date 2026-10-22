using FullSerializer.Internal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace DestroyerTest.Content.Tiles.RoseGarden
{
    /// <summary>
    /// Chandeliers use TileID.Sets.MultiTileSway and TilesRenderer.AddSpecialPoint to draw the tile swaying in the wind and reacting to player interaction.
    /// In addition, this example shows using AdjustMultiTileVineParameters to customize the physics of the wind interaction and using GetTileFlameData to draw a flickering flame overlay. It also shows using Animation.NewTemporaryAnimation to easily implement a custom temporary per-tile animation.
    /// The examples are all clones of existing chandeliers to showcase how they are done.
    /// </summary>
    public class Tile_SanctifiedBrazier : ModTile
    {
        private Asset<Texture2D> glowTexture;

        public override void Load()
        {
            glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow");
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileLighted[Type] = true;
            // We don't set Main.tileFlame

            

            TileID.Sets.MultiTileSway[Type] = true;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 1);
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.LavaDeath = true;
            AnimationFrameHeight = TileObjectData.newTile.CoordinateFullHeight;

            // Rather than many different items, the single item placing this tile places a random style.
            TileObjectData.newTile.DrawYOffset = -2; // Draw this tile 2 pixels up to align visually with the bottom of the tile it is anchored to.
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(235, 166, 135), Language.GetText("MapObject.HangingBrazier"));

            // Since we are using RandomStyleRange without StyleMultiplier, we'll need to manually register the item drop for the tile styles other than style 0. Here we register the default drop for any style.
            RegisterItemDrop(ModContent.ItemType<Item_SanctifiedBrazier>());
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            // Due to MultiTileVine rendering the tile 2 pixels higher than expected for modded tiles using TileObjectData.DrawYOffset, we need to add 2 to fix the math for correct drawing
            offsetY += 2;
            return;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].TileFrameY / 54 != 0)
            {
                return;
            }


            r = 0.8f;
            g = 0.8f;
            b = 0.8f;

            
        }

        public override void EmitParticles(int i, int j, Tile tileCache, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
        {
            if (Main.rand.NextBool(3) && tileFrameY < 54 && visible)
            {
                // The following math makes dust only spawn at the tile coordinates of the flames:
                // ---
                // O-O
                // ---

                int tileColumn = tileFrameX / 18 % 3;
                if (tileFrameY / 18 % 3 == 1 && tileColumn != 1)
                {

                    Dust dust = Dust.NewDustDirect(new Vector2(i * 16, j * 16 + 2), 14, 6, DustID.WhiteTorch, 0f, 0f, 100);
                    if (Main.rand.NextBool(3))
                    {
                        dust.noGravity = true;
                    }

                    dust.velocity *= 0.3f;
                    dust.velocity.Y -= 1.5f;


                    Dust dust2 = Dust.NewDustDirect(new Vector2(i * 16, j * 16 + 2), 14, 6, DustID.Smoke, 0f, 0f, 100);

                    dust2.noGravity = true;


                    dust2.velocity *= 0.3f;
                    dust2.velocity.Y -= 1.5f;
                }
            }

            
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            if (TileObjectData.IsTopLeft(tile))
            {
                // Makes this tile sway in the wind and with player interaction when used with TileID.Sets.MultiTileSway
                Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.MultiTileVine);
            }

            // We must return false here to prevent the normal tile drawing code from drawing the default static tile. Without this a duplicate tile will be drawn.
            return false;
        }

        public override void AdjustMultiTileVineParameters(int i, int j, ref float? overrideWindCycle, ref float windPushPowerX, ref float windPushPowerY, ref bool dontRotateTopTiles, ref float totalWindMultiplier, ref Texture2D glowTexture, ref Color glowColor)
        {

            // Vanilla chandeliers all share these parameters.
            overrideWindCycle = 1f;
            windPushPowerY = 0;

            glowTexture = this.glowTexture.Value;
            glowColor = Color.White;

        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter % 4 == 0)
            {
                frame++;

                if (frame > 7)
                {
                    frame = 0;
                }
            }
        }

        /*
        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            Tile tile = Main.tile[i, j];

            int column = (tile.TileFrameX % 36) / 18;
            int row = (tile.TileFrameY % 54) / 18;

            int topX = i - column;
            int topY = j - row;

            if (Animation.GetTemporaryFrame(topX, topY, out int frameData))
            {
                frameYOffset = 54 * frameData;
            }
        }
        */
    }
}