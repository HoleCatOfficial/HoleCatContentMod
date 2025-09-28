using System;
using DestroyerTest.Content.Entity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Common;

namespace DestroyerTest.Content.Resources
{
	public class ShimmeringShards : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 25;
		}

		public override void SetDefaults() {
			Item.width = 16;
			Item.height = 30;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 1000;
			Item.alpha = 0;
			Item.rare = ModContent.RarityType<ShimmeringRarity>();
		}

		public override void PostUpdate() {
			Lighting.AddLight(Item.Center, ColorLib.TenebrisGradient.ToVector3() * 0.55f * Main.essScale);
		}
	}
}
