using DestroyerTest.Content.Projectiles;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using DestroyerTest.Common;
using System;

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
			int Count = 8;
			for (int i = 0; i < Count; i++)
			{
				float angle = MathHelper.TwoPi * i / Count;
				Vector2 projPos = Main.MouseWorld + 80 * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
				Projectile.NewProjectile(source, projPos, Vector2.Zero, ModContent.ProjectileType<SoulOfNight_Projectile>(), damage, knockback);
			}
            return false;
        }


	}
}