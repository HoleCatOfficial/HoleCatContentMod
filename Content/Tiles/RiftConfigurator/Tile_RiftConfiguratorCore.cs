using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace DestroyerTest.Content.Tiles.RiftConfigurator
{
	public class Tile_RiftConfiguratorCore : ModTile
	{

		// If you want to know more about tiles, please follow this link
		// https://github.com/tModLoader/tModLoader/wiki/Basic-Tile
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileSolidTop[Type] = false;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			// Additional edits here, such as lava immunity, alternate placements, and subtiles
			TileObjectData.addTile(Type);

			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(255, 155, 0), name);
			
			HitSound = new SoundStyle("DestroyerTest/Assets/Audio/TenebrousConstruct/Hit", 5)
            {
                PitchVariance = 0.2f,
                MaxInstances = 0
            };
            DustType = ModContent.DustType<RiftDust>();

		}
	}
}