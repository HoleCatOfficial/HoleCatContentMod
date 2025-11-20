using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;

namespace DestroyerTest.Content.Projectiles
{
    public class TenebrisDart : ModProjectile
    {

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 14; // The width of projectile hitbox
            Projectile.height = 32; // The height of projectile hitbox

            Projectile.DamageType = DamageClass.Generic; // What type of damage does this projectile affect?
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.timeLeft = 180; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = ColorLib.TenebrisGradient;
            DTUtils Utility = new DTUtils();
            SpriteBatch sb = Main.spriteBatch;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);

            Opus.StartSpriteBatchWithBlending(sb, BlendState.Additive, SpriteSortMode.Immediate);
            if (WaitTimer < 20)
            {
                Main.EntitySpriteDraw(DTAssetLib.FadeLine.Value, Projectile.Center - Main.screenPosition, null, ColorLib.TenebrisGradient, Projectile.rotation + MathHelper.PiOver2, new Vector2(DTAssetLib.FadeLine.Value.Width / 2, DTAssetLib.FadeLine.Value.Height / 2), 2, SpriteEffects.None, 0);
            }
            Opus.ReturnToDefaultDrawing(sb);
            return true;
        }

        public int InitTime = 0;
        public int InitAlpha = 0;
        public override void OnSpawn(IEntitySource source)
        {
            InitTime = Projectile.timeLeft;
            InitAlpha = Projectile.alpha;
        }
        public void ManageAlpha(ref int timeLeft)
        {
            // Use elapsed ticks since spawn so we can compute a stable progress value
            int elapsed = InitTime - timeLeft; // how many ticks have passed since spawn

            if (elapsed >= 5 && elapsed < 45)
            {
                float progress = (elapsed - 5) / 40f;
                progress = MathHelper.Clamp(progress, 0f, 1f);
                Projectile.alpha = (int)MathHelper.Lerp(InitAlpha, 0f, progress);
            }

            // Fade IN (or restore): during the final 60 ticks of life, interpolate back to InitAlpha
            else if (timeLeft <= 60)
            {
                float progress = (60 - timeLeft) / 60f; // 0 -> 1 across last 60 ticks
                progress = MathHelper.Clamp(progress, 0f, 1f);
                Projectile.alpha = (int)MathHelper.Lerp(0f, InitAlpha, progress);
            }

            // Keep alpha within valid byte range
            if (Projectile.alpha < 0) Projectile.alpha = 0;
            if (Projectile.alpha > 255) Projectile.alpha = 255;
        }

        public int WaitTimer = 0;
        public bool SoundFlag = false;
        public override void AI()
        {
            ManageAlpha(ref Projectile.timeLeft);
            if (Main.rand.NextBool(3))
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.TintableDustLighted, newColor: ColorLib.TenebrisGradient, Scale: 1.8f, Velocity: Vector2.Zero);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (WaitTimer < 20)
            {
                WaitTimer++;
            }

            if (WaitTimer >= 20)
            {
                if (Projectile.velocity.Length() < 16)
                {
                    if (!SoundFlag)
                    {
                        SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ManaBurst") with { MaxInstances = 0, PitchVariance = 0.3f, Volume = 0.15f }, Projectile.Center);
                        SoundFlag = true;
                    }
                    Projectile.velocity *= 1.2f;
                }
                Projectile.netUpdate = true;
            }

        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 240);
        }
    }
}