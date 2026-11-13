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
    public class SpectralCharge2 : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 480;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 9;
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
                p1.Initialize(Projectile.Center, Projectile.velocity * 0.05f, Color.DeepSkyBlue, Main.rand.NextFloat(0.2f, 0.5f), 10, true);
                ParticleEngine.BehindProjectiles.Add(p1);

                PointGlowPreMultiplied p2 = new();
                p2.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity * 0.05f, Color.SkyBlue, 0.3f, 10, true);
                ParticleEngine.BehindProjectiles.Add(p2);

            }

            Projectile.velocity.Y += 0.01f;

            Lighting.AddLight(Projectile.Center, Color.DeepSkyBlue.ToVector3() * 0.5f);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return null;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[1]++;
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }

            Projectile.velocity *= 0.95f;
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            return Projectile.ai[1] >= 3;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item118, Projectile.Center);
           // SoundEngine.PlaySound(DTAssetLib.Impacts.HeavyCrit with { Volume = 0.5f, Pitch = -1.5f }, Projectile.Center);
            SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/DAHit") with { MaxInstances = 0, Pitch = -0.5f, PitchVariance = 0.5f, Volume = 0.2f }, Projectile.Center);

            BloomRingSharp ring = new();
            ring.Prepare(Projectile.Center, Vector2.Zero, Color.DeepSkyBlue, 0.05f, 0.001f, 0.2f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(ring);

            for (int i = 0; i < 6; i++)
            {
                PointGlowPreMultiplied p = new();
                p.Initialize(Projectile.Center, Main.rand.NextVector2Circular(1, 1), Color.DeepSkyBlue, 0.1f);
                ParticleEngine.BehindProjectiles.Add(p);
            }

            for (int i = 0; i < 3; i++)
            {

            }
        }
    }
}
