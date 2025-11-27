using System.Collections.Generic;
using System.Formats.Tar;
using System.Runtime.CompilerServices;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.CurseRunes;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class MajesticStormOrb : ModProjectile
    {
        public NPC HomingTarget {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

            int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                projectileTexture.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(projectileTexture.Width / 2f, frameHeight / 2f);

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            Main.EntitySpriteDraw(
                projectileTexture,
                Projectile.Center - Main.screenPosition,
                frame,
                Color.White,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

            Opus.ReturnToDefaultDrawing(spriteBatch);

            return false;
        }
        private void AnimateProjectile() {
            if (++Projectile.frameCounter >= 10) {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type]) {
                    Projectile.frame = 0;
                }
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            AnimateProjectile();
            Lighting.AddLight(Projectile.Center, new Color(29, 226, 186).ToVector3() * 0.6f);

            // --- CHANNELING LOGIC ---
            bool shouldDie = !player.channel;
            if (!shouldDie)
                Projectile.timeLeft = 120;

            // --- TARGETING LOGIC ---
            float maxDetectRadius = 220f;

            if (HomingTarget == null)
                HomingTarget = FindClosestNPC(maxDetectRadius);

            if (HomingTarget != null && !IsValidTarget(HomingTarget))
                HomingTarget = null;

            if (HomingTarget != null && Main.GameUpdateCount % 20 == 0)
            {
                Vector2 shootVel =
                    (Projectile.Center - HomingTarget.Center)
                    .ToRotation()
                    .ToRotationVector2() * 6f;

                Projectile.NewProjectile(
                    Projectile.GetSource_FromAI(),
                    Projectile.Center,
                    shootVel,
                    ModContent.ProjectileType<ElectricBolt>(),
                    Projectile.damage,
                    4,
                    Projectile.owner
                );
            }

            if (shouldDie)
                Projectile.Kill();
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

		public bool IsValidTarget(NPC target) {
			return target.CanBeChasedBy();
		}

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ChargeBreak") { PitchVariance = 0.2f, MaxInstances = 0 });
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MajesticStormExplosion>(), Projectile.damage, 12, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 20 * 60);
        }
	}
}