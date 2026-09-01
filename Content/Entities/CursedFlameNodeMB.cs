
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
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
using GlowmaskHelper.Content;
 
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
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
    [AutoloadGlowmask]
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
            NPC.lifeMax = 60000;
            NPC.HitSound = new SoundStyle("DestroyerTest/Assets/Audio/NodeHit");
            NPC.DeathSound = new SoundStyle("DestroyerTest/Assets/Audio/NodeExplode");
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.timeLeft = 150000;
            NPC.boss = false;
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
                new FlavorTextBestiaryInfoElement(DTUtils.GetModNPCLocalizationEntry(this, 1)),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            bool NodeAlive = false;
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type == Type)
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

        public override void OnSpawn(IEntitySource source)
        {
            ResetData();
        }

        public void ResetData()
        {
            SentinelKillTally = 0;
        }

        public override bool CheckActive()
        {
            return true;
        }

        public float ShieldOpacity = 0f;
        public float ShieldScale = 1f;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float progress = (float)SentinelKillTally / (float)SentinelKillRequirement;
            if (CurrentAttack == AttackState.Dormant)
            {

                DTUtils.DrawChargeBar(2f, (NPC.Center + new Vector2(0, 100)) - Main.screenPosition, progress, DTColorUtils.MultiLerp(progress, ColorLib.WretchedColorMap));
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            var v = DTAssetLib.BloomRingSharp.Value;


            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(v, NPC.Center - screenPos, null, ColorLib.WretchedGradient() * ShieldOpacity, 0f, v.Size() / 2, ShieldScale, SpriteEffects.None);
            Utils.DrawBorderString(spriteBatch, $"{SentinelKillTally} / {SentinelKillRequirement}", (NPC.Center + new Vector2(0, -90)) - screenPos, ColorLib.WretchedGradient() * ShieldOpacity, 3f, 0.5f, 0.5f);
            Opus.ReturnToDefaultDrawing(spriteBatch);
            return true;
        }
        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return !DTFlags.NodeCharmEquipped && !(SentinelKillTally < SentinelKillRequirement);
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.friendly)
                return !DTFlags.NodeCharmEquipped && !(SentinelKillTally < SentinelKillRequirement); ; // prevent friendly damage when charm is equipped

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
            Napalm,
            None
        }

        public AttackState CurrentAttack;

        public int DormantPulseTimer = 60;
        public bool HasBuffed = false;
        public bool HasDebuffed = false;
        public int IdleTimer = 60;
        public int SentinelKillTally = 0;
        public const int SentinelKillRequirement = 30;

        public float RotSpeed;
        public int StarShootID = ModContent.ProjectileType<TenebrisStarHostile>();
        public int FlameSwarmTimer = 0;
        public int StarTimer = 0;
        public int StarState = 0;
        public int StarShootCount = 0;
        float off = 0f;
        public Vector2 StarTeleportPos = Vector2.Zero;

        public int MineInterval = 0;
        Dust[] Dusts = new Dust[10];

        public Projectile[] Flares;

        public int MineCount = 0;
        public int MineCooldown = 240;
        public int NapalmRainTimer = 720;
        public int NapalmRainInterval = 0;
        public bool RecordCenterFlag1 = false;
        public bool Flag2 = false;
        public bool Flag3 = false;
        public Vector2 screenCenter;
        public SoundStyle StarShoot = new SoundStyle("DestroyerTest/Assets/Audio/NodeAttackTS") with { MaxInstances = 0, PitchVariance = 1, Volume = 2 };
        public SoundStyle Wallwarn = new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/CursedFlamesWarn") with { MaxInstances = 0, PitchVariance = 1 };
        public SoundStyle WallShoot1 = new SoundStyle("DestroyerTest/Assets/Audio/FlameWall") with { MaxInstances = 0, PitchVariance = 1, Volume = 2 };
        public SoundStyle NapalmShoot = new SoundStyle("DestroyerTest/Assets/Audio/NodeAttackNapalm") with { MaxInstances = 0, PitchVariance = 1 };
        public int DespawnTimer = 60;
        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            DTUtils Utility = new DTUtils();
            DTMusicConfig muscfg = ModContent.GetInstance<DTMusicConfig>();

            if (NPC.alpha > 0 && CurrentAttack != AttackState.None)
            {
                NPC.immortal = true;
                NPC.alpha--;
            }
            else
            {
                NPC.immortal = false;
            }

            if (player.active == false || player.dead == true || !NPC.HasValidTarget)
            {
                if (DespawnTimer > 0)
                {
                    DespawnTimer--;
                }
                else
                {
                    CurrentAttack = AttackState.None;
                }
            }


            TryFindTileBelow();

            if (!Main.dedServ && CurrentAttack != AttackState.Dormant)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/NodeBoss");
            }

            if (Main.expertMode && !Main.masterMode)
            {
                StarShootID = ModContent.ProjectileType<CursedNodeCrystal>();
            }
            if (!Main.expertMode && !Main.masterMode)
            {
                StarShootID = ProjectileID.CursedFlameHostile;
            }

            Vector2 PRTPos;
            PRTPos = NPC.Center;

            if ((SentinelKillTally < SentinelKillRequirement))
            {
                ManageShieldIn();
            }

            if (SentinelKillTally >= SentinelKillRequirement)
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
                        if ((SentinelKillTally < SentinelKillRequirement))
                        {
                            NPC.immortal = true;
                            NPC.dontTakeDamage = true;
                        }
                        else
                        {
                            NPC.immortal = false;
                            NPC.dontTakeDamage = false;
                        }

                        if (NPC.justHit && !DTFlags.NodeCharmEquipped && !(SentinelKillTally < SentinelKillRequirement))
                        {
                            FablesTitleCardSystem.RegisterFablesBossIntro(FablesTitleCardSystem.CursedFlameNodeTitle.Name, FablesTitleCardSystem.CursedFlameNodeTitle.Title, 180, true, ColorLib.WretchedGradient(), ColorLib.WretchedGradient(), ColorLib.WretchedGradient(), ColorLib.WretchedGradient(), FablesTitleCardSystem.CursedFlameNodeTitle.MusicTitle, FablesTitleCardSystem.CursedFlameNodeTitle.MusicArtist);
                            CurrentAttack = AttackState.Idle;
                        }
                        break;
                    }
                case AttackState.Idle:
                    {
                        NPC.boss = true;
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
                        StarTimer++;
                        NPC.velocity *= 0;

                        if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.DeathIsActive)
                        {
                            


                            if (StarTeleportPos == Vector2.Zero)
                            {
                                StarTeleportPos = player.MountedCenter + new Vector2(500, 0).RotatedBy(player.velocity.ToRotation());
                            }

                            if (StarTimer < 90)
                            {
                                if (StarTimer == 1)
                                {
                                    StarTeleportPos = player.MountedCenter + new Vector2(500, 0).RotatedBy(player.velocity.ToRotation());
                                }

                                for (int i = 0; i < 10; i++)
                                {
                                    Vector2 Pos = Main.rand.NextVector2FromRectangle(Utils.CenteredRectangle(StarTeleportPos, new Vector2(140, 140)));
                                    Vector2 Dir = StarTeleportPos - Pos;

                                    Dust D = Dust.NewDustPerfect(Pos, DustID.CursedTorch, Dir * 0.1f);
                                    D.noGravity = true;
                                }
                            }
                            else
                            {

                                NPC.Center = StarTeleportPos;
                                Stars();
                                StarTeleportPos = player.MountedCenter + new Vector2(500, 0).RotatedBy(player.velocity.ToRotation());
                                StarTimer = 0;
                            }


                            if (StarShootCount > 5)
                            {
                                CurrentAttack = AttackState.Mines;
                                StarShootCount = 0;
                                StarTimer = 0;
                            }

                        }
                        else
                        {
                            if (StarTimer % 90 == 0)
                            {
                                SoundEngine.PlaySound(StarShoot);
                                off += MathHelper.PiOver4;

                                for (int i = 0; i < 2; i++)
                                {
                                    Vector2 SpawnPos = NPC.Center;
                                    Vector2 SpawnPos2 = NPC.Center;
                                    Vector2 SpawnPos3 = NPC.Center;

                                    Vector2 Dir = NPC.Center;

                                    switch (i)
                                    {
                                        case 0:
                                            {
                                                SpawnPos = player.MountedCenter + new Vector2(500, 0).RotatedBy(off);
                                                SpawnPos2 = player.MountedCenter + new Vector2(500, 300).RotatedBy(off);
                                                SpawnPos3 = player.MountedCenter + new Vector2(500, -300).RotatedBy(off);

                                                Dir = new Vector2(-12f, 0).RotatedBy(off);
   
                                                break;
                                            }
                                        case 1:
                                            {
                                                SpawnPos = player.MountedCenter + new Vector2(-500, 0).RotatedBy(off);
                                                SpawnPos2 = player.MountedCenter + new Vector2(-500, 300).RotatedBy(off);
                                                SpawnPos3 = player.MountedCenter + new Vector2(-500, -300).RotatedBy(off);

                                                Dir = new Vector2(12f, 0).RotatedBy(off);
                                                break;
                                            }
                                    }

                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), SpawnPos, SpawnPos.DirectionTo(player.MountedCenter) * 12f, ModContent.ProjectileType<CursedNodeCrystal2>(), 40, 5, ai1: 1f);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), SpawnPos, SpawnPos.DirectionTo(player.MountedCenter) * 12f, ModContent.ProjectileType<CursedNodeCrystal2>(), 40, 5, ai1: -1f);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), SpawnPos, SpawnPos.DirectionTo(player.MountedCenter) * 12f, ModContent.ProjectileType<CursedNodeCrystal>(), 40, 5);

                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), SpawnPos2, Dir, ModContent.ProjectileType<CursedNodeCrystal>(), 40, 5);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), SpawnPos3, Dir, ModContent.ProjectileType<CursedNodeCrystal>(), 40, 5);
                                }
                                StarShootCount++;
                            }

                            if (StarShootCount > 8)
                            {
                                CurrentAttack = AttackState.Mines;
                                StarShootCount = 0;
                                StarTimer = 0;
                            }
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
                            if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.DeathIsActive)
                            {
                                for (int q = 0; q < 5; q++)
                                {
                                    Vector2 Position = new Vector2(player.Center.X + Main.rand.Next(-1000, 1000), player.Center.Y + Main.rand.Next(-1000, 1000));
                                    Projectile Mine = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), Position, Vector2.Zero, ModContent.ProjectileType<BlossomMine>(), 25, 5);
                                    Mine.timeLeft = 100;
                                }
                            }
                            else
                            {
                                for (int q = 0; q < 9; q++)
                                {
                                    Vector2 Position = new Vector2(player.Center.X + Main.rand.Next(-1000, 1000), player.Center.Y + Main.rand.Next(-1000, 1000));
                                    Projectile Mine = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), Position, Position.DirectionTo(player.Center) * 3f, ModContent.ProjectileType<CursedFireBomb>(), 25, 5);
                                    Mine.timeLeft = 100;
                                }
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
                        if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.DeathIsActive)
                        {
                            KeepToPlayer(player.Center + new Vector2(0, -200));
                        }
                        else
                        {
                            NPC.velocity *= 0f;
                        }

                    
                        FlameSwarmTimer++;

                        if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.DeathIsActive)
                        {
                            if (FlameSwarmTimer % 20 == 0) // Spawn every 60 ticks (1 second)
                            {

                                FlameSwarm(player);


                            }

                            if (FlameSwarmTimer > 300) // After 5 seconds, switch to Napalm
                            {
                                CurrentAttack = AttackState.Napalm;
                                FlameSwarmTimer = 0;
                            }
                        }
                        else
                        {
                            for(int i = 0; i < 3; i++)
                            {
                                WretchedPointGlow glow = new();
                                Vector2 RPoint = NPC.Center + Main.rand.NextVector2CircularEdge(1100, 1100);
                                glow.Prepare(RPoint, RPoint.DirectionTo(NPC.Center), 2f);
                                ParticleEngine.Particles.Add(glow);
                            }

                            if (player.Center.Distance(NPC.Center) > 1100)
                            {
                                player.Center = NPC.Center + new Vector2(1080, 0).RotatedBy(player.DirectionFrom(NPC.Center).ToRotation());
                            }

                            if (FlameSwarmTimer % 20 == 0)
                            {
                                SoundEngine.PlaySound(Wallwarn);
                                Opus.RadialSpreadProjectile(ModContent.ProjectileType<CursedNodeCrystal>(), 5, NPC.Center, 50, 5f, 18f, offset: NPC.Center.DirectionTo(player.Center).ToRotation());
                                //Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.Center.DirectionTo(player.Center) * 18f, ModContent.ProjectileType<CursedNodeCrystal>(), 40, 5);
                            }

                            if (Flares == null)
                            {
                                Flares = Opus.RadialSpreadProjectile(ModContent.ProjectileType<CursedFlameFlare>(), 8, NPC.Center, 50, 5f, 3f);
                                SoundEngine.PlaySound(WallShoot1);
                            }
                            else
                            {
                                Vector2[] Ideal = Opus.GetEquidistantOrbitVectors(8, NPC.Center, 0.02f, Opus.Sine(100f, 1000f, 0.01f));

                                for (int i = 0; i < Flares.Length; i++)
                                {
                                    Flares[i].SmoothMoveToPoint(Ideal[i], 20f);
                                }
                                 
                                if (FlameSwarmTimer > 1800)
                                {
                                    for (int i = 0; i < Flares.Length; i++)
                                    {
                                        Flares[i].Kill();
                                        Flares[i] = null;
                                    }

                                    CurrentAttack = AttackState.Napalm;
                                    FlameSwarmTimer = 0;
                                    Flares = null;
                                }
                                
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
                            if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.DeathIsActive)
                            {
                                if (NapalmRainTimer % 7 == 0)
                                {
                                    SoundEngine.PlaySound(NapalmShoot, NPC.Center);
                                    Projectile.NewProjectile(Entity.GetSource_FromThis(), NPC.Center, Velocity, ModContent.ProjectileType<CursedFlameNapalm>(), 20, 2);

                                }
                            }
                            else
                            {
                                if (NapalmRainTimer % 120 == 0)
                                {
                                    SoundEngine.PlaySound(NapalmShoot, NPC.Center);
                                    for (int i = -10; i < 11; i++)
                                    {
                                        Vector2 Velocity2 = new Vector2(i * 2f, -15);

                                        
                                        Projectile.NewProjectile(Entity.GetSource_FromThis(), NPC.Center, Velocity2, ModContent.ProjectileType<CursedFlameNapalm>(), 20, 2);
                                    }

                                }
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
                            NapalmRainTimer = 720;
                        }
                        break;
                    }
                case AttackState.None:
                    {
                        NPC.velocity *= 0.8f;

                        if (NPC.alpha < 255)
                        {
                            NPC.immortal = true;
                            NPC.alpha++;
                        }
                        else
                        {
                            NPC.active = false;
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

                BloomRingSharp Ring = new();
                Ring.Prepare(NPC.Center, Vector2.Zero, ColorLib.CursedFlames, 0.03f, 0.01f, 2f, BlendState.Additive);
                ParticleEngine.ShaderParticles.Add(Ring);

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

            int MaxRad = 1200;
            int currad = Opus.Sine(1200, 1000, 0.06f);
            float progress = (float)currad / (float)MaxRad;

            Vector2[] P = Opus.GetEquidistantOrbitVectors(16, NPC.Center, 0.1f, currad);

            for (int i = 0; i < P.Length; i++)
            {
                PointGlowPreMultiplied G = new();
                G.Initialize(P[i], Vector2.Zero, OpusColorUtils.MultiLerp(progress, ColorLib.WretchedColorMap), 1f);
                ParticleEngine.ShaderParticles.Add(G);
            }

            foreach (Player p in Main.player)
            {
                if (p.Center.Distance(NPC.Center) < 1200)
                {
                    if (DTFlags.NodeCharmEquipped)
                    {
                        p.AddBuff(ModContent.BuffType<NodePower>(), 60);
                    }

                    if (SentinelKillTally < SentinelKillRequirement)
                    {
                        SpawnNPCWave();
                    }
                }
            }
        }

        public int SpawnNPCTimer = 0;
        public static string NPCIdentifierContext = "CusedFlameNodeWaveEnemy";
        public int SentinelCount = Main.npc.Where(n => n.active && n.type == ModContent.NPCType<CursedFlameSentinel>()).Count();

        public static List<int> CursedFlameNodeWaveEnemies = new List<int>
        {
            NPCID.EaterofSouls,
            NPCID.Corruptor,
            NPCID.Slimer
        };

        public int WaveTimeout = 0;
        public void SpawnNPCWave()
        {
            SpawnNPCTimer++;
            WaveTimeout++;
            Vector2[] SpawnPositions = Opus.GetEquidistantVectors(3, NPC.Center, 250);
            SentinelCount = Main.npc.Where(n => n.active && n.type == ModContent.NPCType<CursedFlameSentinel>()).Count();


            if ((SpawnNPCTimer % 300 == 0 && SentinelCount == 0) || WaveTimeout > 1800)
            {
                if (WaveTimeout > 1800)
                {
                    CombatText.NewText(NPC.Hitbox, Color.Red, "30 Seconds have passed. Wave failsafe intiated.");
                    Main.NewText("30 Seconds have passed. Wave failsafe intiated.", Color.Red);

                    foreach (NPC child in Main.npc)
                    {
                        if (!child.active) continue;

                        if (child.type == ModContent.NPCType<CursedFlameSentinel>())
                        {
                            child.StrikeInstantKill();
                        }
                    }

                    SentinelKillTally = ((SentinelKillTally + 9) / 10) * 10;
                }
                WaveTimeout = 0;

                SoundEngine.PlaySound(DTAssetLib.Impacts.DarkMagicImpact);

                if (DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive)
                {
                    Opus.RadialSpreadProjectile(ModContent.ProjectileType<CursedFireBomb>(), 3, NPC.Center, 60, 4, 2.2f, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                }

                for (int i = 0; i < SpawnPositions.Length; i++)
                {
                    BloomRingSharp Ring = new();
                    Ring.Prepare(SpawnPositions[i], Vector2.Zero, ColorLib.CursedFlames, 0.1f, 0.01f, 0.4f, BlendState.Additive);
                    ParticleEngine.ShaderParticles.Add(Ring);

                    NPC wavenpc = NPC.NewNPCDirect(NPC.GetSource_FromAI(), SpawnPositions[i], ModContent.NPCType<CursedFlameSentinel>());
                    if (wavenpc.ModNPC is CursedFlameSentinel sentinel)
                    {
                        sentinel.Node = this;
                    }
                    
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
            //Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), NPC.Center, Vector2.Zero, ColorLib.CursedFlames, 0.01f, 1f);
            Opus.RadialSpreadProjectile(ModContent.ProjectileType<CursedNodeCrystal2>(), 7, NPC.Center, 16, 4, 10, ai1: 1, offset: NPC.rotation);
            Opus.RadialSpreadProjectile(ModContent.ProjectileType<CursedNodeCrystal2>(), 7, NPC.Center, 16, 4, 10, ai1: -1, offset: NPC.rotation);
            StarShootCount += 1;
        }

        public void FlameSwarm(Player player)
        {
            SoundEngine.PlaySound(Wallwarn, NPC.Center);
            //Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), NPC.Center, Vector2.Zero, ColorLib.CursedFlames, 0.01f, 1f);
            
            for (int i = 0; i < 2; i++)
            {
                Vector2 spawnPos = new Vector2(player.Center.X + Main.rand.Next(-200, 201), player.Center.Y + 1000);
                Vector2 velocity = (player.Center - spawnPos).SafeNormalize(Vector2.Zero) * 10f; // Adjust speed as needed
                Projectile p = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), spawnPos, velocity, ProjectileID.CursedFlameHostile, 20, 2);
                p.tileCollide = false;
            }
        }

        public override void OnKill()
        {
            SentinelKillTally = 0;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CursedNodeLootBag>()));
        }
    }
}