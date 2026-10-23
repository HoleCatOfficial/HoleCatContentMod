using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using DestroyerTest.Common;
using Microsoft.Xna.Framework.Graphics;
using BreadLibrary.Core.Utilities;
using OpusLib;
using Terraria.DataStructures;
using OpusLib.Content.Helpers;
using DestroyerTest.Content.Buffs;
using Terraria.ID;
using DestroyerTest.Content.Particles;
using BreadLibrary.Core.Graphics.Particles;
using OpusLib.Content.Particles;
using BreadLibrary.Core.Graphics.Pixelation;

namespace DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss
{
    public class TenebrisLaser : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 16 * 500;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.ai[1];
        }

        float WidthScl = 0f;
        Line L;
        int oF = 0;
        int oF2 = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            oF -= 15;
            oF2 -= 10;
            L = new Line(Projectile.Center, Projectile.Center + new Vector2(length, 0).RotatedBy(Projectile.rotation));

            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(3, true), ColorLib.TenebrisGradient with { A = 0 } * 0.8f, Main.spriteBatch, BlendState.Additive, oF2, WidthScl * 0.3f, 1f);

            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(8, true), ColorLib.TenebrisGradient with { A = 0 }, Main.spriteBatch, BlendState.Additive, oF, WidthScl * 0.5f, 1f);

            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(3, true), Color.White with { A = 0 } * 0.8f, Main.spriteBatch, BlendState.Additive, oF2, WidthScl * 0.6f, 1f);

            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.ai[1];
        }
        float length = 2000f;
        public override void AI()
        {
            Projectile.ai[0]++;
            float MaxScl = 1.5f;

            length = PerformBeamHitscan(6);

            if (!DTOptimizationsConfig.instance.DisableExcessParticles && length < 2000f && L != null)
            {
                PointGlowPreMultiplied glow = new PointGlowPreMultiplied();
                glow.Initialize(L.End, Main.rand.NextVector2Circular(0.1f, 0.1f), ColorLib.TenebrisGradient * 0.5f, 2f);
                ParticleEngine.Particles.Add(glow);

                PointGlowPreMultiplied glow2 = new PointGlowPreMultiplied();
                glow2.Initialize(L.End, Main.rand.NextVector2Circular(1f, 1f), Color.White, 0.7f);
                ParticleEngine.Particles.Add(glow2);

                Fire fire = new();
                fire.PrepareFire(L.End, Main.rand.NextVector2Circular(1f, 1f), DTUtils.RandomDirection(2), Main.rand.NextFloat(0.1f, 0.5f), ColorLib.TenebrisGradient, 1f, 30, FireDrawMode.Additive, PixelLayer.AboveTiles);
                ParticleEngine.Particles.Add(fire);
            
            }




            float t = ((Projectile.ai[0] / 30));
            if (Projectile.timeLeft > 210)
            {
                WidthScl = MathHelper.Lerp(0f, MaxScl, t);
            }
            if (Projectile.timeLeft < 270 && Projectile.timeLeft > 30)
            {
                WidthScl = MaxScl;
                Projectile.ai[0] = 0;
            }
            if (Projectile.timeLeft < 30)
            {
                WidthScl = MathHelper.Lerp(MaxScl, 0, t);
            }

            if (Projectile.ai[1] == 1)
            {
                Projectile.rotation += 0.02f;
            }
            if (Projectile.ai[1] == -1)
            {
                Projectile.rotation -= 0.02f;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 300);
        }

        private float PerformBeamHitscan(int NumSamplePoints)
        {
            Vector2 direction = new Vector2(1f, 0f).RotatedBy(Projectile.rotation);

            float[] laserScanResults = new float[NumSamplePoints];

            Collision.LaserScan(
                Projectile.Center,
                direction,
                0f,
                2000f,
                laserScanResults
            );

            float averageLengthSample = 0f;

            for (int i = 0; i < laserScanResults.Length; i++)
            {
                averageLengthSample += laserScanResults[i];
            }

            return averageLengthSample / NumSamplePoints;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {

            Vector2 start = Projectile.Center;

            Vector2 S = Projectile.velocity;
            Vector2 end = start + new Vector2(length, 0).RotatedBy(Projectile.rotation);

            float collisionPoint = 0f;

            float beamWidth = 50f * WidthScl; // scale this how you want

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, beamWidth, ref collisionPoint);
        }
    }

    public class TenebrisLaser2 : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 16 * 500;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.ai[1];
        }

        float WidthScl = 0f;
        Line L;
        int oF = 0;
        int oF2 = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            oF -= 40;
            oF2 -= 30;
            L = new Line(Projectile.Center, Projectile.Center + new Vector2(length, 0).RotatedBy(Projectile.rotation));

            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(3, true), ColorLib.TenebrisGradient with { A = 0 } * 0.8f, Main.spriteBatch, BlendState.Additive, oF2, WidthScl * 0.3f, 1f);
            
            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(8, true), ColorLib.TenebrisGradient with { A = 0 }, Main.spriteBatch, BlendState.Additive, oF, WidthScl * 0.5f, 1f);

            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(3, true), Color.White with { A = 0 } * 0.8f, Main.spriteBatch, BlendState.Additive, oF2, WidthScl * 0.6f, 1f);

            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.ai[1];
        }

        float length = 2000f;

        public override void AI()
        {
            Projectile.ai[0]++;
            float MaxScl = 2f;

            length = PerformBeamHitscan(6);

            if (!DTOptimizationsConfig.instance.DisableExcessParticles && length < 2000f && L != null)
            {
                PointGlowPreMultiplied glow = new PointGlowPreMultiplied();
                glow.Initialize(L.End, Main.rand.NextVector2Circular(0.1f, 0.1f), ColorLib.TenebrisGradient * 0.5f, 2f);
                ParticleEngine.Particles.Add(glow);

                PointGlowPreMultiplied glow2 = new PointGlowPreMultiplied();
                glow2.Initialize(L.End, Main.rand.NextVector2Circular(1f, 1f), Color.White, 0.7f);
                ParticleEngine.Particles.Add(glow2);

                Fire fire = new();
                fire.PrepareFire(L.End, Main.rand.NextVector2Circular(1f, 1f), DTUtils.RandomDirection(2), Main.rand.NextFloat(0.1f, 0.5f), ColorLib.TenebrisGradient, 1f, 30, FireDrawMode.Additive, PixelLayer.AboveTiles);
                ParticleEngine.Particles.Add(fire);

            }

            float t = ((Projectile.ai[0] / 30f));
            if (Projectile.timeLeft > 1170)
            {
                WidthScl = MathHelper.Lerp(0f, MaxScl, t);
            }
            if (Projectile.timeLeft < 1170 && Projectile.timeLeft > 30)
            {
                WidthScl = MaxScl;
                Projectile.ai[0] = 0;
            }
            if (Projectile.timeLeft < 30)
            {
                WidthScl = MathHelper.Lerp(MaxScl, 0, t);
            }

            if (Projectile.ai[1] == 1)
            {
                Projectile.rotation += 0.01f;
            }
            if (Projectile.ai[1] == -1)
            {
                Projectile.rotation -= 0.01f;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 300);
        }

        private float PerformBeamHitscan(int NumSamplePoints)
        {
            Vector2 direction = new Vector2(1f, 0f).RotatedBy(Projectile.rotation);

            float[] laserScanResults = new float[NumSamplePoints];

            Collision.LaserScan(
                Projectile.Center,
                direction,
                0f,
                2000f,
                laserScanResults
            );

            float averageLengthSample = 0f;

            for (int i = 0; i < laserScanResults.Length; i++)
            {
                averageLengthSample += laserScanResults[i];
            }

            return averageLengthSample / NumSamplePoints;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {

            Vector2 start = Projectile.Center;

            Vector2 S = Projectile.velocity;
            Vector2 end = start + new Vector2(length, 0).RotatedBy(Projectile.rotation);

            float collisionPoint = 0f;

            float beamWidth = 50f * WidthScl; // scale this how you want

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, beamWidth, ref collisionPoint);
        }
    }

    public class TenebrisLaserFriendly : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 60;
        }

        float WidthScl = 0f;
        Line L;
        int oF = 0;
        int oF2 = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            oF -= 15;
            oF2 -= 10;
            L = new Line(Projectile.Center, Projectile.Center + new Vector2(length, 0).RotatedBy(Projectile.rotation));

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(6), ColorLib.TenebrisGradient * 0.8f, Main.spriteBatch, BlendState.Additive, oF2, WidthScl * 2.7f, 4f);
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, ColorLib.TenebrisGradient, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl), SpriteEffects.None);

            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(9), Color.White, Main.spriteBatch, BlendState.Additive, oF, WidthScl * 0.2f, 3f);
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl * 0.5f), SpriteEffects.None);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        float length = 2000f;

        public override void AI()
        {
            Projectile.ai[0]++;
            float MaxScl = 1.5f;

            length = PerformBeamHitscan(6);

            if (!DTOptimizationsConfig.instance.DisableExcessParticles && length < 2000f && L != null)
            {
                PointGlowPreMultiplied glow = new PointGlowPreMultiplied();
                glow.Initialize(L.End, Main.rand.NextVector2Circular(0.1f, 0.1f), ColorLib.TenebrisGradient * 0.5f, 2f);
                ParticleEngine.Particles.Add(glow);

                PointGlowPreMultiplied glow2 = new PointGlowPreMultiplied();
                glow2.Initialize(L.End, Main.rand.NextVector2Circular(1f, 1f), Color.White, 0.7f);
                ParticleEngine.Particles.Add(glow2);

                Fire fire = new();
                fire.PrepareFire(L.End, Main.rand.NextVector2Circular(1f, 1f), DTUtils.RandomDirection(2), Main.rand.NextFloat(0.1f, 0.5f), ColorLib.TenebrisGradient, 1f, 30, FireDrawMode.Additive, PixelLayer.AboveTiles);
                ParticleEngine.Particles.Add(fire);

            }


            float t = Utilities.Convert01To010((Projectile.ai[0] / 30));
            WidthScl = MathHelper.Lerp(0f, MaxScl, t);

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        private float PerformBeamHitscan(int NumSamplePoints)
        {
            Vector2 direction = new Vector2(1f, 0f).RotatedBy(Projectile.rotation);

            float[] laserScanResults = new float[NumSamplePoints];

            Collision.LaserScan(
                Projectile.Center,
                direction,
                0f,
                2000f,
                laserScanResults
            );

            float averageLengthSample = 0f;

            for (int i = 0; i < laserScanResults.Length; i++)
            {
                averageLengthSample += laserScanResults[i];
            }

            return averageLengthSample / NumSamplePoints;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {

            Vector2 start = Projectile.Center;

            Vector2 S = Projectile.velocity;
            Vector2 end = start + new Vector2(length, 0).RotatedBy(Projectile.rotation);

            float collisionPoint = 0f;

            float beamWidth = 30f * WidthScl; // scale this how you want

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, beamWidth, ref collisionPoint);
        }
    }
}
