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

namespace DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss
{
    public class TormentedSoul : ModProjectile
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

        public static bool EternityIsActive()
        {
            if (ModLoader.TryGetMod("FargowiltasSouls", out Mod frgo))
            {
                object result = frgo.Call("EternityMode");
                if (result is bool enabled)
                {
                    if (enabled)
                        return true;
                    else
                        return false;
                }
            }
            else
            {

            }
            return false;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24; // The width of projectile hitbox
            Projectile.height = 30; // The height of projectile hitbox

            Projectile.DamageType = DamageClass.Generic; // What type of damage does this projectile affect?
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.timeLeft = 300; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        private void AnimateProjectile()
        {
            // Loop through the frames, assuming each frame lasts 5 ticks
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        Vector2 SoulCenter;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            Asset<Texture2D> texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type];
            DTUtils Utility = new DTUtils();

            // Calculate source rectangle for current frame
            int frameHeight = texture.Value.Height / Main.projFrames[Projectile.type];
            Rectangle sourceRect = new Rectangle(0, Projectile.frame * frameHeight, texture.Value.Width, frameHeight);

            Vector2 origin = new Vector2(texture.Value.Width / 2f, frameHeight / 2f);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            Opus.StartSpriteBatchWithBlending(sb, BlendState.Additive, SpriteSortMode.Immediate);
            if (HomingTarget == null)
            {
                SoulCenter = Projectile.Center;
            }
            TelegraphLine(sb, SoulCenter);
            Opus.DrawGlowOnProj(Projectile, Color.Purple, false, 0f);
            sb.Draw(texture.Value, drawPos, sourceRect, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(sb);

            return false;
        }

        public void TelegraphLine(SpriteBatch SB, Vector2 soulPos)
        {
            float totalLength = 3600f;
            Vector2 start = IntialPos;


            float opacity = 0f;
            if (Projectile.timeLeft > 210)
            {
                // fade in
                float fadeProgress = (270f - Projectile.timeLeft) / 30f;
                opacity = MathHelper.Lerp(0f, 1f, fadeProgress);
            }
            else if (Projectile.timeLeft < 30)
            {
                float fadeProgress = (30f - Projectile.timeLeft) / 30f; // goes 0 → 1 between 30 and 0
                opacity = MathHelper.Lerp(1f, 0f, fadeProgress);
            }
            else
            {
                // fully visible in between
                opacity = 1f;
            }


            if (Projectile.active)
                {
                    // Direction from start to soul
                    Vector2 dir = soulPos - start;
                    if (dir != Vector2.Zero)
                        dir.Normalize();

                    float segmentLength = DTAssetLib.Line(4).Value.Height; // reuse asset’s height as step
                    int numSegments = (int)(totalLength / segmentLength);

                    float rotation = dir.ToRotation() - MathHelper.PiOver2;
                    // Pi/2 offset because your texture seems "upward" by default

                    for (int i = 0; i < numSegments; i++)
                    {
                        Vector2 segmentPos = start + dir * (i * segmentLength);
                        Vector2 drawPos = segmentPos - Main.screenPosition;

                        SB.Draw(
                            DTAssetLib.Line(4).Value,
                            drawPos,
                            null,
                            Color.MediumPurple * opacity,
                            rotation,
                            new Vector2(DTAssetLib.Line(4).Value.Width / 2f, 0f), // middle-bottom origin
                            1f,
                            SpriteEffects.None,
                            0f
                        );
                    }
                }
        }



        public Vector2 IntialPos;

        public override void OnSpawn(IEntitySource source)
        {
            IntialPos = Projectile.Center;
        }

        // Custom AI
        public override void AI()
        {
            AnimateProjectile();
            Dust.NewDustPerfect(Projectile.Center, DustID.DemonTorch, Scale: 1.8f);

            float maxDetectRadius = 120f; // The maximum radius at which a projectile can detect a target

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            // First, we find a homing target if we don't have one
            if (HomingTarget == null)
            {
                HomingTarget = FindClosestPlayer(maxDetectRadius);
            }

            // If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
            if (HomingTarget != null && !IsValidTarget(HomingTarget))
            {
                HomingTarget = null;
            }

            // If we don't have a target, don't adjust trajectory
            if (HomingTarget == null)
                return;

            if (!EternityIsActive())
            {
                float length = Projectile.velocity.Length();
                float targetAngle = Projectile.AngleTo(HomingTarget.Center);
                Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(30)).ToRotationVector2() * length;
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            }
            

        }

        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null
        public Player FindClosestPlayer(float maxDetectDistance)
        {
            Player closestPlayer = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs
            foreach (var target in Main.player)
            {
                // Check if NPC able to be targeted. 
                if (IsValidTarget(target))
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
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
            Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.DemonTorch, Projectile.velocity.X * 0.7f, Projectile.velocity.Y * 0.7f, 0, default, 1);
        }

    }
}