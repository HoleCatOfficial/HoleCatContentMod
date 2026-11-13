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
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class SpectralChargeExplosion : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion with { MaxInstances = 0, PitchVariance = 0.5f }, Projectile.Center);
            SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/DAHit") with { MaxInstances = 0, PitchVariance = 0.5f, Volume = 0.5f }, Projectile.Center);


            BloomRingSharp ring = new();
            ring.Prepare(Projectile.Center, Vector2.Zero, Color.DeepSkyBlue, 0.1f, 0.01f, 0.75f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(ring);

            for (int i = 0; i < 6; i++)
            {
                PointGlowPreMultiplied p = new();
                p.Initialize(Projectile.Center, Main.rand.NextVector2Circular(3, 3), Color.DeepSkyBlue, 0.3f);
                ParticleEngine.BehindProjectiles.Add(p);
            }


        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }
    }
}
