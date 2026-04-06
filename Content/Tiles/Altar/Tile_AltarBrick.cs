using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace DestroyerTest.Content.Tiles.Altar
{
    public class Tile_AltarBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
            
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            MineResist = 1f;
            AddMapEntry(new Color(47, 57, 81), Language.GetText("Altar Brick"));
            HitSound = DTAssetLib.TileMine.AltarBrick;

            //TileObjectData.addTile(Type);
        }

        public override bool Slope(int i, int j)
        {
            return false;
        }
    }
}