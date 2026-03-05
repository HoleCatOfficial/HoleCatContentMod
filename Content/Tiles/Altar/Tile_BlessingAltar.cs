using DestroyerTest.Content.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace DestroyerTest.Content.Tiles.Altar
{
    public class Tile_BlessingAltar : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile = new TileObjectData(copyFrom: TileObjectData.Style1x1);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = [16, 16];
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);

            MinPick = 225;


            AddMapEntry(new Color(100, 100, 100), Language.GetText("Altar"));
        }

        public override bool RightClick(int i, int j)
        {
            var UIsys = ModContent.GetInstance<PrayerUISystem>();
            var UI = ModContent.GetInstance<PrayerUI>();
            if (!Main.hardMode)
            {
                UIsys.Open();
            }
            return true;
        }
    }
}
