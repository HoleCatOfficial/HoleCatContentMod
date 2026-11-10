using System;
using System.Collections.Generic;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Altar;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace DestroyerTest.Common.Systems
{
    public class MiscWorldGenSystem : ModSystem
    {
        public static LocalizedText BeachGrassPassMessage { get; private set; }
        public static LocalizedText AltarPassMessage { get; private set; }

        public override void SetStaticDefaults()
        {
            BeachGrassPassMessage = Language.GetText("Mods.DestroyerTest.WorldGen.BeachGrassPass");
            AltarPassMessage = Language.GetText("Mods.DestroyerTest.WorldGen.AltarPass");
        }

        // 4. We use the ModifyWorldGenTasks method to tell the game the order that our world generation code should run
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int index = tasks.FindIndex(p => p.Name == "Stalac");
            if (index != -1 && DTVanillaChangesConfig.instance.WorldGeneration)
            {
                tasks.Insert(index + 1, new BeachGrassPass("Beach Grass", 100f));
            }

            int index2 = tasks.FindIndex(p => p.Name == "Floating Islands");
            if (index2 != -1)
            {
                tasks.Insert(index2 + 1, new AltarIslandPass("Altar Island", 100f));
            }

            int index3 = tasks.FindIndex(p => p.Name == "Mushroom Patches");
            if (index3 != -1 && DTVanillaChangesConfig.instance.WorldGeneration)
            {
                tasks.Insert(index3 + 1, new SurfaceMushroomPass("Surface Mushrooms", 100f));
            }
        }
    }

    public class SurfaceMushroomPass : GenPass
    {
        public SurfaceMushroomPass(string name, float loadWeight) : base(name, loadWeight)
        {

        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            int minX = 0;
            int maxX = 0;
            bool flag4 = false;
            bool flag5 = false;

            for (int num153 = 5; num153 < Main.maxTilesX - 5; num153++)
            {
                for (int num154 = 0; (double)num154 < Main.worldSurface + 20.0; num154++)
                {
                    if (Main.tile[num153, num154].HasTile && Main.tile[num153, num154].TileType == 60)
                    {
                        minX = num153;
                        flag4 = true;
                        break;
                    }
                }

                if (flag4)
                {
                    break;
                }
            }

            for (int num155 = Main.maxTilesX - 5; num155 > 5; num155--)
            {
                for (int num156 = 0; (double)num156 < Main.worldSurface + 20.0; num156++)
                {
                    if (Main.tile[num155, num156].HasTile && Main.tile[num155, num156].TileType == 60)
                    {
                        maxX = num155;
                        flag5 = true;
                        break;
                    }
                }

                if (flag5)
                {
                    break;
                }
            }

            bool flipacoin = Main.rand.NextBool();

            int startX = flipacoin ? maxX - 400 : minX;
            int endX = flipacoin ? maxX : minX + 400;

            for (int x = startX; x < endX; x++)
            {
                for (int y = 0; y < Main.worldSurface; y++)
                {
                    Tile tile = Main.tile[x, y];

                    if (!tile.HasTile)
                    {
                        continue;
                    }

                    if (tile.TileType == TileID.JungleGrass)
                    {
                        tile.TileType = TileID.MushroomGrass;
                    }
                }
            }

        }
    }

    public class BeachGrassPass : GenPass
    {
        public BeachGrassPass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = MiscWorldGenSystem.BeachGrassPassMessage.Value;
            int grassWidth = 480;

            // Left side
            int leftStartX = 0;
            int leftEndX = leftStartX + grassWidth;

            // Right side
            int rightEndX = Main.maxTilesX;
            int rightStartX = rightEndX - grassWidth;

            // Iterate horizontally over strips
            for (int x = leftStartX; x < leftEndX; x++)
            {
                for (int y = 0; y < Main.worldSurface; y++) // only up to surface
                {
                    Tile tile = Main.tile[x, y];
                    if (!tile.HasTile)
                        continue;

                    if (tile.TileType == TileID.Grass)
                    {
                        tile.TileType = (ushort)ModContent.TileType<BeachGrass>();
                    }
                }
            }

            for (int x = rightStartX; x < rightEndX; x++)
            {
                for (int y = 0; y < Main.worldSurface + 30; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (!tile.HasTile)
                        continue;

                    if (tile.TileType == TileID.Grass)
                    {
                        tile.TileType = (ushort)ModContent.TileType<BeachGrass>();
                    }
                }
            }
        }
    }
    

    public class AltarIslandPass : GenPass
    {
        public AltarIslandPass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        public bool IslandGenerated = false;
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = MiscWorldGenSystem.BeachGrassPassMessage.Value;

            if (!WorldGen.crimson && !WorldGen.drunkWorldGen)
            {
                return;
            }


            int BottomPadding = (int)(Main.maxTilesY - Main.worldSurface + 100);
            Point GenPoint = WorldGen.RandomWorldPoint(100, 300, BottomPadding, 300);

            Point Relative(int x, int y)
            {
                return new Point(GenPoint.X + x, GenPoint.Y + y);
            }

            Point AltarTilePoint = Relative(-1, -10);

            Point Torch1 = new Point(AltarTilePoint.X - 2, AltarTilePoint.Y + 1);
            Point Torch2 = new Point(Torch1.X + 7, Torch1.Y);


            WorldGen.CloudIsland(GenPoint.X, GenPoint.Y);


            void Layer(int startX, int y, int width, int tileType)
            {
                for (int x = 0; x < width; x++)
                {
                    Point p = Relative(startX + x, y);
                    WorldGen.PlaceTile(p.X, p.Y, tileType);
                }
            }

            //Pass 1
            Layer(-3, -8, 8, ModContent.TileType<Tile_AltarBrick>());
            Layer(-4, -7, 10, ModContent.TileType<Tile_AltarBrick>());
            Layer(-5, -6, 12, ModContent.TileType<Tile_AltarBrick>());
            Layer(-6, -5, 14, ModContent.TileType<Tile_AltarBrick>());
            Layer(-7, -4, 16, ModContent.TileType<Tile_AltarBrick>());
            Layer(-8, -3, 18, ModContent.TileType<Tile_AltarBrick>());


            //Remember to do this last, since it needs tiles to stand on.

            //WorldGen.PlaceTile(Torch1.X, Torch1.Y, TileID.Torches, true, style: 15);
            WorldGen.PlaceTile(Torch1.X, Torch1.Y, TileID.Torches, true, style: 23);
            WorldGen.PlaceTile(Torch2.X, Torch2.Y, TileID.Torches, true, style: 23);

            //WorldGen.PlaceTile(AltarTilePoint.X, AltarTilePoint.Y, TileID.EmeraldGemspark, true);
            WorldGen.PlaceObject(AltarTilePoint.X, AltarTilePoint.Y, ModContent.TileType<Tile_BlessingAltar>());
            //WorldGen.Place4x2(AltarTilePoint.X, AltarTilePoint.Y, (ushort)ModContent.TileType<Tile_BlessingAltar>());
            //WorldGen.Place4x2(AltarTilePoint.X - 1, AltarTilePoint.Y, TileID.Bathtubs, style: 2);
        }
    }
}
