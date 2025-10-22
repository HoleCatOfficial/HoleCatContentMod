
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.ObjectData;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using Terraria.Audio;
using System;

namespace DestroyerTest.Content.Tiles
{
    /// <summary>
    /// An example for creating a Pylon, identical to how they function in Vanilla. Shows off <seealso cref="ModPylon"/>, an abstract
    /// extension of <seealso cref="ModTile"/> that has additional functionality for Pylon specific tiles.
    /// <br>
    /// If you are going to make multiple pylons that all act the same (like in Vanilla), it is recommended you make a base class
    /// with override functionality in order to prevent writing boilerplate. (For example, making a "CrystalTexture" property that you can
    /// override in order to streamline that process.)
    /// </br>
    /// </summary>
    public class Tile_RiftPylon : ModPylon
    {
        public const int CrystalVerticalFrameCount = 13;

        public Asset<Texture2D> crystalTexture;
        public Asset<Texture2D> crystalHighlightTexture;
        public Asset<Texture2D> mapIcon;

        public override void Load()
        {
            // We'll need these textures for later, it's best practice to cache them on load instead of continually requesting every draw call.
            crystalTexture = ModContent.Request<Texture2D>(Texture + "_Crystal");
            crystalHighlightTexture = ModContent.Request<Texture2D>(Texture + "_Crystal_Highlight");
            mapIcon = ModContent.Request<Texture2D>(Texture + "_MapIcon");
        }

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;

            VanillaFallbackOnModDeletion = TileID.TeleportationPylon;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            // These definitions allow for vanilla's pylon TileEntities to be placed.
            // tModLoader has a built in Tile Entity specifically for modded pylons, which we must extend (see SimplePylonTileEntity)
            TEModdedPylon moddedPylon = ModContent.GetInstance<RiftPylonTileEntity>();
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(moddedPylon.PlacementPreviewHook_CheckIfCanPlace, 1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(moddedPylon.Hook_AfterPlacement, -1, 0, false);
            

            TileObjectData.addTile(Type);

            TileID.Sets.InteractibleByNPCs[Type] = true;
            TileID.Sets.PreventsSandfall[Type] = true;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;

            // Adds functionality for proximity of pylons; if this is true, then being near this tile will count as being near a pylon for the teleportation process.
            AddToArray(ref TileID.Sets.CountsAsPylon);

            LocalizedText pylonName = CreateMapEntryName(); // Name is in the localization file
            AddMapEntry(ColorLib.Rift, pylonName);

            DustType = ModContent.DustType<RiftDust>();
			HitSound = new SoundStyle("DestroyerTest/Assets/Audio/Scholar/ShieldHit", 3)
			{
				PitchVariance = 0.5f
			};
        }

        public override NPCShop.Entry GetNPCShopEntry()
        {
            // In this method we can customize the shop entry for the pylon item.
            // The default method, base.GetNPCShopEntry(), generates a shop entry for the pylon item with the typical pylon conditions: Condition.HappyEnoughToSellPylons, Condition.AnotherTownNPCNearby, and Condition.NotInEvilBiome
            NPCShop.Entry shopEntry = base.GetNPCShopEntry();

            // We will take that shop entry and add an additional condition to check for ExampleBiome, as this is typical for biome pylons
            // This does not affect the teleport conditions, only the sale conditions
            shopEntry.AddCondition(RiftSurface.InRift);

            // and finally we return the shop entry
            return shopEntry;
        }

        public override void MouseOver(int i, int j)
        {
            // Show a little pylon icon on the mouse indicating we are hovering over it.
            Main.LocalPlayer.cursorItemIconEnabled = true;
            Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<Item_RiftPylon>();
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            // We need to clean up after ourselves, since this is still a "unique" tile, separate from Vanilla Pylons, so we must kill the TileEntity.
            ModContent.GetInstance<RiftPylonTileEntity>().Kill(i, j);
        }

        public override bool ValidTeleportCheck_NPCCount(TeleportPylonInfo pylonInfo, int defaultNecessaryNPCCount)
        {
            // Let's say for fun sake that no NPCs need to be nearby in order for this pylon to function. If you want your pylon to function just like vanilla,
            // you don't need to override this method at all.
            return true;
        }

        public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData)
        {
            // Right before this hook is called, the sceneData parameter exports its information based on wherever the destination pylon is,
            // and by extension, it will call ALL ModSystems that use the TileCountsAvailable method. This means, that if you determine biomes
            // based off of tile count, when this hook is called, you can simply check the tile threshold, like we do here. In the context of ExampleMod,
            // something is considered within the Example Surface/Underground biome if there are 40 or more example blocks at that location.

            bool b1 = ModContent.GetInstance<RiftSurfaceTileCount>().RiftSurfaceBlockCount >= 40;
            bool b2 = ModContent.GetInstance<RiftDesertTileCount>().RiftDesertBlockCount >= 40;
            bool b3 = ModContent.GetInstance<RiftTundraTileCount>().RiftTundraBlockCount >= 40;

            return b1 || b2 || b3;
        }

