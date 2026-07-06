using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
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

        public override void AtFullExtension()
        {
            SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 12, ProjectileID.DD2PhoenixBowShot, 90, 5, Owner.whoAmI);
        }
    }
}