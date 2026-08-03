using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
 
using InnoVault.Trails;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
	public class FrostBurst : ModProjectile
	{
        public override string Texture => DTUtils.NoTexture;

		private NPC HomingTarget {
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set {
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		public ref float DelayTimer => ref Projectile.ai[1];

		public override void SetStaticDefaults() {
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10; 

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = false;
		}

		public float trailOffset = 0f;
		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = Color.SkyBlue;
			trailOffset -= 0.04f;


			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();

			Opus.StartSpriteBatchForTrails(spriteBatch, BlendState.NonPremultiplied, SpriteSortMode.Immediate);
			
			if (TrailPositions.Count > 1)
			{
				List<ColoredVertex> ve = new List<ColoredVertex>();
				float a = 0;

				for (int i = TrailPositions.Count - 1; i > 0; i--)
				{
					float u = i / (float)(TrailPositions.Count - 1);
					float widthFactor = (float)Math.Sin(u * MathHelper.Pi);

					float width = 32f * widthFactor;

					Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
					Vector2 perp = dir.RotatedBy(MathHelper.PiOver2);
					Vector2 offset  = perp * width;
					Vector2 offset2 = -perp * width;

					DTUtils.AddStrips(ve, TrailPositions, i, offset, offset2, u, lightColor, trailOffset);
				}

				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
					gd.Textures[0] = DTAssetLib.Streak(7).Value;
					gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
				}
			}

			Opus.ReturnToDefaultDrawing(spriteBatch);

			return false;
		}

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 20 && Projectile.ManualCanHitFriendly(target);
        }


		public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
		private const int TrailLength = 200;
		public override void AI()
		{
			Vector2 lastPos = TrailPositions.Count > 0 ? TrailPositions[0] : Projectile.Center;
			Vector2 newPos  = Projectile.Center;

			float dist = Vector2.Distance(lastPos, newPos);
			float step = 2f; // how closely to sample. tweak this!

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

			Lighting.AddLight(Projectile.Center, Color.SkyBlue.ToVector3() * 0.5f);

			PointGlowPreMultiplied Glow = new();
			Glow.Initialize(Projectile.Center, Vector2.Zero, Color.SkyBlue * 0.75f, 2f);
            ParticleEngine.BehindProjectiles.Add(Glow);

            if (DelayTimer < 20)
			{
				DelayTimer += 1;
				return;
			}
		
			float maxDetectRadius = 1200f;
			if (HomingTarget == null) {
				HomingTarget = FindClosestNPC(maxDetectRadius);
			}

			if (HomingTarget != null && !IsValidTarget(HomingTarget)) {
				HomingTarget = null;
			}

			if (HomingTarget == null)
				return;

			float length = Projectile.velocity.Length();
			float targetAngle = Projectile.AngleTo(HomingTarget.Center);
			Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(3)).ToRotationVector2() * length;
			Projectile.rotation = Projectile.velocity.ToRotation();
		}

		public NPC FindClosestNPC(float maxDetectDistance) {
			NPC closestNPC = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			foreach (var target in Main.ActiveNPCs) {
				if (IsValidTarget(target)) {
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					if (sqrDistanceToTarget < sqrMaxDetectDistance) {
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}

			return closestNPC;
		}

		public bool IsValidTarget(NPC target)
		{
			return target.CanBeChasedBy();
		}

		/*
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            foreach (var trail in new[] { TrailPositions})
            {
                for (int i = 1; i < trail.Count; i++)
                {
                    Vector2 point1 = trail[i - 1];
                    Vector2 point2 = trail[i];
                    if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), point1, point2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
		*/
		
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			target.AddBuff(ModContent.BuffType<HaepiensBlizzard>(), 600);

			
		}
	}
}