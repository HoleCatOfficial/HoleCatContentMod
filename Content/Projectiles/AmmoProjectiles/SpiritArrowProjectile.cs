using System.Linq;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.AmmoProjectiles
{
    public class SpiritArrowProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 480;
        }

        float off = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            off -= 0.05f;

            SpriteBatch spriteBatch = Main.spriteBatch;
            var cap = spriteBatch.Capture();

            spriteBatch.End();
            cap.BlendState = BlendState.Additive;
            spriteBatch.Begin(cap);

            // DTTrail.DrawTrail(Main.spriteBatch, DTAssetLib.Streak(1, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 16, ColorLib.LifeEcho, off);

            //Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White with { A = 0 } * 0.5f));
            Main.spriteBatch.ResetToDefault();
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));
            return false;
        }

        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.ai[0]++;

            if (Projectile.ai[0] > 10)
            {
                Projectile.velocity.Y += 0.2f;
            }

            Lighting.AddLight(Projectile.Center, ColorLib.LifeEcho.ToVector3());

            //if (Projectile.ai[0]++ % 2 == 0)
            //{
            BloomRingSharp ring = new();
            ring.Prepare(Projectile.Center, Vector2.Zero, Color.Yellow * 0.8f, 0.005f, 0.001f, 0.08f, Utilities.SubtractiveBlending);
            ParticleEngine.Particles.Add(ring);


            BloomRingSharp ring2 = new();
            ring2.Prepare(Projectile.Center, Vector2.Zero, ColorLib.LifeEcho * 0.5f, 0.005f, 0.001f, 0.1f, BlendState.Additive);
            ParticleEngine.Particles.Add(ring2);

            // }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {


        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                PointGlowPreMultiplied Particle = new();
                Particle.Initialize(Projectile.Center, Main.rand.NextVector2Circular(4, 4), ColorLib.LifeEcho, 0.75f);
                ParticleEngine.Particles.Add(Particle);
            }
        }
    }
}