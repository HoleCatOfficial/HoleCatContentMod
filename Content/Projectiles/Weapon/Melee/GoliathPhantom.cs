using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Ichor;
 
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Peripherals.RGB;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
	public class GoliathPhantom : ModProjectile
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

        public SoundStyle Hit = new SoundStyle("DestroyerTest/Assets/Audio/Impacts/ShortShine", 3) with { PitchVariance = 1.0f, MaxInstances = 0 };

        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 78;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 80;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Main.spriteBatch.UseBlendState(BlendState.Additive);
			Opus.DrawProjectileShadowsRotating(Projectile, 4, Color.Red, Opacity: 0.35f);
			var T = TextureAssets.Projectile[Projectile.type].Value;

            //Opus.ReturnToDefaultDrawing(Main.spriteBatch);

            Main.EntitySpriteDraw(T, Projectile.Center, null, Color.Red, Projectile.rotation, T.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            
            return false;
        }

        public override void AI()
        {
            NPC target = FindClosestNPC();
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

        public override bool? CanHitNPC(NPC target)
        {
            return State == AIState.Dashing && Projectile.ManualCanHitFriendly(target);
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

            Spark Spark = new Spark();
            Spark.PrepareSpark(Projectile.Center, Projectile.velocity * 0.1f, 0f, Color.Red, 0.5f, false, 30, SparkDrawMode.Additive);
            ParticleEngine.BehindProjectiles.Add(Spark);

            if (Timer == 1f) // first tick of dashing phase
            {
                SoundEngine.PlaySound(DTAssetLib.SwordSounds.SwiftSwing with { PitchVariance = 0.4f }, Projectile.Center);
                Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = direction * 50f;
                Projectile.netUpdate = true;
            }
        }

        private NPC FindClosestNPC()
        {
            NPC closest = null;
            float minDistance = float.MaxValue;

            foreach (NPC n in Main.npc)
            {
                if (n.active)
                {
                    float dist = Vector2.Distance(n.Center, Projectile.Center);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        closest = n;
                    }
                }
            }

            return closest;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(Hit, Projectile.Center);
        }
    }
}