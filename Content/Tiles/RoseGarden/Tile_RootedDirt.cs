using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.Audio;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Buffs;

namespace DestroyerTest.Content.Tiles.RoseGarden
{
	public class Tile_RootedDirt : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.ChecksForMerge[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(111, 86, 101), name);

			DustType = DustID.CorruptionThorns;
			HitSound = SoundID.DD2_MonkStaffGroundImpact;
			MineResist = 1.15f;
			MinPick = 35;
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			for (int f = 0; f < Main.maxPlayers; f++)
			{
				Player player = Main.player[f];
				if (player == null || !player.active)
					continue;

				Vector2 tileWorldPos = new Vector2(i * 16 + 8, j * 16 + 8); // center of tile
				float distance = Vector2.Distance(player.Center, tileWorldPos);

				if (distance < 700f && player.HasBuff(BuffID.Sunflower))
				{
					player.DelBuff(BuffID.Sunflower);
				}
			}
		}

		public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
		{
			makeDust = true;
			dustType = DustID.Dirt;
		}
	}
}