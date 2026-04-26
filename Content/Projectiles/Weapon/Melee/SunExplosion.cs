using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using OpusLib;
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
    public class SunExplosion : ModProjectile
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
            SoundEngine.PlaySound(DTAssetLib.Impacts.ExplosiveImpactBig, Projectile.Center);
            BasePRT P1 = Opus.NewParticleFloatAI(PRTLoader.GetParticleID<SimpleExplosionParticle>(), Projectile.Center, Vector2.Zero, Color.DarkOrange, 0.01f, 6f, 0.3f, 0.01f);
            P1.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            BasePRT P2 = Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), Projectile.Center, Vector2.Zero, DTColorUtils.Pastel(Color.Orange, 0.5f), 0.01f, 1f, 0.1f, 0.02f);
            P2.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);

            //Fires
            BasePRT P3 = Opus.NewParticleFloatAI(PRTLoader.GetParticleID<Boom3>(), Projectile.Center, Vector2.Zero, Color.DarkOrange, 0.01f, 0.15f, 0.03f, 0.005f);
            P3.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);

            BasePRT P4 = Opus.NewParticleFloatAI(PRTLoader.GetParticleID<Boom3>(), Projectile.Center, Vector2.Zero, Color.DarkOrange, 0.01f, 0.25f, 0.03f, 0.005f);
            P4.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);

            //Opus.RadialParticleRandomDir(PRTLoader.GetParticleID<SparkParticleNoGravity>(), 9, Projectile.Center, 1f, Color.White, 1f, 9, ai1: 2);

            //Opus.RingProjectileOutward(ModContent.ProjectileType<HomingShadowflame>(), 8, Projectile.Center, 40, Projectile.damage / 4, 7, 9, RandomOffset: true);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Daybreak, 300);
        }
    }
}
