using System;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using System.Collections.Generic;
using OpusLib.Content.Helpers;
using Terraria.Audio;

namespace DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed
{
    public class HallowBolt : ModProjectile
    {

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
            Main.projFrames[Type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        private void AnimateProjectile()
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


        public float trailOffset = 0f;
        public int WOffset = 0;

        public float WarnOpacity = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            BuildRainbow();
            SpriteBatch sb = Main.spriteBatch;
            Asset<Texture2D> texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type];
            DTUtils Utility = new DTUtils();
            lightColor = Main.DiscoColor;
            trailOffset += 0.04f;
            WOffset += 3;

            // Calculate source rectangle for current frame
            int frameHeight = texture.Value.Height / Main.projFrames[Projectile.type];
            Rectangle sourceRect = new Rectangle(0, Projectile.frame * frameHeight, texture.Value.Width, frameHeight);

            Vector2 origin = new Vector2(texture.Value.Width / 2f, frameHeight / 2f);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            Opus.StartSpriteBatchWithBlending(sb, BlendState.Additive, SpriteSortMode.Immediate);

            DTOptimizationsConfig OptCfg = ModContent.GetInstance<DTOptimizationsConfig>();
            if (!OptCfg.DisableExcessTrails)
            {
                Opus.StartSpriteBatchForTrails(sb, BlendState.Additive, SpriteSortMode.Immediate);

                if (TrailPositions.Count > 1)
                {
                    List<ColoredVertex> ve = new List<ColoredVertex>();
                    float a = 0;

                    for (int i = TrailPositions.Count - 1; i > 0; i--)
                    {
                        float t = 1f - (i / (float)TrailPositions.Count);
                        Color b = GetTrailColor(t) * t;

                        Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
                        Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 9;
                        Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 9;

                        DTUtils.AddStrips(ve, TrailPositions, i, offset, offset2, t, b, trailOffset);
                    }


                    GraphicsDevice gd = Main.graphics.GraphicsDevice;
                    if (ve.Count >= 3)
                    {
                        gd.Textures[0] = DTAssetLib.Streak(1).Value;
                        gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    }
                }
            }

            sb.Draw(texture.Value, drawPos, sourceRect, Main.DiscoColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            

            sb.Draw(texture.Value, drawPos, sourceRect, Color.White, Projectile.rotation, origin, Projectile.scale * 0.65f, SpriteEffects.None, 0f);
            Opus.ReturnToDefaultDrawing(sb);

            return false;
        }

        public List<Color> RainbowColormap = new List<Color>();

        private bool b1 = false;
        public void BuildRainbow()
        {
            RainbowColormap.Clear();

            Vector3 C = Main.rgbToHsl(Main.DiscoColor);

            for (int i = 0; i < 20; i++)
            {
                float shiftedHue = (C.X + i * 0.05f) % 1f; //X = H
                RainbowColormap.Add(Main.hslToRgb(shiftedHue, C.Y, C.Z)); //C.Y = S, and C.Z = L
            }
        }

        private Color GetTrailColor(float t)
        {
            if (RainbowColormap.Count == 0)
                return Color.White;

            t = MathHelper.Clamp(t, 0f, 1f);

            float scaled = t * (RainbowColormap.Count - 1);
            int low = (int)scaled;
            int high = Math.Min(low + 1, RainbowColormap.Count - 1);

            float lerp = scaled - low;
            return Color.Lerp(RainbowColormap[low], RainbowColormap[high], lerp);
        }


        public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();
        private const int TrailLength = 500;
        public override void AI()
        {
            AnimateProjectile();
            Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Scale: 0.6f);

            if (!DTOptimizationsConfig.instance.DisableExcessTrails)
            {
                Vector2 lastPos = TrailPositions.Count > 0 ? TrailPositions[0] : Projectile.Center;
                Vector2 newPos = Projectile.Center;

                float dist = Vector2.Distance(lastPos, newPos);
                float step = 0.3f;

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

            float maxDetectRadius = 1600f;

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (WarnOpacity > 0)
            {
                WarnOpacity -= 0.02f;
            }


            if (HomingTarget == null)
            {
                HomingTarget = FindClosestPlayer(maxDetectRadius);
            }

            if (HomingTarget != null && !IsValidTarget(HomingTarget))
            {
                HomingTarget = null;
            }

            if (HomingTarget == null)
                return;

            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            Projectile.velocity = Projectile.velocity.RotatedByRandom(0.1f).ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(2)).ToRotationVector2() * length;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public Player FindClosestPlayer(float maxDetectDistance)
        {
            Player closestPlayer = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            foreach (var target in Main.player)
            {
                if (IsValidTarget(target))
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestPlayer = target;
                    }
                }
            }

            return closestPlayer;
        }

        public bool IsValidTarget(Player target)
        {

            return (target.active == true && target.statLife > 1);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {

            Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Opus.RadialSpreadDustRandom(DustID.AncientLight, 5, Projectile.Center, 50, Main.DiscoColor, 0.7f, 2f);
            Opus.RadialSpreadDustRandom(DustID.AncientLight, 3, Projectile.Center, 50, Color.White, 1f, 3.5f);
        }

    }
}