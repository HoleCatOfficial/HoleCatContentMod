using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
 
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Common;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;  

namespace DestroyerTest.Content.Scepter
{
	public class BalanceScepter : ScepterItem
	{
		public override int Width => 54;
        public override int Height => 54;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

		public override void SetDefaults()
		{
			base.SetDefaults();
			ShootDMG = 90;
			ShootCrit = 2;
			ThrowCrit = 8;
			KB = 8;
			AdditiveValue = Item.sellPrice(silver: 80);
			Rarity = ModContent.RarityType<WineRarity>();

			ShootID = ModContent.ProjectileType<BalanceBolt>();
			ThrowID = ModContent.ProjectileType<BalanceScepterThrown>();

			ShootSound = new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/CursedFlamesWarn") { PitchVariance = 0.4f, MaxInstances = 0 };
			ThrowSound = SoundID.Item169;

			base.SetDefaults();
		}

        public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.SoulofLight, 16)
				.AddIngredient(ItemID.SoulofNight, 16)
				.AddIngredient(ItemID.IronBar, 8)
				.AddTile(TileID.Anvils)
				.Register();
		}
    }
} 