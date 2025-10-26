using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Audio;

namespace DestroyerTest.Content.Tiles
{
	internal class Tile_HeliciteCrate : ModTile
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

			DustType = ModContent.DustType<HeliciteCrystalDust>();

			// Etc
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(255, 155, 0), name);
		}
		
		public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
			makeDust = true;
			dustType = ModContent.DustType<RiftDust>();
        }
	}
}