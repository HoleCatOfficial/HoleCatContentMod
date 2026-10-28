using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
	public class HeliciteStarFriendly : ModProjectile, IHomingProjectile
	{
		public override string Texture => DTUtils.NoTexture;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 18f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

		float IHomingProjectile.HomingAccelAmount => 1.05f;

        float IHomingProjectile.HomingMaxAccel => 46;

        float IHomingProjectile.DetectRadius => 1200;

        bool IHomingProjectile.CanHome => DelayTimer > 10 && DelayTimer < 180;

        public float DelayTimer;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
			ProjectileID.Sets.TrailCacheLength[Type] = 90;
			ProjectileID.Sets.TrailingMode[Type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = false;
		}

		public float trailOffset = 0f;
		public override bool PreDraw(ref Color lightColor)
		{
			trailOffset += 0.04f;
			SpriteBatch spriteBatch = Main.spriteBatch;
            Color color = OpusColorUtils.MultiLerp(Opus.Sine(1f, 0f), ColorLib.HeliciteColorMap);

			DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(8, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 15, color, trailOffset);

			Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

			Opus.DrawGlowOnProj(Projectile, color, true);

			Opus.ReturnToDefaultDrawing(spriteBatch);
			
			Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, color with { A = 0  }, true, 0f, 0.9f, 0.9f);
            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, Color.White with { A = 0 }, true, 0f, 0.7f, 0.7f);

            return false;
		}
        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 10 && Projectile.ManualCanHitFriendly(target);
        }

		public override void AI()
		{
			Projectile.ResetExcessTrailPoints();
			DelayTimer++;
			Projectile.rotation += Projectile.direction * Main.rand.NextFloat(0.01f, 0.07f);
            Color color = OpusColorUtils.MultiLerp(Opus.Sine(1f, 0f), ColorLib.HeliciteColorMap);
            
			Lighting.AddLight(Projectile.Center, color.ToVector3() * 0.2f);

			if (DelayTimer < 20 || DelayTimer > 180)
			{
				return;
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(ModContent.BuffType<DaylightOverload>(), 300);
		}

        public override void OnKill(int timeLeft)
        {
            PixelParticle Pixel = new();
            Pixel.Initialize(Projectile.Center, Main.rand.NextVector2Circular(2f, 2f), ColorLib.HeliciteColorMap[Main.rand.Next(ColorLib.HeliciteColorMap.Length)], 2f);
            ParticleEngine.Particles.Add(Pixel);

            PointGlowPreMultiplied Glow = new();
            Glow.Initialize(Projectile.Center, Main.rand.NextVector2Circular(2f, 2f), ColorLib.HeliciteColorMap[Main.rand.Next(ColorLib.HeliciteColorMap.Length)] * 0.2f, 2f);
            ParticleEngine.Particles.Add(Glow);
        }

    }
}