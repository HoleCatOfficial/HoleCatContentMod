using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;

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

namespace DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss
{
    public class VileSpike : ModProjectile, IDrawPixelated
    {

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.light = 0.1f;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        Line Line;
        Vector2 Origin;
        public override void OnSpawn(IEntitySource source)
        {
            Origin = Projectile.Center;

        }

        public float trailOffset = 0f;
        public int offset = 0;
        public float WarnOpac = 1f;
        public override bool PreDraw(ref Color lightColor)
        {


            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White * Projectile.Opacity));

            return false;
        }

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveTiles;

        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            var Cap = spriteBatch.Capture();
            spriteBatch.End();

            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            spriteBatch.Begin(Cap);

            var Primitive = ModContent.Request<Texture2D>(Texture + "Primitive");
            offset += 7;

            DTUtils.instance.ScrollingTextureSpine(Line, Primitive, Color.White * Projectile.Opacity, spriteBatch, BlendState.AlphaBlend, offset, 1f);

            spriteBatch.ResetToDefault();
        }


        public override void AI()
        {
            Projectile.velocity *= 0.99f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.timeLeft < 60)
            {
                Projectile.Opacity -= 0.02f;
            }

            Line = new Line(Origin, Projectile.Center);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float length = Line.GetLineLength;

            Vector2 start = Line.Start;

            Vector2 end = Line.End;

            float collisionPoint = 0f;

            float beamWidth = 50f;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, beamWidth, ref collisionPoint);
        }
    }
}