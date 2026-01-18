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
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Equips;

namespace DestroyerTest.Content.Projectiles.ShadeThrasherFriendly
{
    public class ShadeThrasherFriendlyHead : ModProjectile
    {
        private NPC HomingTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }
        private bool spawned;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }


        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.frame = 0;
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

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CheckActive(player);

            if (!spawned)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int latestProj = Projectile.whoAmI;
                    int[] Type = { 0, 2, 3, 4 };
                    for (int i = 0; i < Type.Length; ++i)
                    {
                        int bodyType = ModContent.ProjectileType<ShadeThrasherFriendlyBody1>();
                        switch (Type[i])
                        {
                            case 2:
                                bodyType = ModContent.ProjectileType<ShadeThrasherFriendlyBody2>();
                                break;
                            case 3:
                                bodyType = ModContent.ProjectileType<ShadeThrasherFriendlyBody3>();
                                break;
                            case 4:
                                bodyType = ModContent.ProjectileType<ShadeThrasherFriendlyBody4>();
                                break;
                        }
                        latestProj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, bodyType, 0, 0, player.whoAmI, Projectile.whoAmI, latestProj);
                    }
                }
                spawned = true;
            }

            if (HomingTarget == null)
            {
                Projectile.frame = 0;
                // Movement logic toward player
                Vector2 toPlayer = player.Center - Projectile.Center;
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

                Projectile.rotation = Projectile.velocity.ToRotation();
                // Desync catch-up
                if (Main.myPlayer == player.whoAmI && Projectile.DistanceSQ(player.Center) > 2000 * 2000)
                {
                    Projectile.position = player.Center;
                    Projectile.velocity *= 0.1f;
                    Projectile.netUpdate = true;
                }
            }

            float maxDetectRadius = 4000f;

            if (HomingTarget == null)
            {
                HomingTarget = FindClosestNPC(maxDetectRadius);
            }

            if (HomingTarget != null && !IsValidTarget(HomingTarget))
            {
                HomingTarget = null;
            }

            if (HomingTarget == null)
                return;

            Projectile.frame = 1;

            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(15)).ToRotationVector2() * 40f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff<ShadeThrasherBuff>())
            {
                Projectile.timeLeft = 2;
            }
        }

        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            foreach (var target in Main.ActiveNPCs)
            {
                if (IsValidTarget(target))
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

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
            return target.CanBeChasedBy();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 600);
        }
    }

    public class ShadeThrasherFriendlyBody1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 54;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(Projectile.velocity);
            writer.WriteVector2(Projectile.Center);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.velocity = reader.ReadVector2();
            Projectile.Center = reader.ReadVector2();
        }
        public void CheckActive(Player player)
        {
            if (!player.dead)
            {
                Projectile.timeLeft = 2;
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile proj = Main.projectile[(int)Projectile.ai[0]];
            CheckActive(player);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!proj.active || proj.type != ModContent.ProjectileType<ShadeThrasherFriendlyHead>())
                    Projectile.active = false;
            }
            if (Projectile.ai[1] >= 0 && Projectile.ai[1] < Main.maxProjectiles)
            {
                Projectile follow = Main.projectile[(int)Projectile.ai[1]];
                if (!follow.active)
                    return;

                Vector2 toFollow = follow.Center - Projectile.Center;
                float distance = toFollow.Length();

                // Maintain spacing of 48 pixels between segment centers
                float desiredSpacing = 30f;
                if (distance > 0f)
                {
                    float moveFactor = (distance - desiredSpacing) / distance;
                    Projectile.position += toFollow * moveFactor;
                }

                // Face the segment we're following
                Projectile.rotation = toFollow.ToRotation();

                Projectile.velocity = Vector2.Zero;

                // Flip sprite based on direction
                //Projectile.spriteDirection = (toFollow.X < 0f) ? 1 : -1;
            }

        }
    }

    public class ShadeThrasherFriendlyBody2 : ShadeThrasherFriendlyBody1
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 38;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(Projectile.velocity);
            writer.WriteVector2(Projectile.Center);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.velocity = reader.ReadVector2();
            Projectile.Center = reader.ReadVector2();
        }
    }

    public class ShadeThrasherFriendlyBody3 : ShadeThrasherFriendlyBody1
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 42;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(Projectile.velocity);
            writer.WriteVector2(Projectile.Center);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.velocity = reader.ReadVector2();
            Projectile.Center = reader.ReadVector2();
        }
    }

    public class ShadeThrasherFriendlyBody4 : ShadeThrasherFriendlyBody1
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 22;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(Projectile.velocity);
            writer.WriteVector2(Projectile.Center);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.velocity = reader.ReadVector2();
            Projectile.Center = reader.ReadVector2();
        }
    }
}