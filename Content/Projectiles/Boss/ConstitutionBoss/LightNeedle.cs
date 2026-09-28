using System;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss
{
    public class LightNeedle : ModProjectile
    {
        private enum AIState
        {
            Slowing,
            Dashing
        }

        private AIState State
        {
            get => (AIState)(int)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        private ref float Timer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Player target = FindClosestPlayer();
            Timer++;

            switch (State)
            {
                case AIState.Slowing:
                    DoSlowingPhase(target);
                    break;
                case AIState.Dashing:
                    DoDashingPhase(target);
                    break;
            }

            // Apply diagonal sprite rotation correction

        }

        private void DoSlowingPhase(Player target)
        {
            Projectile.rotation += Projectile.direction * Projectile.velocity.Length() * 0.1f;
            Projectile.velocity *= 0.96f;

            if (Projectile.velocity.Length() < 1f || Timer > 60f)
            {
                Timer = 0f;
                State = AIState.Dashing;
            }
        }

        private void DoDashingPhase(Player target)
        {
            if (target == null || !target.active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Timer == 1f) // first tick of dashing phase
            {
                SoundEngine.PlaySound(DTAssetLib.SwordSounds.TenebrisSwing with { PitchVariance = 0.4f });
                Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = direction * 42f;
                Projectile.netUpdate = true;
            }
        }

        private Player FindClosestPlayer()
        {
            Player closest = null;
            float minDistance = float.MaxValue;

            foreach (Player p in Main.player)
            {
                if (p.active && !p.dead)
                {
                    float dist = Vector2.Distance(p.Center, Projectile.Center);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        closest = p;
                    }
                }
            }

            return closest;
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 FlankLeft = Projectile.velocity.RotatedBy(MathHelper.PiOver2);
            Vector2 FlankRight = Projectile.velocity.RotatedBy(-MathHelper.PiOver2);

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (State == AIState.Dashing)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    float Mult = MathHelper.Lerp(1f, 0f, (float)i / (float)Projectile.oldPos.Length);
                    Color color = OpusColorUtils.MultiLerp((float)i / (float)Projectile.oldPos.Length, ColorLib.StellarFireColormap);
                    Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, (color with { A = 0 } * 0.7f * Mult) * Projectile.Opacity, 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, 1f, SpriteEffects.None);

                    Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, (color with { A = 0 } * 0.8f * Mult) * Projectile.Opacity, i == 0 ? Projectile.velocity.ToRotation() : Projectile.oldRot[i], DTAssetLib.SparkSmoothThin.Value.Size() / 2, 0.1f, SpriteEffects.None);
                }
            }

            Main.EntitySpriteDraw(DTAssetLib.LightNeedleTrail.Value, Projectile.Center, null, ColorLib.StellarFire5 with { A = 0 }, Projectile.rotation, DTAssetLib.LightNeedleTrail.Value.Size() / 2, 1f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White with { A = 0 }));
           
            return false; 
        }
    }
}