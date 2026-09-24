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
using DestroyerTest.Content.Particles.Stellar;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class Ping : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = 180;
            Projectile.height = 180;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 80;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DTUtils Utility = new DTUtils();
            float opacity = Projectile.Opacity;

            

            return false;
        }

        SoundStyle Sound = new SoundStyle(DTAssetLib.AudioPath + "/CosmicPing/Ping", 4) { MaxInstances = 0, PitchVariance = 0.1f, PauseBehavior = PauseBehavior.PauseWithGame };

        public override void AI()
        {
            Projectile.ai[1]++;

            if (Projectile.ai[1] == 1)
            {
                SoundEngine.PlaySound(Sound);

                SmallShine shine = new();
                shine.Prepare(Projectile.Center, Vector2.Zero, Color.White, 0.5f);
                ParticleEngine.ShaderParticles.Add(shine);
            }
            if (Projectile.ai[1] == 60)
            {
                for (int i = 0; i < 8; i++)
                {
                    StellarPointGlow Glow = new();
                    Glow.Prepare(Projectile.Center, Main.rand.NextVector2Circular(4f, 4f), 1.8f);
                    ParticleEngine.BehindProjectiles.Add(Glow);
                }


                LerpingBloomRingSharp Ring = new();
                Ring.Prepare(Projectile.Center, Vector2.Zero, ColorLib.StellarFireColormap, 0.05f, 0.005f, 0.75f);
                ParticleEngine.BehindProjectiles.Add(Ring);

                LerpingBloomRingSharp Ring2 = new();
                Ring2.Prepare(Projectile.Center, Vector2.Zero, ColorLib.StellarFireColormap, 0.01f, 0.001f, 0.6f);
                ParticleEngine.BehindProjectiles.Add(Ring2);
            }


            Lighting.AddLight(Projectile.Center, ColorLib.StellarFire3.ToVector3() * 0.5f);


        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ai[1] >= 60;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 600);
        }


    }
}
