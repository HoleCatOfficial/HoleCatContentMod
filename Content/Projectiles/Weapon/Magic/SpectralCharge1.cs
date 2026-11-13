using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Stellar;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class SpectralCharge1 : ModProjectile, IHomingProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 0.5f;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 10f;

        float IHomingProjectile.DetectRadius => 1200;

        bool IHomingProjectile.CanHome => Projectile.ai[0] > 60;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 12;
        }

        public override bool PreDraw(ref Color lightColor)
        {


            SpriteBatch spriteBatch = Main.spriteBatch;

           

            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {

        }

        public override void AI()
        {
            Projectile.ai[0]++;

            
           
            

            //Dust.NewDustPerfect(Projectile.Center, DustID.HallowSpray, Vector2.Zero);
            if (Projectile.ai[0] % 3 == 0)
            {
                //Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), DustID.SnowSpray, Vector2.Zero);

                PointGlowPreMultiplied p1 = new();
                p1.Initialize(Projectile.Center, Projectile.velocity * 0.05f, Color.DeepSkyBlue, Main.rand.NextFloat(0.4f, 0.8f), 10, true);
                ParticleEngine.BehindProjectiles.Add(p1);

                PointGlowPreMultiplied p2 = new();
                p2.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity * 0.05f, Color.SkyBlue, 0.4f, 10, true);
                ParticleEngine.BehindProjectiles.Add(p2);


                PointGlowPreMultiplied p3 = new();
                p3.Initialize(Projectile.Center, Projectile.velocity * 0.05f, Color.FloralWhite, Main.rand.NextFloat(0.2f, 0.4f), 10, true);
                ParticleEngine.BehindProjectiles.Add(p3);

            }

            Lighting.AddLight(Projectile.Center, Color.DeepSkyBlue.ToVector3());
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ai[0] > 60 && Projectile.ManualCanHitFriendly(target);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item110, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SpectralChargeExplosion>(), Projectile.damage, 4, Projectile.owner);

            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Circular(2, 2), ModContent.ProjectileType<SpectralCharge2>(), Projectile.damage / 2, 4, Projectile.owner);
            }
        }
    }
}
