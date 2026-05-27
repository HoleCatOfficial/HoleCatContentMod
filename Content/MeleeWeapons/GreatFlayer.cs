
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UtfUnknown.Core.Models.SingleByte.Finnish;

namespace DestroyerTest.Content.MeleeWeapons
{

	public class GreatFlayer : ModItem
	{
        public override void SetStaticDefaults()
        {
			DTUtils.TooltipScaleMult[Type] = 1.1f;
			DTUtils.isSpecialSwingSword.Add(Type);
        }
		public override void SetDefaults() 
		{

			Item.width = 160;
			Item.height = 160;
			Item.value = Item.sellPrice(gold: 2, silver: 50);
			Item.rare = ModContent.RarityType<CrimsonSpecialRarity>();
            Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 17;
			Item.autoReuse = true;
			Item.damage = 1100;
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true;
			Item.noUseGraphic = true;

			Item.SetSpecialMeleeStats();

			Item.shoot = ModContent.ProjectileType<GreatFlayerProjectile>();
		}

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<GreatFlayerDash>()] < 1;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }


		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.altFunctionUse != 2)
			{
				Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer);
			
				return false;
			}

			if (player.altFunctionUse == 2)
				{
					Vector2 dashDir = Vector2.Normalize(Main.MouseWorld - player.Center);
					float dashSpeed = 15f;
					player.velocity = dashDir * dashSpeed;

					// Spawn projectile for visuals/arm handling
					Projectile.NewProjectile(
						player.GetSource_ItemUse(Item),
						player.Center,
						Vector2.Zero,
						ModContent.ProjectileType<GreatFlayerDash>(),
						damage,
						knockback,
						player.whoAmI);

					return false;
				}
			return false; 
		}

	
		public override bool MeleePrefix() {
			return true; 
		}

	}
}