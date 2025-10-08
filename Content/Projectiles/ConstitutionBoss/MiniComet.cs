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

namespace DestroyerTest.Content.Projectiles.ConstitutionBoss
{
	/// <summary>
	/// Multipurpose Projectile.
	/// <para/> Projectile ai slots 0 and 1 should not be set to anything when spawning, as they store NPC and Player values respectively.
	/// <para/> Projectile ai slot 2 controls whether the projectile is friendly or harmful.
	/// </summary>
	public class MiniComet : ModProjectile
	{
		private NPC NPCTarget
		{
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set
			{
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		private Player PLRTarget
		{
			get => Projectile.ai[1] == 0 ? null : Main.player[(int)Projectile.ai[1] - 1];
			set
			{
				Projectile.ai[1] = value == null ? 0 : value.whoAmI + 1;
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
            lightColor = ColorLib.StellarColor;
            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();

            Utility.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            /*
            for (int i = 0; i < TrailPositions.Count - 1; i++)
            {
                Vector2 start = TrailPositions[i] - Main.screenPosition;
                Vector2 end = TrailPositions[i + 1] - Main.screenPosition;
                Vector2 diff = end - start;

                float length = diff.Length();
                if (length < 0.5f)
                    continue;

                float rotation = diff.ToRotation();
                float width = MathHelper.Lerp(0.01f, 0.0007f, i / (float)TrailLength);
                float alpha = MathHelper.Lerp(1f, 0f, i / (float)TrailLength);

                Main.spriteBatch.Draw(
                    DTAssetLib.Square.Value,
                    start,
                    null,
                    lightColor,
                    rotation,
                    new Vector2(DTAssetLib.Square.Value.Width / 2, DTAssetLib.Square.Value.Height / 2),
                    new Vector2(length, width),
                    SpriteEffects.None,
                    0f
                );
            }
            */

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


            //Utility.DrawGlowOnProj(Projectile, lightColor, true);

            Utility.ReturnToDefaultDrawing(spriteBatch);

            Utility.DrawTextureOnProj(ProjTex, Projectile, lightColor, true, Projectile.rotation, 1f, 1f);

            return false;
        }


		/// <summary>
		/// Controls whether the Projectile is Hostile or Friendly.
		/// <para/> 1 = Friendly, 2 = Hostile
		/// <para/> Attempting to return an invalid value will kill the projectile.
		/// </summary>
		public int Mode;

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
            Mode = (int)Projectile.ai[2];

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Mode > 4 || Mode <= 0)
            {
                Projectile.Kill();
                //throw new Exception("Non-Fatal Error in Oil Projectile Targeting. Value must be 1 or 2.");
                Mod.Logger.Warn("OilProjectile: Invalid Mode in ai[2]. Expected 1 or 2.");
            }

            Lighting.AddLight(Projectile.Center, ColorLib.StellarColor.ToVector3() * 0.2f);

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

            if (Mode == 1)
            {
                Projectile.friendly = true;
                Projectile.hostile = false;

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
            if (Mode == 2)
            {
                Projectile.friendly = false;
                Projectile.hostile = true;

                if (PLRTarget == null)
                {
                    PLRTarget = FindClosestPlayer(maxDetectRadius);
                }


                if (PLRTarget != null && !IsValidPlayer(PLRTarget))
                {
                    PLRTarget = null;
                }

                if (PLRTarget == null)
                    return;

                float targetAngle = Projectile.AngleTo(PLRTarget.Center);
                if (HomingTime > 0)
                {
                    Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(5)).ToRotationVector2() * Projectile.velocity.Length();
                }

                // Acceleration
                float speed = Projectile.velocity.Length();
                float desiredSpeed = 18f;
                float acceleration = 0.25f;
                if (HomingTime > 0)
                {
                    if (speed < desiredSpeed)
                        speed += acceleration;
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;
                }

            }
            if (Mode == 3)
            {
                Projectile.friendly = true;
                Projectile.hostile = false;
            }
            if (Mode == 4)
            {
                Projectile.friendly = false;
                Projectile.hostile = true;
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

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Mode == 1 || Mode == 3)
			{
				target.AddBuff(ModContent.BuffType<GalantineBurn>(), 300);
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			if (Mode == 2 || Mode == 4)
			{
				target.AddBuff(ModContent.BuffType<GalantineBurn>(), 300);
			}
		}

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10);
			Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.WhiteTorch, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.StellarColor, 2f);
        }

    }
}