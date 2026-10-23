using BreadLibrary.Core.Graphics;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Utilities;
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

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class HeliosBeam : ModProjectile, IDrawPixelated
    {
        public override string Texture => DTUtils.NoTexture;

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AbovePlayer;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        float WidthScl = 0f;
        Line L;

        int oF = 0;
        float Rot = 0f;
        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            oF -= 50;
            L = new Line(Projectile.Center, Projectile.Center + new Vector2(length, 0).RotatedBy(Projectile.rotation));

            Color color = OpusColorUtils.MultiLerp(Opus.Sine(1f, 0f), ColorLib.HeliciteColorMap);

            if (WidthScl < 0.0001f || (L == null || L.GetLineLength < 0.001f))
            {
                return;
            }

            var Cap = spriteBatch.Capture();
            spriteBatch.End();

            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;
            

            spriteBatch.Begin(Cap);

            
            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(8, true), color with { A = 0 } * 0.5f, Main.spriteBatch, BlendState.Additive, oF, WidthScl);
            spriteBatch.End();
            spriteBatch.Begin(Cap);

            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(10, true), color with { A = 0 }, Main.spriteBatch, BlendState.Additive, oF, WidthScl);
            spriteBatch.End();
            spriteBatch.Begin(Cap);

            //Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, color with { A = 0 }, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl * 0.75f), SpriteEffects.None);

            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(10, true), Color.White with { A = 0 }, Main.spriteBatch, BlendState.Additive, oF, WidthScl * 0.5f, 1f);
            spriteBatch.End();
            spriteBatch.Begin(Cap);
            //Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl * 0.15f), SpriteEffects.None);

            Rot += 0.3f;

            Main.EntitySpriteDraw(DTAssetLib.Star(3).Value, Projectile.Center - Main.screenPosition, null, color with { A = 0 }, Rot, DTAssetLib.Star(3).Value.Size() / 2, WidthScl * 1.4f , SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.Star(3).Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Rot, DTAssetLib.Star(3).Value.Size() / 2, WidthScl * 1f, SpriteEffects.None);


            Main.EntitySpriteDraw(DTAssetLib.MiscSparkle144.Value, L.End - Main.screenPosition, null, color with { A = 0 }, 0f, DTAssetLib.MiscSparkle144.Value.Size() / 2, new Vector2(1f, WidthScl * 4f), SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.MiscSparkle144.Value, L.End - Main.screenPosition, null, Color.White with { A = 0 }, 0f, DTAssetLib.MiscSparkle144.Value.Size() / 2, new Vector2(1f, WidthScl * 5f), SpriteEffects.None);
            
            Main.EntitySpriteDraw(DTAssetLib.MiscSparkle144.Value, L.End - Main.screenPosition, null, color with { A = 0 }, MathHelper.PiOver2, DTAssetLib.MiscSparkle144.Value.Size() / 2, new Vector2(1f, WidthScl * 10f), SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.MiscSparkle144.Value, L.End - Main.screenPosition, null, Color.White with { A = 0 }, MathHelper.PiOver2, DTAssetLib.MiscSparkle144.Value.Size() / 2, new Vector2(1f, WidthScl * 8f), SpriteEffects.None);
            
            spriteBatch.ResetToDefault();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.ai[0];
        }

        public bool GoodBeam = false;

        SlotId LoopSlot;

        public SoundStyle Loop = DTAssetLib.ElectricLoopSound(2) with 
        {
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };

        float length = 2000f;

        public override void AI()
        {
            Projectile.rotation = Projectile.ai[0];
            UpdateSound();

            length = PerformBeamHitscan(6);

            Color color = OpusColorUtils.MultiLerp(Opus.Sine(1f, 0f), ColorLib.HeliciteColorMap);

            if (!DTOptimizationsConfig.instance.DisableExcessParticles && length < 2000f)
            {
                PointGlowPreMultiplied glow = new PointGlowPreMultiplied();
                glow.Initialize(Projectile.Center + new Vector2(length, 0).RotatedBy(Projectile.rotation), Main.rand.NextVector2Circular(0.1f, 0.1f), color * 0.5f, 2f);
                ParticleEngine.Particles.Add(glow);

                PointGlowPreMultiplied glow2 = new PointGlowPreMultiplied();
                glow2.Initialize(Projectile.Center + new Vector2(length, 0).RotatedBy(Projectile.rotation), Main.rand.NextVector2Circular(1f, 1f), Color.White, 0.7f);
                ParticleEngine.Particles.Add(glow2);

                PointGlowPreMultiplied glow3 = new PointGlowPreMultiplied();
                glow3.Initialize(Projectile.Center + new Vector2(length, 0).RotatedBy(Projectile.rotation), new Vector2(0f, Main.rand.NextFloat(-2f, -1f)), color, 0.5f);
                ParticleEngine.Particles.Add(glow3);

                PixelParticle pixel = new();
                pixel.Initialize(Projectile.Center + new Vector2(length, 0).RotatedBy(Projectile.rotation), Main.rand.NextVector2Circular(1f, 1f), Color.White, 2f);
                ParticleEngine.Particles.Add(pixel);
            }



            if (Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<HeliciteStaffHoldout>()] < 1)
            {
                GoodBeam = false;
            }

            if (GoodBeam)
            {
                if (WidthScl < 0.5f)
                {
                    WidthScl += 0.04f;
                }
                Projectile.timeLeft = 180;
            }
            else
            {
                if (WidthScl > 0)
                {
                    WidthScl -= 0.04f;
                }
                else
                {
                    Projectile.timeLeft = 0;
                }
            }
        }

        void UpdateSound()
        {
            if (!SoundEngine.TryGetActiveSound(LoopSlot, out var activeSound))
            {
                var tracker = new ProjectileAudioTracker(Projectile);
                LoopSlot = SoundEngine.PlaySound(Loop, Projectile.Center, soundInstance => {
                    soundInstance.Position = Projectile.Center;
                    soundInstance.Pitch = WidthScl;
                    return tracker.IsActiveAndInGame();
                });
            }
            else
            {
                activeSound.Position = Projectile.Center;
                activeSound.Pitch = WidthScl * 0.15f;
            }
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
 

            Vector2 start = L == null ? Projectile.Center : L.Start;

            Vector2 S = Projectile.velocity;
            Vector2 end = L == null ? Projectile.Center : L.End;

            float collisionPoint = 0f;

            float beamWidth = 30f * WidthScl; // scale this how you want

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, beamWidth, ref collisionPoint) && GoodBeam;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Color color = OpusColorUtils.MultiLerp(Opus.Sine(1f, 0f), ColorLib.HeliciteColorMap);
            SoundEngine.PlaySound(SoundID.DD2_KoboldIgnite with { MaxInstances = 0 }, target.Center);
            Player Owner = Main.player[Projectile.owner];
            for (int i = 0; i < 12; i++)
            {

                Spark spark = new();
                spark.PrepareSpark(target.Center, target.Center.DirectionFrom(Owner.MountedCenter).RotatedByRandom(0.2f) * (Main.rand.NextFloat(20f, 30f)), target.Center.DirectionFrom(Owner.MountedCenter).ToRotation() + MathHelper.PiOver2, color, 0.5f, false, 15, SparkDrawMode.Additive, 8f);
                ParticleEngine.Particles.Add(spark);
            }


            target.AddBuff(ModContent.BuffType<DaylightOverload>(), 240);
        }

        
    }

}
