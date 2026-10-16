using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Summon
{
    public class MalachiteLevel3Snap : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
        }

        public override void SetStaticDefaults()
        {

        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(DTAssetLib.SwordSounds.LightSnap, Projectile.Center);
            SoundEngine.PlaySound(DTAssetLib.SwordSounds.TenebrisSwing with {  Volume = 0.6f }, Projectile.Center);

            LerpingBloomRingSharp ring  = new();
            ring.Prepare(Projectile.Center, Vector2.Zero, Color.LightCyan, Color.Aquamarine, 0.01f, 0.001f, 0.3f);
            ParticleEngine.Particles.Add(ring);

            for (int i = 0; i < 2; i++)
            {
                HeatseekerSilohSpark spark = new();
                spark.PrepareSpark(Projectile.Center, Vector2.Zero, Main.rand.NextFloat(MathHelper.TwoPi), Color.Aquamarine, 1f, false, 20, SparkDrawMode.Additive, Main.rand.NextFloat(2f, 4.5f));
                ParticleEngine.Particles.Add(spark);
            }

            for (int i = 0; i < 16; i++)
            {
                Fire fire = new();
                fire.PrepareFire(Projectile.Center, Main.rand.NextVector2Circular(2f, 2f), DTUtils.RandomDirection(2), Color.LightCyan * 0.5f, 0.7f, 60, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
                ParticleEngine.Particles.Add(fire);
            }
        }

        public override void AI()
        {

        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            
        }
    }
}
