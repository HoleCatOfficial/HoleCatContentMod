using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static FargowiltasSouls.Core.Systems.DashManager;

namespace DestroyerTest.Content.Tiles.RoseGarden
{
    public class Tile_DecoCrate : ModTile
    {
        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileFrameImportant[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileTable[Type] = true;

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(Type);

            DustType = DustID.Ebonwood;

            HitSound = DTAssetLib.Impacts.WoodBreak;
           

            // Etc
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(60, 58, 72), name);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            //Common Coins
           
            Item.NewItem(Item.GetSource_None(), Utils.CenteredRectangle(new Point(i, j).ToWorldCoordinates(), new Vector2(2f, 2f)), ItemID.CopperCoin, Main.rand.Next(3, 17));
            Item.NewItem(Item.GetSource_None(), Utils.CenteredRectangle(new Point(i, j).ToWorldCoordinates(), new Vector2(2f, 2f)), ItemID.SilverCoin, Main.rand.Next(1, 9));

            if (Main.rand.NextBool(3))
            {
                Item.NewItem(Item.GetSource_None(), Utils.CenteredRectangle(new Point(i, j).ToWorldCoordinates(), new Vector2(2f, 2f)), ItemID.GoldCoin, Main.rand.Next(1, 3));
            }
        }
    }

 
}
