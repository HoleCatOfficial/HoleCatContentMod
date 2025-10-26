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

namespace DestroyerTest.Content.Projectiles.HellWeapons
{
    public class HellScimitar : ModProjectile
    {
        public override string GlowTexture => Texture;
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
            Projectile.width = 40;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 80;
        }

        public override void AI()
        {
            NPC target = FindClosestNPC();
            Lighting.AddLight(Projectile.Center, TorchID.Torch);
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

        private void DoSlowingPhase(NPC target)
        {
            Projectile.rotation += Projectile.direction * Projectile.velocity.Length() * 0.1f;
            Projectile.velocity *= 0.96f;
            Projectile.timeLeft = 80;

            if (Projectile.velocity.Length() < 1f || Timer > 60f)
            {
                Timer = 0f;
                State = AIState.Dashing;
            }
        }

        private void DoDashingPhase(NPC target)
        {
            if (target == null || !target.active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            if (Timer == 1f)
            {
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/HellWeaponDash", 3) with { PitchVariance = 0.4f });
                Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = direction * 22f;
                Projectile.netUpdate = true;
            }
        }

        private NPC FindClosestNPC()
        {
            NPC closest = null;
            float minDistance = 129000;

            foreach (NPC n in Main.npc)
            {
                if (n.active)
                {
                    float dist = Vector2.Distance(n.Center, Projectile.Center);
                    if (dist < minDistance)
                    {
                        //minDistance = dist;
                        closest = n;
                    }
                }
            }

            return closest;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Impacts/HellWeaponImpact") with { PitchVariance = 1f });
            //Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center, Vector2.Zero, ProjectileID.InfernoFriendlyBlast, (int)(Projectile.damage * 0.75f), 2, Projectile.owner);
            target.AddBuff(BuffID.OnFire3, 600);
        }
    }
}