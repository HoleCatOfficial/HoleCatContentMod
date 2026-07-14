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
	public class Tile_RiftConfigurator : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileFrameImportant[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            AnimationFrameHeight = TileObjectData.newTile.CoordinateFullHeight;
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

        

        public override void AnimateTile(ref int frame, ref int frameCounter) {

            if (++frameCounter >= 8) {
                frameCounter = 0;
                if (++frame >= 25) {
                    frame = 0;
                }
            }
        }
	}
}