using System;
using System.Collections.Generic;
using System.Linq;
  
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{
	public class MalachiteKnives : ModItem
	{
		public override void SetDefaults() {
			
			Item.width = 32;
			Item.height = 28; 
			Item.rare = ModContent.RarityType<CorruptionSpecialRarity>();

		
			Item.useTime = 5;
			Item.useAnimation = 5;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.autoReuse = true;

            Item.UseSound = new SoundStyle("DestroyerTest/Assets/Audio/MCTrident", 2) with { PitchVariance = 0.5f, MaxInstances = 0 };

			Item.DamageType = DamageClass.Melee;
			Item.damage = 4;
			Item.knockBack = 1f;
			Item.noMelee = true; 
            Item.noUseGraphic = true;

			Item.shoot = ModContent.ProjectileType<MalachiteKnife>();
			Item.shootSpeed = 35f;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			float numberProjectiles = 3 + Main.rand.Next(4); // 3, 4, or 5 shots
			float rotation = MathHelper.ToRadians(20);

			position += Vector2.Normalize(velocity) * 45f;
			velocity *= 0.2f; // Slow the projectile down to 1/5th speed so we can see it. This is only here because this example shares ModItem.SetDefaults code with other examples. If you are making your own weapon just change Item.shootSpeed as normal.

			for (int i = 0; i < numberProjectiles; i++) {
				Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))); // Watch out for dividing by 0 if there is only 1 projectile.
				Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
			}

			return false; // return false to stop vanilla from calling Projectile.NewProjectile.
		}


	}
}