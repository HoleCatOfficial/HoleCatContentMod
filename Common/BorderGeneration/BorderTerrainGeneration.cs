using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common.Systems;
using Microsoft.Xna.Framework;
using Terraria;
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
    5. Tile Smoothing.

    For the time being, since this is just a proof of concept, Titanstone will be used.
    */
    public class BorderTerrainGeneration : ModSystem
    {
        public int[] FloorY;
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int index = tasks.FindIndex(p => p.Name == "Full Desert");

            int height()
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
            if (index != -1)
            {
                tasks.Insert(index + 1, new BorderGroundworkPass("Border Generation: Groundwork", 100f, Main.UnderworldLayer, Main.UnderworldLayer - height()));

                tasks.Insert(index + 2, new BorderHorizontalCavePass("Border Generation: Long Caves", 100f, Main.UnderworldLayer, Main.UnderworldLayer - height()));

                tasks.Insert(index + 3, new BorderCeilingHolesPass("Border Generation: Ceiling Continuity", 100f, Main.UnderworldLayer - (height() + 15)));

                tasks.Insert(index + 4, new BorderChutePass("Border Generation: Chutes", 100f, 2, 9));
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

                WorldGen.TileRunner(X, lowerBound, 22, 9, TileID.Titanstone, addTile: true, overRide: true);
                WorldGen.TileRunner(X, upperBound, 22, 9, TileID.Titanstone, addTile: true, overRide: true);

                for (int j = 0; j < height; j++)
                {
                    WorldGen.TileRunner(X, upperBound + (j * 10), 22, 9, TileID.Titanstone, addTile: true, overRide: true);
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

                WorldGen.TileRunner(intervals[i], YSample, 18, 9, TileID.Titanstone, overRide: true);
                WorldGen.TileRunner(intervals[i], YSample - 10, 18, 9, TileID.Titanstone, overRide: true);

                if (WorldGen.genRand.NextBool(5))
                {
                    WorldGen.TileRunner(intervals[i], YSample - 22, 24, 9, TileID.Titanstone, overRide: true);
                }

                if (WorldGen.genRand.NextBool(12))
                {
                    WorldGen.TileRunner(intervals[i], YSample - 24, 30, 9, TileID.Titanstone, overRide: true);
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
}
