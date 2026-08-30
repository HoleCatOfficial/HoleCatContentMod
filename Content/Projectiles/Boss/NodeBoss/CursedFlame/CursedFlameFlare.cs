using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.NodeBoss.CursedFlame
{
    public class CursedFlameFlare : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 6000;
            Projectile.tileCollide = false;
        }

        public bool DrawTrail = true;
        public bool FadeTrail = false;
        public float TrailOpacity = 1f;
        float rOff = 0f;

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            rOff += 0.1f;

            if (Main.rand.NextBool(3))
            {
                LerpingFire fire = new LerpingFire();
                fire.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.3f, 0.3f), ColorLib.WretchedColorMap, 1.6f, 100, FireDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(fire);
            }

            if (Main.rand.NextBool(6))
            {
                WretchedPointGlow glow = new();
                glow.Prepare(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Main.rand.NextVector2Circular(0.5f, 0.5f), 3.5f);
                ParticleEngine.Particles.Add(glow);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            var glowTex = DTAssetLib.FeatheredCircle.Value;
            DTUtils Utility = new DTUtils();

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float progress = i / (float)Projectile.oldPos.Length;
                float scale = MathHelper.Lerp(1.5f, 0.0005f, progress);
                Color color = DTColorUtils.MultiLerp(progress, ColorLib.WretchedColorMap) * TrailOpacity;

                Main.EntitySpriteDraw(
                    glowTex,
                    Projectile.OldCenter()[i] - Main.screenPosition,
                    null,
                    color,
                    Projectile.rotation,
                    glowTex.Size() / 2f,
                    scale,
                    SpriteEffects.None,
                    0
                );

                Main.EntitySpriteDraw(
                    glowTex,
                    Projectile.OldCenter()[i] - Main.screenPosition,
                    null,
                    DTColorUtils.Pastel(color, 0.8f),
                    Projectile.rotation,
                    glowTex.Size() / 2f,
                    scale * 0.4f,
                    SpriteEffects.None,
                    0
                );
            }

            Main.EntitySpriteDraw(
                DTAssetLib.MiscSparkle144.Value,
                Projectile.Center - Main.screenPosition,
                null,
                ColorLib.Wretched1,
                rOff,
                DTAssetLib.MiscSparkle144.Value.Size() / 2f,
                new Vector2(1f, 4f),
                SpriteEffects.None,
                0
            );

            Main.EntitySpriteDraw(
                DTAssetLib.MiscSparkle144.Value,
                Projectile.Center - Main.screenPosition,
                null,
                ColorLib.Wretched1,
                rOff + MathHelper.PiOver2,
                DTAssetLib.MiscSparkle144.Value.Size() / 2f,
                new Vector2(1f, 4f),
                SpriteEffects.None,
                0
            );

            Main.EntitySpriteDraw(
                glowTex,
                Projectile.Center - Main.screenPosition,
                null,
                ColorLib.Wretched1,
                Projectile.velocity.ToRotation(),
                glowTex.Size() / 2f,
                1.5f,
                SpriteEffects.None,
                0
            );

            Main.EntitySpriteDraw(
                glowTex,
                Projectile.Center - Main.screenPosition,
                null,
                DTColorUtils.Pastel(ColorLib.Wretched1, 0.8f),
                Projectile.velocity.ToRotation(),
                glowTex.Size() / 2f,
                1.5f * 0.4f,
                SpriteEffects.None,
                0
            );

            Opus.ReturnToDefaultDrawing(spriteBatch);

            return false;
        }

        public bool Flag1 = false;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            DrawTrail = true;
            FadeTrail = true;

            if (!Flag1)
            {
                Projectile.netUpdate = true;
                Flag1 = true;
            }
            return false;
        }


        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.CursedInferno, 240);
        }
    }
}
