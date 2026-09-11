using System.IO;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Scepter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter.DiscordScepter
{
	public class NebulaFlame : ModProjectile, IHomingProjectile
	{
		public float DelayTimer;

		public override void SetStaticDefaults() 
		{
            Main.projFrames[Projectile.type] = 4;
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 240;
			Projectile.tileCollide = false;
		}

        private void AnimateProjectile()
		{
        
            if (++Projectile.frameCounter >= 2) {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type]) {
                    Projectile.frame = 0;
                }
            }
        }

		public override bool PreDraw(ref Color lightColor)
		{
			Opus.DrawTextureOnProj(DTAssetLib.PointGlowPreMultiplied, Projectile, ColorLib.Nebula with { A = 0 }, false, ScaleX: 2f, ScaleY: 2f);

			Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, ColorLib.Nebula with { A = 0 }) with { scale = new Vector2(1.2f, 1.2f) });

			Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White with { A = 0 }));
			return false;
        }

		bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 15f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

		float IHomingProjectile.HomingAccelAmount => 1.04f;

        float IHomingProjectile.HomingMaxAccel => 30f;

        float IHomingProjectile.DetectRadius => 400f;

        bool IHomingProjectile.CanHome => DelayTimer > 60;

        public override void AI() 
		{
			DelayTimer++;

            Lighting.AddLight(Projectile.Center, ColorLib.Nebula.ToVector3());

            if (Projectile.TryGetGlobalProjectile<HomingGlobal>(out var Homing) && Homing.TrackingNPC != null && Homing.TrackingNPC.whoAmI == -1)
            {
                Projectile.velocity *= 0.85f;
            }
            
			AnimateProjectile();
            Projectile.rotation = (Projectile.velocity.X) * 0.1f;


			if (Main.rand.NextBool(4))
			{
				PointGlowPreMultiplied glow = new();
				glow.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), -Projectile.velocity * 0.5f, ColorLib.Nebula, 1f);
				ParticleEngine.BehindProjectiles.Add(glow);
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[Projectile.owner];
			if (player.HeldItem.ModItem is CelestialDiscord CD && CD.Charge < 100)
			{
				CD.Charge++;
				CD.ChargeIncrementInterval = 60;
			}

			
		}
    }
}