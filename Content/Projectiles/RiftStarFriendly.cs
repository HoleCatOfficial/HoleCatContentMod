using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
	public class RiftStarFriendly : ModProjectile, IHomingProjectile
	{
		public override string Texture => DTUtils.NoTexture;
		private NPC NPCTarget
		{
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set
			{
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 10f;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

		float IHomingProjectile.HomingMaxAccel => 20f;

        float IHomingProjectile.DetectRadius => 200f;

        bool IHomingProjectile.CanHome => DelayTimer > 20;

        public float DelayTimer;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
			ProjectileID.Sets.TrailCacheLength[Type] = 200;
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
			Projectile.timeLeft = 600;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
		}

		public float trailOffset = 0f;
		public override bool PreDraw(ref Color lightColor)
		{
			trailOffset += 0.04f;
			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();

			DTTrail.DrawTrail(spriteBatch, DTAssetLib.ZapTrail.Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 40f, ColorLib.Rift, trailOffset, Projectile.oldPos.Length);

			Opus.DrawGlowOnProj(Projectile, ColorLib.Rift with { A = 0 }, true);
			
			Opus.DrawTextureOnProj(DTAssetLib.RiftStar, Projectile, ColorLib.Rift, true, 0f, 0.9f, 0.9f);

            Opus.DrawTextureOnProj(DTAssetLib.RiftStar, Projectile, ColorLib.Rift with { A = 0 }, true, 0f, 0.9f, 0.9f);

            return false;
		}

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 20 && Projectile.ManualCanHitFriendly(target);
        }

		public override void AI()
		{
			Projectile.ResetExcessTrailPoints();

			DelayTimer++;
			Projectile.rotation += Projectile.direction * Main.rand.NextFloat(0.01f, 0.07f);

            if (Main.rand.NextBool(12))
            {
                ElectricArc Arc = new();
                Arc.Create(Projectile.Center + Main.rand.NextVector2Circular(10, 10), ColorLib.Rift, Main.rand.NextFloat(0.5f, 1f), 0.2f);
                ParticleEngine.ShaderParticles.Add(Arc);
            }

			Lighting.AddLight(Projectile.Center, ColorLib.Rift.ToVector3() * 0.2f);
			
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/RiftCharge") with { MaxInstances = 0, PitchVariance = 0.3f, Volume = 0.25f }, target.Center);
			target.AddBuff(ModContent.BuffType<HeliouricShock>(), 300);
		}
		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
			{
				Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, ModContent.DustType<RiftDust>(), Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, default, 2f);
			}
		}
    }
}