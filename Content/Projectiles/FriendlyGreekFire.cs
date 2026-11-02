using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class FriendlyGreekFire : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.GreekFire1);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;

            Projectile.friendly = true;
        }

        public int timer = 0;

        public override bool? CanHitNPC(NPC target)
        {
            return timer > 30 && !target.friendly;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            Projectile.rotation = 0f;
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch).noGravity = true;
            timer++;
        }
    }
}
