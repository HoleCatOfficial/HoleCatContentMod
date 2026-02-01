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

namespace DestroyerTest.Common.Systems
{
	public class MiscWorldGenSystem : ModSystem
	{
		public static LocalizedText BeachGrassPassMessage { get; private set; }

		public override void SetStaticDefaults() {
			BeachGrassPassMessage = Language.GetText("Mods.DestroyerTest.WorldGen.BeachGrassPass");
		}

		// 4. We use the ModifyWorldGenTasks method to tell the game the order that our world generation code should run
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight) {
			int index = tasks.FindIndex(p => p.Name == "Stalac");
            if (index != -1)
            {
                tasks.Insert(index + 1, new BeachGrassPass("Beach Grass", 100f));
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
}