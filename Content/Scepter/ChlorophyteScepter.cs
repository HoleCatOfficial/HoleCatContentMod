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
	public class ChlorophyteScepter : ScepterItem
	{
		public override int Width => 54;
        public override int Height => 54;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

		public override void SetDefaults()
		{
			// First let the base class handle core setup
			base.SetDefaults();

			// Override stats unique to this scepter
			ShootDMG = 90;
			ShootCrit = 2;
			ThrowCrit = 8;
			KB = 8;
			AdditiveValue = Item.sellPrice(silver: 80);
			Rarity = ModContent.RarityType<CerisePinkRarity>();

			// Assign projectile types
			ShootID = ProjectileID.BladeOfGrass;
			ThrowID = ModContent.ProjectileType<ChlorophyteScepterThrown>();

			// Optional: change sounds
			ShootSound = SoundID.Item25;
			ThrowSound = SoundID.Item169;

			// Refresh defaults after overriding values
			base.SetDefaults();
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            if (player.altFunctionUse != 2)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<NatureShot>(), 60, 3.2f, player.whoAmI);
            }
			return true; 
		}

        public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.ChlorophyteBar, 22)
				.AddIngredient(ItemID.SporeSac)
				.AddIngredient<NatureScepter>(1)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
    }
} 