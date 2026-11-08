using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.BorderGeneration;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.BorderBiome.Tiles
{
    // This file is an example of making a grass tile. Grass tiles have a few unique behaviors:
    // 1. Grass tiles are tiles that place over an existing "dirt" tile, unlike other tiles which can only be placed in empty spaces.
    // 2. When mined, grass tiles revert to their "dirt" tile instead of being destroyed.
    // 3. Grass tiles spread to nearby "dirt" tiles and also usually spawn other plants as well.
    //
    // In Terraria, Dirt, Mud, and Ash are all "dirt" tile options, but modded "dirt" tiles are also supported. Some grasses are placeable on multiple "dirt" tiles.
    // This example includes ExampleGrass_Dirt and ExampleGrass_ExampleBlock. ExampleGrass_Dirt is placed on Dirt and ExampleGrass_ExampleBlock is placed on ExampleBlock.
    // The ExampleGrass abstract class contains the shared logic.
    //
    // ExampleGrassSeeds places both of these grass tiles, similar to how Corrupt Grass and Corrupt Jungle Grass are both placed by Corrupt Seeds.
    public abstract class MidnightMoss : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileBrick[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.Conversion.Moss[Type] = true;
            TileID.Sets.ResetsHalfBrickPlacementAttempt[Type] = false; // Prevents the existing tile from losing its half-brick status when this grass is placed over it.
            TileID.Sets.DoesntPlaceWithTileReplacement[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            //TileID.Sets.GrassNotKilledWhenSurrounded[Type] = true; // Would allow the tile to stay even when surrounded by solid tiles.
            TileID.Sets.Infectable[Type] = true;

            DustType = ModContent.DustType<MidnightMossDust>();

            HitSound = new SoundStyle(DTAssetLib.AudioPath + "/Border/ShaleMine", 3) { PitchVariance = 0.5f, MaxInstances = 0 };
            MineResist = 4f;
            MinPick = 12;

            AddMapEntry(new Color(172, 23, 232));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {

            r = 172 * 0.005f;
            g = 23 * 0.005f;
            b = 232 * 0.005f;
            
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (!Main.gameInactive && !Main.gamePaused)
            {
                if (Main.rand.NextBool(15))
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
        
        public override void OverridePlacementTile(int i, int j, Item item, ref int tileToCreate, ref int previewPlaceStyle, ref int? forcedRandom, ref bool? overrideCanPlace)
        {
            // When this tile is about to be placed, this method allows us to change which tile is actually placed depending on the existing "dirt" tile at the target location. This is how grass tiles can support multiple "dirt" tiles. This method can be removed for grass tiles that do not have alternate "dirt" tiles.
            Tile tile = Main.tile[i, j];
            if (tile.HasTile)
            {
                if (tile.TileType == BorderTerrainGeneration.TileType)
                {
                    tileToCreate = ModContent.TileType<MidnightMoss_CarbonaceousShale>();
                }

                if (tile.TileType == ModContent.TileType<Tile_CorruptCarbonaceousShale>())
                {
                    tileToCreate = ModContent.TileType<MidnightMoss_CorruptCarbonaceousShale>();
                }

                if (tile.TileType == ModContent.TileType<Tile_CrimsonCarbonaceousShale>())
                {
                    tileToCreate = ModContent.TileType<MidnightMoss_CrimsonCarbonaceousShale>();
                }

                if (tile.TileType == ModContent.TileType<Tile_HallowCarbonaceousShale>())
                {
                    tileToCreate = ModContent.TileType<MidnightMoss_HallowCarbonaceousShale>();
                }
            }
            return;
        }

        // We use RandomUpdate to spread this grass and also grow ExamplePlants on top.
        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int grassType = tile.TileType;

            Tile above = Main.tile[i, j - 1];

            // Attempt to spread grass to neighboring "dirt" tiles.
            TileColorCache tileColor = tile.BlockColorAndCoating();
            bool grassHasSpread = false;
            for (int i2 = i - 1; i2 <= i + 1; i2++)
            {
                for (int j2 = j - 1; j2 <= j + 1; j2++)
                {
                    Tile neighbor = Main.tile[i2, j2];
                    if (i == i2 && j == j2 || !neighbor.HasTile)
                    {
                        continue;
                    }
                    else if (neighbor.TileType == BorderTerrainGeneration.TileType)
                    {
                        WorldGen.SpreadGrass(i2, j2, BorderTerrainGeneration.TileType, grassType, repeat: false, tileColor);
                        if (neighbor.TileType == grassType)
                        {
                            WorldGen.SquareTileFrame(i2, j2);
                            grassHasSpread = true;
                        }
                    }

                    else if (neighbor.TileType == ModContent.TileType<Tile_CorruptCarbonaceousShale>())
                    {
                        WorldGen.SpreadGrass(i2, j2, ModContent.TileType<Tile_CorruptCarbonaceousShale>(), grassType, repeat: false, tileColor);
                        if (neighbor.TileType == grassType)
                        {
                            WorldGen.SquareTileFrame(i2, j2);
                            grassHasSpread = true;
                        }
                    }

                    else if (neighbor.TileType == ModContent.TileType<Tile_CrimsonCarbonaceousShale>())
                    {
                        WorldGen.SpreadGrass(i2, j2, ModContent.TileType<Tile_CrimsonCarbonaceousShale>(), grassType, repeat: false, tileColor);
                        if (neighbor.TileType == grassType)
                        {
                            WorldGen.SquareTileFrame(i2, j2);
                            grassHasSpread = true;
                        }
                    }

                    else if (neighbor.TileType == ModContent.TileType<Tile_HallowCarbonaceousShale>())
                    {
                        WorldGen.SpreadGrass(i2, j2, ModContent.TileType<Tile_HallowCarbonaceousShale>(), grassType, repeat: false, tileColor);
                        if (neighbor.TileType == grassType)
                        {
                            WorldGen.SquareTileFrame(i2, j2);
                            grassHasSpread = true;
                        }
                    }
                }
            }

            if (Main.netMode == NetmodeID.Server && grassHasSpread)
            {
                NetMessage.SendTileSquare(-1, i, j, 3);
            }
        }
    }
    public class MidnightMoss_CarbonaceousShale : MidnightMoss
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            TileID.Sets.NeedsGrassFraming[Type] = true;

            TileID.Sets.NeedsGrassFramingDirt[Type] = BorderTerrainGeneration.TileType;
        }
    }

    public class MidnightMoss_CorruptCarbonaceousShale : MidnightMoss
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            TileID.Sets.NeedsGrassFraming[Type] = true;

            TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<Tile_CorruptCarbonaceousShale>();

            //To
            TileLoader.RegisterSimpleConversion(ModContent.TileType<MidnightMoss_CarbonaceousShale>(), BiomeConversionID.Corruption, Type);
            //And fro
            TileLoader.RegisterConversion(Type, BiomeConversionID.Purity, ModContent.TileType<MidnightMoss_CarbonaceousShale>());
        }
    }

    public class MidnightMoss_CrimsonCarbonaceousShale : MidnightMoss
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            TileID.Sets.NeedsGrassFraming[Type] = true;

            TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<Tile_CrimsonCarbonaceousShale>();


            //To
            TileLoader.RegisterSimpleConversion(ModContent.TileType<MidnightMoss_CarbonaceousShale>(), BiomeConversionID.Crimson, Type);
            //And fro
            TileLoader.RegisterConversion(Type, BiomeConversionID.Purity, ModContent.TileType<MidnightMoss_CarbonaceousShale>());
        }
    }

    public class MidnightMoss_HallowCarbonaceousShale : MidnightMoss
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            TileID.Sets.NeedsGrassFraming[Type] = true;

            TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<Tile_HallowCarbonaceousShale>();


            //To
            TileLoader.RegisterSimpleConversion(ModContent.TileType<MidnightMoss_CarbonaceousShale>(), BiomeConversionID.Hallow, Type);
            //And fro
            TileLoader.RegisterConversion(Type, BiomeConversionID.Purity, ModContent.TileType<MidnightMoss_CarbonaceousShale>());
        }
    }

}