        public int customframecounter = 0;

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            customframecounter++;
            if (customframecounter > 13 * 13)
            {
                customframecounter = 0;
            }
            r = ColorLib.Rift.R * 0.075f;
            g = ColorLib.Rift.G * 0.075f;
            b = ColorLib.Rift.B * 0.075f;
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            CustomDrawPylonCrystal(spriteBatch, i, j, crystalTexture, crystalHighlightTexture, new Vector2(0f, -12f), Color.White * 0.1f, Color.White, 1, 13);
        }
        
        public void CustomDrawPylonCrystal(SpriteBatch spriteBatch, int i, int j, Asset<Texture2D> crystalTexture, Asset<Texture2D> crystalHighlightTexture, Vector2 crystalOffset, Color pylonShadowColor, Color dustColor, int dustChanceDenominator, int crystalVerticalFrameCount)
        {
            // Gets offscreen vector for different lighting modes
            Vector2 offscreenVector = new Vector2(Main.offScreenRange);
            if (Main.drawToScreen) {
                offscreenVector = Vector2.Zero;
            }

            // Double check that the tile exists
            Point point = new Point(i, j);
            Tile tile = Main.tile[point.X, point.Y];
            if (tile == null || !tile.HasTile) {
                return;
            }

            TileObjectData tileData = TileObjectData.GetTileData(tile);

            // Calculate frame based on vanilla counters in order to line up the animation

            

            

            int frameY = customframecounter / crystalVerticalFrameCount;

            

            // Frame our modded crystal sheet accordingly for proper drawing
            Rectangle crystalFrame = crystalTexture.Frame(1, crystalVerticalFrameCount, 0, frameY);
            Rectangle smartCursorGlowFrame = crystalHighlightTexture.Frame(1, crystalVerticalFrameCount, 0, frameY);

            // I have no idea what is happening here; but it fixes the frame bleed issue. All I know is that the vertical sinusoidal motion has something to with it.
            // If anyone else has a clue as to why, please do tell. - MutantWafflez
            crystalFrame.Height -= 1;
            smartCursorGlowFrame.Height -= 1;

            // Calculate positional variables for actually drawing the crystal
            Vector2 origin = crystalFrame.Size() / 2f;
            Vector2 tileOrigin = new Vector2(tileData.CoordinateFullWidth / 2f, tileData.CoordinateFullHeight / 2f);
            Vector2 crystalPosition = point.ToWorldCoordinates(tileOrigin.X - 2f, tileOrigin.Y) + crystalOffset;

            // Calculate additional drawing positions with a sine wave movement
            float sinusoidalOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * (Math.PI * 2) / 5);
            Vector2 drawingPosition = crystalPosition + offscreenVector + new Vector2(0f, sinusoidalOffset * 4f);

            // Do dust drawing
            if (!Main.gamePaused && Main.instance.IsActive && (!Lighting.UpdateEveryFrame || Main.rand.NextBool(4)) && Main.rand.NextBool(dustChanceDenominator)) {
                Rectangle dustBox = Utils.CenteredRectangle(crystalPosition, crystalFrame.Size());
                int numForDust = Dust.NewDust(dustBox.TopLeft(), dustBox.Width, dustBox.Height, DustID.TintableDustLighted, 0f, 0f, 254, dustColor, 0.5f);
                Dust obj = Main.dust[numForDust];
                obj.velocity *= 0.1f;
                Main.dust[numForDust].velocity.Y -= 0.2f;
            }

            // Get color value and draw the crystal
            Color color = Lighting.GetColor(point.X, point.Y);
            color = Color.Lerp(color, ColorLib.Rift, 0.8f);
            spriteBatch.Draw(crystalTexture.Value, drawingPosition - Main.screenPosition, crystalFrame, color * 0.7f, 0f, origin, 1f, SpriteEffects.None, 0f);

            // Draw the shadow effect for the crystal
            float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * ((float)Math.PI * 2f) / 1f) * 0.2f + 0.8f;
            Color shadowColor = pylonShadowColor * scale;
            for (float shadowPos = 0f; shadowPos < 1f; shadowPos += 1f / 6f) {
                spriteBatch.Draw(crystalTexture.Value, drawingPosition - Main.screenPosition + ((float)Math.PI * 2f * shadowPos).ToRotationVector2() * (6f + sinusoidalOffset * 2f), crystalFrame, shadowColor, 0f, origin, 1f, SpriteEffects.None, 0f);
            }

            // Interpret smart cursor outline color & draw it
            int selectionLevel = 0;
            if (Main.InSmartCursorHighlightArea(point.X, point.Y, out bool actuallySelected)) {
                selectionLevel = 1;
                if (actuallySelected) {
                    selectionLevel = 2;
                }
            }

            if (selectionLevel == 0) {
                return;
            }

            int averageBrightness = (color.R + color.G + color.B) / 3;

            if (averageBrightness <= 10) {
                return;
            }

            Color selectionGlowColor = Colors.GetSelectionGlowColor(selectionLevel == 2, averageBrightness);
            spriteBatch.Draw(crystalHighlightTexture.Value, drawingPosition - Main.screenPosition, smartCursorGlowFrame, selectionGlowColor, 0f, origin, 1f, SpriteEffects.None, 0f);
        }


        public override void DrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, TeleportPylonInfo pylonInfo, bool isNearPylon, Color drawColor, float deselectedScale, float selectedScale)
        {
            // Just like in SpecialDraw, we want things to be handled the EXACT same way vanilla would handle it, which ModPylon also has built in methods for:
            bool mouseOver = DefaultDrawMapIcon(ref context, mapIcon, pylonInfo.PositionInTiles.ToVector2() + new Vector2(1.5f, 2f), drawColor, deselectedScale, selectedScale);
            DefaultMapClickHandle(mouseOver, pylonInfo, ModContent.GetInstance<Item_RiftPylon>().DisplayName.Key, ref mouseOverText);
        }
    }
    
    /// <summary>
	/// This is an empty child class that acts exactly like the default implementation of the abstract <seealso cref="TEModdedPylon"/>
	/// class, which itself acts nearly identical to vanilla pylon TEs. This inheritance only exists so that modded pylon entities
	/// will properly have their "Mod" property set, for I/O purposes. Has the sealed modifier since this TE acts identical to its parent.
	/// </summary>
	public sealed class RiftPylonTileEntity : TEModdedPylon { }
}