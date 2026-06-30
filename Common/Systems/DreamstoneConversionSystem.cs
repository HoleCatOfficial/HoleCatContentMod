using DestroyerTest.Common;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Lorebooks;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Altar;
using DestroyerTest.Content.Tiles.RoseGarden;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using SteelSeries.GameSense;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.WorldBuilding;
using static Terraria.GameContent.Animations.IL_Actions.NPCs;

namespace DestroyerTest.Common.Systems
{
    public class DreamstoneConversionSystem : ModSystem
    {
        public override void PostUpdateWorld()
        {
            if (!Main.dayTime)
            {
                for (int i = 0; i < Main.maxTilesX - 1; i++)
                {
                    Point Sample = new Point(i, Main.rand.Next(Main.maxTilesY - 1));

                    Point SampleUp = new Point(Sample.X, Sample.Y - 1);
                    Point SampleDown = new Point(Sample.X, Sample.Y + 1);
                    Point SampleLeft = new Point(Sample.X - 1, Sample.Y);
                    Point SampleRight = new Point(Sample.X + 1, Sample.Y);

                    Tile tile = Framing.GetTileSafely(Sample);
                    Tile tileUp = Framing.GetTileSafely(SampleUp);
                    Tile tileDown = Framing.GetTileSafely(SampleDown);
                    Tile tileLeft = Framing.GetTileSafely(SampleLeft);
                    Tile tileRight = Framing.GetTileSafely(SampleRight);

                    ushort DreamstoneID = (ushort)ModContent.TileType<Tile_Dreamstone>();

                    if (tile != null && tile.HasTile && tile.TileType == (ushort)ModContent.TileType<Tile_VesperOre>())
                    {

                        if (tileUp != null && tileUp.HasTile && tileUp.TileType == DreamstoneID && Main.rand.NextBool(10))
                        {
                            if (SampleUp.ToWorldCoordinates().Distance(Main.LocalPlayer.Center) < 1200)
                            {
                                SoundEngine.PlaySound(DTAssetLib.Impacts.KCrystalConsume, SampleUp.ToWorldCoordinates());
                                ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.Keybrand, new ParticleOrchestraSettings() with { PositionInWorld = SampleUp.ToWorldCoordinates() });
                            }
                            tileUp.TileType = (ushort)ModContent.TileType<Tile_VesperOre>();
                        }

                        if (tileDown != null && tileDown.HasTile && tileDown.TileType == DreamstoneID && Main.rand.NextBool(10))
                        {
                            if (SampleDown.ToWorldCoordinates().Distance(Main.LocalPlayer.Center) < 1200)
                            {
                                SoundEngine.PlaySound(DTAssetLib.Impacts.KCrystalConsume, SampleDown.ToWorldCoordinates());
                                ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.Keybrand, new ParticleOrchestraSettings() with { PositionInWorld = SampleDown.ToWorldCoordinates() });
                            }
                            tileDown.TileType = (ushort)ModContent.TileType<Tile_VesperOre>();
                        }

                        if (tileLeft != null && tileLeft.HasTile && tileLeft.TileType == DreamstoneID && Main.rand.NextBool(10))
                        {
                            if (SampleLeft.ToWorldCoordinates().Distance(Main.LocalPlayer.Center) < 1200)
                            {
                                SoundEngine.PlaySound(DTAssetLib.Impacts.KCrystalConsume, SampleLeft.ToWorldCoordinates());
                                ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.Keybrand, new ParticleOrchestraSettings() with { PositionInWorld = SampleLeft.ToWorldCoordinates() });
                            }
                            tileLeft.TileType = (ushort)ModContent.TileType<Tile_VesperOre>();
                        }

                        if (tileRight != null && tileRight.HasTile && tileRight.TileType == DreamstoneID && Main.rand.NextBool(10))
                        {
                            if (SampleRight.ToWorldCoordinates().Distance(Main.LocalPlayer.Center) < 1200)
                            {
                                SoundEngine.PlaySound(DTAssetLib.Impacts.KCrystalConsume, SampleRight.ToWorldCoordinates());
                                ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.Keybrand, new ParticleOrchestraSettings() with { PositionInWorld = SampleRight.ToWorldCoordinates() });
                            }
                            tileRight.TileType = (ushort)ModContent.TileType<Tile_VesperOre>();
                        }
                    }
                }
            }
        }
    }
}
