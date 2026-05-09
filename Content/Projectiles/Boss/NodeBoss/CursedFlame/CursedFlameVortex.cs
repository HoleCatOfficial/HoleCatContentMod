using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.NodeBoss.CursedFlame
{
	public class CursedFlameVortex : ModProjectile
	{
        public override string Texture => DTUtils.NoTexture;
        private Player HomingTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.player[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }

        public ref float DelayTimer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 100;
            ProjectileID.Sets.TrailingMode[Type] = 3;

        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.alpha = 255;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.light = 0.1f;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        Line Line;
        public override void OnSpawn(IEntitySource source)
        {
            Line = new Line(Projectile.Center, Projectile.Center + (Projectile.velocity * 200));

        }

        public float trailOffset = 0f;
        public int warnoffset = 0;
        public float WarnOpac = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            trailOffset += 0.04f;

            warnoffset += 20;


            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            DTUtils.instance.ScrollingTextureSpine(Line, DTAssetLib.Line(1), ColorLib.CursedFlames * 0.5f, spriteBatch, BlendState.Additive, warnoffset, 2f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, PixelationSystem.PixelationMatrix);



            if (Projectile.OldCenter().Length > 1)
            {
                List<ColoredVertex> ve = new List<ColoredVertex>();
                List<ColoredVertex> ve2 = new List<ColoredVertex>();
                float a = 0;

                for (int i = Projectile.OldCenter().Length - 1; i > 0; i--)
                {
                    float u = i / (float)(Projectile.OldCenter().Length - 1);
                    float widthFactor = (float)Math.Sin(u * MathHelper.Pi);

                    float width = 32f * widthFactor;

                    Vector2 dir = (Projectile.OldCenter()[i] - Projectile.OldCenter()[i - 1]).ToRotation().ToRotationVector2();
                    Vector2 perp = dir.RotatedBy(MathHelper.PiOver2);
                    Vector2 offset = perp * width;
                    Vector2 offset2 = -perp * width;

                    DTUtils.AddStrips(ve, Projectile.OldCenter().ToList(), i, offset, offset2, u, ColorLib.CursedFlames with { A = 0 }, trailOffset);
                }

                for (int i = Projectile.OldCenter().Length - 1; i > 0; i--)
                {
                    float u = i / (float)(Projectile.OldCenter().Length - 1);
                    float widthFactor = (float)Math.Sin(u * MathHelper.Pi);

                    float width = 32f * widthFactor;

                    Vector2 dir = (Projectile.OldCenter()[i] - Projectile.OldCenter()[i - 1]).ToRotation().ToRotationVector2();
                    Vector2 perp = dir.RotatedBy(MathHelper.PiOver2);
                    Vector2 offset = perp * width;
                    Vector2 offset2 = -perp * width;

                    DTUtils.AddStrips(ve2, Projectile.OldCenter().ToList(), i, offset, offset2, u, Color.White with { A = 0 }, trailOffset);
                }

                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    gd.Textures[0] = DTAssetLib.Streak(9).Value;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    gd.Textures[0] = DTAssetLib.Streak(4).Value;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }

            Opus.ReturnToDefaultDrawing(spriteBatch);

            return false;
        }


        public override void AI()
        {
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                {
                    Projectile.oldPos[i] = Projectile.Center;
                }
            }

            if (WarnOpac > 0)
            {
                WarnOpac -= 0.07f;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            foreach (var trail in new[] { Projectile.OldCenter(), Projectile.OldCenter() })
            {
                for (int i = 1; i < trail.Length; i++)
                {
                    Vector2 point1 = trail[i - 1];
                    Vector2 point2 = trail[i];
                    if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), point1, point2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}