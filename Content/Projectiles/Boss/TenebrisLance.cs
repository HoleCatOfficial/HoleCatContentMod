using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using System.Collections.Generic;
using OpusLib;

namespace DestroyerTest.Content.Projectiles.Boss
{
    public class TenebrisLance : ModProjectile
    {

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 50; // The width of projectile hitbox
            Projectile.height = 50; // The height of projectile hitbox
            Projectile.DamageType = DamageClass.Generic; // What type of damage does this projectile affect?
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.timeLeft = 120; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public float trailOffset = 0;
        public override bool PreDraw(ref Color lightColor)
		{
			lightColor = ColorLib.TenebrisGradient;
            trailOffset += 0.04f;
			
			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(3).Value, TrailPositions, TrailRotations, 40, ColorLib.TenebrisGradient, trailOffset);

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
        
			Opus.DrawGlowOnProj(Projectile, lightColor, true);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

			Opus.ReturnToDefaultDrawing(spriteBatch);

			return false;
		}

        public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
		private const int TrailLength = 400;

		public override void AI()
		{


			Vector2 lastPos = TrailPositions.Count > 0 ? TrailPositions[0] : Projectile.Center;
			Vector2 newPos  = Projectile.Center;

			float dist = Vector2.Distance(lastPos, newPos);
			float step = 1f; // how closely to sample. tweak this!

			if (dist > 0f)
			{
				int segments = (int)(dist / step);

				for (int i = 1; i <= segments; i++)
				{
					Vector2 pos = Vector2.Lerp(lastPos, newPos, i / (float)segments);
					TrailPositions.Insert(0, pos);
					TrailRotations.Insert(0, Projectile.rotation);
				}
			}
			else
			{
				TrailPositions.Insert(0, newPos);
				TrailRotations.Insert(0, Projectile.rotation);
			}


			// Cap trail
			while (TrailPositions.Count > TrailLength)
				TrailPositions.RemoveAt(TrailPositions.Count - 1);
			while (TrailRotations.Count > TrailLength)
				TrailRotations.RemoveAt(TrailRotations.Count - 1);

            if (Main.rand.NextBool(3))
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.TintableDustLighted, newColor: ColorLib.TenebrisGradient, Scale: 1.8f, Velocity: Vector2.Zero);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Vector2 FlankLeft = Projectile.velocity.RotatedBy(MathHelper.PiOver2);
            Vector2 FlankRight = Projectile.velocity.RotatedBy(-MathHelper.PiOver2);

            if (Main.GameUpdateCount % 10 == 0 && Projectile.velocity.Length() > 2)
            {
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SmallShine>(), Projectile.Center, Vector2.Zero, Color.White * 0.5f, 0.5f);
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/TenebrisBlock") with { MaxInstances = 0, PitchVariance = 0.3f, Volume = 0.15f }, Projectile.Center);
                Projectile.NewProjectile(Entity.GetSource_FromAI(), Projectile.Center, FlankLeft * 0.02f, ModContent.ProjectileType<TenebrisDart>(), Projectile.damage / 2, 3);
                Projectile.NewProjectile(Entity.GetSource_FromAI(), Projectile.Center, FlankRight * 0.02f, ModContent.ProjectileType<TenebrisDart>(), Projectile.damage / 2, 3);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 240);
        }

        public override void OnKill(int timeLeft)
        {
            Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.TintableDustLighted, Projectile.velocity.X * 0.7f, Projectile.velocity.Y * 0.7f, 0, ColorLib.TenebrisGradient, 1);
            int numProjectiles = 12;
            float rotationStep = MathHelper.TwoPi / numProjectiles;

            for (int i = 0; i < numProjectiles; i++)
            {
                Vector2 velocity = new Vector2(8f, 0f).RotatedBy(rotationStep * i);
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    velocity,
                    ModContent.ProjectileType<TenebrisDart>(),
                    Projectile.damage / 2,
                    Projectile.knockBack
                );
            }
        }

    }
}