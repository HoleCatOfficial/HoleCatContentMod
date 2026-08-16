using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Weapon.Rogue.StealthStrike;
using DestroyerTest.Content.RogueItems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
	public class Chroma_Projectile : ModProjectile
	{
        public override void SetStaticDefaults() 
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 100;
			ProjectileID.Sets.TrailingMode[Type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 48;
			Projectile.height = 48;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Throwing; 
            Projectile.penetrate = 3;
			Projectile.timeLeft = 600;
			Projectile.light = 0.5f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.ArmorPenetration = 10;
		}

		float offset = 0f;

		float ShakeMag = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
			offset += 0.02f;
			DTTrail.DrawTrail(Main.spriteBatch, DTAssetLib.Streak(2).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 16, ColorLib.CelestialGradient, offset, 0);

			Asset<Texture2D> Tex = TextureAssets.Projectile[Type];
            SpriteEffects FX = Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            float rotationoffset = Projectile.direction < 0 ? MathHelper.PiOver2 : 0f;

			if (Dying)
			{
				ShakeMag += 0.06f;
			}

			Vector2 Pos = Projectile.Center + Main.rand.NextVector2Circular(ShakeMag, ShakeMag);

            Main.EntitySpriteDraw(Tex.Value, Pos - Main.screenPosition, null, Color.White, Projectile.rotation + rotationoffset, Tex.Value.Size() / 2, Projectile.scale, FX, 0f);

            return false;
        }

		public float GlowOpacity = 0f;
        public override void PostDraw(Color lightColor)
        {
			Asset<Texture2D> Glow = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/ChromaFade");

			SpriteEffects FX = Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			float rotationoffset = Projectile.direction < 0 ? MathHelper.PiOver2 : 0f;
            Vector2 Pos = Projectile.Center + Main.rand.NextVector2Circular(ShakeMag, ShakeMag);

            Main.EntitySpriteDraw(Glow.Value, Pos  - Main.screenPosition, null, Color.White * GlowOpacity, Projectile.rotation + rotationoffset, Glow.Value.Size() / 2, Projectile.scale, FX, 0f);
        }

		public bool Dying = false;
		public override void AI() 
		{
			//Just for Testing
			//StealthStrike = true;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                {
                    Projectile.oldPos[i] = Projectile.Center;
                }
            }
            
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
			Dust Trail = Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Vector2.Zero, 0, Color.White, 1f);
			Trail.noGravity = true;

			if (Dying)
			{
				Projectile.velocity *= 0.99f;
				if (GlowOpacity < 1f)
				{
					
					GlowOpacity += 0.01f;
				}
				else
				{
					Projectile.timeLeft = 1;
				}
			}

		}

        public override bool? CanHitNPC(NPC target)
        {
            if (Dying)
			{
				return false;
			}
			return null;
        }

		float pitch = -0.4f;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			SoundEngine.PlaySound(DTAssetLib.Impacts.FleshHit with { PitchVariance = 0.7f, Pitch = pitch }, Projectile.Center);

			pitch += 0.1f;

			Rectangle DustR = Utils.CenteredRectangle(Projectile.Center, new Vector2(70, 70));

			for (int t = 0; t < 5; t++)
			{
				Dust.NewDust(DustR.TopLeft(), DustR.Width, DustR.Height, DustID.FireworksRGB, -Projectile.velocity.X, -Projectile.velocity.Y, 0, Color.White, 2f);
			}

            var Dirs = Opus.RadialVectorOutwardRandom(2, Projectile.Center, Main.rand.NextFloat(2f, 5f));

            for (int i = 0; i < 2; i++)
            {
                StarParticle Star = new();
                Star.Initialize(Projectile.Center, Dirs[i], Color.White, 0.7f);
                ParticleEngine.BehindProjectiles.Add(Star);
            }

            if (Projectile.penetrate > 2)
			{
				Projectile.velocity *= 0.4f;
			}
			else
			{
                Dying = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Vector2[] oldCenters = Projectile.OldCenter();

            SoundEngine.PlaySound(DTAssetLib.Impacts.IceImpact, Projectile.Center);
            SoundEngine.PlaySound(DTAssetLib.Impacts.SpiritOfJusticeParry with { Pitch = 0.6f, PitchVariance = 0.3f }, Projectile.Center);

            Dust[] D1 = Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 3, Projectile.Center, 0, Color.White, 4, 2f);
			foreach (Dust D in D1)
			{
				D.noGravity = true;
			}

            Opus.RingSpreadDustRandom(DustID.FireworksRGB, 5, Projectile.Center, 10, 0, Color.White, 4, 1f);

            Opus.RingSpreadDustRandom(DustID.FireworksRGB, 9, Projectile.Center, 15, 0, Color.White, 3, 0.6f);

			/*
			for(int i = 0; i < oldCenters.Length; i++)
			{
				float Opac = MathHelper.Lerp(1f, 0f, i / (float)oldCenters.Length - 1);
				float scl = MathHelper.Lerp(2f, 0f, i / (float)oldCenters.Length - 1);
                Dust effect = Dust.NewDustPerfect(oldCenters[i], DustID.AncientLight, Vector2.Zero, 0, ColorLib.CelestialGradient * Opac, scl);
				effect.noGravity = true;
			}
			*/

            var Dirs = Opus.RadialVectorOutwardRandom(6, Projectile.Center, Main.rand.NextFloat(2f, 5f));

            for (int i = 0; i < 6; i++)
            {
                StarParticle Star = new();
                Star.Initialize(Projectile.Center, Dirs[i], Color.White, 0.7f);
                ParticleEngine.BehindProjectiles.Add(Star);
            }

			if (Dying && Projectile.StealthStrike(Main.player[Projectile.owner]))
			{
				Vector2 Vel = Projectile.oldVelocity;
				Vel.Normalize();

				Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vel.RotatedBy(0.2f) * 18, ModContent.ProjectileType<ChromaSolar>(), Projectile.damage / 3, 6, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vel.RotatedBy(0.05f) * 18, ModContent.ProjectileType<ChromaVortex>(), Projectile.damage / 3, 6, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vel.RotatedBy(-0.05f) * 18, ModContent.ProjectileType<ChromaNebula>(), Projectile.damage / 3, 6, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vel.RotatedBy(-0.2f) * 18, ModContent.ProjectileType<ChromaStardust>(), Projectile.damage / 3, 6, Projectile.owner);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
			/*
			Vector2 P1 = Projectile.Center + new Vector2(-Projectile.width / 2, Projectile.height / 2).RotatedBy(Projectile.rotation);
            Vector2 P2 = Projectile.Center + new Vector2(Projectile.width / 2, -Projectile.height / 2).RotatedBy(Projectile.rotation);
			Line L = new Line(P1, P2);
			return L.Collision(8, 1);
			*/

			return null;
        }
	}
}