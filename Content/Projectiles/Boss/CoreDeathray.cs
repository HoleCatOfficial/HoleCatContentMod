using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss
{
    public class CoreDeathray : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
        }

        Line spine = null;
        float Length = 2000;
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }

            

            spine = new Line(Projectile.Center, Projectile.Center + new Vector2(Length, 0).RotatedBy(Projectile.rotation));
            if (spine == null)
            {

            }

            else
            {
                var startTex = TextureAssets.Projectile[Projectile.type];
                var midTex = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/CoreDeathrayMid");
                var endTex = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/CoreDeathrayEnd");
                Color color71 = new Color(255, 255, 255, 0) * 0.9f;
                Main.EntitySpriteDraw(startTex.Value, Projectile.Center - Main.screenPosition, null, color71, Projectile.rotation, startTex.Value.Size() / 2f, Projectile.scale, SpriteEffects.None);
                Vector2 center3 = spine.End;

                DTUtils.instance.ScrollingTextureSpine(spine, midTex, color71, Main.spriteBatch, BlendState.AlphaBlend, 0, Projectile.scale);
                   
 
                
                Main.EntitySpriteDraw(endTex.Value, center3 - Main.screenPosition, null, color71, Projectile.rotation, endTex.Value.Size() / 2, Projectile.scale, SpriteEffects.None);
            }
            return false;
        }

        float angle = 0f;
        public override void AI()
        {
            if (!Main.npc[(int)Projectile.ai[1]].active)
            {
                Projectile.Kill();
                return;

            }
            else
            {
                angle += 0.1f;
                if (angle > MathHelper.TwoPi)
                {
                    angle -= MathHelper.TwoPi;
                }
                Projectile.Center = Main.npc[(int)Projectile.ai[1]].Center;
                Projectile.rotation += Projectile.ai[0];
            }

            Length = PerformBeamHitscan(6);
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
            Vector2 start = spine == null ? Projectile.Center : spine.Start;

            Vector2 S = Projectile.velocity;
            Vector2 end = spine == null ? Projectile.Center : spine.End;

            float collisionPoint = 0f;

            float beamWidth = 30f;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, beamWidth, ref collisionPoint);
        }
    }
}
