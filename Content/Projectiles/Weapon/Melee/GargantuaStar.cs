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
using OpusLib.Content.Particles;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
	public class GargantuaStar : ModProjectile
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

		public float DelayTimer;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;

			Projectile.DamageType = DamageClass.MeleeNoSpeed;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.light = 0.15f;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
		}

		public float trailOffset = 0;
		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = Color.Red;
			trailOffset += 0.04f;
			
			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();

            Opus.StartSpriteBatchForTrails(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

			
			if (TrailPositions.Count > 1)
			{
				List<ColoredVertex> ve = new List<ColoredVertex>();
				List<ColoredVertex> ve2 = new List<ColoredVertex>();
				float a = 0;

				for (int i = TrailPositions.Count - 1; i > 0; i--)
				{
					float t = 1f - (i / (float)TrailPositions.Count);
					Color b = lightColor * t;

					Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
					Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 16;
                    Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 16;

					DTUtils.AddStrips(ve, TrailPositions, i, offset, offset2, t, b, trailOffset);
				}

				for (int i = TrailPositions2.Count - 1; i > 0; i--)
				{
					float t = 1f - (i / (float)TrailPositions2.Count);
					Color b = Color.White * t;

					Vector2 dir = (TrailPositions2[i] - TrailPositions2[i - 1]).ToRotation().ToRotationVector2();
					Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 24;
                    Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 24;

					DTUtils.AddStrips(ve2, TrailPositions2, i, offset, offset2, t, b, trailOffset);
				}


				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
					gd.Textures[0] = DTAssetLib.Streak(2).Value;
					gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
					gd.Textures[0] = DTAssetLib.ZapTrail.Value;
					gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve2.ToArray(), 0, ve2.Count - 2);
				}
			}

			Opus.DrawGlowOnProj(Projectile, lightColor, true);

			Opus.ReturnToDefaultDrawing(spriteBatch);
			
			Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, Color.Red, true, 0f, 1f, 1f);

			return false;
		}

        public override bool? CanHitNPC(NPC target)
        {
			return DelayTimer >= 20;
        }


		public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
		private const int TrailLength = 400;

		public List<Vector2> TrailPositions2 = new();
		public List<float> TrailRotations2 = new();
		private const int TrailLength2 = 200;

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
					TrailPositions2.Insert(0, pos);
					TrailRotations2.Insert(0, Projectile.rotation);
				}
			}
			else
			{
				TrailPositions.Insert(0, newPos);
				TrailRotations.Insert(0, Projectile.rotation);
				TrailPositions2.Insert(0, newPos);
				TrailRotations2.Insert(0, Projectile.rotation);
			}

			while (TrailPositions.Count > TrailLength)
				TrailPositions.RemoveAt(TrailPositions.Count - 1);
			while (TrailRotations.Count > TrailLength)
				TrailRotations.RemoveAt(TrailRotations.Count - 1);
			while (TrailPositions2.Count > TrailLength2)
				TrailPositions2.RemoveAt(TrailPositions2.Count - 1);
			while (TrailRotations2.Count > TrailLength2)
				TrailRotations2.RemoveAt(TrailRotations2.Count - 1);

			DelayTimer++;
			Projectile.rotation += Projectile.direction * Main.rand.NextFloat(0.01f, 0.07f);
			Lighting.AddLight(Projectile.Center, Color.Red.ToVector3() * 0.2f);

			if (DelayTimer < 20)
			{
				return;
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
			Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(15)).ToRotationVector2() * Projectile.velocity.Length();

			float speed = Projectile.velocity.Length();
			float desiredSpeed = 35f;
			float acceleration = 0.3f;
			if (speed < desiredSpeed)
				speed += acceleration;
			Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;
			
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

		public Player FindClosestPlayer(float maxDetectDistance)
		{
			Player closestPlayer = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			foreach (var target in Main.player)
			{
				if (IsValidPlayer(target))
				{
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					if (sqrDistanceToTarget < sqrMaxDetectDistance)
					{
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestPlayer = target;
					}
				}
			}

			return closestPlayer;
		}

		public bool IsValidPlayer(Player target)
		{
			return target.active == true && target.statLife > 1;
		}
        public override void OnKill(int timeLeft)
        {
			BloomRingSharp Ring = new();
			Ring.Prepare(Projectile.Center, Vector2.Zero, Color.Red, 0.3f, 0.02f, 0.5f, BlendState.Additive);
			Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.FireworksRGB, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, Color.Red, 2f);
        }
    }
}