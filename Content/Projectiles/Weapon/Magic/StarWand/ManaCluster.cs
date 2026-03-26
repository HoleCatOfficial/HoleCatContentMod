using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic.StarWand
{
    public class ManaCluster : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var PTex = TextureAssets.Projectile[Type];

            lightColor = Color.CornflowerBlue;
            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();

            DTOptimizationsConfig OptCfg = ModContent.GetInstance<DTOptimizationsConfig>();
            if (!OptCfg.DisableExcessTrails)
            {
                Opus.StartSpriteBatchForTrails(spriteBatch, BlendState.Opaque, SpriteSortMode.Immediate);

                if (TrailPositions.Count > 1)
                {
                    List<ColoredVertex> ve = new List<ColoredVertex>();
                    List<ColoredVertex> ve2 = new List<ColoredVertex>();
                    float a = 0;

                    for (int i = TrailPositions.Count - 1; i > 0; i--)
                    {
                        float t = 1f - (i / (float)TrailPositions.Count); // fade toward tail
                        Color b = lightColor * t;
                        Color b2 = DTColorUtils.Pastel(lightColor, 0.75f) * t;

                        Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
                        Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 10;
                        Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 10;

                        DTUtils.AddStrips(ve, TrailPositions, i, offset, offset2, t, b, 0f);
                    }


                    GraphicsDevice gd = Main.graphics.GraphicsDevice;
                    if (ve.Count >= 3)
                    {
                        gd.Textures[0] = DTAssetLib.Streak(13).Value;
                        gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    }

                }
            }

            Opus.ReturnToDefaultDrawing(spriteBatch);

            Main.EntitySpriteDraw(PTex.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, PTex.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }



        public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();
        private const int TrailLength = 400;

        public void CacheTrail()
        {
            Vector2 lastPos = TrailPositions.Count > 0 ? TrailPositions[0] : Projectile.Center;
            Vector2 newPos = Projectile.Center;

            float dist = Vector2.Distance(lastPos, newPos);
            float step = 0.2f;

            if (dist > 0f)
            {
                int segments = (int)(dist / step);

                for (int i = 1; i <= segments; i++)
                {
                    Vector2 pos = Vector2.Lerp(lastPos, newPos, i / (float)segments);
                    TrailPositions.Insert(0, pos);
                    TrailRotations.Insert(0, Projectile.rotation);
                }
            }
            else
            {
                TrailPositions.Insert(0, newPos);
                TrailRotations.Insert(0, Projectile.rotation);
            }

            while (TrailPositions.Count > TrailLength)
                TrailPositions.RemoveAt(TrailPositions.Count - 1);
            while (TrailRotations.Count > TrailLength)
                TrailRotations.RemoveAt(TrailRotations.Count - 1);
        }

        public override void AI()
        {
            Projectile.velocity *= 0.94f;
            CacheTrail();

            Player player = Main.player[Projectile.owner];

            Projectile.rotation += (0.4f * Projectile.velocity.Length()) * Projectile.direction;

            if (Main.rand.NextBool (5))
            {
                Projectile.velocity = Projectile.velocity.RotatedByRandom(1f);
            }

            if (Main.rand.NextBool(20))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10), DustID.ManaRegeneration, Projectile.velocity * 0.2f, 100, default, 1.2f);
                dust.noGravity = true;
                dust.fadeIn = 1.5f;
            }


        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, Projectile.Center);
        }
    }
}
