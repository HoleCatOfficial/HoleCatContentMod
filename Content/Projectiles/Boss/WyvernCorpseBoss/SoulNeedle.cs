using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss
{
    public class SoulNeedle : ModProjectile
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
            DTUtils.OwnedByBossNPC[Type] = ModContent.NPCType<WyvernCorpseHead>();
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
            Timer++;

            switch (State)
            {
                case AIState.Slowing:
                    DoSlowingPhase();
                    break;
                case AIState.Dashing:
                    DoDashingPhase();
                    break;
            }

            // Apply diagonal sprite rotation correction

        }

        private void DoSlowingPhase()
        {
            Projectile.rotation += Projectile.direction * Projectile.velocity.Length() * 0.1f;
            Projectile.velocity *= 0.96f;

            if (Projectile.velocity.Length() < 1f || Timer > 60f)
            {
                Timer = 0f;
                State = AIState.Dashing;
            }
        }

        private void DoDashingPhase()
        {

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Timer == 1f)
            {
                SoundEngine.PlaySound(SoundID.Item92 with { PitchVariance = 0.4f });
                SoundEngine.PlaySound(DTAssetLib.SwordSounds.TenebrisSwing with { PitchVariance = 0.4f, Volume = 0.3f });
                Vector2 direction = -Projectile.velocity;
                direction.Normalize();
                Projectile.velocity = direction * 42f;
                Projectile.netUpdate = true;
            }
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
                    Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, (ColorLib.Soul3 with { A = 0 } * 0.7f * Mult) * Projectile.Opacity, 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, 1f, SpriteEffects.None);

                    Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, (ColorLib.Soul with { A = 0 } * 0.8f * Mult) * Projectile.Opacity, i == 0 ? Projectile.velocity.ToRotation() : Projectile.oldRot[i], DTAssetLib.SparkSmoothThin.Value.Size() / 2, 0.1f, SpriteEffects.None);
                }
            }

            Main.EntitySpriteDraw(DTAssetLib.LightNeedleTrail.Value, Projectile.Center, null, ColorLib.Soul3 with { A = 0 }, Projectile.rotation, DTAssetLib.LightNeedleTrail.Value.Size() / 2, 1f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White with { A = 0 }));

            return false;
        }
    }
}