using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace DestroyerTest.Content.Projectiles.EntitiesProjectiles
{
    public class DarkGluttonTrail : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }


        public override void AI()
        {
            Projectile.velocity *= 0.99f;

            Fire fire = new();
            fire.PrepareFire(Projectile.Center, Main.rand.NextVector2Circular(1f, 1f), DTUtils.RandomDirection(2), ColorLib.TenebrisGradient, 0.5f, 30, FireDrawMode.Additive, PixelLayer.AboveNPCs);
            ParticleEngine.Particles.Add(fire);

            if (Projectile.timeLeft == 1)
            {
                Projectile.Resize(200, 200);
            }
        }

        public SoundStyle Burst = SoundID.DD2_KoboldExplosion with { PitchVariance = 0.5f };

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(Burst);

            ImpactCracks Cracks = new();
            Cracks.Prepare(Projectile.Center, ColorLib.TenebrisGradient, 1f);
            ParticleEngine.BehindProjectiles.Add(Cracks);

            //Beeg boy
            SimpleExplosionParticle Burst1 = new SimpleExplosionParticle();
            Burst1.Prepare(Projectile.Center, Vector2.Zero, ColorLib.TenebrisGradient * 0.5f, 0.1f, 0.01f, 3.2f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Burst1);

            SimpleExplosionParticle Burst2 = new SimpleExplosionParticle();
            Burst2.Prepare(Projectile.Center, Vector2.Zero, ColorLib.TenebrisGradient, 0.1f, 0.01f, 2f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Burst2);

            for (int d = 0; d < 24; d++)
            {
                Fire fire = new();
                fire.PrepareFire(Projectile.Center, Main.rand.NextVector2Circular(5f, 5f), DTUtils.RandomDirection(2), ColorLib.TenebrisGradient, 1f, 30, FireDrawMode.Additive, PixelLayer.AboveNPCs);
                ParticleEngine.Particles.Add(fire);
            }
        }
    }
}
