using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using DestroyerTest.Content.RiftBiome.RiftDesertResources;
using DestroyerTest.Content.RiftBiome;
using System;
using Terraria.ModLoader;
using DestroyerTest.Content.RiftBiome.RiftTundraResources;

namespace DestroyerTest.Common.Systems
{
    public class RiftSurfaceTileCount : ModSystem
    {
        public int RiftSurfaceBlockCount;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
            RiftSurfaceBlockCount =
                tileCounts[ModContent.TileType<Tile_RiftDirt>()] +
                tileCounts[ModContent.TileType<Tile_RiftStone>()];
        }
    }

    public class RiftDesertTileCount : ModSystem
    {
        public int RiftDesertBlockCount;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            RiftDesertBlockCount =
                tileCounts[ModContent.TileType<Tile_RiftSiltStone>()] +
                tileCounts[ModContent.TileType<Tile_HardenedRiftSilt>()] +
                tileCounts[ModContent.TileType<Tile_RiftSilt>()];
        }
    }
    
    public class RiftTundraTileCount : ModSystem
    {
        public int RiftTundraBlockCount;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
            RiftTundraBlockCount =
                tileCounts[ModContent.TileType<Tile_RiftSnow>()] +
                tileCounts[ModContent.TileType<Tile_RiftIce>()];
        }
    }
}
