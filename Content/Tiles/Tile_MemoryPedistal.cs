using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using SDL2;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Utilities;

namespace DestroyerTest.Content.Tiles
{
    public class Tile_MemoryPedistal : ModTile
    {
        public override string Texture => "DestroyerTest/Content/Tiles/MemoryPedistal";

        public enum Variants
        {
            NightmareRose,
            WyvernCorpse
        }

        public Variants variant;


        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(Type);

            

            AddMapEntry(new Color(239, 216, 112), Language.GetText("Mods.DestroyerTest.Tiles.MemoryPedistal"));
        }

        public override void MouseOver(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;

            switch (tile.TileFrameY)
            {
                case 0:
                    {
                        player.cursorItemIconID = ModContent.ItemType<Item_NightmareRoseMemoryPedistal>();
                        break;
                    }
                case 36:
                    {
                        player.cursorItemIconID = ModContent.ItemType<Item_WyvernCorpseMemoryPedistal>();
                        break;
                    }
            }
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true;
        }

        public Color GlowColor(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            switch (tile.TileFrameY)
            {
                case 0:
                    {
                        variant = Variants.NightmareRose;
                        return Color.Red;
                    }
                case 36:
                    {
                        variant = Variants.WyvernCorpse;
                        return ColorLib.Soul2;
                    }
            }
            return Color.White;
        }

        float r = 0f;
        Point p;
        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            p = new Point(i, j);
           

            
            r += 0.003f;

            float scl = Opus.Sine(1.3f, 0.5f);

            Color color = GlowColor(p.X, p.Y);

            if (!TileDrawing.IsVisible(tile) || tile.TileFrameX != 36 || tile.TileFrameY % 36 != 0)
            {
                return;
            }





            Main.EntitySpriteDraw(DTAssetLib.Swirl.Value, (new Point(p.X, p.Y - 1).ToWorldCoordinates() + new Vector2(8f, 8f)) - Main.screenPosition, null, color with { A = 0 } * 0.15f, r * -1, DTAssetLib.Swirl.Value.Size() / 2, scl * 0.4f, SpriteEffects.FlipHorizontally);
            Main.EntitySpriteDraw(DTAssetLib.Swirl.Value, (new Point(p.X, p.Y - 1).ToWorldCoordinates() + new Vector2(8f, 8f)) - Main.screenPosition, null, color with { A = 0 } * 0.5f, r * -2, DTAssetLib.Swirl.Value.Size() / 2, scl * 0.2f, SpriteEffects.FlipHorizontally);


            Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, (new Point(p.X, p.Y - 1).ToWorldCoordinates() + new Vector2(8f, 8f)) - Main.screenPosition, null, color with { A = 0 }, 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, scl, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.Sparkle(5).Value, (new Point(p.X, p.Y - 1).ToWorldCoordinates() + new Vector2(8f, 8f)) - Main.screenPosition, null, Color.White with { A = 0 }, r, DTAssetLib.Sparkle(5).Value.Size() / 2, scl * 0.25f, SpriteEffects.None);

        }



        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomNonSolid);
            Main.instance.TilesRenderer.AddSpecialPoint(i, j, Terraria.GameContent.Drawing.TileDrawing.TileCounterType.CustomNonSolid);


            // This code spawns the music notes when the music box is open.
            if (Lighting.UpdateEveryFrame && new FastRandom(Main.TileFrameSeed).WithModifier(i, j).Next(4) != 0)
            {
                return;
            }

            Tile tile = Main.tile[i, j];

 

            if (!TileDrawing.IsVisible(tile) || tile.TileFrameX != 36 || tile.TileFrameY % 36 != 0 || (int)Main.timeForVisualEffects % 7 != 0 || !Main.rand.NextBool(3))
            {
                return;
            }

            

            int MusicNote = Main.rand.Next(570, 573);
            Vector2 SpawnPosition = new Vector2(i * 16 + 8, j * 16 - 8);
            Vector2 NoteMovement = new Vector2(Main.WindForVisuals * 2f, -0.5f);
            NoteMovement.X *= Main.rand.NextFloat(0.5f, 1.5f);
            NoteMovement.Y *= Main.rand.NextFloat(0.5f, 1.5f);
            switch (MusicNote)
            {
                case 572:
                    SpawnPosition.X -= 8f;
                    break;
                case 571:
                    SpawnPosition.X -= 4f;
                    break;
            }

            Gore.NewGore(new EntitySource_TileUpdate(i, j), SpawnPosition, NoteMovement, MusicNote, 0.8f);

            
        }

        
    }
}