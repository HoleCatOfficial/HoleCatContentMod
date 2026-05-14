using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace DestroyerTest.Content.Tiles.RoseGarden
{
    public class Tile_TallBookcase : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileObjectData.newTile = new TileObjectData(copyFrom: TileObjectData.Style1x1); // Create a new instance
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 12;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16];
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidWithTop | AnchorType.Table, TileObjectData.newTile.Width, 0);
            // Additional edits here, such as lava immunity, alternate placements, and subtiles
            TileObjectData.addTile(Type);


            AddMapEntry(new Color(100, 100, 100), Language.GetText("Bookshelf"));

            DustType = DustID.Ebonwood;
        }
    }
}
