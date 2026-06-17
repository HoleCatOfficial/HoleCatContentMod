using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.Gores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
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

namespace DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss
{
    public class DarkLaserMine : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 24; // The width of projectile hitbox
            Projectile.height = 24; // The height of projectile hitbox

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
        }

        public override bool PreDrawExtras()
        {
            SpriteBatch sb = Main.spriteBatch;
            DTUtils Utility = new DTUtils();

            DTUtils.DrawCrystalCore(sb, Projectile.Center, Color.White, ColorLib.TenebrisGradient, RotOff, 2f);

            Opus.StartSpriteBatchWithBlending(sb, BlendState.Additive, SpriteSortMode.Immediate);
            Opus.DrawGlowOnProj(Projectile, new Color(43, 37, 154), false);
            TelegraphLine(sb);
            Opus.ReturnToDefaultDrawing(sb);
            return false;
        }

        public void TelegraphLine(SpriteBatch SB)
        {
            var LineTex = DTAssetLib.Line(1).Value;
            Vector2 start = IntialPos;

            if (Projectile.active)
            {
                for (int dir = 0; dir < 8; dir++)
                {
                    float angle = MathHelper.TwoPi * dir / 8f;
                    Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                    Vector2 drawPos = start - Main.screenPosition;
                    Vector2 scale = new Vector2(3600, 1f);

                    SB.Draw(DTAssetLib.Line(1).Value, drawPos, null, ColorLib.TenebrisGradient, angle, new Vector2(0, DTAssetLib.Line(1).Value.Height / 2f), scale, SpriteEffects.None, 0f);
                }
            }
        }


        public Vector2 IntialPos;

        public override void OnSpawn(IEntitySource source)
        {
            IntialPos = Projectile.Center;
        }

        float RotOff = 0;
        int DustAlpha = 255;
        float SoundPitch = 0f;
        public override void AI()
        {
            Vector2 ToPlayer = Projectile.Center - Main.LocalPlayer.Center;
            Projectile.velocity *= 0.999f;

            RotOff -= 0.08f;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {

            SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/NightmareRose/DarkMineLaser") with { MaxInstances = 1 }, Projectile.Center);

            Opus.RadialSpreadProjectile(ModContent.ProjectileType<DarkLaser>(), 8, Projectile.Center, Projectile.damage, 8, 0.001f, offset: 0);
        }
    }

    public class DarkLaser : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.ai[1];
        }

        float WidthScl = 0f;
        Line L;
        int oF = 0;
        int oF2 = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            oF -= 15;
            oF2 -= 10;
            L = new Line(Projectile.Center, Projectile.Center + new Vector2(2000, 0).RotatedBy(Projectile.rotation));

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(14), ColorLib.TenebrisGradient * 0.8f, Main.spriteBatch, BlendState.Additive, oF2, WidthScl * 0.8f, 4f);
          

            Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, ColorLib.TenebrisGradient, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl * 0.8f), SpriteEffects.None);

            Main.spriteBatch.UseBlendState(BlendState.AlphaBlend);

            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(14), Color.Black, Main.spriteBatch, BlendState.Additive, oF, WidthScl * 0.65f, 3f);
           
            Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, Color.Black, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl * 0.5f), SpriteEffects.None);
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.ai[1];
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            float MaxScl = 0.85f;

            float t = ((Projectile.ai[0] / 30));
            if (Projectile.timeLeft > 210)
            {
                WidthScl = MathHelper.Lerp(0f, MaxScl, t);
            }
            if (Projectile.timeLeft < 270 && Projectile.timeLeft > 30)
            {
                WidthScl = MaxScl;
                Projectile.ai[0] = 0;
            }
            if (Projectile.timeLeft < 30)
            {
                WidthScl = MathHelper.Lerp(MaxScl, 0, t);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 300);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float length = 2000f; // however long your laser should be

            Vector2 start = Projectile.Center;

            Vector2 S = Projectile.velocity;
            Vector2 end = start + new Vector2(length, 0).RotatedBy(Projectile.rotation);

            float collisionPoint = 0f;

            float beamWidth = 30f * WidthScl; // scale this how you want

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, beamWidth, ref collisionPoint);
        }
    }

}
