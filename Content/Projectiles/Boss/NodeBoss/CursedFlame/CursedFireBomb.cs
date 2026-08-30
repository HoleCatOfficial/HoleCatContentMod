using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.NodeBoss.CursedFlame
{
    public class CursedFireBomb : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }

        Color WarnColor = Color.Black;
        float f = 0;
        public override bool PreDraw(ref Color lightColor)
        { 
            f += 0.1f;
            DTUtils.DrawCrystalCore(Main.spriteBatch, Projectile.Center, Color.White, ColorLib.Wretched2, f, 1f);

            Main.EntitySpriteDraw(DTAssetLib.BloomRingSharp.Value, Projectile.Center - Main.screenPosition, null, WarnColor with { A = 0 }, 0f, DTAssetLib.BloomRingSharp.Value.Size() / 2, 0.395f, SpriteEffects.None);
            return false;
        }

        public override void AI()
        {
            float prog = ((float)Projectile.timeLeft / 300f);

            WarnColor = OpusColorUtils.MultiLerp(prog, ColorLib.WretchedColorMap);

            WretchedPointGlow glow = new();
            glow.Prepare(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Main.rand.NextVector2Circular(1, 1), 2f);
            ParticleEngine.Particles.Add(glow);

        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CursedFireBombExplosion>(), Projectile.damage, 10);
        }
    }

    public class CursedFireBombExplosion : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 15;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(DTAssetLib.Impacts.ExplosiveImpactBig with { PitchVariance = 0.2f, MaxInstances = 0 }, Projectile.Center);
            SoundEngine.PlaySound(DTAssetLib.Impacts.FlameImpact with { PitchVariance = 0.2f, MaxInstances = 0 }, Projectile.Center);

            LerpingBloomRingSharp Ring = new();
            Ring.Prepare(Projectile.Center, Vector2.Zero, ColorLib.WretchedColorMap, 0.2f, 0.01f, 2f);
            ParticleEngine.Particles.Add(Ring);

            LerpingBloomRingSharp Ring2 = new();
            Ring2.Prepare(Projectile.Center, Vector2.Zero, ColorLib.WretchedColorMap, 0.2f, 0.01f, 1.5f);
            ParticleEngine.Particles.Add(Ring2);

            for (int i = 0; i < 10; i++)
            {
                WretchedPointGlow glow = new();
                glow.Prepare(Projectile.Center, Main.rand.NextVector2Circular(5, 5), 4f);
                ParticleEngine.Particles.Add(glow);
            }

            //Opus.RadialSpreadProjectile(ModContent.ProjectileType<CursedFlameProj>(), 3, Projectile.Center, 20, 2, 9f, offset: Main.rand.NextFloat(MathHelper.TwoPi));
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.CursedInferno, 600);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utilities.CircularHitboxCollision(Projectile.Center, 400f, targetHitbox);
        }
    }
}
