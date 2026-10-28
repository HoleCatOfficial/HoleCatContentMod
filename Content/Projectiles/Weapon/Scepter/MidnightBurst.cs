using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class MidnightBurst : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public float GlobalMult => Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = (int)(100 * GlobalMult);
            Projectile.height = (int)(100 * GlobalMult);
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SimpleExplosionParticle explosion = new();
            explosion.Prepare(Projectile.Center, Vector2.Zero, Color.Blue, 0.1f * (GlobalMult), 0.01f * (GlobalMult), 1.5f * GlobalMult, BlendState.Additive);
            ParticleEngine.Particles.Add(explosion);

            for (int i = 0; i < 10; i++)
            {
                PixelParticle Particle = new PixelParticle();
                Particle.Initialize(Projectile.Center, Main.rand.NextVector2Circular(GlobalMult, GlobalMult), Color.DarkViolet, 2f);
                ParticleEngine.Particles.Add(Particle);

                TintableSmoke cloud = new();
                cloud.CreateWithBlending(Projectile.Center, Main.rand.NextVector2Circular(2f * GlobalMult, 2f * GlobalMult), Color.Blue, 1f, 1.2f, 120, PixelLayer.AboveTiles, BlendState.Additive, true);
                ParticleEngine.Particles.Add(cloud);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }
    }
}
