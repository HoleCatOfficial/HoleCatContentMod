using DestroyerTest.Content.Projectiles.ParentClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class TestSpearProjectile : BaseSpearProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            MinExtension = 0.6f;
            MaxExtension = 50f;

            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 40;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            JabSound = SoundID.Item71;
        }
    }
}
