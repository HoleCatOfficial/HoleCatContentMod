
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
using System.Collections.Generic;
using DestroyerTest.Content.Buffs;
 
using DestroyerTest.Content.Particles;
using Terraria.Audio;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
	public class ShimmeringMushroom : ModProjectile
	{
		private NPC HomingTarget {
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set {
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		public ref float DelayTimer => ref Projectile.ai[1];

		
		public override void SetStaticDefaults() {
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
			ProjectileID.Sets.TrailingMode[Type] = 3;
			ProjectileID.Sets.TrailCacheLength[Type] = 12;
            Main.projFrames[Type] = 3;
		}
        
        public int Variant;

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;

			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Variant = Main.rand.Next(3);
            Projectile.frame = Variant;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                projectileTexture.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(projectileTexture.Width / 2f, frameHeight / 2f);


			Main.EntitySpriteDraw(
                projectileTexture,
                Projectile.Center - Main.screenPosition,
                frame,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

			return false;
		}

        public Color EffectColor;
        public int projType = 0;

		public override void AI()
		{
            if (Variant == 0)
            {
                EffectColor = ColorLib.TenebrisBeige;
                projType = ModContent.ProjectileType<ShimmeringMushroomBeigeSmoke>();
            }
            if (Variant == 1)
            {
                EffectColor = ColorLib.TenebrisMagenta;
                projType = ModContent.ProjectileType<ShimmeringMushroomMagentaSmoke>();
            }
            if (Variant == 2)
            {
                EffectColor = ColorLib.TenebrisBlue;
                projType = ModContent.ProjectileType<ShimmeringMushroomBlueSmoke>();
            }
			Projectile.rotation += (Projectile.velocity.Length() * 0.05f) * Projectile.direction;
            Homing();
		}

        public void Homing()
		{
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
			{
				Projectile.velocity *= 0.94f;
                return;
			}

            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(30)).ToRotationVector2() * length;
			if (length < 10)
			{
				Projectile.velocity *= 1.1f;
			}
			Projectile.penetrate = 1;
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

        public void Explosion()
        {
            //Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BoomCloud>(), Projectile.Center, Vector2.Zero, EffectColor, 0.01f, 1.5f);
            
        }

        public override void OnKill(int timeLeft)
        {
			Explosion();
			SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Thud1") with { PitchVariance = 0.4f, MaxInstances = 0 }, Projectile.Center);
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionBoss/ConstitutionBossJab") with { PitchVariance = 0.4f, MaxInstances = 0 }, Projectile.Center);
			Opus.RadialSpreadProjectile(projType, 10, Projectile.Center, Projectile.damage / 4, 0, 2, offset: Projectile.rotation);
        }
    }
}