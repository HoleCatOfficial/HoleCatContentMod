
using DestroyerTest.Common;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using System.Collections.Generic;
using DestroyerTest.Content.Buffs;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using Terraria.Audio;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
	public class DarkUnityThrown : ModProjectile
	{
		private NPC HomingTarget {
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set {
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		public ref float DelayTimer => ref Projectile.ai[1];

		
		public override void SetStaticDefaults() {
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
			ProjectileID.Sets.TrailingMode[Type] = 3;
			ProjectileID.Sets.TrailCacheLength[Type] = 12;
		}

		public override void SetDefaults()
		{
			Projectile.width = 110;
			Projectile.height = 110;

			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = false;
            Projectile.penetrate = 1;
		}
		public int trailLength = 10;
		public override bool PreDraw(ref Color lightColor)
		{
			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
			DTUtils Utility = new DTUtils();

			Main.EntitySpriteDraw(
				projectileTexture,
				Projectile.Center - Main.screenPosition,
				null,
				Color.White,
				Projectile.rotation,
				projectileTexture.Size() / 2,
				Projectile.scale,
				SpriteEffects.None,
				0
			);

			return false;
		}

        public float ShineScale = 0f;
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			Projectile.rotation += (Projectile.velocity.Length() * 0.5f) * Projectile.direction;

			Projectile.velocity *= 0.98f;

            Projectile.ai[1]++;

			if (Projectile.ai[1] > 60)
			{
				Projectile.Center += Main.rand.NextVector2Circular(3, 3);
			}
		}

        public override void OnKill(int timeLeft)
        {
			Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), Projectile.Center, Vector2.Zero, ColorLib.TenebrisGradient, 0.05f, 2f);
			SoundEngine.PlaySound(DTAssetLib.Impacts.StellarFox with { PitchVariance = 0.4f, MaxInstances = 0 }, Projectile.Center);
			Opus.RadialSpreadProjectile(ModContent.ProjectileType<TenebrisStarFriendly>(), 12, Projectile.Center, Projectile.damage / 8, 4, 8, offset: Projectile.rotation);
        }
    }
	
}