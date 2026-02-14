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
using DestroyerTest.Content.Projectiles.ParentClasses;
using System.Collections.Generic;

namespace DestroyerTest.Content.Projectiles.Fargos
{
    [JITWhenModsEnabled(DTCrossMod.FargosSoulsName)]
	public class GaiaOrb : ModProjectile
	{
        private NPC HomingTarget {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }
        public ref float DelayTimer => ref Projectile.ai[1];
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            ScepterRegistry.AllScepterShots.Add(Projectile);
            Projectile.width = 24;
            Projectile.height = 24;

            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 0.1f;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
        }

        public float trailOffset = 0f;
		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = Color.White;
			trailOffset += 0.01f;

			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();

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
					Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 16;
                    Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 16;

					DTUtils.AddStrips(ve, TrailPositions, i, offset, offset2, t, b, trailOffset);
				}


				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
                    gd.Textures[0] = DTAssetLib.Streak(10).Value;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2); 
				}
			}

			Opus.DrawGlowOnProj(Projectile, lightColor, true);

			Opus.ReturnToDefaultDrawing(spriteBatch);

			Main.EntitySpriteDraw(DTAssetLib.FeatheredCircle.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, DTAssetLib.FeatheredCircle.Value.Size() / 2, new Vector2(0.5f, 1 + (0.1f * Projectile.velocity.Length())), SpriteEffects.None, 0);

			return false;
		}

        public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();
        private const int TrailLength = 200;
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
            return DelayTimer >= 20;
        }

        public override void AI() 
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            
            if(DelayTimer < 20)
            {
                DelayTimer++;
                return;
            }

            if (HomingTarget == null) {
                HomingTarget = FindClosestNPC(1200);
            }

            if (HomingTarget != null && !IsValidTarget(HomingTarget)) {
                HomingTarget = null;
            }

            if (HomingTarget == null)
                return;

            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            int turn = 20;
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(turn)).ToRotationVector2() * length;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity *= 1.08f;
            if (Main.GameUpdateCount % 3 == 0)
            {
                turn++;
            }

            Projectile.velocity.Clamp(30);
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
            if (Projectile.tileCollide == true)
            {
                return target.CanBeChasedBy() && Collision.CanHit(Projectile.Center, 1, 1, target.position, target.width, target.height);
            }
            else
            {
                return target.CanBeChasedBy();
            }
        }
    }
}