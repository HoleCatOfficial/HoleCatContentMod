using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using DestroyerTest.Common;
using SteelSeries.GameSense;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.UI;
using Terraria.ID;
using Terraria.WorldBuilding;
using Terraria.IO;
using Terraria.Localization;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Altar;

namespace DestroyerTest.Common.Systems
{
	public class MiscWorldGenSystem : ModSystem
	{
		public static LocalizedText BeachGrassPassMessage { get; private set; }
        public static LocalizedText AltarPassMessage { get; private set; }

        public override void SetStaticDefaults() {
			BeachGrassPassMessage = Language.GetText("Mods.DestroyerTest.WorldGen.BeachGrassPass");
            AltarPassMessage = Language.GetText("Mods.DestroyerTest.WorldGen.AltarPass");
        }

		// 4. We use the ModifyWorldGenTasks method to tell the game the order that our world generation code should run
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight) {
			int index = tasks.FindIndex(p => p.Name == "Stalac");
            if (index != -1)
            {
                tasks.Insert(index + 1, new BeachGrassPass("Beach Grass", 100f));
            }

            int index2 = tasks.FindIndex(p => p.Name == "Floating Islands");
            if (index2 != -1)
            {
                tasks.Insert(index2 + 1, new AltarIslandPass("Altar Island", 100f));
            }
        }
	}

	public class BeachGrassPass : GenPass
	{
		public BeachGrassPass(string name, float loadWeight) : base(name, loadWeight) {
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration) 
        {
            progress.Message = MiscWorldGenSystem.BeachGrassPassMessage.Value;
            int grassWidth = 180;

            // Left side
            int leftStartX = WorldGen.beachDistance;
            int leftEndX = leftStartX + grassWidth;

            // Right side
            int rightEndX = Main.maxTilesX - WorldGen.beachDistance;
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
                        WorldGen.PlaceTile(x, y, ModContent.TileType<BeachGrass>(), true, true);
                    }
                }
            }

            for (int x = rightStartX; x < rightEndX; x++)
            {
                for (int y = 0; y < Main.worldSurface; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (!tile.HasTile)
                        continue;

                    if (tile.TileType == TileID.Grass)
                    {
                        WorldGen.PlaceTile(x, y, ModContent.TileType<BeachGrass>(), true, true);
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

            if (!WorldGen.crimson)
            {
                return;
            }


            int BottomPadding = (int)(Main.maxTilesY - Main.worldSurface + 300);
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
            Layer(-3, -8, 8, TileID.StoneSlab);
            Layer(-4, -7, 10, TileID.StoneSlab);
            Layer(-5, -6, 12, TileID.StoneSlab);
            Layer(-6, -5, 14, TileID.StoneSlab);
            Layer(-7, -4, 16, TileID.StoneSlab);
            Layer(-8, -3, 18, TileID.StoneSlab);


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