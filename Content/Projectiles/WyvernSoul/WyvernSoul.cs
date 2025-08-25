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
using DestroyerTest.Content.Entity;
using Terraria.GameContent.Drawing;

namespace DestroyerTest.Content.Projectiles.WyvernSoul
{
    public class WyvernSoulHead : ModProjectile
    {
        private bool spawned;

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = false;
            Projectile.netImportant = true;
            if (!Pet)
            {
                Projectile.DamageType = DamageClass.Magic;
            }
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

        public bool Pet = false;
        public Projectile Soul;
        public KeeperSoulProj SoulReference;
        public float RotationOffset = 0;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CheckActive(player);
            RotationOffset += 0.25f;

            if (Projectile.ai[2] == 2)
            {
                Pet = true;
            }
            else
            {
                Pet = false;
            }

            if (!spawned)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int latestProj = Projectile.whoAmI;
                    int[] Type = { 0, 0, 1, 0, 0, 1, 0, 0, 2, 3, 4, 5 };
                    for (int i = 0; i < Type.Length; ++i)
                    {
                        int bodyType = ModContent.ProjectileType<WyvernSoulBody>();
                        switch (Type[i])
                        {
                            case 1:
                                bodyType = ModContent.ProjectileType<WyvernSoulLegs>();
                                break;
                            case 2:
                                bodyType = ModContent.ProjectileType<WyvernSoulBody>();
                                break;
                            case 3:
                                bodyType = ModContent.ProjectileType<WyvernSoulBody2>();
                                break;
                            case 4:
                                bodyType = ModContent.ProjectileType<WyvernSoulBody3>();
                                break;
                            case 5:
                                bodyType = ModContent.ProjectileType<WyvernSoulTail>();
                                break;
                        }
                        latestProj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, bodyType, 0, 0, player.whoAmI, Projectile.whoAmI, latestProj);
                        ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.Keybrand, new ParticleOrchestraSettings() { PositionInWorld = Projectile.Center });
                    }
                }
                spawned = true;
            }

            if (Pet)
            {
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

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

                // Desync catch-up
                if (Main.myPlayer == player.whoAmI && Projectile.DistanceSQ(player.Center) > 2000 * 2000)
                {
                    Projectile.position = player.Center;
                    Projectile.velocity *= 0.1f;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ModContent.ProjectileType<KeeperSoulProj>())
                    {
                        Vector2 offset = new Vector2(400, 0);
                        Vector2 rotatedOffset = offset.RotatedBy(RotationOffset);
                        Vector2 finalPos = proj.Center + rotatedOffset;
                        Projectile.Center = finalPos;
                        Projectile.rotation = Projectile.Center.ToRotation() + MathHelper.PiOver2;
                    }
                    if (!proj.active && proj.type == ModContent.ProjectileType<KeeperSoulProj>())
                    {
                        Projectile.Center = player.Center;
                    }
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        private void CheckActive(Player player)
        {
            if (Pet)
            {
                if (!player.dead && player.HasBuff(ModContent.BuffType<WyvernSoulPetBuff>()))
                    Projectile.timeLeft = 2;
                else
                    Projectile.active = false;
            }
            else
            {
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ModContent.ProjectileType<KeeperSoulProj>())
                    {
                        Projectile.active = true;
                        Projectile.timeLeft = 2;
                    }
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.Keybrand, new ParticleOrchestraSettings() { PositionInWorld = Projectile.Center });
        }
    }

    public class WyvernSoulBody : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mini Gigapora");
            if (Head.Pet)
            {
                Main.projPet[Projectile.type] = true;
            }
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 44;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = true;
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
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public void CheckActive(Player player)
        {
            if (Head.Pet)
            {
                if (!player.dead && player.HasBuff(ModContent.BuffType<WyvernSoulPetBuff>()))
                {
                    Projectile.timeLeft = 2;
                }
            }
            if (proj.active && proj.type == ModContent.ProjectileType<WyvernSoulHead>())
            {
                Projectile.timeLeft = 2;
            }
        }

        public WyvernSoulHead Head = new WyvernSoulHead();
        public Projectile proj;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            proj = Main.projectile[(int)Projectile.ai[0]];
            CheckActive(player);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!proj.active || proj.type != ModContent.ProjectileType<WyvernSoulHead>())
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
                float desiredSpacing = 27.5f;
                if (distance > 0f)
                {
                    float moveFactor = (distance - desiredSpacing) / distance;
                    Projectile.position += toFollow * moveFactor;
                }

                // Face the segment we're following
                Projectile.rotation = toFollow.ToRotation() + MathHelper.PiOver2;

                Projectile.velocity = Vector2.Zero;

                // Flip sprite based on direction
                Projectile.spriteDirection = (toFollow.X < 0f) ? 1 : -1;
            }

        }

        public override void OnKill(int timeLeft)
        {
            ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.Keybrand, new ParticleOrchestraSettings() { PositionInWorld = Projectile.Center });
        }
    }

    public class WyvernSoulLegs : WyvernSoulBody
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mini Gigapora");
            if (Head.Pet)
            {
                Main.projPet[Projectile.type] = true;
            }
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 48;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = true;
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
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
    }

    public class WyvernSoulBody2 : WyvernSoulBody
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mini Gigapora");
            if (Head.Pet)
            {
                Main.projPet[Projectile.type] = true;
            }
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 48;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = true;
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
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
    }

    public class WyvernSoulBody3 : WyvernSoulBody
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mini Gigapora");
            if (Head.Pet)
            {
                Main.projPet[Projectile.type] = true;
            }
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 48;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = true;
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
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
    }

    public class WyvernSoulTail : WyvernSoulBody
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mini Gigapora");
            if (Head.Pet)
            {
                Main.projPet[Projectile.type] = true;
            }
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 48;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = true;
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
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
    }
    
    public class WyvernSoulPetBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.buffTime[buffIndex] = 18000;

			int projType = ModContent.ProjectileType<WyvernSoulHead>();

			if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
			{
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, new Vector2(4, -30), projType, 0, 0f, player.whoAmI);
			}
		}
	}

}