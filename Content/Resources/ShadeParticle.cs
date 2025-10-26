using System;
using DestroyerTest.Content.Entities;
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
	public class ShadeParticle : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 15));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;
		}

		public override void SetDefaults() {
			Item.width = 10;
			Item.height = 12;
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
