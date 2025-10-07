using DestroyerTest.Content.Projectiles;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using DestroyerTest.Common;
using System;
using DestroyerTest.Content.Projectiles.ConstitutionBoss;

namespace DestroyerTest.Content.MeleeWeapons
{

	public class Horizon : ModItem
	{
		public override void SetDefaults()
		{
			Item.height = 39;
			Item.width = 39;
			Item.useTime = 80;
			Item.useAnimation = 80;
			Item.useStyle = ItemUseStyleID.Shoot;

			Item.shoot = ProjectileID.PurificationPowder;
			Item.damage = 20;
			Item.shootSpeed = 10;
			Item.channel = true;
			Item.noUseGraphic = true;
		}

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] < 1;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			
			Projectile.NewProjectile(source, Main.MouseWorld, new Vector2(0.001f, 0), ModContent.ProjectileType<ConstitutionStar>(), damage, knockback, ai2: 1);
			
            return false;
        }


	}
}