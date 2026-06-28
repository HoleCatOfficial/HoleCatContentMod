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
using System.Linq;

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

            ProjectileID.Sets.TrailCacheLength[Type] = 300;
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2400;
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
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
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

        public float trailOffset = 0f;
        public int WOffset = 0;

        public float WarnOpacity = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            Asset<Texture2D> texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type];
            DTUtils Utility = new DTUtils();
            trailOffset += 0.04f;
            WOffset += 3;

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


            Vector2 ScrCTR = Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
            Line l = new Line(new Vector2(InitialPos.X, InitialPos.Y - 4000), new Vector2(InitialPos.X, InitialPos.Y));
            if (Projectile.ai[2] == 1)
            {
                l = new Line(new Vector2(InitialPos.X - 4000, InitialPos.Y), new Vector2(InitialPos.X, InitialPos.Y));
            }
            if (Projectile.ai[2] == 2)
            {
                l = new Line(new Vector2(InitialPos.X + 4000, InitialPos.Y), new Vector2(InitialPos.X, InitialPos.Y));
            }
            DTUtils.instance.ScrollingTextureSpine(l, DTAssetLib.SoulStreak, Color.MediumPurple * WarnOpacity, Main.spriteBatch, BlendState.Additive, WOffset, 1f);
            

            DTTrail.DrawTrail(sb, DTAssetLib.SoulStreak.Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 16, Color.MediumPurple, trailOffset, 5);
            
            Opus.ReturnToDefaultDrawing(sb);

            sb.Draw(texture.Value, drawPos, sourceRect, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        public Vector2 InitialPos;

        public override void OnSpawn(IEntitySource source)
        {
            InitialPos = Projectile.Center;
            WarnOpacity = 1f;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();
            AnimateProjectile();
            Dust.NewDustPerfect(Projectile.Center, DustID.DemonTorch, Scale: 1.8f);

            float maxDetectRadius = 120f; // The maximum radius at which a projectile can detect a target

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            if (WarnOpacity > 0)
            {
                WarnOpacity -= 0.02f;
            }

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

    public class TormentedSoulWarnLine : ModPlayer
    {

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {

            
        }
        
    }
}