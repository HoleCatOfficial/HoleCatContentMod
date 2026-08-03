using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.AmmoProjectiles
{
    public class TenebrisSniperBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CanHitPastShimmer[Type] = true;
            ProjectileID.Sets.WindPhysicsImmunity[Type] = true;
            Main.projFrames[Type] = 3;
        }

        float Mode
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 180;
            Projectile.penetrate = 3;
            Projectile.frame = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(1, 4);
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

        void shine()
        {
            SmallShine Shine = new SmallShine();
            Shine.Prepare(Projectile.Center, Vector2.Zero, ColorLib.TenebrisGradient, 1f);
            ParticleEngine.BehindProjectiles.Add(Shine);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            shine();
            if (Mode == 1)
            {
                target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 240);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            shine();
            if (Mode == 2)
            {
                target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 240);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Research, Projectile.Center);
            shine();

            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<TenebrisDarkmatterDust>());
                dust.noGravity = true;
                dust.velocity *= 1.5f;
                dust.scale *= 0.9f;
            }
        }
    }
}