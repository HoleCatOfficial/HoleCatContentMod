using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.BorderBiome.Tiles;
using DestroyerTest.Content.BorderBiome.Walls;
using DestroyerTest.Content.Tiles;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace DestroyerTest.Common.BorderGeneration
{
    /*
    Border terrain generation is mostly shelves with some chutes. A very open area.

    It will be ideally be split into multiple passes:
    
    1. Groundwork (DONE): A large strip of terrain tiles will be generated in the vertical bounds of the biome (that being from where ash generates to somewhere in the caverns).
    2. Shelf Digging (DONE): The horizontal spaces will be generated first.
    3. Chute Digging (DONE-ish): The sparse chutes will generate after.
    4. Lava: The border has lava pools, though not as many as the caverns.
    4.5: Water where lava would be: Under the jungle, moss will grow on the shale.
    5. Tile Smoothing.

    For the time being, since this is just a proof of concept, Titanstone will be used.
    */

    public class BorderTerrainGeneration : ModSystem
    {
        public int[] FloorY;

        public static int TileType;

        public int height()
        {
            if (WorldGen.GetWorldSize() == 0)
            {
                return 150;
            }
            if (WorldGen.GetWorldSize() == 1)
            {
                return 250;
            }
            return 450;
        }

        public override void SetStaticDefaults()
        {
            TileType = ModContent.TileType<Tile_CarbonaceousShale>();
        }

        public int MossStartX;
        public int MossEndX;

        private ILHook _mushroomHook;

        public override void Load()
        {
            //Mushroom Edit
            Type hiddenClassType = typeof(WorldGen).GetNestedType("<>c", BindingFlags.NonPublic);
            if (hiddenClassType != null)
            {
                MethodInfo targetMethod = hiddenClassType.GetMethod("<AddGenPasses>b__288_20", BindingFlags.NonPublic | BindingFlags.Instance);
                if (targetMethod != null)
                {
                    _mushroomHook = new ILHook(targetMethod, PatchMushroomGeneration);
                }
            }


        }


        private void PatchMushroomGeneration(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            int yCoordLocalIndex = -1;
            int flagLocalIndex = -1;

            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdsfld(typeof(Main), nameof(Main.maxTilesY)),
                i => i.MatchLdcI4(300),
                i => i.MatchSub(),
                i => i.MatchCallvirt(out _),
                i => i.MatchStloc(out yCoordLocalIndex),
                i => i.MatchLdcI4(0),
                i => i.MatchStloc(out flagLocalIndex)))
            {
                Mod.Logger.Error("Could not find Mushroom patch generation bytecode!");
                return;
            }

            c.Emit(OpCodes.Ldloc, yCoordLocalIndex);

            c.EmitDelegate<Func<int, bool>>((yCoord) => {
                // Blocks the patch if it tries to spawn in your custom biome depth
                return yCoord > (MinimumY);
            });

            ILLabel skipOverrideLabel = c.DefineLabel();
            c.Emit(OpCodes.Brfalse_S, skipOverrideLabel);

            c.Emit(OpCodes.Ldc_I4_1);
            c.Emit(OpCodes.Stloc, flagLocalIndex);

            c.MarkLabel(skipOverrideLabel);
        }

        int MinimumY => Main.UnderworldLayer - (height() + 40);

        public override void Unload()
        {
            _mushroomHook?.Dispose();
            TileType = -1;
        }


        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int index = tasks.FindIndex(p => p.Name == "Full Desert");

            
            if (index != -1)
            {
                tasks.Insert(index + 1, new BorderGroundworkPass("Border Generation: Groundwork", 100f, Main.UnderworldLayer, Main.UnderworldLayer - height()));

                tasks.Insert(index + 2, new BorderWallPass("Border Generation: Wall Runner", 100f, Main.UnderworldLayer, Main.UnderworldLayer - (height() + 20)));

                tasks.Insert(index + 3, new BorderHorizontalCavePass("Border Generation: Long Caves", 100f, Main.UnderworldLayer, Main.UnderworldLayer - height()));

                tasks.Insert(index + 4, new BorderCeilingHolesPass("Border Generation: Ceiling Continuity", 100f, Main.UnderworldLayer - (height() + 15)));

                tasks.Insert(index + 5, new BorderChutePass("Border Generation: Chutes", 100f, 2, 9));

                tasks.Insert(index + 6, new BorderLakePass("Border Generation: Midnight Lake", 100f, 1));

                tasks.Insert(index + 7, new BorderMossPass("Border Generation: Midnight Moss", 100f, Main.UnderworldLayer - height(), Main.UnderworldLayer));

                tasks.Insert(index + 8, new BorderVinePass("Border Generation: Midnight Moss Vines", 100f, Main.UnderworldLayer, Main.UnderworldLayer - height()));
            }
        }
    }

    public class BorderGroundworkPass : GenPass
    {
        int lowerBound = 0;
        int upperBound = 0;
        public BorderGroundworkPass(string name, float loadWeight, int LowerBound, int UpperBound) : base(name, loadWeight)
        {
            lowerBound = LowerBound;
            upperBound = UpperBound;
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            int[] intervals = Enumerable.Range(0, Main.maxTilesX + 1).Where(i => i % 10 == 0).ToArray();
            int height = 45;

            
            for (int i = 1; i < intervals.Length; i++)
            {
                int X = intervals[i];

                WorldGen.TileRunner(X, lowerBound, 22, 9, BorderTerrainGeneration.TileType, addTile: true, overRide: true);
                WorldGen.TileRunner(X, upperBound, 22, 9, BorderTerrainGeneration.TileType, addTile: true, overRide: true);

                for (int j = 0; j < height; j++)
                {
                    WorldGen.TileRunner(X, upperBound + (j * 10), 22, 9, BorderTerrainGeneration.TileType, addTile: true, overRide: true);
                }


            }


        }
    }

    public class BorderHorizontalCavePass : GenPass
    {
        int lowerBound = 0;
        int upperBound = 0;
        public BorderHorizontalCavePass(string name, float loadWeight, int LowerBound, int UpperBound) : base(name, loadWeight)
        {
            lowerBound = LowerBound;
            upperBound = UpperBound;
        }

        bool f1 = false;
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            int[] intervals = Enumerable.Range(40, (Main.maxTilesX + 1) - 80).Where(i => i % 30 == 0).ToArray();

            int floorinterval()
            {
                if (WorldGen.GetWorldSize() == 0)
                {
                    return 23;
                }
                if (WorldGen.GetWorldSize() == 1)
                {
                    return 46;
                }

                return 70;
            }
            int[] floors = Enumerable.Range(upperBound, lowerBound - upperBound + 1).Where(i => i % floorinterval() == 0).ToArray();

            ModContent.GetInstance<BorderTerrainGeneration>().FloorY = floors;

            int cavelength()
            {
                if (WorldGen.GetWorldSize() == 0)
                {
                    return 10;
                }
                if (WorldGen.GetWorldSize() == 1)
                {
                    return 12;
                }

                return 18;
            }

            int cavesize()
            {
                if (WorldGen.GetWorldSize() == 0)
                {
                    return 7;
                }
                if (WorldGen.GetWorldSize() == 1)
                {
                    return 10;
                }

                return 12;
            }



            List<int> gapstartinternal = new();
            
            int gapSize = 16;

            if (!f1)
            {
                gapstartinternal = Enumerable.Range(0, WorldGen.genRand.Next(3, 10)).Select(_ => intervals[WorldGen.genRand.Next(intervals.Length)]).ToList();
                f1 = true;
            }

            int[] gapStart = gapstartinternal.ToArray(); ;

            for (int i = 0; i < floors.Length - 1; i++)
            {
                for (int j = 0; j < intervals.Length; j++)
                {
                    for (int k = 0; k < gapStart.Length; k++)
                    {
                        if (j >= gapStart[k] && j < gapStart[k] + gapSize)
                        {
                            continue;
                        }
                    }

                    WorldGen.digTunnel(intervals[j], floors[i] + 50, WorldGen.genRand.Next(-10, 11), 0, cavelength(), cavesize());


                    if (Main.rand.NextBool(3))
                    {
                        Point wallHolePoint = new Point(intervals[j] + Main.rand.Next(-6, 6), floors[i] + 50 + Main.rand.Next(-6, 6));
                        WorldUtils.Gen(wallHolePoint, new Shapes.Circle(Main.rand.Next(15, 30), Main.rand.Next(6, 10)), new Actions.ClearWall(true));
                    }

                }
            }

            for (int i = 0; i < intervals.Length; i++)
            {
                WorldGen.digTunnel(intervals[i], upperBound, WorldGen.genRand.Next(-10, 11), 0, cavelength() + 2, cavesize() + 2);
            }
        }
    }

    public class BorderCeilingHolesPass : GenPass
    {
        int YSample = 0;
        public BorderCeilingHolesPass(string name, float loadWeight, int ySample) : base(name, loadWeight)
        {
            YSample = ySample;
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            int[] intervals = Enumerable.Range(0, Main.maxTilesX + 1).Where(i => i % 10 == 0).ToArray();

            for (int i = 0; i < intervals.Length; i++)
            {
                Point Sample = new Point(intervals[i], YSample);

                WorldGen.TileRunner(intervals[i], YSample, 18, 9, BorderTerrainGeneration.TileType, overRide: true);
                WorldGen.TileRunner(intervals[i], YSample - 10, 18, 9, BorderTerrainGeneration.TileType, overRide: true);

                if (WorldGen.genRand.NextBool(5))
                {
                    WorldGen.TileRunner(intervals[i], YSample - 22, 24, 9, BorderTerrainGeneration.TileType, overRide: true);
                }

                if (WorldGen.genRand.NextBool(12))
                {
                    WorldGen.TileRunner(intervals[i], YSample - 24, 30, 9, BorderTerrainGeneration.TileType, overRide: true);
                }
            }
        }

    }

    public class BorderChutePass : GenPass
    {
        int numChutes;

        public BorderChutePass(string name, float loadWeight, int MinChutes, int MaxChutes) : base(name, loadWeight)
        {
            numChutes = Main.rand.Next(MinChutes, MaxChutes + 1);
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            int[] intervals = Enumerable.Range(40, (Main.maxTilesX + 1) - 80).Where(i => i % 10 == 0).ToArray();
            var system = ModContent.GetInstance<BorderTerrainGeneration>();

            int chutelength()
            {
                if (WorldGen.GetWorldSize() == 0)
                {
                    return 8;
                }
                if (WorldGen.GetWorldSize() == 1)
                {
                    return 9;
                }

                return 13;
            }


            for (int i = 0; i < system.FloorY.Length - 1; i++)
            {
                int[] chuteX = new int[numChutes];

                for (int t = 0; t < numChutes; t++)
                {
                    chuteX[t] = intervals[Main.rand.Next(intervals.Length)];
                }

                if (chuteX.Length >= numChutes)
                {
                    for (int j = 0; j < chuteX.Length; j++)
                    {
                        WorldGen.digTunnel(chuteX[j], system.FloorY[i] - 16, WorldGen.genRand.Next(5, 10) * (WorldGen.genRand.NextBool() ? 1 : -1), 10, chutelength(), 12);
                    }

                }
            }


        }
    }

    public class BorderMossPass : GenPass
    {

        int upperBound;
        int lowerBound;

        public BorderMossPass(string name, float loadWeight, int UpperBound, int LowerBound) : base(name, loadWeight)
        {
            upperBound = UpperBound;
            lowerBound = LowerBound;
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


            

            int width = (int)(maxX - minX);
            Rectangle scanArea = new Rectangle(GenVars.jungleOriginX - (width / 2), upperBound, width, lowerBound - upperBound);

            //WorldGen.TileRunner(scanArea.X, scanArea.Y, 23, 4, TileID.AmethystGemspark, true);

            var system = ModContent.GetInstance<BorderTerrainGeneration>();
            system.MossStartX = minX;
            system.MossEndX = maxX;

            for (int i = 0; i < scanArea.Width; i++)
            {
                for (int j = 0; j < scanArea.Height; j++)
                {
                    int x = scanArea.X + i;
                    int y = scanArea.Y + j;

                    Tile tile = Framing.GetTileSafely(x, y);

                    if (tile.HasTile)
                    {
                        bool HasExposedSide()
                        {
                            return !Framing.GetTileSafely(x + 1, y).HasTile ||
                                   !Framing.GetTileSafely(x - 1, y).HasTile ||
                                   !Framing.GetTileSafely(x, y + 1).HasTile ||
                                   !Framing.GetTileSafely(x, y - 1).HasTile;
                        }
                        if (HasExposedSide() && tile.TileType == BorderTerrainGeneration.TileType)
                        {
                            tile.TileType = (ushort)ModContent.TileType<MidnightMoss_CarbonaceousShale>();
                        }
                    }

                    //system.Mod.Logger.Info($"Scanned: {scanned}, Matching tiles: {matchingTiles}");
                }
            }
        }
    }

    public class BorderLakePass : GenPass
    {
        int width = 0;
        public BorderLakePass(string name, float loadWeight, int Width) : base(name, loadWeight)
        {
            width = Width;
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            
           
            var system = ModContent.GetInstance<BorderTerrainGeneration>();

            WorldUtils.Gen(new Point(GenVars.jungleOriginX, system.FloorY[2] - 8), new Shapes.Mound(80, 40), new Actions.ClearTile(true));

            WorldUtils.Gen(new Point(GenVars.jungleOriginX, system.FloorY[2] - 8), new Shapes.Circle(80, 20), new Actions.ClearTile(true));

            WorldUtils.Gen(new Point(GenVars.jungleOriginX, system.FloorY[2] - 8), new Shapes.Circle(80, 8), new Actions.SetLiquid(LiquidID.Water));

        }
    }

    public class BorderWallPass : GenPass
    {
        int lowerBound = 0;
        int upperBound = 0;
        public BorderWallPass(string name, float loadWeight, int LowerBound, int UpperBound) : base(name, loadWeight)
        {
            lowerBound = LowerBound;
            upperBound = UpperBound;
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int sine = (int)(Math.Sin(i * (Math.PI * 2 / 24)) * 4);

                for (int j = 0; j < lowerBound - upperBound; j++)
                {
                    Main.tile[i, upperBound + j].WallType = (ushort)ModContent.WallType<Wall_CarbonaceousShaleWall>();
                    //WorldGen.PlaceWall(i, upperBound + j, ModContent.WallType<Wall_CarbonaceousShaleWall>());
                }

                for (int k = 0; k < 4 + sine; k++)
                {
                    Main.tile[i, upperBound - k].WallType = (ushort)ModContent.WallType<Wall_CarbonaceousShaleWall>();
                    //WorldGen.PlaceWall(i, upperBound - k, ModContent.WallType<Wall_CarbonaceousShaleWall>());
                }
            }



        }
    }

    public class BorderVinePass : GenPass
    {
        int lowerBound = 0;
        int upperBound = 0;
        public BorderVinePass(string name, float loadWeight, int LowerBound, int UpperBound) : base(name, loadWeight)
        {
            lowerBound = LowerBound;
            upperBound = UpperBound;
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            var system = ModContent.GetInstance<BorderTerrainGeneration>();
            int range = (lowerBound - upperBound);
            int width = system.MossEndX - system.MossStartX;

            bool IsMidnightMoss(ushort type)
            {
                return type == ModContent.TileType<MidnightMoss>()
                    || type == ModContent.TileType<MidnightMoss_CarbonaceousShale>()
                    || type == ModContent.TileType<MidnightMoss_CorruptCarbonaceousShale>()
                    || type == ModContent.TileType<MidnightMoss_CrimsonCarbonaceousShale>()
                    || type == ModContent.TileType<MidnightMoss_HallowCarbonaceousShale>();
            }

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int x = i;

                
                for (int j = 0; j < range; j++)
                {
                    int y = upperBound + j;

                    Tile t = Main.tile[x, y];
                   

                    if (t.HasTile)
                    {

                        if (IsMidnightMoss(t.TileType) && !t.BottomSlope && Main.rand.NextBool(4))
                        {
                            Tile below = Main.tile[x, y + 1];

                            if (!below.HasTile)
                            {
                                new ActionVines(3, 8, ModContent.TileType<MidnightMossVine>()).Apply(new Point(x, y + 1), x, y + 1);
                                WorldGen.RangeFrame(x - 1, y - 1, x + 1, y + 10);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }



        }
    }
}
