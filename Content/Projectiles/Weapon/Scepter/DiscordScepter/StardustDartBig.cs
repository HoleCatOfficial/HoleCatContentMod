using System.Collections.Generic;
using System.IO;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter.DiscordScepter
{
	public class StardustDartBig : ModProjectile
	{
		// Store the target NPC using Projectile.ai[0]
		private NPC HomingTarget
		{
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set
			{
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		public ref float DelayTimer => ref Projectile.ai[1];

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
			ProjectileID.Sets.TrailingMode[Type] = 2;
			ProjectileID.Sets.TrailCacheLength[Type] = 12;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;

			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
		}

		public float trailOffset = 0f;
		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = new Color(0, 174, 238);
			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

			trailOffset += 0.01f;
            Opus.StartSpriteBatchForTrails(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
			
			if (TrailPositions.Count > 1)
			{
				List<ColoredVertex> ve = new List<ColoredVertex>();
				float a = 0;

				for (int i = TrailPositions.Count - 1; i > 0; i--)
				{
					float t = 1f - (i / (float)TrailPositions.Count); // fade toward tail
					Color b = lightColor * t;

					Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
					Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 20;
                    Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 20;

					DTUtils.AddStrips(ve, TrailPositions, i, offset, offset2, t, b, trailOffset);
				}


				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
                    gd.Textures[0] = DTAssetLib.Streak(2).Value;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2); 
				}
			}

			Opus.ReturnToDefaultDrawing(spriteBatch);

			Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));
			return false;
		}

		public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();
        private const int TrailLength = 150;
        private void CacheTrail()
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
        }

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 35;
        }


		public override void AI()
		{
			CacheTrail();
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			float maxDetectRadius = 800f; 

			if (DelayTimer < 35)
			{
				DelayTimer++;
				return;
			}

			if (HomingTarget == null)
			{
				HomingTarget = FindClosestNPC(maxDetectRadius);
			}

			if (HomingTarget != null && !IsValidTarget(HomingTarget))
			{
				HomingTarget = null;
			}

			if (HomingTarget == null)
				return;

			float length = Projectile.velocity.Length();
			float targetAngle = Projectile.AngleTo(HomingTarget.Center);
			Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(15)).ToRotationVector2() * length;
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.velocity *= 1.05f;
		}

		public NPC FindClosestNPC(float maxDetectDistance)
		{
			NPC closestNPC = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			foreach (var target in Main.ActiveNPCs)
			{
				if (IsValidTarget(target))
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

		public bool IsValidTarget(NPC target)
		{
			return target.CanBeChasedBy();
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			SoundEngine.PlaySound(DTAssetLib.Impacts.DarkMagicImpact, Projectile.Center);
			Opus.RadialSpreadProjectile(ModContent.ProjectileType<StardustDartSmall>(), 8, Projectile.Center, Projectile.damage / 2, 0, 4, RandomOffset: true);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			SoundEngine.PlaySound(DTAssetLib.Impacts.DarkMagicImpact, Projectile.Center);
			Opus.RadialSpreadProjectile(ModContent.ProjectileType<StardustDartSmall>(), 8, Projectile.Center, Projectile.damage / 2, 0, 4, RandomOffset: true);
			return true;
		}
		
    }
}