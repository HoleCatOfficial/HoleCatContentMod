
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Security.Policy;
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.BossBar;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.CursedFlame;
using InnoVault;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Entities
{

    [AutoloadBossHead]
    public class CursedFlameNodeMB : ModNPC
    {
        public override string BossHeadTexture => "DestroyerTest/Content/Entities/CursedFlameNode_Head_Boss";
        public override void SetStaticDefaults()
        {
            NPCID.Sets.CanHitPastShimmer[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.TrailCacheLength[Type] = 20;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            { // Influences how the NPC looks in the Bestiary
                CustomTexturePath = "DestroyerTest/Content/Entities/NodesBestiary", // If the NPC is multiple parts like a worm, a custom texture for the Bestiary is encouraged.
                Position = Vector2.Zero,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 100;
            NPC.aiStyle = -1;
            NPC.damage = 25;
            NPC.defense = 24;
            NPC.lifeMax = 22000;
            NPC.HitSound = new SoundStyle("DestroyerTest/Assets/Audio/NodeHit");
            NPC.DeathSound = new SoundStyle("DestroyerTest/Assets/Audio/NodeExplode");
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.timeLeft = 150000;
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.netUpdate = true;
            NPC.netID = ModContent.NPCType<CursedFlameNodeMB>();
            NPC.BossBar = ModContent.GetInstance<CorruptBossBar>();
            NPC.alpha = 255;
            NPC.friendly = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("Elemental Constructs that strengthen the potency of Cursed Flames and Ichor."),
                new FlavorTextBestiaryInfoElement("They are often times found idly floating above the ground. Though the nodes will become retaliatory if provoked."),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCorrupt == true && spawnInfo.Player.ZoneOverworldHeight == true && DownedBossSystem.downedPlanteraBoss == true)
            {
                return 0.01f;
            }
            return 0f;
        }

        public override bool CheckActive()
        {
            return false;
        }
            

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return !DTUtils.NodeCharmEquipped;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.friendly)
                return !DTUtils.NodeCharmEquipped; // prevent friendly damage when charm is equipped

            // hostile projectiles behave normally
            return null;
        }



        public enum AttackState
        {
            Dormant,
            Idle,
            Stars,
            FlameSwarm,
            Mines,
            Napalm
        }

        public AttackState CurrentAttack;

        public int DormantPulseTimer = 60;
        public bool HasBuffed = false;
        public bool HasDebuffed = false;
        public int IdleTimer = 60;
        public float RotSpeed;
        public int StarShootID = ModContent.ProjectileType<TenebrisStarHostile>();
        public int FlameSwarmTimer = 0;
        public int StarShootCount = 0;
        public int MineInterval = 0;
        public int MineCount = 0;
        public int MineCooldown = 240;
        public int NapalmRainTimer = 800;
        public int NapalmRainInterval = 0;
        public bool RecordCenterFlag1 = false;
        public Vector2 screenCenter;
        public SoundStyle StarShoot = new SoundStyle("DestroyerTest/Assets/Audio/NodeAttackTS") with { MaxInstances = 0, PitchVariance = 1, Volume = 2 };
        public SoundStyle Wallwarn = new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/CursedFlamesWarn") with { MaxInstances = 0, PitchVariance = 1 };
        public SoundStyle WallShoot1 = new SoundStyle("DestroyerTest/Assets/Audio/FlameWall") with { MaxInstances = 0, PitchVariance = 1, Volume = 2 };
        public SoundStyle NapalmShoot = new SoundStyle("DestroyerTest/Assets/Audio/NodeAttackNapalm") with { MaxInstances = 0, PitchVariance = 1 };
        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            DTUtils Utility = new DTUtils();
            DTMusicConfig muscfg = ModContent.GetInstance<DTMusicConfig>();

            if (NPC.alpha > 0)
            {
                NPC.immortal = true;
                NPC.alpha--;
            }
            else
            {
                NPC.immortal = false;
            }

            if (player.active == false || player.dead == true)
            {
                CurrentAttack = AttackState.Dormant;
            }


            TryFindTileBelow();

            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/NodeBoss");
            }
            if (!Main.dedServ && CurrentAttack == AttackState.Dormant && muscfg.NodeIdleMusic == true)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/NodeIdle");
            }
            if (!Main.dedServ && CurrentAttack == AttackState.Dormant && muscfg.NodeIdleMusic == false)
            {
                Music = MusicID.Corruption;
            }

            if (Main.expertMode && !Main.masterMode)
            {
                StarShootID = ModContent.ProjectileType<CursedNodeCrystal>();
            }
            if ((Main.expertMode && DownedBossSystem.downedLunarBoss) || Main.masterMode)
            {
                StarShootID = ModContent.ProjectileType<TenebrisStarHostile>();
            }
            if (!Main.expertMode && !Main.masterMode)
            {
                StarShootID = ProjectileID.CursedFlameHostile;
            }

            Vector2 PRTPos;
            PRTPos = NPC.Center;

            switch (CurrentAttack)
            {
                case AttackState.Dormant:
                    {
                        DormantPulseTimer--;
                        if (DormantPulseTimer <= 0)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_WitherBeastAuraPulse, NPC.Center);
                            Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), NPC.Center, Vector2.Zero, ColorLib.CursedFlames, 0.01f, 1f);
                            DormantPulseTimer = 120;
                        }

                        float bobSpeed = 0.03f;
                        float bobHeight = 16f;
                        NPC.velocity.Y = (float)Math.Sin(Main.GameUpdateCount * bobSpeed) * 0.5f;
                        NPC.position.Y += (float)Math.Sin(Main.GameUpdateCount * bobSpeed) * (bobHeight / 100f);


                        foreach (NPC npc in Main.npc)
                        {
                            if (npc.Center.Distance(NPC.Center) < 1000
                            && npc.type != ModContent.NPCType<IchorNodeMB>()
                            && npc.type != ModContent.NPCType<CursedFlameNodeMB>()
                            && npc.type != ModContent.NPCType<IchorNode>()
                            && npc.type != ModContent.NPCType<CursedFlameNode>())
                            {
                                npc.AddBuff(ModContent.BuffType<NodePower>(), 60);
                            }
                        }

                        foreach (Player p in Main.player)
                        {
                            
                            if (p.Center.Distance(NPC.Center) < 1000 && DTUtils.NodeCharmEquipped)
                            {
                                p.AddBuff(ModContent.BuffType<NodePower>(), 60);
                            }
                        }

                        if (NPC.justHit && !DTUtils.NodeCharmEquipped)
                        {
                            CurrentAttack = AttackState.Idle;
                        }
                        break;
                    }
                case AttackState.Idle:
                    {
                        KeepToPlayer(player.Center + new Vector2(0, -200));
                        if (IdleTimer > 0)
                        {
                            IdleTimer--;
                        }

                        if (IdleTimer <= 0)
                        {
                            CurrentAttack = AttackState.Stars;
                            IdleTimer = 60;
                        }
                        break;
                    }
                case AttackState.Stars:
                    {
                        float XOffMin = -350;
                        float XOffMax = 350;
                        float XOff = Opus.Sine(XOffMin, XOffMax, 0.05f);
                        KeepToPlayer(player.Center + new Vector2(XOff, -200));

                        if (Main.GameUpdateCount % 90 == 0)
                        {
                            Stars();
                        }

                        if (StarShootCount >= 6)
                        {
                            CurrentAttack = AttackState.Mines;
                            StarShootCount = 0;
                        }

                        break;
                    }
                case AttackState.Mines:
                    {
                        KeepToPlayer(player.Center + new Vector2(0, -200));

                        if (MineInterval > 0)
                        {
                            MineInterval--;
                        }

                        if (MineInterval <= 0)
                        {
                            for (int q = 0; q < 12; q++)
                            {
                                Vector2 Position = new Vector2(player.Center.X + Main.rand.Next(-1000, 1000), player.Center.Y + Main.rand.Next(-1000, 1000));
                                Projectile Mine = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), Position, Vector2.Zero, ModContent.ProjectileType<BlossomMine>(), 30, 5);
                                Mine.timeLeft = 60;
                            }
                            MineInterval = 120;
                            MineCount += 1;
                        }

                        if (MineCount >= 3)
                        {
                            MineCooldown--;
                        }

                        if (MineCount >= 3 && MineCooldown <= 0)
                        {
                            CurrentAttack = AttackState.FlameSwarm;
                            MineCount = 0;
                            MineCooldown = 240;
                        }
                        break;
                    }
                case AttackState.FlameSwarm:
                {
                    KeepToPlayer(player.Center + new Vector2(0, -200));
                    
                    FlameSwarmTimer++;
                    if (FlameSwarmTimer % 60 == 0) // Spawn every 60 ticks (1 second)
                    {
                        FlameSwarm();
                    }
                    
                    if (FlameSwarmTimer > 300) // After 5 seconds, switch to Napalm
                    {
                        CurrentAttack = AttackState.Napalm;
                        FlameSwarmTimer = 0;
                    }
                    break;
                }
                case AttackState.Napalm:
                    {
                        KeepToPlayer(player.Center + new Vector2(0, -200));

                        Vector2 Velocity = new Vector2(Main.rand.Next(-20, 20), -15);

                        if (NapalmRainTimer > 0)
                        {
                            if (NapalmRainInterval <= 0)
                            {
                                SoundEngine.PlaySound(NapalmShoot, NPC.Center);
                                Projectile.NewProjectile(Entity.GetSource_FromThis(), NPC.Center, Velocity, ModContent.ProjectileType<CursedFlameNapalm>(), 20, 2);
                                NapalmRainInterval = 5;
                            }
                            NapalmRainTimer--;
                        }

                        if (NapalmRainInterval > 0)
                        {
                            NapalmRainInterval--;
                        }

                        if (NapalmRainTimer <= 0)
                        {
                            CurrentAttack = AttackState.Idle;
                            NapalmRainTimer = 800;
                        }
                        break;
                    }
            }
        }

        public void TryFindTileBelow()
        {
            Vector2 Probe = NPC.Center + new Vector2(0, 400);

            int left = (int)(Probe.X / 16);
            int right = (int)((Probe.X + NPC.width) / 16);
            int top = (int)(Probe.Y / 16);
            int bottom = (int)((Probe.Y + NPC.height) / 16);

            bool surrounded =
                Collision.SolidTiles(left - 1, right + 1, top - 1, bottom + 1) &&
                Collision.SolidTiles(left, right, top - 1, top - 1) && // Above
                Collision.SolidTiles(left, right, bottom + 1, bottom + 1) && // Below
                Collision.SolidTiles(left - 1, left - 1, top, bottom) && // Left
                Collision.SolidTiles(right + 1, right + 1, top, bottom); // Right

            if (surrounded)
            {
                NPC.velocity.Y = -6f;
                NPC.Center += new Vector2(0, -16);
            }
        }

        public void TryFindAirTile(Vector2 Probe, out bool surrounded)
        {
            int left = (int)(Probe.X / 16);
            int right = (int)((Probe.X + 32) / 16);
            int top = (int)(Probe.Y / 16);
            int bottom = (int)((Probe.Y + 32) / 16);

            surrounded =
                Collision.SolidTiles(left - 1, right + 1, top - 1, bottom + 1) &&
                Collision.SolidTiles(left, right, top - 1, top - 1) && // Above
                Collision.SolidTiles(left, right, bottom + 1, bottom + 1) && // Below
                Collision.SolidTiles(left - 1, left - 1, top, bottom) && // Left
                Collision.SolidTiles(right + 1, right + 1, top, bottom); // Right
        }

        public void KeepToPlayer(Vector2 CTR)
        {
            // Calculate vector from NPC to target
            Vector2 toTarget = CTR - NPC.Center;

            // If distance is too small, slow it down
            float distance = toTarget.Length();
            if (distance > 32f)
            {
                // Move half-way towards target
                NPC.velocity = toTarget * 0.25f;
            }
            else
            {
                // Move slower if close
                NPC.velocity = toTarget * 0.25f;
            }
        }

        public void Stars()
        {
            SoundEngine.PlaySound(StarShoot, NPC.Center);
            Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), NPC.Center, Vector2.Zero, ColorLib.CursedFlames, 0.01f, 1f);
            Opus.RadialSpreadProjectile(ModContent.ProjectileType<NightmareRoseCursedCrystal>(), 9, NPC.Center, 16, 4, 10, AI1: 1, RandomOffset: true);
            Opus.RadialSpreadProjectile(ModContent.ProjectileType<NightmareRoseCursedCrystal>(), 9, NPC.Center, 16, 4, 10, AI1: -1, RandomOffset: true);
            StarShootCount += 1;
        }

        public void FlameSwarm(Player player)
        {
            SoundEngine.PlaySound(Wallwarn, NPC.Center);
            Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), NPC.Center, Vector2.Zero, ColorLib.CursedFlames, 0.01f, 1f);
            
            for (int i = 0; i < 2; i++)
            {
                Vector2 spawnPos = new Vector2(player.Center.X + Main.rand.Next(-200, 201), player.Center.Y + 1000);
                Vector2 velocity = (player.Center - spawnPos).SafeNormalize(Vector2.Zero) * 10f; // Adjust speed as needed
                Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), spawnPos, velocity, ProjectileID.CursedFlameHostile, 20, 2);
            }
        }

        public void FlameSwarm()
        {
            
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CursedNodeLootBag>()));
        }
    }
}