
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

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            if (CurrentAttack == AttackState.FlameGrid && FlameGridWarnTimer > 0)
            {
                int rows = 10;
                int cols = 12;
                float spacing = 200f;

                // Pulse alpha for a "warning" effect
                float pulse = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f) * 0.5f + 0.5f;
                float alpha = MathHelper.Clamp((float)FlameGridWarnTimer / 60f, 0f, 1f); // fades out as timer runs
                Color warnColor = ColorLib.CursedFlames * (0.5f + 0.5f * pulse) * alpha;

                // Horizontal lines
                for (int y = 0; y < rows; y++)
                {
                    float lineY = screenCenter.Y + (y - rows / 2) * spacing;
                    Rectangle lineRect = new Rectangle(
                        (int)(screenCenter.X - (cols / 2f) * spacing - screenPos.X),
                        (int)(lineY - screenPos.Y),
                        (int)(cols * spacing),
                        2 // thickness
                    );
                    spriteBatch.Draw(pixel, lineRect, warnColor);
                }

                // Vertical lines
                for (int x = 0; x < cols; x++)
                {
                    float lineX = screenCenter.X + (x - cols / 2) * spacing;
                    Rectangle lineRect = new Rectangle(
                        (int)(lineX - screenPos.X),
                        (int)(screenCenter.Y - (rows / 2f) * spacing - screenPos.Y),
                        2, // thickness
                        (int)(rows * spacing)
                    );
                    spriteBatch.Draw(pixel, lineRect, warnColor);
                }
            }
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
            FlameGrid,
            Mines,
            Napalm
        }

        public AttackState CurrentAttack;

        public int DormantPulseTimer = 60;
        public bool HasBuffed = false;
        public bool HasDebuffed = false;
        public int IdleTimer = 60;
        public float RotSpeed;
        public int StarShootID = ModContent.ProjectileType<TenebrisStar>();
        public int StarShootInterval = 0;
        public int StarShootCount = 0;
        public int MineInterval = 0;
        public int MineCount = 0;
        public int MineCooldown = 240;
        public int FlameGridWarnTimer = 120;
        public bool ShotGrid = false;
        public int FlameGridCooldownTimer = 240;
        public bool WarnSound1 = false;
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
                StarShootID = ModContent.ProjectileType<TenebrisStar>();
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
                        KeepToPlayer(player.Center + new Vector2(0, -200));

                        int numVectors = Main.rand.Next(8, 23);
                        float angleStep = MathHelper.TwoPi / numVectors;
                        float baseAngle = 0f;
                        int StartRad = 22;


                        // Decide randomly if it's negative or positive
                        if (Main.rand.NextBool()) // 50/50 chance
                        {
                            // Negative range: -24 to -12
                            RotSpeed = Main.rand.NextFloat(-16f, -8f);
                        }
                        else
                        {
                            // Positive range: 12 to 24
                            RotSpeed = Main.rand.NextFloat(8f, 16f);
                        }

                        if (StarShootInterval > 0)
                        {
                            StarShootInterval--;
                        }

                        if (StarShootInterval <= 0)
                        {
                            SoundEngine.PlaySound(StarShoot, NPC.Center);
                            Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), NPC.Center, Vector2.Zero, ColorLib.CursedFlames, 0.01f, 1f);
                            Opus.RadialSpreadProjectile(ModContent.ProjectileType<NightmareRoseCursedCrystal>(), 9, NPC.Center, 16, 4, 10, AI1: 1, RandomOffset: true);
                            Opus.RadialSpreadProjectile(ModContent.ProjectileType<NightmareRoseCursedCrystal>(), 9, NPC.Center, 16, 4, 10, AI1: -1, RandomOffset: true);
                            StarShootInterval = 60;
                            StarShootCount += 1;
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
                            CurrentAttack = AttackState.FlameGrid;
                            MineCount = 0;
                            MineCooldown = 240;
                        }
                        break;
                    }
                case AttackState.FlameGrid:
                    {
                        KeepToPlayer(player.Center + new Vector2(0, -200));

                        if (!RecordCenterFlag1)
                        {
                            screenCenter = Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
                            RecordCenterFlag1 = true;
                        }

                        int rows = 10;
                        int cols = 12;
                        float spacing = 200f; // distance between lines


                        if (FlameGridWarnTimer > 0)
                        {
                            if (WarnSound1 == false)
                            {
                                SoundEngine.PlaySound(Wallwarn);
                                WarnSound1 = true;
                            }
                            FlameGridWarnTimer--;
                        }
                        if (!ShotGrid && FlameGridWarnTimer <= 0)
                        {
                            SoundEngine.PlaySound(WallShoot1);
                            player.GetModPlayer<ScreenshakePlayer>().screenshakeTimer = 10;
                            player.GetModPlayer<ScreenshakePlayer>().screenshakeMagnitude = 2;
                            Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), NPC.Center, Vector2.Zero, ColorLib.CursedFlames, 0.01f, 1f);

                            // Horizontal walls (one per row)
                            for (int y = 0; y < rows; y++)
                            {
                                float lineY = screenCenter.Y + (y - rows / 2) * spacing;
                                Projectile.NewProjectile(
                                    Entity.GetSource_FromThis(),
                                    new Vector2(screenCenter.X, lineY),
                                    Vector2.Zero,
                                    ModContent.ProjectileType<CursedFlameWallHorizontal>(),
                                    60,
                                    2
                                );
                            }

                            // Vertical walls (one per column)
                            for (int x = 0; x < cols; x++)
                            {
                                float lineX = screenCenter.X + (x - cols / 2) * spacing;
                                Projectile.NewProjectile(
                                    Entity.GetSource_FromThis(),
                                    new Vector2(lineX, screenCenter.Y),
                                    Vector2.Zero,
                                    ModContent.ProjectileType<CursedFlameWallVertical>(),
                                    60,
                                    2
                                );
                            }

                            ShotGrid = true;
                        }

                        if (ShotGrid)
                        {
                            FlameGridCooldownTimer--;
                            if (FlameGridCooldownTimer <= 0)
                            {
                                CurrentAttack = AttackState.Napalm;
                                screenCenter = Vector2.Zero;
                                RecordCenterFlag1 = false;
                                FlameGridWarnTimer = 120;
                                FlameGridCooldownTimer = 240;
                                ShotGrid = false;
                            }
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


        public void TelegraphFlames()
        {


        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CursedNodeLootBag>()));
        }
    }
}