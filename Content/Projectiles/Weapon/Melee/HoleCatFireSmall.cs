using System;
using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss;
using DestroyerTest.Content.Projectiles.Boss.VampireBoss;
using DestroyerTest.Content.RiftArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class HoleCatFireSmall : ModProjectile
    {
        private NPC HomingTarget {
				get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
				set {
					Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
				}
			}

        public ref float DelayTimer => ref Projectile.ai[1];
            
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ManualCanHitFriendly(target) && Projectile.HomingTimerCheck(40, (int)DelayTimer);
        }

        public override void PostDraw(Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            float progress = (float)(Projectile.timeLeft / 300);
            Color C = DTColorUtils.MultiLerp(progress, ColorLib.HoleCatFireColormap);
            DTUtils.DrawCrystalCore(spriteBatch, Projectile.Center, Color.White, C, TrailPositions, TextureRotationOffset, Projectile, TrailLength, 0.8f);
        }
        
        public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
        private const int TrailLength = 40;
        
        public float TextureRotationOffset = 0f;
        public override void AI()
        {
            TrailPositions.Insert(0, Projectile.Center);
            TrailRotations.Insert(0, Projectile.rotation);

            while (TrailPositions.Count > TrailLength)
                TrailPositions.RemoveAt(TrailPositions.Count - 1);
            while (TrailRotations.Count > TrailLength)
                TrailRotations.RemoveAt(TrailRotations.Count - 1);

            TextureRotationOffset -= 0.5f;
            Lighting.AddLight(Projectile.Center, ColorLib.HoleCatFireGradient.ToVector3());

            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TintableDustLighted, 0f, 0f, 0, ColorLib.HoleCatFireGradient, 2f);

            float maxDetectRadius = 1400f; // The maximum radius at which a projectile can detect a target

            if (DelayTimer < 40)
            {
                Projectile.velocity *= 0.95f;
                DelayTimer += 1;
                return;
            }

            // First, we find a homing target if we don't have one
            if (HomingTarget == null) {
                HomingTarget = FindClosestNPC(maxDetectRadius);
            }

            // If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
            if (HomingTarget != null && !IsValidTarget(HomingTarget)) {
                HomingTarget = null;
            }

            // If we don't have a target, don't adjust trajectory
            if (HomingTarget == null)
                return;

            // If found, we rotate the projectile velocity in the direction of the target.
            // We only rotate by 3 degrees an update to give it a smooth trajectory. Increase the rotation speed here to make tighter turns
            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(20)).ToRotationVector2() * length;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity *= 1.06f;
        }

        public NPC FindClosestNPC(float maxDetectDistance) {
				NPC closestNPC = null;

				// Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
				float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

				// Loop through all NPCs
				foreach (var target in Main.ActiveNPCs) {
					// Check if NPC able to be targeted. 
					if (IsValidTarget(target)) {
						// The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
						float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

						// Check if it is within the radius
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

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Inflate(20, 20);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HoleCatFire>(), 300);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<HoleCatFire>(), 300);
        }
        

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
            Vector2 Outer = Projectile.Center + Main.rand.NextVector2CircularEdge(200, 200);
            Vector2 Dir = Outer - Projectile.Center;
            for (int u = 0; u < 15; u++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.TintableDustLighted, Dir, 0, ColorLib.HoleCatFireGradient, 2);
            }
        }
    }
}