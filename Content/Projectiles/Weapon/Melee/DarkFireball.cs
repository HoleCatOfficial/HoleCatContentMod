using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class DarkFireball : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = 5;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 5f)
            {
                Projectile.ai[0] = 5f;
                Projectile.velocity.Y += 0.1f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }

            int[] types = new int[]
            {
                PRTLoader.GetParticleID<ColoredFire1>(),
                PRTLoader.GetParticleID<ColoredFire2>(),
                PRTLoader.GetParticleID<ColoredFire3>(),
                PRTLoader.GetParticleID<ColoredFire4>(),
                PRTLoader.GetParticleID<ColoredFire5>(),
                PRTLoader.GetParticleID<ColoredFire6>(),
                PRTLoader.GetParticleID<ColoredFire7>()
            };

            PRTLoader.NewParticle(types[Main.rand.Next(types.Length)], Projectile.Center, Vector2.Zero, new Color(199, 66, 199), 0.75f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.ai[0] += 0.1f;
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
                Projectile.velocity *= 0.75f;
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            }
            return false;
        }
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            target.AddBuff(BuffID.ShadowFlame, 120);
		}
	}
}