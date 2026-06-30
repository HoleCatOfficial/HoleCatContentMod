using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles
{
    public class Tile_DreamstoneBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileShine2[Type] = true; // Modifies the draw color slightly.
            Main.tileShine[Type] = 975; // How often tiny dust appear off this tile. Larger is less frequently
            DustType = ModContent.DustType<DreamstoneDust>();
            MineResist = 1f;
            MinPick = 15;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(119, 104, 86), name);
        }
    }
}