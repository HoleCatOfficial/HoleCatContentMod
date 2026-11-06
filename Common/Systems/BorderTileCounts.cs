using System;
using DestroyerTest.Content.RiftBiome.RiftDesertResources;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using DestroyerTest.Content.RiftBiome.RiftTundraResources;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Common.Systems
{
    public class BorderTileCounts : ModSystem
    {
        public int BlockCount;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            BlockCount = tileCounts[TileID.Titanstone];
              
        }
    }
}
