using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles.Stellar;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss
{
    public class ConstitutionBeam : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public int Lifetime = 300;
		public int Time = 0;

		public bool StartKill = false;
		public void UpdateLerpTime()
		{
			Time++;

			if (Time > Lifetime)
			{
				StartKill = true;
			}
		}
		public float LifetimeCompletion
		{
			get
			{
				if (Lifetime <= 0)
				{
					return 0f;
				}

				return (float)Time / (float)Lifetime;
			}
		}
        public override void AI()
        {
            UpdateLerpTime();
            if (Main.rand.NextBool(4) && !StartKill)
            {
                ConstitutionParticle Particle = new();
                Particle.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity * 0.15f, 1f, 120);
                ParticleEngine.BehindProjectiles.Add(Particle);
            }	

            if (!StartKill)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(0.05f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            if (StartKill)
            {
                Projectile.damage = 0;
                Projectile.velocity *= 0.99f;
                Projectile.Opacity -= 0.05f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Color BeamColor = ColorLib.StellarFireGradient(LifetimeCompletion);
            lightColor = BeamColor * Projectile.Opacity;
            SpriteBatch SB = Main.spriteBatch;

            Opus.StartSpriteBatchWithBlending(SB, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(DTAssetLib.ConstitutionBeamGlow.Value, Projectile.Center, null, lightColor * 0.8f, Projectile.rotation, new Vector2(Projectile.width / 2, Projectile.height / 2), Projectile.scale * 0.5f, SpriteEffects.None, 0);
            Opus.ReturnToDefaultDrawing(SB);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(Projectile.width / 2, Projectile.height / 2), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}