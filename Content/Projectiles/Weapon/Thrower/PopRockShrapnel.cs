using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Thrower
{
    public class PopRockShrapnel : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.frame = Main.rand.Next(2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return true;
        }

        public float LifeTime => Projectile.ai[0];

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            //Dust trail = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<RiftDust>(), 0f, 0f, 0, Color.White, 1f);
            //trail.noGravity = true;

            if (LifeTime < 15)
            {

            }
            else
            {
                Projectile.velocity.Y += 0.2f;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return LifeTime >= 15 && Projectile.ManualCanHitFriendly(target);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
        }
    }
}
