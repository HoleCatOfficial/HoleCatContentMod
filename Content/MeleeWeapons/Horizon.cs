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
using OpusLib;

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

			Item.shoot = ModContent.ProjectileType<GalantineLance>();
			Item.damage = 20;
			Item.shootSpeed = 10;
			Item.channel = true;
			Item.noUseGraphic = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			RingProjectileInward(type, 3, Main.MouseWorld, 500, damage, (int)knockback, 0.01f, 4f);
			
            return false;
        }

		public void RingProjectileInward(int ID, int Amount, Vector2 CTR, float Radius, int Dmg = 0, int KB = 0, float Speed = 2, float AI0 = 0f, float AI1 = 0f, float AI2 = 0f, bool RandomOffset = false)
		{
			float num = MathF.PI * 2f / (float)Amount;
			float num2 = (RandomOffset ? Main.rand.NextFloat(MathF.PI * 2f) : 0f);
			for (int i = 0; i < Amount; i++)
			{
				float num3 = num * (float)i + num2;
				Vector2 vector = CTR + new Vector2(Radius, 0f).RotatedBy(num3);
				Vector2 velocity1 = (CTR - vector).SafeNormalize(Vector2.Zero) * Speed;
				float rotation = velocity1.ToRotation();
				Projectile.NewProjectile(Entity.GetSource_FromThis(), vector, velocity1, ID, Dmg, KB, Main.myPlayer, AI0, rotation, AI2);
			}
		}
	}
}