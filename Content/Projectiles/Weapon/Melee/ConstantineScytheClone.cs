using DestroyerTest.Common;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class ConstantineScytheClone : ModProjectile
    {
        private int soundCooldown = 0; // Initialize a cooldown timer

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
        }
        
        private NPC HomingTarget {
				get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
				set {
					Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
				}
			}

		public ref float DelayTimer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.width = 94;
            Projectile.height = 102;
            Projectile.friendly = true;
            Projectile.penetrate = 1; // Infinite pierce
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600; // 10 seconds max lifespan
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.Opacity = 0f;
        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(soundCooldown);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            soundCooldown = reader.ReadInt32();
        }

        
		public override bool PreDraw(ref Color lightColor) {
			// Draws an afterimage trail. See https://github.com/tModLoader/tModLoader/wiki/Basic-Projectile#afterimage-trail for more information.

			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);

            SpriteEffects Fx = SpriteEffects.None;

            if (Projectile.direction < 0)
            {
                Fx = SpriteEffects.FlipHorizontally;
            }
            else
            {
                Fx = SpriteEffects.None;
            }
			for (int k = Projectile.oldPos.Length - 1; k > 0; k--) {
				Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, Fx, 0);
			}

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, Fx, 0);
            return false;
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.Opacity < 1f)
            {
                Projectile.Opacity += 0.05f;
            }

            /*
            if (Main.rand.NextBool(5))
            {
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionBoss/ConstitutionBossHit"), Projectile.Center);
                Opus.RadialProjectileRandomDir(ModContent.ProjectileType<ConstantineScytheNeedle>(), 2, Projectile.Center, (int)(Projectile.damage * 0.75f), 2, 6);
            }
            */
            
			Projectile.rotation += 0.2f * Projectile.direction;

            float maxDetectRadius = 700f;
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
			Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(30)).ToRotationVector2() * length;
            if (length < 20)
            {
                Projectile.velocity *= 1.06f;
            }

            

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

        






        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            

        }

        
    }
}

