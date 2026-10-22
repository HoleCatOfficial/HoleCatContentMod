using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles.RoseGarden
{
    public class Tile_SanctifiedBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            Main.tileBlockLight[Type] = true;
            DustType = ModContent.DustType<SanctifiedBrickDust>();
            MineResist = 1.7f;
            MinPick = 200;

            HitSound = SoundID.NPCHit2 with { Pitch = -2f, PitchVariance = 0.2f };

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(79, 47, 81), name);
        }
    }
}
