using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Ranged
{
    public class ForsakenMaelstromHomingFireball : ModProjectile, IHomingProjectile
    {
        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 5f;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 1f;

        float IHomingProjectile.DetectRadius => 1200f;

        bool IHomingProjectile.CanHome => Timer >= 60;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }

        float RAMT = 0f;
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            RAMT = Main.rand.NextFloat(-0.02f, 0.02f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, lightColor));
            return false;
        }

        public void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }


        int Timer = 0;
        public override void AI()
        {
            AnimateProjectile();
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;

            if (Timer < 60)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(RAMT);
            }

            

            Dust D = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch);
            D.noGravity = true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Timer >= 90 && Projectile.ManualCanHitFriendly(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            target.AddBuff(ModContent.BuffType<SpiritDrift>(), 600);


        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.Center);
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/DAHit") with { pitchVariance = 0.5f, MaxInstances = 0}, Projectile.Center);

            SimpleExplosionParticle Explosion1 = new();
            Explosion1.Prepare(Projectile.Center, Vector2.Zero, Color.MediumPurple, 0.1f, 0.02f, 1.6f, BlendState.Additive);
            ParticleEngine.Particles.Add(Explosion1);

            SimpleExplosionParticle Explosion2 = new();
            Explosion2.Prepare(Projectile.Center, Vector2.Zero, Color.DeepSkyBlue, 0.2f, 0.02f, 2f, BlendState.Additive);
            ParticleEngine.Particles.Add(Explosion2);
        }
    }
}
