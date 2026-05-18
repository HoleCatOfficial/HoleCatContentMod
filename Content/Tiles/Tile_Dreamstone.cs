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
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using BreadLibrary.Core.Graphics.Particles;

namespace DestroyerTest.Content.Tiles
{
    public class Tile_Dreamstone : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.Ore[Type] = true;
            TileID.Sets.FriendlyFairyCanLureTo[Type] = true;
            Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
            Main.tileOreFinderPriority[Type] = 100; // Metal Detector value, see https://terraria.wiki.gg/wiki/Metal_Detector
            Main.tileShine2[Type] = true; // Modifies the draw color slightly.
            Main.tileShine[Type] = 600; // How often tiny dust appear off this tile. Larger is less frequently
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlockOverride[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(119, 104, 86), name);

            DustType = ModContent.DustType<VesperOreDust>();

            VanillaFallbackOnModDeletion = TileID.Iron;
            HitSound = SoundID.Tink;
            MineResist = 1f;
            MinPick = 6;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {


        }

    }
}