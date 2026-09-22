using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class AccursedScepterShadowflame : ModProjectile, IHomingProjectile
    {
        public float DelayTimer;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 4f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.02f;

        float IHomingProjectile.HomingMaxAccel => 20f;

        float IHomingProjectile.DetectRadius => 600;

        bool IHomingProjectile.CanHome => DelayTimer >= 20;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;

            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D T = TextureAssets.Projectile[Type].Value;

            int frameHeight = T.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                T.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(T.Width / 2f, frameHeight / 2f);

            SpriteBatch spriteBatch = Main.spriteBatch;

            Opus.DrawGlowOnProj(Projectile, Color.DarkMagenta with { A = 0 }, true);

            Vector2 drawOrigin = new Vector2(T.Width * 0.5f, T.Height * 0.5f);
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                Color color = Color.White with { A = 0 } * MathHelper.Lerp(0.5f, 0f, (float)(k) / (float)Projectile.oldPos.Length);
                Color color2 = Color.Indigo with { A = 0 } * MathHelper.Lerp(0.5f, 0f, (float)(k) / (float)Projectile.oldPos.Length);

                Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, Projectile.OldCenter()[k] - Main.screenPosition, null, color2, Projectile.rotation, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

                Main.EntitySpriteDraw(T, Projectile.OldCenter()[k] - Main.screenPosition, frame, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }

            Opus.ReturnToDefaultDrawing(spriteBatch);

            Main.EntitySpriteDraw(T, Projectile.Center - Main.screenPosition, frame, Color.White with { A = 0 }, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 20 && !target.friendly;
        }

        public void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }



        public override void AI()
        {
            AnimateProjectile();

            DelayTimer++;
            Projectile.rotation = 0.03f * Projectile.velocity.X;

            PointGlowPreMultiplied X = new();
            X.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Vector2.Zero, Color.Indigo, 0.5f, 30);
            ParticleEngine.BehindProjectiles.Add(X);

            

            Lighting.AddLight(Projectile.Center, Color.DarkMagenta.ToVector3() * 0.2f);

            Lighting.AddLight(Projectile.Center, Color.DarkMagenta.ToVector3() * 0.2f);

        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.Center);
            SoundEngine.PlaySound(DTAssetLib.ChargeBreak with { Pitch = -0.5f, PitchVariance = 0.1f }, Projectile.Center);

            FireRing fire1 = new();
            fire1.Prepare(Projectile.Center, Vector2.Zero, Color.DarkMagenta, 0.01f, 0.0001f, 0.1f, BlendState.Additive);
            ParticleEngine.Particles.Add(fire1);

            FireRing fire2 = new();
            fire2.Prepare(Projectile.Center, Vector2.Zero, Color.DarkMagenta, 0.01f, 0.0001f, 0.06f, BlendState.Additive);
            ParticleEngine.Particles.Add(fire2);

            FireRing fire3 = new();
            fire3.Prepare(Projectile.Center, Vector2.Zero, Color.Indigo, 0.01f, 0.0001f, 0.03f, BlendState.Additive);
            ParticleEngine.Particles.Add(fire3);

            for (int i = 0; i < 9; i++)
            {
                HeatseekerSilohSpark spark = new();
                spark.PrepareSpark(Projectile.Center, Main.rand.NextVector2CircularEdge(6f, 6f), 0f, Color.Pink, 0.5f, false, 30, SparkDrawMode.Additive, 3f);
                ParticleEngine.Particles.Add(spark);
            }
        }
    }
}