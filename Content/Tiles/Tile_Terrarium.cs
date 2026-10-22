using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Content.MeleeWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace DestroyerTest.Content.Tiles
{
    public class Tile_Terrarium : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile = new TileObjectData(copyFrom: TileObjectData.Style3x2);

            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.PlatformNonHammered | AnchorType.SolidTile | AnchorType.SolidWithTop, 7, 0);
            TileObjectData.addTile(Type);


            AddMapEntry(new Color(62, 121, 148), Language.GetText("Mods.DestroyerTest.Tiles.Terrarium"));

            DustType = DustID.WoodFurniture;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Tile t = Main.tile[i, j];
            int left = i - t.TileFrameX % (7 * 18) / 18;
            int top = j - t.TileFrameY % (3 * 18) / 18;
            Tile T = Main.tile[left, top];

            T.ClearTile();
        }
    }
}
