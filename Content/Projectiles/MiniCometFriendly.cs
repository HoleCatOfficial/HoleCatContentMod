using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;

namespace DestroyerTest.Content.Projectiles
{
	public class MiniCometFriendly : ModProjectile
	{
		private NPC NPCTarget
		{
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set
			{
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		public float DelayTimer;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 72;
			Projectile.height = 72;

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = false;
		}

        private Asset<Texture2D> ProjTex => ModContent.Request<Texture2D>(Texture);
		public override bool PreDraw(ref Color lightColor)
        {
            lightColor = ColorLib.StellarFireGradientLooping();
            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            for (int k = TrailPositions.Count - 1; k > 0; k--)
            {
                Vector2 drawPos = TrailPositions[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((TrailPositions.Count - k) / (float)TrailPositions.Count);
                Main.EntitySpriteDraw(
                    ProjTex.Value,
                    drawPos,
                    null,
                    color,
                    Projectile.rotation,
                    ProjTex.Size() / 2f,  // proper origin
                    Projectile.scale,
                    SpriteEffects.None,
                    0
                );
            }

            Opus.ReturnToDefaultDrawing(spriteBatch);

            Opus.DrawTextureOnProj(ProjTex, Projectile, Color.White, true, Projectile.rotation, 1f, 1f);

            return false;
        }



		public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
		private const int TrailLength = 40;
        public int HomingTime = 60;
		public override void AI()
        {
            TrailPositions.Insert(0, Projectile.Center);
            TrailRotations.Insert(0, Projectile.rotation);

            // Cap trail
            while (TrailPositions.Count > TrailLength)
                TrailPositions.RemoveAt(TrailPositions.Count - 1);
            while (TrailRotations.Count > TrailLength)
                TrailRotations.RemoveAt(TrailRotations.Count - 1);

            DelayTimer++;
            

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Lighting.AddLight(Projectile.Center,  ColorLib.StellarFireGradientLooping().ToVector3() * 0.2f);

            if (DelayTimer < 20)
            {
                DelayTimer += 1;
                return;
            }

            if (HomingTime > 0 && DelayTimer >= 20)
            {
                HomingTime--;
            }
            float maxDetectRadius = 2800f;


            if (NPCTarget == null)
            {
                NPCTarget = FindClosestNPC(maxDetectRadius);
            }


            if (NPCTarget != null && !IsValidNPC(NPCTarget))
            {
                NPCTarget = null;
            }


            if (NPCTarget == null)
                return;

            float targetAngle = Projectile.AngleTo(NPCTarget.Center);
            if (HomingTime > 0)
            {
                Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(15)).ToRotationVector2() * Projectile.velocity.Length();
            }

            // Acceleration
            float speed = Projectile.velocity.Length();
            float desiredSpeed = 20f; // your top speed
            float acceleration = 0.3f; // how quickly it ramps up
            if (HomingTime > 0)
            {
                if (speed < desiredSpeed)
                    speed += acceleration;
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;
            }
        
        }
		public NPC FindClosestNPC(float maxDetectDistance)
		{
			NPC closestNPC = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			foreach (var target in Main.ActiveNPCs)
			{
				if (IsValidNPC(target))
				{

					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					if (sqrDistanceToTarget < sqrMaxDetectDistance)
					{
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}

			return closestNPC;
		}

		public bool IsValidNPC(NPC target)
		{
			return target.CanBeChasedBy();
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(ModContent.BuffType<GalantineBurn>(), 300);
		}



        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10);
			Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, ModContent.DustType<ConstitutionDust1>(), Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0,  ColorLib.StellarFireGradientLooping(), 2f);
        }

    }
}