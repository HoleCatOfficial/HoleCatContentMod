using System.IO;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter.DiscordScepter
{
public class NebulaFlame : ModProjectile
		{
			// Store the target NPC using Projectile.ai[0]
			public NPC HomingTarget {
				get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
				set {
					Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
				}
			}

			public ref float DelayTimer => ref Projectile.ai[1];

			public override void SetStaticDefaults() {
                Main.projFrames[Projectile.type] = 4; // Set the number of frames for the projectile
				ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.

			}

		public override void SetDefaults()
		{
			Projectile.width = 32; // The width of projectile hitbox
			Projectile.height = 32; // The height of projectile hitbox

			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 1f; // How much light emit around the projectile
			Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = false;
			Projectile.damage += 15;
			Projectile.netImportant = true;
			Projectile.netUpdate = true;
			}

            private void AnimateProjectile() {
            // Loop through the frames, assuming each frame lasts 5 ticks
            if (++Projectile.frameCounter >= 2) {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type]) {
                    Projectile.frame = 0;
                }
            }
            }

			

			public int killtimer = 120;

			// Custom AI
			public override void AI() {
                killtimer--;
				AnimateProjectile();
                Projectile.rotation = (Projectile.velocity.ToRotation() + MathHelper.PiOver2) * 0.1f;
				float maxDetectRadius = 400f; // The maximum radius at which a projectile can detect a target
				if (HomingTarget == null) {
					HomingTarget = FindClosestNPC(maxDetectRadius);
				}

				if (HomingTarget != null && !IsValidTarget(HomingTarget)) {
					HomingTarget = null;
				}

                if (HomingTarget != null && IsValidTarget(HomingTarget)) {
                    killtimer = 120;
                }

				if (HomingTarget == null)
					return;

                if (HomingTarget == null && killtimer <= 0) {
                    Projectile.Kill();
                }

				float length = Projectile.velocity.Length();
				float targetAngle = Projectile.AngleTo(HomingTarget.Center);
				Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(30)).ToRotationVector2() * length;
                Projectile.rotation = (Projectile.velocity.ToRotation() + MathHelper.PiOver2) * 0.2f;
				Projectile.velocity *= 1.05f;
                AnimateProjectile();
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

			public bool IsValidTarget(NPC target) {
				return target.CanBeChasedBy();
			}

		

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[Projectile.owner];
			player.AddBuff(BuffID.NebulaUpLife1, 300);
		}
    }
}