
using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles.Riftplate
{
	public class Tile_Riftplate : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
            HitSound = new SoundStyle("DestroyerTest/Assets/Audio/TenebrousConstruct/Hit", 5)
            {
                PitchVariance = 0.2f,
                MaxInstances = 0
            };
            DustType = ModContent.DustType<RiftDust>();


			AddMapEntry(new Color(16, 16, 16));
		}
        public override void HitWire(int i, int j) {
        Tile tile = Main.tile[i, j];
            ushort newTileType = (tile.TileType == ModContent.TileType<Tile_Riftplate>()) ? (ushort)ModContent.TileType<Tile_RiftplateActive>() : (ushort)ModContent.TileType<Tile_Riftplate>();

            // Change the tile type
            tile.TileType = newTileType;

            SoundEngine.PlaySound(SoundID.Item30);

            // Skip further wire processing for this tile
            Wiring.SkipWire(i, j);

            // Avoid trying to send packets in singleplayer
            if (Main.netMode != NetmodeID.SinglePlayer) {
                NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
            }

            // Update the tile
            WorldGen.SquareTileFrame(i, j, true);
        }

	}
}
