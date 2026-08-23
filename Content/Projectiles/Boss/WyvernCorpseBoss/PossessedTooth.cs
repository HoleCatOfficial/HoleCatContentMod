using System;
using System.Xml;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss
{
    public class PossessedTooth : ModProjectile
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

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 12;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 80;
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
        }

        private void DoSlowingPhase(Player target)
        {
            Projectile.rotation += Projectile.direction * Projectile.velocity.Length() * 0.1f;
            Projectile.velocity *= 0.99f;
            Projectile.timeLeft = 180;

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

            if (Timer == 1f)
            {
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ChargeBreak") with { PitchVariance = 0.4f });
                Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = direction * 22f;
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
        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTAssetLib.PossessedToothOutline.Value, Projectile.Center - Main.screenPosition, null, ColorLib.IchorCrystalGradient, Projectile.rotation, new Vector2(DTAssetLib.PossessedToothOutline.Value.Width / 2, DTAssetLib.PossessedToothOutline.Value.Height / 2), Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(Projectile.width / 2, Projectile.height / 2), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}