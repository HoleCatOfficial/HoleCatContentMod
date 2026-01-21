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
using DestroyerTest.Content.Resources;

namespace DestroyerTest.Content.Scepter
{
	public class DarkUnity : ScepterItem
	{
		public override int Width => 110;
        public override int Height => 110;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

		public override void SetDefaults()
		{
			// First let the base class handle core setup
			base.SetDefaults();

			// Override stats unique to this scepter
			ShootDMG = 29;
			ShootCrit = 2;
			ThrowCrit = 8;
			KB = 8;
			AdditiveValue = Item.sellPrice(silver: 80);
			Rarity = ModContent.RarityType<CerisePinkRarity>();

			// Assign projectile types
			ShootID = ModContent.ProjectileType<ShimmeringMushroom>();
			ThrowID = ModContent.ProjectileType<DarkUnityThrown>();

			// Optional: change sounds
			ShootSound = SoundID.Item25;
			ThrowSound = SoundID.Item169;

			// Refresh defaults after overriding values
			base.SetDefaults();
		}
	

        public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<FungalScepter>(1)
                .AddIngredient<HolyScepter>(1)
                .AddIngredient<Tenebris>(16)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
    }
} 