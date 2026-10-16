using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Summon
{
    public class TenebrisElementalMinionShot : ModProjectile, IDrawPixelated, IHomingProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public float DelayTimer;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {


        }

        public float trailOffset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            

            return false;
        }

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveProjectiles;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float turnSpeed = 3f;
        float IHomingProjectile.HomingTurnSpeed => turnSpeed;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 1f;

        float IHomingProjectile.DetectRadius => 3200;

        bool IHomingProjectile.CanHome => DelayTimer > 10;

        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            trailOffset += 0.05f;

            var Cap = spriteBatch.Capture();
            spriteBatch.End();

            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;
            Cap.SamplerState = SamplerState.PointWrap;

            spriteBatch.Begin(Cap);



            if (Projectile.OldCenter().Length > 1)
            {
                List<ColoredVertex> ve = new List<ColoredVertex>();
                List<ColoredVertex> ve2 = new List<ColoredVertex>();
                float a = 0;

                for (int i = Projectile.OldCenter().Length - 1; i > 0; i--)
                {
                    float u = i / (float)(Projectile.OldCenter().Length - 1);
                    float widthFactor = (float)Math.Sin(u * MathHelper.Pi);

                    float width = 20f * widthFactor;

                    Vector2 dir = (Projectile.OldCenter()[i] - Projectile.OldCenter()[i - 1]).ToRotation().ToRotationVector2();
                    Vector2 perp = dir.RotatedBy(MathHelper.PiOver2);
                    Vector2 offset = perp * width;
                    Vector2 offset2 = -perp * width;

                    DTUtils.AddStrips(ve, Projectile.OldCenter().ToList(), i, offset, offset2, 1f, ColorLib.TenebrisGradient with { A = 0 }, trailOffset);
                }

                var Cap2 = spriteBatch.Capture();
                spriteBatch.End();

                Cap2.TransformMatrix = PixelationSystem.PixelationMatrix;
                Cap.SamplerState = SamplerState.PointWrap;

                spriteBatch.Begin(Cap2);

                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    gd.Textures[0] = DTAssetLib.Streak(15, true).Value;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }


        public override void AI()
        {
            DelayTimer++;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                {
                    Projectile.oldPos[i] = Projectile.Center;
                }
            }

            turnSpeed += 0.5f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/BurnMagic") { MaxInstances = 0, PitchVariance = 0.5f, Volume = 0.6f, Pitch = 1f }, Projectile.Center);
            BloomRingSharp ring = new();
            ring.Prepare(Projectile.Center, Vector2.Zero, ColorLib.TenebrisGradient, 0.01f, 0.001f, 0.3f, BlendState.Additive);
            ParticleEngine.Particles.Add(ring);

            for (int i = 0; i < 16; i++)
            {
                Fire fire = new();
                fire.PrepareFire(Projectile.Center, Main.rand.NextVector2Circular(2f, 2f), DTUtils.RandomDirection(2), ColorLib.TenebrisGradient, 0.7f, 60, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
                ParticleEngine.Particles.Add(fire);
            }

            ShimmeringFlames.ShimmerBurn(target, true);
        }

    }
}

