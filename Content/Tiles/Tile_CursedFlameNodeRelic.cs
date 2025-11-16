using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace DestroyerTest.Content.Tiles
{
	public class Tile_CursedFlameNodeRelic : ModTile
	{
		public const int FrameWidth = 18 * 2;
		public const int FrameHeight = 18 * 4;
		public const int HorizontalFrames = 1;
		public const int VerticalFrames = 1;

		public Asset<Texture2D> RelicTexture;

		// Every relic has its own extra floating part, should be 50x50. Optional: Expand this sheet if you want to add more, stacked vertically
		// If you do not use the Item.placeStyle approach, and you extend from this class, you can override this to point to a different texture
		public virtual string RelicTextureName => "DestroyerTest/Content/Tiles/Tile_CursedFlameNodeRelic";

		// All relics use the same pedestal texture, this one is copied from vanilla
		public override string Texture => "DestroyerTest/Content/Tiles/CursedFlameNodeRelicPedestal";

		public override void Load() {
			// Cache the extra texture displayed on the pedestal
			RelicTexture = ModContent.Request<Texture2D>(RelicTextureName);
		}

		public override void SetStaticDefaults() {
			Main.tileShine[Type] = 400; // Responsible for golden particles
			Main.tileFrameImportant[Type] = true; // Any multitile requires this
			TileID.Sets.InteractibleByNPCs[Type] = true; // Town NPCs will palm their hand at this tile

			TileObjectData.newTile = new TileObjectData(copyFrom:TileObjectData.Style1x1); // Create a new instance
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, TileObjectData.newTile.Height);
			TileObjectData.newTile.LavaDeath = false; // Does not break when lava touches it
			TileObjectData.newTile.DrawYOffset = 2; // So the tile sinks into the ground
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft; // Player faces to the left
			TileObjectData.newTile.StyleHorizontal = false; // Based on how the alternate sprites are positioned on the sprite (by default, true)

			// This controls how styles are laid out in the texture file. This tile is special in that all styles will use the same texture section to draw the pedestal.
			TileObjectData.newTile.StyleWrapLimitVisualOverride = 2;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.styleLineSkipVisualOverride = 0; // This forces the tile preview to draw as if drawing the 1st style.

			// Register an alternate tile data with flipped direction
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile); // Copy everything from above, saves us some code
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight; // Player faces to the right
			TileObjectData.addAlternate(1);

			// Register the tile data itself
			TileObjectData.addTile(Type);
            
            DustType = DustID.Silver;

			// Register map name and color
			// "MapObject.Relic" refers to the translation key for the vanilla "Relic" text
			AddMapEntry(new Color(209, 216, 217), Language.GetText("MapObject.Relic"));
		}

		 public override void SetDrawPositions(
            int i, int j, ref int width, ref int offsetY, ref int height,
            ref short tileFrameX, ref short tileFrameY)
        {
            tileFrameX %= FrameWidth;
            tileFrameY %= FrameHeight * 2; // 2 directions
        }

        public override void DrawEffects(
            int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX % FrameWidth == 0 &&
                drawData.tileFrameY % FrameHeight == 0)
            {
                Main.instance.TilesRenderer.AddSpecialPoint(
                    i, j,
                    Terraria.GameContent.Drawing.TileDrawing.TileCounterType.CustomNonSolid);
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch) {
            Point p = new(i, j);
            Tile tile = Main.tile[p.X, p.Y];
            if (!tile.HasTile)
                return;

            Texture2D texture = RelicTexture.Value;

            int frameY = tile.TileFrameX / FrameWidth;
            Rectangle frame = texture.Frame(HorizontalFrames, VerticalFrames, 0, frameY);

            Vector2 origin = frame.Size() / 2f;
            origin.X = FrameWidth / 2f; // FrameWidth = 32 for your new width

            Vector2 worldPos = p.ToWorldCoordinates(18, 64f);

            Color color = Lighting.GetColor(i, j);

            bool flip = tile.TileFrameY / FrameHeight != 0;
            SpriteEffects effects = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            const float TwoPi = (float)Math.PI * 2f;
            float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);

            Vector2 drawPos =
                worldPos - Main.screenPosition +
                new Vector2(0f, -40f) +
                new Vector2(0f, offset * 4f);

            spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);

            float scale =
                (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 2f) * 0.3f + 0.7f;

            Color effectColor = color * 0.1f * scale;
            effectColor.A = 0;

            for (float k = 0f; k < 1f; k += 355f / (678f * (float)Math.PI)) {
                spriteBatch.Draw(
                    texture,
                    drawPos + (TwoPi * k).ToRotationVector2() * (6f + offset * 2f),
                    frame,
                    effectColor,
                    0f,
                    origin,
                    1f,
                    effects,
                    0f);
            }
        }

        public override void PostDrawPlacementPreview(
        int i, int j, SpriteBatch spriteBatch, Rectangle frame,
        Vector2 position, Color color, bool validPlacement, SpriteEffects spriteEffects)
        {
            // Determine sprite flipping based on the tile’s frame.Y
            bool facingRight = frame.Y / FrameHeight != 0;
            spriteEffects = facingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Strip out multi-row junk
            frame.Y %= FrameHeight;

            // Convert tile preview frame (18px grid) to relic frame (16px grid)
            int tileX = frame.X / 18;        // now 0 or 1 for 2 tiles
            int tileY = frame.Y / 18;        // usually 0 unless animated

            frame.X = tileX * 16;            // map to 0 or 16
            frame.Y = tileY * 16;

            // No more manual frame swapping when flipped.
            // SpriteEffects handles it cleanly now that width = 2 tiles.

            spriteBatch.Draw(
                RelicTexture.Value,
                position,
                frame,
                color,
                0f,
                Vector2.Zero,
                1f,
                spriteEffects,
                0f
            );
        }

	}

	// If you want to make more relics but do not use the Item.placeStyle approach, you can use inheritance to avoid using duplicate code:
	// Your tile code would then inherit from the MinionBossRelic class (which you should make abstract) and should look like this:
	/*
	public class MyBossRelic : MinionBossRelic
	{
		public override string RelicTextureName => "ExampleMod/Content/Tiles/Furniture/MyBossRelic";

		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
		}
	}
	*/

	// Your item code would then just use the MyBossRelic tile type, and keep placeStyle on 0
	// The textures for MyBossRelic item/tile have to be supplied separately
}