using DestroyerTest.Content.MeleeWeapons;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace DestroyerTest.Content.Tiles.RoseGarden.Flowers
{
    public class Tile_HoleisBed : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile = new TileObjectData(copyFrom: TileObjectData.Style1x1);


            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(0, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.PlatformNonHammered | AnchorType.SolidTile | AnchorType.SolidWithTop, 5, 0);
            TileObjectData.addTile(Type);


            AddMapEntry(new Color(224, 35, 0), Language.GetText("Mods.DestroyerTest.Garden.HoleisBedMap"));

            DustType = DustID.LavaMoss;
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
