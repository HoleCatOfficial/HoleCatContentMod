using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class ShadowExplosion : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, Projectile.Center);

            SimpleExplosionParticle Explosion = new();
            Explosion.Prepare(Projectile.Center, Vector2.Zero, Color.DarkMagenta, 0.1f, 0.01f, 1f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Explosion);

            BloomRing Ring1 = new();
            Ring1.Prepare(Projectile.Center, Vector2.Zero, Color.DarkMagenta * 0.5f, 0.1f, 0.01f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Ring1);

            BloomRingSharp Ring2 = new();
            Ring2.Prepare(Projectile.Center, Vector2.Zero, Color.Magenta, 0.1f, 0.01f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Ring1);


            Opus.RingSpreadProjectile(ModContent.ProjectileType<HomingShadowflame>(), 8, Projectile.Center, 40, Projectile.damage / 4, 7, 9, offset: Main.rand.NextFloat(MathHelper.TwoPi));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
        }
    }

    public class ShadowExplosion2 : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, Projectile.Center);

            SimpleExplosionParticle Explosion = new();
            Explosion.Prepare(Projectile.Center, Vector2.Zero, Color.DarkMagenta, 0.1f, 0.01f, 1f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Explosion);

            BloomRing Ring1 = new();
            Ring1.Prepare(Projectile.Center, Vector2.Zero, Color.DarkMagenta * 0.5f, 0.1f, 0.01f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Ring1);

            BloomRingSharp Ring2 = new();
            Ring2.Prepare(Projectile.Center, Vector2.Zero, Color.Magenta, 0.1f, 0.01f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Ring1);

            //Opus.RadialParticleRandomDir(PRTLoader.GetParticleID<SparkParticleNoGravity>(), 9, Projectile.Center, 1f, Color.LavenderBlush, 1f, 9, ai1: 2);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
        }
    }
}
