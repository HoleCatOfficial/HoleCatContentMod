using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles.Riftplate;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Microsoft.Xna.Framework;
using Terraria.WorldBuilding;
using Terraria.GameContent;
using Terraria.Audio;

namespace DestroyerTest.Content.Tiles.RoseGarden
{
	public class Tile_GardenUndergroundLock : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileShine2[Type] = true; // Modifies the draw color slightly.
			Main.tileShine[Type] = 1000; // How often tiny dust appear off this tile. Larger is less frequently

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.UsesCustomCanPlace = true;

            // Horizontal multi-tile
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 1;

            // Origin in the center tile
            TileObjectData.newTile.Origin = new Point16(1, 0);

            // Frame coordinates
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = new[] { 16 };
            TileObjectData.newTile.CoordinatePadding = 2;

            // Anchored to solid tiles on both sides
            TileObjectData.newTile.AnchorLeft  = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.newTile.AnchorRight = new AnchorData(AnchorType.SolidTile, 1, 0);

            // Custom placement is needed because it’s multi-tile
            TileObjectData.newTile.UsesCustomCanPlace = true;

            TileObjectData.addTile(Type);


			DustType = DustID.Demonite;
			HitSound = SoundID.Item50;
			MineResist = 16000f;
			MinPick = 255;

			AddMapEntry(new Color(120, 99, 197));
		}

        public override bool RightClick(int i, int j)
        {
            Player player = Main.player[Main.myPlayer];
            Tile tileIndex = Main.tile[i, j];
            if (player.HeldItem.type == ItemID.GoldenKey)
            {
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/HopeScabbardOpen"));
                KillMultiTile(i, j, tileIndex.TileFrameX, tileIndex.TileFrameY);
                return true;
            }
            return false;
        }

	}
}