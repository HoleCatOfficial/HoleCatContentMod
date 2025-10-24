using DestroyerTest.Content.Projectiles;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace DestroyerTest.Content.MeleeWeapons
{
	public class HoleCatHook : ModItem
	{
		public int attackType = 0; // keeps track of which attack it is
		public int comboExpireTimer = 0; // we want the attack pattern to reset if the weapon is not used for certain period of time

		public override void SetDefaults()
        {
            Item.width = 200; // The item texture's width.
            Item.height = 174; // The item texture's height.

            Item.useStyle = ItemUseStyleID.Shoot; // The useStyle of the Item.
            Item.useTime = 20; // The time span of using the weapon. Remember in terraria, 60 frames is a second.
            Item.useAnimation = 20; // The time span of the using animation of the weapon, suggest setting it the same as useTime.
            Item.autoReuse = true; // Whether the weapon can be used more than once automatically by holding the use button.

            Item.DamageType = DamageClass.Melee; // Whether your item is part of the melee class.
            Item.damage = 1200; // The damage your item deals.
            Item.knockBack = 9f; // The force of knockback of the weapon. Maximum is 20
            Item.crit = 66; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.

            Item.value = Item.buyPrice(gold: 16); // The value of the weapon in copper coins.
            Item.rare = ModContent.RarityType<TestRarity>(); 
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<HoleCatHookSwing>();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player) {
			// Ensures no more than one spear can be thrown out, use this when using autoReuse
			return player.ownedProjectileCounts[Item.shoot] < 1;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.altFunctionUse != 2)
			{
				// Using the shoot function, we override the swing projectile to set ai[0] (which attack it is)
				Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, attackType);
				attackType = (attackType + 1) % 3; // Increment attackType to cycle through three attack types
				comboExpireTimer = 0; // Every time the weapon is used, we reset this so the combo does not expire
				return false;
			}

			if (player.altFunctionUse == 2)
            {
                    Vector2 jabVelocity = velocity.SafeNormalize(Vector2.Zero) * 3.7f;
					// Spawn projectile for visuals/arm handling
					Projectile.NewProjectile(
						player.GetSource_ItemUse(Item),
						player.Center,
						jabVelocity,
						ModContent.ProjectileType<HoleCatHookJab>(),
						damage,
						knockback,
						player.whoAmI);

					return false;
				}
			return false; // return false to prevent original projectile from being shot
		}

		public override void UpdateInventory(Player player) {
			if (comboExpireTimer++ >= 120) // after 120 ticks (== 2 seconds) in inventory, reset the attack pattern
				attackType = 0;
		}

		public override bool MeleePrefix() {
			return true; // return true to allow weapon to have melee prefixes (e.g. Legendary)
		}


		

		
	}
}
