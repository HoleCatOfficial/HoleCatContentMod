
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.RogueItems;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
	public class RiftTeardrop_Thrown : ModProjectile, IHomingProjectile, IStickyProjectile
	{
        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 10f;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 0f;

        float IHomingProjectile.HomingMaxAccel => 1f;

        float IHomingProjectile.DetectRadius => 1400;

        bool IHomingProjectile.CanHome => Projectile.StealthStrike(Main.player[Projectile.owner]);

		bool sticking = false;

        bool IStickyProjectile.IsStickingToTarget { get => sticking; set => sticking = value; }

        bool IStickyProjectile.CanStickToTargets => true;

        bool IStickyProjectile.CanBeUnstuck => true;

        int IStickyProjectile.MaxStuckProjectiles => 8;

        bool IStickyProjectile.DealsDamageWhileStuck => true;

        NPC.HitInfo IStickyProjectile.StuckDamageInfo => new NPC.HitInfo() { Damage = 20, HitDirection = Projectile.direction };

        void IStickyProjectile.OnStickToTarget(NPC target)
        {
			SoundEngine.PlaySound(DTAssetLib.Impacts.ShortShine);
			Projectile.timeLeft = 240;
        }

        void IStickyProjectile.OnUnstick(NPC target, Projectile Replacing)
        {
			Projectile.velocity += new Vector2(0, -24);
            SoundEngine.PlaySound(DTAssetLib.Impacts.DarkShot);
        }

        void IStickyProjectile.DuringStick(NPC target)
        {
            
        }

        public override void SetStaticDefaults() 
		{

		}

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 96;
			Projectile.aiStyle = 0;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = ModContent.GetInstance<DTRogueClass>();
            Projectile.penetrate = -1;
			Projectile.timeLeft = 600;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 60;
		}


		public override void AI() 
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}

		
		



		public override void OnKill(int timeLeft) 
		{
			SoundEngine.PlaySound(new SoundStyle($"DestroyerTest/Assets/Audio/RiftMaker_Boom"), Projectile.position);
			Vector2 usePos = Projectile.position;
			Vector2 rotationVector = (Projectile.rotation - MathHelper.ToRadians(90f)).ToRotationVector2(); 
			usePos += rotationVector * 16f;
			for (int i = 0; i < 20; i++) 
			{
				Dust dust = Dust.NewDustDirect(usePos, Projectile.width, Projectile.height, DustID.Tin);
				dust.position = (dust.position + Projectile.Center) / 2f;
				dust.velocity += rotationVector * 2f;
				dust.velocity *= 0.5f;
				dust.noGravity = true;
				usePos -= rotationVector * 8f;
                if (i == 0) 
				{
                    Dust flash = Dust.NewDustDirect(usePos, Projectile.width, Projectile.height, ModContent.DustType<RiftDust>());
                    flash.position = Projectile.Center;
                    flash.velocity = Vector2.Zero;
                    flash.noGravity = true;
                    flash.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    flash.scale = 1.5f;
                }
			}
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) 
		{

			width = height = 10;
			return true;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) 
		{
			if (targetHitbox.Width > 8 && targetHitbox.Height > 8) 
			{
				targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
			}
			return projHitbox.Intersects(targetHitbox);
		}
    }
}