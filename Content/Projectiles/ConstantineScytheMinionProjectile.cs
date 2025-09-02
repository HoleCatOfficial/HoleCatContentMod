using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.DataStructures;
using System.IO;
using tModPorter;
using System;
using Terraria.ModLoader.IO;
using System.Collections.Generic;
using DestroyerTest.Content.MeleeWeapons;
using ReLogic.Content;

namespace DestroyerTest.Content.Projectiles
{
    public class ConstantineScytheMinionProjectile : ModProjectile
    {
        public override string GlowTexture => $"{Texture}_Glow";
        private bool spawned;
        private NPC HomingTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }

        public override void SetDefaults()
        {
            Projectile.width = 94;
            Projectile.height = 102;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = false;
            Projectile.netImportant = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(spawned);
            writer.WriteVector2(Projectile.velocity);
            writer.WriteVector2(Projectile.Center);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            spawned = reader.ReadBoolean();
            Projectile.velocity = reader.ReadVector2();
            Projectile.Center = reader.ReadVector2();
        }

        public int DashCooldown = -1;
        private Dictionary<int, int> npcHitCooldowns = new Dictionary<int, int>();
        private const int HitCooldownTicks = 100;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CheckActive(player);
            Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;


            Vector2 toPlayer = player.Center - Projectile.Center;
            if (HomingTarget == null)
            {
                float speed = 0.2f;
                if (toPlayer.Length() < 200f) speed = 0.12f;
                if (toPlayer.Length() < 140f) speed = 0.06f;

                if (toPlayer.Length() > 100f)
                {
                    if (Math.Abs(toPlayer.X) > 20f)
                        Projectile.velocity.X += speed * Math.Sign(toPlayer.X);
                    if (Math.Abs(toPlayer.Y) > 10f)
                        Projectile.velocity.Y += speed * Math.Sign(toPlayer.Y);
                }
                else if (Projectile.velocity.Length() > 2f)
                    Projectile.velocity *= 0.96f;

                if (Math.Abs(Projectile.velocity.Y) < 1f)
                    Projectile.velocity.Y -= 0.1f;

                float maxSpeed = 15f;
                if (Projectile.velocity.Length() > maxSpeed)
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * maxSpeed;

                Projectile.rotation += 0.01f * Projectile.velocity.X;

                // Desync catch-up
                if (Main.myPlayer == player.whoAmI && Projectile.DistanceSQ(player.Center) > 2000 * 2000)
                {
                    Projectile.position = player.Center;
                    Projectile.velocity *= 0.05f;
                    Projectile.netUpdate = true;
                }
            }

            float maxDetectRadius = 1400f; // The maximum radius at which a projectile can detect a target

            // First, we find a homing target if we don't have one
            if (HomingTarget == null)
            {
                HomingTarget = FindClosestNPC(maxDetectRadius);
            }

            // If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
            if (HomingTarget != null && !IsValidTarget(HomingTarget))
            {
                HomingTarget = null;
            }

            // If we don't have a target, don't adjust trajectory
            if (HomingTarget == null)
                return;

            if (DashCooldown > 0)
            DashCooldown--;
            
            if (HomingTarget != null)
            {
                int npcID = HomingTarget.whoAmI;

                // Check hit cooldown
                if (npcHitCooldowns.ContainsKey(npcID) && npcHitCooldowns[npcID] > 0)
                {
                    npcHitCooldowns[npcID]--;
                    HomingTarget = null; // temporarily lose target
                    return;
                }
                else
                {
                    float length = Projectile.velocity.Length() * 1.5f;
                    length = MathHelper.Clamp(length, 3f, 28f);
                    float targetAngle = Projectile.AngleTo(HomingTarget.Center);
                    Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(18)).ToRotationVector2() * length;
                    Projectile.rotation += 0.03f * Projectile.velocity.X;
                }
                
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int npcID = target.whoAmI;
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/SOJ-M_Slash") with { PitchVariance = 0.3f }, Projectile.position);
            DashCooldown = 120;
            npcHitCooldowns[npcID] = HitCooldownTicks;
            Projectile.netUpdate = true;
        }

        private void CheckActive(Player player)
        {
            if (!player.dead && player.HeldItem.type == ModContent.ItemType<ConstantineScythe>())
                Projectile.timeLeft = 2;
            else
                Projectile.timeLeft = 0;
        }

        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs
            foreach (var target in Main.ActiveNPCs)
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
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public bool IsValidTarget(NPC target)
        {
            // This method checks that the NPC is:
            // 1. active (alive)
            // 2. chaseable (e.g. not a cultist archer)
            // 3. max life bigger than 5 (e.g. not a critter)
            // 4. can take damage (e.g. moonlord core after all it's parts are downed)
            // 5. hostile (!friendly)
            // 6. not immortal (e.g. not a target dummy)
            // 7. doesn't have solid tiles blocking a line of sight between the projectile and NPC
            return target.CanBeChasedBy();
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, 0f, 0f, 150, default(Color), 1.5f);
                Main.dust[dustIndex].velocity *= 1.4f;
            }
            int Gore1 = Mod.Find<ModGore>("ScytheMinionGore1").Type;
            int Gore2 = Mod.Find<ModGore>("ScytheMinionGore2").Type;
            int Gore3 = Mod.Find<ModGore>("ScytheMinionGore3").Type;

            var entitySource = Projectile.GetSource_Death();
            Gore.NewGore(entitySource, Projectile.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(0, 10)), Gore1);
            Gore.NewGore(entitySource, Projectile.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(0, 10)), Gore2);
            Gore.NewGore(entitySource, Projectile.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(0, 10)), Gore3);
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/NodeExplode"), Projectile.position);
        }
        
        public override void PostDraw(Color lightColor)
        {
            if (Main.mapFullscreen) return;

            Vector2 mapPos = Main.mapFullscreen ? Vector2.Zero : Main.LocalPlayer.Center; // adjust for map offset
            Asset<Texture2D> icon = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/ScytheMapIcon");
            Main.spriteBatch.Draw(icon.Value, mapPos - Main.screenPosition, Color.White);
        }
    }
}