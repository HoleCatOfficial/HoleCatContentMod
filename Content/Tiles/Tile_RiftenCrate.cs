using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace DestroyerTest.Content.Tiles
{
	internal class Tile_RiftenCrate : ModTile
	{
		public override void SetStaticDefaults() {
			// Properties
			Main.tileFrameImportant[Type] = true;
			Main.tileSolidTop[Type] = true;
			Main.tileTable[Type] = true;

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.CoordinateHeights = [16, 18];
			TileObjectData.newTile.StyleHorizontal = true; // Optional, if you add more placeStyles for the item 
			TileObjectData.addTile(Type);

			// Etc
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(255, 155, 0), name);
		}

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = ModContent.DustType<RiftDust>();
            return true;
		}
	}
}