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

namespace DestroyerTest.Content.Tiles.RoseGarden
{
    public class Tile_IdriPainting : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;


            TileObjectData.newTile = new TileObjectData(copyFrom: TileObjectData.Style3x3Wall);
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Width = 9;
            TileObjectData.newTile.Height = 11;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16];
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.addTile(Type);


            AddMapEntry(new Color(42, 80, 68), Language.GetText("Mods.DestroyerTest.Garden.IdriPainting"));
        }
    }
}
