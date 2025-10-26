using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;

namespace DestroyerTest.Content.Projectiles
{
    public class HoleCatFireSwirl : ModProjectile
    {

        public SoundStyle Fire = new SoundStyle("DestroyerTest/Assets/Audio/FlameImpact1") with { MaxInstances = 0 };
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.friendly = true;
            Projectile.penetrate = -1; // Infinite pierce
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600; // 10 seconds max lifespan
            Projectile.DamageType = DamageClass.Generic;
            Projectile.alpha = 255;
        }



        public override void AI()
        {
            Projectile.velocity *= 0.99f;

            Player player = Main.player[Projectile.owner];

            Projectile.rotation += 0.1f * Projectile.direction;
            
            for (int u = 0; u < 10; u++)
            {
                int RadOuter = 200;
                int RadInner = 180;
                Vector2 Outer = Projectile.Center + Main.rand.NextVector2CircularEdge(Main.rand.NextFloat(RadInner, RadOuter), Main.rand.NextFloat(RadInner, RadOuter));
                Dust.NewDustPerfect(Outer, DustID.TintableDustLighted, Projectile.velocity, 100, ColorLib.HoleCatFireGradient, 3);
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Outer, Projectile.velocity, ColorLib.HoleCatFireDeepRed * 0.5f, 0.4f);
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Opus.Opus opus = new Opus.Opus();
            SpriteBatch spriteBatch = Main.spriteBatch;
            opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            opus.DrawTextureOnProj(DTAssetLib.Swirl, Projectile, ColorLib.HoleCatFireGradient, true, Projectile.rotation, 2f, 2f);
            opus.DrawTextureOnProj(DTAssetLib.FireRing, Projectile, ColorLib.HoleCatFireGradient * 0.85f, false, -Projectile.rotation, 0.2f, 0.2f);
            opus.DrawTextureOnProj(DTAssetLib.FireRing, Projectile, ColorLib.HoleCatFireGradient * 0.85f, false, Projectile.rotation, 0.2f, 0.2f);
            opus.ReturnToDefaultDrawing(spriteBatch);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(Fire, Projectile.Center);
            Opus.Opus opus = new Opus.Opus();
            opus.RadialSpreadProjectile(ModContent.ProjectileType<HoleCatFireSmall>(), 8, Projectile.Center, Projectile.damage / 3, 3, 10);
            for (int u = 0; u < 20; u++)
            {
                int speed = 10;
                Vector2 Outer = new Vector2(speed, 0);
                Vector2 Dir = Outer.RotatedByRandom(MathHelper.TwoPi);
                Dust.NewDustPerfect(Projectile.Center, DustID.TintableDustLighted, Dir, 100, ColorLib.HoleCatFireGradient, 5);
                PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Projectile.Center, Dir, ColorLib.HoleCatFireDeepRed, 5f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HoleCatFire>(), 120);
            if (target.HasBuff<HoleCatFire>())
            {
                HCFTarget.instance.Level++;
            }
        }

    }

}