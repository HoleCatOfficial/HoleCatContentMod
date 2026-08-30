using DestroyerTest.Content.MeleeWeapons;
using GlowmaskHelper.Content;
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
    [AutoloadGlowmask]
    public class Tile_ForeignTree : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile = new TileObjectData(copyFrom: TileObjectData.Style1x1);


            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Width = 11;
            TileObjectData.newTile.Height = 12;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16];
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(5, 11);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.PlatformNonHammered | AnchorType.SolidTile | AnchorType.SolidWithTop, 5, 0);
           
            TileObjectData.addTile(Type);


            AddMapEntry(new Color(110, 8, 149), Language.GetText("Mods.DestroyerTest.Garden.ForeignTreeMap"));

            DustType = DustID.SpookyWood;
            MinPick = 300;
            
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.47f * 0.5f;
            g = 0.01f * 0.5f;
            b = 0.70f * 0.5f;
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
