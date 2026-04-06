
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
using OpusLib.Content.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
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
            bool NodeAlive = false;
            foreach (NPC npc in Main.npc)
            {
                if (NPC.active && npc.type == Type)
                {
                    NodeAlive = true;
                }
            }
            if (spawnInfo.Player.ZoneCorrupt == true && spawnInfo.Player.ZoneOverworldHeight == true && DownedBossSystem.downedPlanteraBoss == true && !NodeAlive)
            {
                return 0.1f;
            }
            return 0f;
        }

        public override bool CheckActive()
        {
            return true;
        }

        public float ShieldOpacity = 0f;
        public float ShieldScale = 1f;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            var v = DTAssetLib.BloomRingSharp.Value;

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(v, NPC.Center - screenPos, null, ColorLib.WretchedGradient() * ShieldOpacity, 0f, v.Size() / 2, ShieldScale, SpriteEffects.None);
            Utils.DrawBorderString(spriteBatch, $"{DormantNPCKillTally} / {DormantNPCKillRequirement}", (NPC.Center + new Vector2(0, -90)) - screenPos, ColorLib.WretchedGradient() * ShieldOpacity, 3f, 0.5f, 0.5f);
            Opus.ReturnToDefaultDrawing(spriteBatch);
            return true;
        }
        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return !DTUtils.NodeCharmEquipped && !(DormantNPCKillTally < DormantNPCKillRequirement);
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.friendly)
                return !DTUtils.NodeCharmEquipped && !(DormantNPCKillTally < DormantNPCKillRequirement); ; // prevent friendly damage when charm is equipped

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
        public int DormantNPCKillTally = 0;
        public const int DormantNPCKillRequirement = 50;

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
        public bool Flag2 = false;
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

            if ((DormantNPCKillTally < DormantNPCKillRequirement))
            {
                ManageShieldIn();
            }

            if (DormantNPCKillTally >= DormantNPCKillRequirement)
            {
                if (!Flag2)
                {
                    SoundEngine.PlaySound(DTAssetLib.ScholarShieldSounds.Activate, NPC.Center);
                    Flag2 = true;
                }
                ManageShieldOut();
            }

            switch (CurrentAttack)
            {
                case AttackState.Dormant:
                    {
                        
                        DormantAI();
                        if ((DormantNPCKillTally < DormantNPCKillRequirement))
                        {
                            NPC.immortal = true;
                            NPC.dontTakeDamage = true;
                        }
                        else
                        {
                            NPC.immortal = false;
                            NPC.dontTakeDamage = false;
                        }

                        if (NPC.justHit && !DTUtils.NodeCharmEquipped && !(DormantNPCKillTally < DormantNPCKillRequirement))
                        {
                            CurrentAttack = AttackState.Idle;
                        }
                        break;
                    }
                case AttackState.Idle:
                    {
                        NPC.npcSlots = 10f;
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
                    if (FlameSwarmTimer % 20 == 0) // Spawn every 60 ticks (1 second)
                    {
                        FlameSwarm(player);
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

        public void ManageShieldIn()
        {
            if (ShieldScale > 0.1f)
            {
                ShieldScale -= 0.01f;
            }
            if (ShieldOpacity < 1)
            {
                ShieldOpacity += 0.05f;
            }
        }

        public void ManageShieldOut()
        {
            if (ShieldScale < 1f)
            {
                ShieldScale += 0.05f;
            }
            if (ShieldOpacity > 0)
            {
                ShieldOpacity -= 0.1f;
            }
        }

        public void DormantAI()
        {
            DormantPulseTimer--;
            if (DormantPulseTimer <= 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastAuraPulse, NPC.Center);
                Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), NPC.Center, Vector2.Zero, ColorLib.CursedFlames, 0.01f, 1f);
                DormantPulseTimer = 120;
            }

            NPC.velocity.Y = Opus.Sine(1f, -1f, 0.01f);


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

            Vector2[] P = Opus.GetEquidistantOrbitVectors(16, NPC.Center, 0.1f, 1200);

            for (int i = 0; i < P.Length; i++)
            {
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), P[i], Vector2.Zero, ColorLib.WretchedGradient(), 1f);
            }

            foreach (Player p in Main.player)
            {
                if (p.Center.Distance(NPC.Center) < 1200)
                {
                    if (DTUtils.NodeCharmEquipped)
                    {
                        p.AddBuff(ModContent.BuffType<NodePower>(), 60);
                    }

                    if (DormantNPCKillTally < DormantNPCKillRequirement)
                    {
                        SpawnNPCWave();
                    }
                }
            }
        }

        public int SpawnNPCTimer = 0;
        public static string NPCIdentifierContext = "CusedFlameNodeWaveEnemy";

        public static List<int> CursedFlameNodeWaveEnemies = new List<int>
        {
            NPCID.EaterofSouls,
            NPCID.Corruptor,
            NPCID.Slimer,
            NPCID.DevourerHead,
            NPCID.SeekerHead
        };

        public int WaveTimeout = 0;
        public void SpawnNPCWave()
        {
            SpawnNPCTimer++;
            WaveTimeout++;
            Vector2[] SpawnPositions = Opus.GetEquidistantVectors(5, NPC.Center, 250);

            if ((SpawnNPCTimer % 300 == 0 && CFNGlobal.WaveNPCCount == 0) || WaveTimeout > 1800)
            {
                if (WaveTimeout > 1800)
                {
                    CombatText.NewText(NPC.Hitbox, Color.Red, "30 Seconds have passed. Wave failsafe intiated.");
                    Main.NewText("TALID: 30 Seconds have passed. Wave failsafe intiated.", Color.Red);

                    foreach (NPC child in Main.npc)
                    {
                        if (!child.active) continue;

                        var g = child.GetGlobalNPC<CFNGlobal>();

                        if (g.IsNodeSpawned && g.Node == this)
                        {
                            child.StrikeInstantKill();
                        }
                    }

                    DormantNPCKillTally = ((DormantNPCKillTally + 9) / 10) * 10;
                }
                WaveTimeout = 0;

                SoundEngine.PlaySound(DTAssetLib.Impacts.DarkMagicImpact);
                for (int i = 0; i < SpawnPositions.Length; i++)
                {
                    Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), SpawnPositions[i], Vector2.Zero, ColorLib.CursedFlames, 0.01f, 0.4f);
                    NPC wavenpc = NPC.NewNPCDirect(NPC.GetSource_FromAI(NPCIdentifierContext), SpawnPositions[i], CursedFlameNodeWaveEnemies[Main.rand.Next(CursedFlameNodeWaveEnemies.Count)]);
                    wavenpc.scale = 1.5f;
                    wavenpc.knockBackResist = 0f;
                    var g = wavenpc.GetGlobalNPC<CFNGlobal>();
                    g.Node = this;
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
            Opus.RadialSpreadProjectile(ModContent.ProjectileType<CursedNodeCrystal2>(), 9, NPC.Center, 16, 4, 10, AI1: 1, RandomOffset: true);
            Opus.RadialSpreadProjectile(ModContent.ProjectileType<CursedNodeCrystal2>(), 9, NPC.Center, 16, 4, 10, AI1: -1, RandomOffset: true);
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
                Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), spawnPos, velocity, ModContent.ProjectileType<CursedFlameVortex>(), 20, 2);
            }
        }

        public override void OnKill()
        {
            CFNGlobal.WaveNPCCount = 0;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CursedNodeLootBag>()));
        }
    }

    public class CFNGlobal : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public static int WaveNPCCount = 0;

        public bool IsNodeSpawned = false;

        public CursedFlameNodeMB Node = null;

        public override void Unload()
        {
            WaveNPCCount = 0;
            IsNodeSpawned = false;
            Node = null;
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Context == CursedFlameNodeMB.NPCIdentifierContext)
            {
                WaveNPCCount += 1;
                IsNodeSpawned = true;
            }

        }

        public int TexOffset = 0;
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Line L = new Line(npc.Center, Node.NPC.Center);
            TexOffset += 2;
            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(10), ColorLib.WretchedGradient(), spriteBatch, BlendState.Additive, TexOffset, 0.5f);
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (IsNodeSpawned)
            {
                Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
                Main.EntitySpriteDraw(DTAssetLib.CorruptSigil.Value, npc.Center - screenPos, null, ColorLib.CursedFlames * 0.5f, 0f, DTAssetLib.CorruptSigil.Value.Size() / 2, 0.15f, SpriteEffects.None, 0f);
                Opus.ReturnToDefaultDrawing(spriteBatch);
            }
        }

        public override void AI(NPC npc)
        {

            
        }

        public override bool CheckActive(NPC npc)
        {
            if (IsNodeSpawned)
            {
                return false;
            }
            return true;
        }

        public override void OnKill(NPC npc)
        {
            if (IsNodeSpawned)
            {
                if (Node != null)
                {
                    Node.DormantNPCKillTally += 1;
                }
                WaveNPCCount--;
            }

            if (npc.type == ModContent.NPCType<CursedFlameNodeMB>())
            {
                if (Node != null)
                {
                    Node.DormantNPCKillTally = 0;
                }
                WaveNPCCount = 0;
            }
        }
    }
}