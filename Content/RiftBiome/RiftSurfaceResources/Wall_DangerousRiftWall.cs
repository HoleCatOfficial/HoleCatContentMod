﻿using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RiftBiome.RiftSurfaceResources
{
	public class Wall_DangerousRiftWall : ModWall
	{
		public override void SetStaticDefaults() {
			// As an example of an unsafe wall, "Main.wallHouse[Type] = true;" is omitted.

			DustType = ModContent.DustType<Sparkle>();

			AddMapEntry(new Color(150, 150, 150));

			// We need to manually register the item drop, since no item places this wall. This wall can only be obtained by using ExampleSolution on natural spider walls.
			RegisterItemDrop(ModContent.ItemType<Item_RiftWall>());
		}

		public override void NumDust(int i, int j, bool fail, ref int num) {
			num = fail ? 1 : 3;
		}
	}
}