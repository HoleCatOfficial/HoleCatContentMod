using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace DestroyerTest.Content.Tiles
{
    public class Tile_VesperOre : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.Ore[Type] = true;
            TileID.Sets.FriendlyFairyCanLureTo[Type] = true;
            Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
            Main.tileOreFinderPriority[Type] = 235; // Metal Detector value, see https://terraria.wiki.gg/wiki/Metal_Detector
            Main.tileShine2[Type] = true; // Modifies the draw color slightly.
            Main.tileShine[Type] = 780; // How often tiny dust appear off this tile. Larger is less frequently
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(119, 104, 86), name);

            DustType = ModContent.DustType<VesperOreDust>();
            VanillaFallbackOnModDeletion = TileID.Silver;
            HitSound = SoundID.Tink;
            MineResist = 1.5f;
            MinPick = 10;
        }

        // Example of how to enable the Biome Sight buff to highlight this tile. Biome Sight is technically intended to show "infected" tiles, so this example is purely for demonstration purposes.
        public override bool IsTileBiomeSightable(int i, int j, ref Color sightColor)
        {
            sightColor = new Color(119, 104, 86);
            return true;
        }
        
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
            Texture2D Glow = ModContent.Request<Texture2D>("DestroyerTest/Content/Tiles/Tile_VesperOre_Glow").Value;
			Tile tile = Main.tile[i, j];
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen) {
				zero = Vector2.Zero;
			}
			int height = tile.TileFrameY == 36 ? 18 : 16;
			if (tile.Slope == 0 && !tile.IsHalfBlock) 
			{
				Main.spriteBatch.Draw(Glow, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
		}
	}

	// ExampleOreSystem contains code related to spawning ExampleOre. It contains both spawning ore during world generation, seen in ModifyWorldGenTasks, and spawning ore after defeating a boss, seen in BlessWorldWithExampleOre and MinionBossBody.OnKill.
	public class VesperSystem : ModSystem
	{
		public static LocalizedText VesperOrePassMessage { get; private set; }
		public static LocalizedText BlessedWithVesperOreMessage { get; private set; }

		public override void SetStaticDefaults() {
			VesperOrePassMessage = Mod.GetLocalization($"WorldGen.{nameof(VesperOrePassMessage)}");
			BlessedWithVesperOreMessage = Mod.GetLocalization($"WorldGen.{nameof(BlessedWithVesperOreMessage)}");
		}

		// This method is called from MinionBossBody.OnKill the first time the boss is killed.
		// The logic is located here for organizational purposes.
		public void BlessWorldWithVesperOre() {
			if (Main.netMode == NetmodeID.MultiplayerClient) {
				return; // This should not happen, but just in case.
			}

			// Since this happens during gameplay, we need to run this code on another thread. If we do not, the game will experience lag for a brief moment. This is especially necessary for world generation tasks that would take even longer to execute.
			// See https://github.com/tModLoader/tModLoader/wiki/World-Generation/#long-running-tasks for more information.
			ThreadPool.QueueUserWorkItem(_ => {
				// Broadcast a message to notify the user.
				if (Main.netMode == NetmodeID.SinglePlayer) {
					Main.NewText(BlessedWithVesperOreMessage.Value, 255, 255, 255);
				}
				else if (Main.netMode == NetmodeID.Server) {
					ChatHelper.BroadcastChatMessage(BlessedWithVesperOreMessage.ToNetworkText(), new Color(255, 255, 255));
				}

				// 100 controls how many splotches of ore are spawned into the world, scaled by world size. For comparison, the first 3 times altars are smashed about 275, 190, or 120 splotches of the respective hardmode ores are spawned. 
				int splotches = (int)(200 * (Main.maxTilesX / 4200f));
                int highestY = (int)Main.rockLayer - 200;
				for (int iteration = 0; iteration < splotches; iteration++) {
					// Find a point in the lower half of the rock layer but above the underworld depth.
					int i = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
					int j = WorldGen.genRand.Next(highestY, Main.UnderworldLayer);

					// OreRunner will spawn ExampleOre in splotches. OnKill only runs on the server or single player, so it is safe to run world generation code.
					WorldGen.OreRunner(i, j, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), (ushort)ModContent.TileType<Tile_VesperOre>());
				}
			});
		}

		// World generation is explained more in https://github.com/tModLoader/tModLoader/wiki/World-Generation
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight) {
			// Because world generation is like layering several images on top of each other, we need to do some steps between the original world generation steps.

			// Most vanilla ores are generated in a step called "Shinies", so for maximum compatibility, we will also do this.
			// First, we find out which step "Shinies" is.
			int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));

			if (ShiniesIndex != -1) {
				// Next, we insert our pass directly after the original "Shinies" pass.
				// ExampleOrePass is a class seen bellow
				tasks.Insert(ShiniesIndex + 1, new VesperPass("Vesper Ore Genearation", 237.4298f));
			}
		}
	}

	public class VesperPass : GenPass
	{
		public VesperPass(string name, float loadWeight) : base(name, loadWeight) {
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration) {
			// progress.Message is the message shown to the user while the following code is running.
			// Try to make your message clear. You can be a little bit clever, but make sure it is descriptive enough for troubleshooting purposes.
			progress.Message = VesperSystem.VesperOrePassMessage.Value;

			// Ores are quite simple, we simply use a for loop and the WorldGen.TileRunner to place splotches of the specified Tile in the world.
			// "6E-05" is "scientific notation". It simply means 0.00006 but in some ways is easier to read.
			for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 6E-05); k++) {
				// The inside of this for loop corresponds to one single splotch of our Ore.
				// First, we randomly choose any coordinate in the world by choosing a random x and y value.
				int x = WorldGen.genRand.Next(0, Main.maxTilesX);

				// WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use WorldGen.rockLayer or other WorldGen values.
				int y = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, Main.maxTilesY);

				// Then, we call WorldGen.TileRunner with random "strength" and random "steps", as well as the Tile we wish to place.
				// Feel free to experiment with strength and step to see the shape they generate.
				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(2, 6), ModContent.TileType<Tile_VesperOre>());

				// Alternately, we could check the tile already present in the coordinate we are interested.
				// Wrapping WorldGen.TileRunner in the following condition would make the ore only generate in Snow.
				// Tile tile = Framing.GetTileSafely(x, y);
				// if (tile.HasTile && tile.TileType == TileID.SnowBlock) {
				// 	WorldGen.TileRunner(.....);
				// }
			}
		}
	}
}