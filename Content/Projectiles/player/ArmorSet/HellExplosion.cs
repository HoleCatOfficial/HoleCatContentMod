using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.ArmorSet
{
    public class HellExplosion : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
        }

        public override void SetStaticDefaults()
        {

        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/HellExplosion") { MaxInstances = 0, PitchVariance = 0.4f }, Projectile.Center);
            SoundEngine.PlaySound(DTAssetLib.Impacts.FlameImpact, Projectile.Center);

            ShockwaveExplosionParticle Explosion = new();
            Explosion.Prepare(Projectile.Center, Vector2.Zero, Color.Red, 0.01f, 0.001f, 0.2f, BlendState.Additive);
            ParticleEngine.Particles.Add(Explosion);

            FireRing fire1 = new();
            fire1.Prepare(Projectile.Center, Vector2.Zero, Color.OrangeRed, 0.01f, 0.001f, 0.2f, BlendState.Additive);
            ParticleEngine.Particles.Add(fire1);

            FireRing fire2 = new();
            fire2.Prepare(Projectile.Center, Vector2.Zero, Color.OrangeRed, 0.01f, 0.001f, 0.15f, BlendState.Additive);
            ParticleEngine.Particles.Add(fire2);

            FireRing fire3 = new();
            fire3.Prepare(Projectile.Center, Vector2.Zero, Color.Orange, 0.01f, 0.001f, 0.1f, BlendState.Additive);
            ParticleEngine.Particles.Add(fire3);

            for (int i = 0; i < 9; i++)
            {
                PointGlowPreMultiplied Glow = new PointGlowPreMultiplied();
                Glow.Initialize(Projectile.Center, Main.rand.NextVector2Circular(3, 3), Color.OrangeRed, 1.5f);
                ParticleEngine.ShaderParticles.Add(Glow);

                PointGlowPreMultiplied Glow2 = new PointGlowPreMultiplied();
                Glow2.Initialize(Projectile.Center, Main.rand.NextVector2Circular(3, 3), Color.Orange, 1.5f);
                ParticleEngine.ShaderParticles.Add(Glow2);
            }
        }

        public override void AI()
        {

        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 180);
        }
    }
}
