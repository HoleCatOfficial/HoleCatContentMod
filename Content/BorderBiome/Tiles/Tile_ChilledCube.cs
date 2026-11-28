using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace DestroyerTest.Content.BorderBiome.Tiles
{
    public class Tile_ChilledCube : ModTile
    {
        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            DustType = ModContent.DustType<CarbonaceousShaleDust>();
            // Names
            AddMapEntry(new Color(42, 54, 53), Language.GetText("Mods.DestroyerTest.Tiles.ChilledCube"));

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.CoordinateHeights = [16, 16];
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.addTile(Type);

            
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        int Radius = 12;

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            Point playerCoords = player.MountedCenter.ToTileCoordinates();
            Vector2 playerPosition = playerCoords.ToWorldCoordinates();


            Point coords = new Point(i, j);
            Vector2 position = coords.ToWorldCoordinates();

            if (player.MountedCenter.Distance(position) < Radius * 10)
            {

                player.GetModPlayer<BorderHeatPlayer>().IsInCoolingState = true;
            }
            if (player.MountedCenter.Distance(position) > (Radius * 10) && player.MountedCenter.Distance(position) < (Radius * 10) + 100)
            {
                player.GetModPlayer<BorderHeatPlayer>().IsInCoolingState = false;
            }

        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomNonSolid);

        }


        public override void EmitParticles(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
        {
            Point coords = new Point(i, j);
            Vector2 position = coords.ToWorldCoordinates();

            if (!TileDrawing.IsVisible(tile) || tile.TileFrameY > 8)
            {
                return;
            }

            for (int id = 0; id < 3; id++)
            {
                PointGlowPreMultiplied prt = new();
                Vector2 outer = position + Main.rand.NextVector2CircularEdge(Radius * 10, Radius * 10);
                prt.Initialize(outer, outer.DirectionTo(position).RotatedBy(-0.6f) * 2f, Color.SkyBlue * 0.5f, 1f, 30, true);
                ParticleEngine.BehindProjectiles.Add(prt);
            }

            var Positions = Opus.GetEquidistantOrbitVectors(8, position, 0.008f, Radius * 10);
            for (int id = 0; id < Positions.Length; id++)
            {
                PointGlowPreMultiplied px = new();
                px.Initialize(Positions[id], Vector2.Zero, Color.SkyBlue, 0.8f, 60, true);
                ParticleEngine.BehindProjectiles.Add(px);
            }
        }

        float R = 0;
        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            R += 0.01f;
            Point coords = new Point(i, j);
            Vector2 position = new Vector2((i * 16) + 8, (j * 16));

            Tile tile = Main.tile[coords];
            if (!TileDrawing.IsVisible(tile) || tile.TileFrameY % 32 != 0)
            {
                return;
            }


            Main.EntitySpriteDraw(DTAssetLib.Swirl.Value, position - Main.screenPosition, null, Color.SkyBlue with { A = 0 } * 0.1f, R * 1.5f, DTAssetLib.Swirl.Value.Size() / 2, 1f, SpriteEffects.None, 0f);

            Main.EntitySpriteDraw(DTAssetLib.FaintGlow.Value, position - Main.screenPosition, null, Color.SkyBlue with { A = 0 }, 0f, DTAssetLib.FaintGlow.Value.Size() / 2, 0.6f, SpriteEffects.None, 0f);

            Main.EntitySpriteDraw(DTAssetLib.TinyBloom.Value, position - Main.screenPosition, null, Color.SkyBlue with { A = 0 }, 0f, DTAssetLib.TinyBloom.Value.Size() / 2, 0.8f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(DTAssetLib.TinyBloom.Value, position - Main.screenPosition, null, Color.White with { A = 0 }, 0f, DTAssetLib.TinyBloom.Value.Size() / 2, 0.6f, SpriteEffects.None, 0f);



            Main.EntitySpriteDraw(DTAssetLib.FireRing.Value, position - Main.screenPosition, null, Color.SkyBlue with { A = 0 } * 0.15f, R, DTAssetLib.FireRing.Value.Size() / 2, 0.15f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(DTAssetLib.FireRing.Value, position - Main.screenPosition, null, Color.SkyBlue with { A = 0 } * 0.15f, R * 1.1f, DTAssetLib.FireRing.Value.Size() / 2, 0.15f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(DTAssetLib.FireRing.Value, position - Main.screenPosition, null, Color.SkyBlue with { A = 0 } * 0.15f, R * -0.5f, DTAssetLib.FireRing.Value.Size() / 2, 0.15f, SpriteEffects.None, 0f);

            Main.EntitySpriteDraw(DTAssetLib.BloomRing.Value, position - Main.screenPosition, null, Color.SkyBlue with { A = 0 } * 0.35f, R, DTAssetLib.BloomRing.Value.Size() / 2, 1.6f, SpriteEffects.None, 0f);
        }
    }
}
