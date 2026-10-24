using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class RiftStaffBeam : ModProjectile, IDrawPixelated
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
            Projectile.tileCollide = false;
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
            oF -= 30;
            L = new Line(Projectile.Center, Projectile.Center + new Vector2(length, 0).RotatedBy(Projectile.rotation));

            var Cap = spriteBatch.Capture();
            spriteBatch.End();

            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;


            spriteBatch.Begin(Cap);

            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(5, true), ColorLib.Rift with { A = 0 } * 0.2f, Main.spriteBatch, BlendState.Additive, oF, WidthScl * 1.2f);
            spriteBatch.End();
            spriteBatch.Begin(Cap);
            
            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.ZapTrail, Color.White, Main.spriteBatch, BlendState.Additive, oF, WidthScl);
            spriteBatch.End();
            spriteBatch.Begin(Cap);

            Rot += 0.3f;

            Main.EntitySpriteDraw(DTAssetLib.Star(3).Value, Projectile.Center - Main.screenPosition, null, ColorLib.Rift with { A = 0 }, Rot, DTAssetLib.Star(3).Value.Size() / 2, 1.4f, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.Star(3).Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Rot, DTAssetLib.Star(3).Value.Size() / 2, 1f, SpriteEffects.None);
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

        public SoundStyle Loop = DTAssetLib.ElectricLoopSound(3) with
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
            if (!DTOptimizationsConfig.instance.DisableExcessParticles && length < 2000f)
            {
                if (L != null)
                {
                    ElectricArc Arc = new();
                    Arc.Create(L.End, ColorLib.Rift, Main.rand.NextFloat(0.5f, 1f), 0.5f);
                    ParticleEngine.ShaderParticles.Add(Arc);

                    PointGlowPreMultiplied glow = new PointGlowPreMultiplied();
                    glow.Initialize(L.End, Main.rand.NextVector2Circular(0.1f, 0.1f), ColorLib.Rift, 2f);
                    ParticleEngine.Particles.Add(glow);
                }
            }


            if (Main.rand.NextBool(4) && !DTOptimizationsConfig.instance.DisableExcessParticles)
            {
                if (L != null)
                {
                    ElectricArc Arc = new();
                    Arc.Create(L.GetPointsAlongLine(20)[Main.rand.Next(20)], ColorLib.Rift, Main.rand.NextFloat(0.5f, 1f), 1f);
                    ParticleEngine.ShaderParticles.Add(Arc);
                }
            }




            if (Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<RiftStaffHoldout>()] < 1)
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
                activeSound.Pitch = WidthScl;
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
            float length = 2000f; // however long your laser should be

            Vector2 start = Projectile.Center;

            Vector2 S = Projectile.velocity;
            Vector2 end = start + new Vector2(length, 0).RotatedBy(Projectile.rotation);

            float collisionPoint = 0f;

            float beamWidth = 30f * WidthScl; // scale this how you want

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, beamWidth, ref collisionPoint) && GoodBeam;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HeliouricShock>(), 240);
        }

        
    }
    
}
