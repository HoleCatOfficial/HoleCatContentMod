using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class Hellfire : ModProjectile, IHomingProjectile
    {
        
        public float DelayTimer;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 7f;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 10f;

        float IHomingProjectile.DetectRadius => 700f;

        bool IHomingProjectile.CanHome => DelayTimer >= 30;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.

        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true; 
            Projectile.timeLeft = 180;
            Projectile.tileCollide = true;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Fire fire = new Fire();
            fire.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.3f, 0.3f), ColorLib.HellFire, 1f, 100, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(fire);

            Fire fire2 = new Fire();
            fire2.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.3f, 0.3f), ColorLib.HellFire, 0.5f, 100, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(fire2);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            DelayTimer++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 300);
        }
    }
}