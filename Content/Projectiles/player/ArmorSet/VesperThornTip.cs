using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.ArmorSet
{
    public class VesperThornTip : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 32; // Width of the projectile hitbox
            Projectile.height = 32; // Height of the projectile hitbox
            Projectile.aiStyle = -1;
            Projectile.friendly = true; // Can damage enemies
            Projectile.hostile = false; // Does not damage players
            Projectile.penetrate = -1; // Infinite penetration
            Projectile.timeLeft = 140; // Lifetime of the projectile in ticks
            Projectile.ignoreWater = true; // Ignores water physics
            Projectile.tileCollide = false; // Does not collide with tiles
            Projectile.DamageType = DamageClass.Generic; // Set the damage type
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.timeLeft < 120)
            {
                Projectile.alpha += 2;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Bleeding, 300);
        }
    }
}