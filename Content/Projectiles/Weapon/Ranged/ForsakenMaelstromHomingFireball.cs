using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using Microsoft.Xna.Framework;
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

        float IHomingProjectile.HomingTurnSpeed => 9f;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 1f;

        float IHomingProjectile.DetectRadius => 1200f;

        bool IHomingProjectile.CanHome => Timer >= 90;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }

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
            Timer++;

            Projectile.rotation = Projectile.velocity.ToRotation();

            Dust D = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch);
            D.noGravity = true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Timer >= 90 && Projectile.ManualCanHitFriendly(target);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.Center);
        }
    }
}
