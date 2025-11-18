using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using System;
using Terraria.DataStructures;
using Terraria.Audio;

namespace DestroyerTest.Content.Projectiles.ConstitutionBoss
{
    public class GalantineLance : ModProjectile
    {
        /// <summary>
        /// The X scale with which we draw the warning texture extending out in front of the projectile. The higher the Final Speed, the longer it extends out. If Final Speed is equal to or less than BaseSpeed, this is 0.
        /// </summary>
        public float ScaleByVelocity = 0;
        /// <summary>
        /// The speed, in units per tick, that the projectile will move in the specified direction if no modifiers are applied. It's guaranteed to move.
        /// </summary>
        public const int BaseSpeed = 50;
        /// <summary>
        /// The name explains it all, to be entirely honest.
        /// </summary>
        public float SpeedModifier => Projectile.ai[0];
        /// <summary>
        /// The product of <i>BaseSpeed x SpeedModifier</i>.
        /// </summary>
        public float FinalSpeed = 0;
        /// <summary>
        /// The set amount of time it will take for the alpha to go from 255 to 0 after spawning.
        /// </summary>
        public const int FadeInTimer = 40;
        /// <summary>
        /// The current progress of fading from 255 to 0 over the course of FadeInTimer.
        /// </summary>
        public int FadeIn = 0;
        /// <summary>
        /// Works similarly to FadeInTimer, but for moving alpha from 0 back to 255.
        /// </summary>
        public const int FadeOutTimer = 60;
        /// <summary>
        /// You know what this is for.
        /// </summary>
        public int FadeOut = 0;
        /// <summary>
        /// The amount of idle time after spawning to display the velocity warning texture before moving.
        /// </summary>
        public const int WarnTimer = 100;
        /// <summary>
        /// The current progress of the warning state from 0 to 100.
        /// </summary>
        public int Warn = 0;

        private bool initialized = false;
        private Vector2 storedDirection;

        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //Hitbox is dynamic and rotates with projectile.
            float colpoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 60f, 16f, ref colpoint);
        }

        public enum State
        {
            /// <summary>
            /// Displays a warning and remains stationary.
            /// </summary>
            Idle,
            /// <summary>
            /// Moves FinalSpeed units per tick in the specified direction.
            /// </summary>
            Move
        }
        public State state = State.Idle;

        public int InitTime = 0;
        public int InitAlpha = 0;
        public void ManageAlpha(ref int timeLeft)
        {
            // Use elapsed ticks since spawn so we can compute a stable progress value
            int elapsed = InitTime - timeLeft; // how many ticks have passed since spawn

            // Fade OUT: start after 20 ticks have passed, and perform the fade across the next 60 ticks
            if (elapsed >= 20 && elapsed < 80)
            {
                float progress = (elapsed - 20) / 60f; // 0 -> 1 across 60 ticks
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

        public bool Sound1 = false;
        public override void AI()
        {
            ManageAlpha(ref Projectile.timeLeft);
            if (!initialized)
            {
                initialized = true;
                InitTime = Projectile.timeLeft;
                InitAlpha = Projectile.alpha;

                storedDirection = Projectile.velocity.RotatedBy(Projectile.ai[1]);
                Projectile.rotation = storedDirection.ToRotation();
            }


            FinalSpeed = BaseSpeed * SpeedModifier;
            ScaleByVelocity = MathHelper.Clamp((FinalSpeed - BaseSpeed) * 0.1f, 0f, 6f);
            Projectile.ai[1] = MathHelper.PiOver4;

            if (Warn < WarnTimer)
            {
                state = State.Idle;
                Warn++;
                FadeIn = Math.Min(FadeInTimer, FadeIn + 1);
                FadeOut = 0;
            }
            else
            {
                state = State.Move;
                FadeOut = Math.Min(FadeOutTimer, FadeOut + 1);
            }

            Projectile.rotation = storedDirection.ToRotation();

            if (state == State.Move)
            {
                Projectile.velocity = storedDirection * FinalSpeed;
                if (!Sound1)
                {
                    SoundEngine.PlaySound(SoundID.Item68);
                    Sound1 = true;
                }
            }
        }

        public void DisplayWarningTex()
        {
            Color Drawcolor = ColorLib.StellarColor * (1f - FadeIn / (float)FadeInTimer);
            Vector2 Scale = new Vector2(0.05f, ScaleByVelocity);

            Main.EntitySpriteDraw(DTAssetLib.Trail(3).Value, Projectile.Center - Main.screenPosition, null, Drawcolor, Projectile.rotation - MathHelper.PiOver2, DTAssetLib.Trail(3).Value.Size() / 2, Scale, SpriteEffects.None, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color BeamColor = ColorLib.StellarColor;
            lightColor = BeamColor;
            SpriteBatch SB = Main.spriteBatch;

            Opus.StartSpriteBatchWithBlending(SB, BlendState.Additive, SpriteSortMode.Immediate);

            if (state == State.Idle)
                DisplayWarningTex();

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, BeamColor, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            Opus.ReturnToDefaultDrawing(SB);
            return false;
        }
    }
}
