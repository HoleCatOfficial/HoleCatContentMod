using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.AmmoProjectiles
{
    public class TenebrousBullet1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CanHitPastShimmer[Type] = true;
            ProjectileID.Sets.WindPhysicsImmunity[Type] = true;
        }

        float Mode
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 180;
            Projectile.penetrate = 3;
        }

        public override void AI()
        {
            if (Mode > 2)
            {
                Mode = 2;
            }
            if (Mode < 1)
            {
                Mode = 1;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Mode == 1)
            {
                target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 240);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Mode == 2)
            {
                target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 240);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Research, Projectile.Center);

            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<TenebrisDarkmatterDust>());
                dust.noGravity = true;
                dust.velocity *= 1.5f;
                dust.scale *= 0.9f;
            }
        }
    }

    public class TenebrousBullet2 : TenebrousBullet1
    {

    }

    public class TenebrousBullet3 : TenebrousBullet1
    {

    }
}