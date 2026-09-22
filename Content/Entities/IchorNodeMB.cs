
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.DropRules;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.BossBar;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.CursedFlame;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Ichor;
using DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tools;
using GlowmaskHelper.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using OpusLib;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
using rail;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace DestroyerTest.Content.Entities
{

    [AutoloadBossHead]
    [AutoloadGlowmask]
    public class IchorNodeMB : ModNPC
    {
        public override string BossHeadTexture => "DestroyerTest/Content/Entities/IchorNode_Head_Boss";
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
            NPC.netID = ModContent.NPCType<IchorNodeMB>();
            NPC.BossBar = ModContent.GetInstance<CrimsonBossBar>();
            NPC.alpha = 255;
            NPC.friendly = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(DTUtils.GetModNPCLocalizationEntry(this, 1)),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson
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
            if (spawnInfo.Player.ZoneCrimson == true && spawnInfo.Player.ZoneOverworldHeight == true && DownedBossSystem.downedPlanteraBoss == true && !NodeAlive)
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

        int Roff = 0;
        float Opa = 0f;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Roff -= 10;

            Line R = new Line(NPC.Center, new Vector2(NPC.Center.X, NPC.Center.Y + 2200f));
            DTUtils.instance.ScrollingTextureSpine(R, DTAssetLib.ArrowTelegraphCont, ColorLib.Ichor with { A = 0 } * Opa, spriteBatch, BlendState.Additive, Roff, 0.3f, 1f);

            
            return true;
        }

        public float ShieldOpacity = 0f;
        public float ShieldScale = 1f;

        float BorderRotation = 0f;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            var v = DTAssetLib.BloomRingSharp.Value;

            BorderRotation += 0.13f;

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            Main.EntitySpriteDraw(v, NPC.Center - Main.screenPosition, null, OpusColorUtils.MultiLerp(((float)SentinelKillTally / (float)SentinelKillRequirement).Inverse(), ColorLib.IchorCrystalColorMap) * ShieldOpacity, 0f, v.Size() / 2, ShieldScale, SpriteEffects.None);

            Main.EntitySpriteDraw(DTAssetLib.BarrierRing.Value, NPC.Center - Main.screenPosition, null, OpusColorUtils.MultiLerp(((float)SentinelKillTally / (float)SentinelKillRequirement).Inverse(), ColorLib.IchorCrystalColorMap) * ShieldOpacity, BorderRotation, DTAssetLib.BarrierRing.Value.Size() / 2, DTAssetLib.BarrierRing.Value.ScaleRingTextureToMatchRadius(1200f, 1300), SpriteEffects.None);

            //Utils.DrawBorderString(spriteBatch, $"{SentinelKillTally} / {SentinelKillRequirement}", (NPC.Center + new Vector2(0, -90) - Main.screenPosition), ColorLib.IchorCrystalGradient * ShieldOpacity, 3f, 0.5f, 0.5f);

            spriteBatch.DrawString(DTAssetLib.Doxent.Value, $"{SentinelKillTally} / {SentinelKillRequirement}", (NPC.Center + new Vector2(0, -90)) - screenPos, ColorLib.IchorCrystal3 * ShieldOpacity, 0f, DTAssetLib.Doxent.Value.MeasureString($"{SentinelKillTally} / {SentinelKillRequirement}") * 0.5f, 0.5f, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(spriteBatch);

            if (CurrentAttack == AttackState.Dormant)
            {
                DTUtils.DrawChargeBar(2f, (NPC.Center + new Vector2(0, 100)) - Main.screenPosition, (float)SentinelKillTally / (float)SentinelKillRequirement, OpusColorUtils.MultiLerp(((float)SentinelKillTally / (float)SentinelKillRequirement), ColorLib.IchorCrystalColorMap));
            }
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
            Spikes,
            ToothBombs,
            Pikes,
            GroundSlam,
            None
        }

        public AttackState CurrentAttack;

        public int InternalTimer = 0;

        public int DormantPulseTimer = 60;
        public int SentinelKillTally = 0;
        public const int SentinelKillRequirement = 30;
        public float SpikeOffsetEternity = 0f;

        public int IdleTime = 60;

        public int SpikeTime => IdleTime + 600;
        public int SpikeTimeTrans => SpikeTime + 90;
        public int MineTime => SpikeTimeTrans + 360;
        public int MineTimeTrans => MineTime + 90;
        public int PikesTime => MineTimeTrans + 600;

        public int MineInterval = 0;
        public int MineCount = 0;
        public int MineCooldown = 240;
        public int SlamCharge = 120;
        public int SlamCount = 0;
        public int WaveTimer = 0;
        public int WaveIndex = 0;
        public float WaveRecoredY = 0f;
        public float WaveRecordedX = 0f;
        public bool SoundFlag1 = false;

        int IS_Timer = 0;

        public bool Flag2 = false;
        public bool Flag3 = false;
        public SoundStyle SlamWarn = new SoundStyle("DestroyerTest/Assets/Audio/ChimeIn") with { MaxInstances = 0, PitchVariance = 1 };
        public SoundStyle Spiralwarn = new SoundStyle("DestroyerTest/Assets/Audio/RailGunCharge") with { MaxInstances = 0 };
        public SoundStyle GroundImpact = new SoundStyle("DestroyerTest/Assets/Audio/TenebrisTesticleKill") with { MaxInstances = 0, PitchVariance = 0.5f };
        public int DespawnTimer = 60;

        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            DTUtils Utility = new DTUtils();
            DTMusicConfig muscfg = ModContent.GetInstance<DTMusicConfig>();

            if (CurrentAttack != AttackState.Dormant)
            {
                InternalTimer++;
            }

            if (DrawSlamTelegraph)
            {
                if (Opa < 1f)
                {
                    Opa += 0.05f;
                }
            }
            else
            {
                if (Opa > 0f)
                {
                    Opa -= 0.05f;
                }
            }

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

            if (CurrentAttack != AttackState.GroundSlam)
            {
                TryFindTileBelow();
            }

            if (!Main.dedServ && CurrentAttack != AttackState.Dormant)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/NodeBoss");
            }

            if (WaveTimer > 0)
            {
                WaveTimer--;

                if (WaveTimer % 10 == 0)
                {
                    WaveIndex++;

                    float Y = WaveRecoredY + 300;
                    float X = WaveRecordedX + (WaveIndex * 16) * 4;
                    float AltX = WaveRecordedX + (WaveIndex * 16) * -4;

                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy with { Volume = 2f, MaxInstances = 0 });

                    if (WaveIndex != 0)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(X, Y), new Vector2(0, -40), ModContent.ProjectileType<CrimsonSpike>(), 15, 2);
                    
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(AltX, Y), new Vector2(0, -40), ModContent.ProjectileType<CrimsonSpike>(), 15, 2);
                    }
                }
            }
            else
            {
                WaveIndex = 0;
            }

            Vector2 PRTPos;
            PRTPos = NPC.Center;

            if ((SentinelKillTally < SentinelKillRequirement))
            {
                ManageShieldIn();
            }

            if (!(SentinelKillTally < SentinelKillRequirement))
            {
                if (!Flag2)
                {
                    SoundEngine.PlaySound(DTAssetLib.ScholarShieldSounds.Activate, NPC.Center);
                    Flag2 = true;
                }
                ManageShieldOut();
            }

            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.active && projectile.Distance(NPC.Center) < 30 && projectile.type == ModContent.ProjectileType<BloodCloudBall>())
                {
                    projectile.Kill();
                }
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
                            FablesTitleCardSystem.RegisterFablesBossIntro(FablesTitleCardSystem.IchorNodeTitle.Name, FablesTitleCardSystem.IchorNodeTitle.Title, 180, true, ColorLib.WretchedGradient(), ColorLib.IchorCrystalGradient, ColorLib.IchorCrystalGradient, ColorLib.IchorCrystalGradient, FablesTitleCardSystem.IchorNodeTitle.MusicTitle, FablesTitleCardSystem.IchorNodeTitle.MusicArtist);
                            CurrentAttack = AttackState.Idle;
                        }
                        break;
                    }
                case AttackState.Idle:
                    {
                        NPC.boss = true;
                        NPC.npcSlots = 10f;
                        KeepToPlayer(player.Center + new Vector2(0, -200));
                        
                        if (InternalTimer >= IdleTime)
                        {
                            CurrentAttack = AttackState.Spikes;
                        }
                        break;
                    }
                case AttackState.Spikes:
                    {
                        

                        if ((DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive) && InternalTimer < SpikeTime)
                        {
                            NPC.velocity *= 0;
                            SpikeOffsetEternity += 0.08f;

                            if (InternalTimer % 3 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy with { Volume = 2f, MaxInstances = 0 });
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(50, 0).RotatedBy(SpikeOffsetEternity), ModContent.ProjectileType<CrimsonSpike>(), 15, 2);
                            }
                        }
                        else
                        {
                            KeepToPlayer(player.Center + new Vector2(0, -200));
                            if (InternalTimer % 150 == 0 && InternalTimer < SpikeTime)
                            {
                                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy with { Volume = 2f, MaxInstances = 0 });
                                for (int i = 0; i < 15; i++)
                                {
                                    float X = NPC.Center.X + ((i * 16) * 6);
                                    float AltX = NPC.Center.X - ((i * 16) * 6);
                                    float Y = NPC.Center.Y + 900;


                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(X, Y), new Vector2(0, -40), ModContent.ProjectileType<CrimsonSpike>(), 15, 2);
                                    if (i != 0)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(AltX, Y), new Vector2(0, -40), ModContent.ProjectileType<CrimsonSpike>(), 15, 2);
                                    }


                                }

                                for (int i = 0; i < 30; i++)
                                {
                                    int Side = i % 2 == 0 ? -1 : 1;
                                    float X = NPC.Center.X + (1000 * Side);
                                    float Y = NPC.Center.Y + (-900 + ((i * 16) * 12));


                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(X, Y), new Vector2(-40 * Side, 0), ModContent.ProjectileType<CrimsonSpike>(), 15, 2);
                                }


                            }
                        }
                        if (InternalTimer >= SpikeTimeTrans)
                        {
                            CurrentAttack = AttackState.ToothBombs;
                        }

                        break;
                    }
                case AttackState.ToothBombs:
                    {
                        KeepToPlayer(player.Center + new Vector2(0, -200));
                        MineAI();
                        break;
                    }
                case AttackState.Pikes:
                    {
                        KeepToPlayer(player.Center + new Vector2(0, -200));

                        if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.DeathIsActive)
                        {
                            if (InternalTimer % 120 == 0)
                            {
                                SoundEngine.PlaySound(DTAssetLib.ScholarShieldSounds.Activate, NPC.Center);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(30, -4), ModContent.ProjectileType<NodeBossDistendedPike>(), 15, 2);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-30, -4), ModContent.ProjectileType<NodeBossDistendedPike>(), 15, 2);


                            }
                        }
                        else
                        {
                            if (InternalTimer % 20 == 0)
                            {
                                SoundEngine.PlaySound(DTAssetLib.ScholarShieldSounds.Activate, NPC.Center);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.Center.DirectionFrom(player.Center) * 10f, ModContent.ProjectileType<NodeBossDistendedPike2>(), 15, 2);


                            }
                        }

                        if (InternalTimer >= PikesTime)
                        {
                            CurrentAttack = AttackState.GroundSlam;
                        }
                        break;
                    }
                case AttackState.GroundSlam:
                    {
                        SlamAI(player);
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

                LerpingBloomRingSharp Ring = new();
                Ring.Prepare(NPC.Center, Vector2.Zero, ColorLib.IchorCrystalColorMap, 0.2f, 2f, BlendState.Additive);
                ParticleEngine.BehindProjectiles.Add(Ring);
                
                DormantPulseTimer = 120;

            }

            NPC.velocity.Y = Opus.Sine(1f, -1f, 0.01f);

            foreach (NPC npc in Main.npc)
            {
                if (npc.Center.Distance(NPC.Center) < 1000
                && npc.type != ModContent.NPCType<IchorNodeMB>()
                && npc.type != ModContent.NPCType<CursedFlameNodeMB>()
                && npc.type != ModContent.NPCType<IchorNode>()
                && npc.type != ModContent.NPCType<CursedFlameNode>() && !npc.boss)
                {
                    npc.AddBuff(ModContent.BuffType<NodePower>(), 60);
                }
            }

            Vector2[] P = Opus.GetEquidistantOrbitVectors(16, NPC.Center, 0.1f, 1200);

         
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
        public int SentinelCount = Main.npc.Where(n => n.active && n.type == ModContent.NPCType<Glutton>()).Count();

        public int WaveTimeout = 0;
        public void SpawnNPCWave()
        {
            SpawnNPCTimer++;
            WaveTimeout++;
            Vector2[] SpawnPositions = Opus.GetEquidistantVectors(3, NPC.Center, 250);
            SentinelCount = Main.npc.Where(n => n.active && n.type == ModContent.NPCType<Glutton>()).Count();


            if ((SpawnNPCTimer % 300 == 0 && SentinelCount == 0) || WaveTimeout > 1800)
            {
                if (WaveTimeout > 1800)
                {
                    CombatText.NewText(NPC.Hitbox, Color.Red, "30 Seconds have passed. Wave failsafe intiated.");
                    Main.NewText("30 Seconds have passed. Wave failsafe intiated.", Color.Red);

                    foreach (NPC child in Main.npc)
                    {
                        if (!child.active) continue;

                        if (child.type == ModContent.NPCType<Glutton>())
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
                    Opus.RadialSpreadProjectile(ModContent.ProjectileType<FleshBomb>(), 10, NPC.Center, 12, 2, 9, offset: 0);
                }

                for (int i = 0; i < SpawnPositions.Length; i++)
                {
                    BloomRingSharp Ring = new();
                    Ring.Prepare(SpawnPositions[i], Vector2.Zero, ColorLib.Ichor, 0.1f, 0.01f, 0.4f, BlendState.Additive);
                    ParticleEngine.ShaderParticles.Add(Ring);

                    NPC wavenpc = NPC.NewNPCDirect(NPC.GetSource_FromAI(), SpawnPositions[i], ModContent.NPCType<Glutton>());
                    if (wavenpc.ModNPC is Glutton sentinel)
                    {
                        sentinel.Node = this;
                    }

                }
            }
        }

        public void MineAI()
        {

            if (InternalTimer % 120 == 0 && InternalTimer < MineTime)
            {
                if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.DeathIsActive)
                {
                    for (int q = 0; q < 6; q++)
                    {
                        Vector2 Position = NPC.Center + new Vector2(Main.rand.Next(-400, 400), Main.rand.Next(-400, 400));
                        Vector2 Velocity = Position - NPC.Center;
                        Projectile Mine = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), NPC.Center, Velocity * 0.03f, ModContent.ProjectileType<CrystalBomb>(), 30, 5);
                        Mine.timeLeft = 120;
                    }
                }
                else
                {
                    Opus.RadialSpreadProjectile(ModContent.ProjectileType<CrimsonSpike>(), 6, NPC.Center, 12, 2, 30, offset: 0);
                    Opus.RadialSpreadProjectile(ModContent.ProjectileType<CrimsonSpike>(), 6, NPC.Center, 12, 2, 15, offset: MathHelper.TwoPi / 12);

                    foreach (Projectile Mine in Opus.RadialSpreadProjectile(ModContent.ProjectileType<FleshBomb>(), 6, NPC.Center, 12, 2, 17, offset: 0))
                    {
                        Mine.timeLeft = 90;
                    }

                    foreach (Projectile Mine in Opus.RadialSpreadProjectile(ModContent.ProjectileType<FleshBomb>(), 6, NPC.Center, 12, 2, 6, offset: MathHelper.TwoPi / 12))
                    {
                        Mine.timeLeft = 90;
                    }
                }
            }

            if (InternalTimer > MineTimeTrans)
            {
                CurrentAttack = AttackState.Pikes;
                MineCount = 0;
                MineCooldown = 240;
            }
        }

        bool DrawSlamTelegraph = false;
        public void SlamAI(Player player)
        {
            if (SlamCharge > 0)
            {
                NPC.noTileCollide = true;
                NPC.SmoothMoveToPoint(new Vector2(player.Center.X, player.Center.Y - 300f), 30, 200);
                SlamCharge--;
                if (SlamCharge == 20)
                {
                    SoundEngine.PlaySound(SlamWarn, NPC.Center);
                }
                if (SlamCharge >= 20)
                {
                    DrawSlamTelegraph = true;
                }
                else
                {
                    DrawSlamTelegraph = false;
                }
            }
            if (SlamCharge <= 0)
            {
                if (SoundFlag1 == false)
                {
                    SoundEngine.PlaySound(SoundID.Item63, NPC.Center);
                    SoundFlag1 = true;
                }
                Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y + NPC.height / 2), 2, 2, DustID.Ichor, 2f, -1.5f, 0, ColorLib.Ichor, 2f);
                Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y + NPC.height / 2), 2, 2, DustID.Ichor, -2f, -1.5f, 0, ColorLib.Ichor, 2f);

                PointGlowPreMultiplied Glow1 = new PointGlowPreMultiplied();
                Glow1.Initialize(NPC.Bottom, new Vector2(3f, 0f), ColorLib.Ichor, 1f);
                ParticleEngine.BehindProjectiles.Add(Glow1);

                PointGlowPreMultiplied Glow2 = new PointGlowPreMultiplied();
                Glow2.Initialize(NPC.Bottom, new Vector2(-3f, 0f), ColorLib.Ichor, 1f);
                ParticleEngine.BehindProjectiles.Add(Glow2);

                if ((DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive) && InternalTimer % 4 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(15f, 0f), ModContent.ProjectileType<IchorNodeCrystal2>(), 16, 2);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-15f, 0f), ModContent.ProjectileType<IchorNodeCrystal2>(), 16, 2);
                }

                NPC.noTileCollide = false;
                NPC.velocity.Y = 0f;
                NPC.velocity.Y = 40f;
                NPC.velocity.X = 0f;
            }
            if (NPC.collideY && NPC.velocity.Y >= 0f)
            {
                NPC.velocity = Vector2.Zero;
                SoundEngine.PlaySound(GroundImpact, NPC.Center);
                player.GetModPlayer<ScreenshakePlayer>().screenshakeTimer = 10;
                player.GetModPlayer<ScreenshakePlayer>().screenshakeMagnitude = 2;
                SlamWave();
                SlamSpray();
                SlamCharge = 120;
                SlamCount += 1;
            }
            if (SlamCount > 5)
            {
                CurrentAttack = AttackState.Idle;
                SlamCharge = 120;
                SlamCount = 0;
                InternalTimer = 0;
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

        public bool TryFindAirTile(Vector2 Probe, out bool surrounded)
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
            if (surrounded)
            {
                return false;
            }
            return true;
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


        
        public void SlamWave()
        {
            WaveRecoredY = NPC.Center.Y;
            WaveRecordedX = NPC.Center.X;
            WaveTimer = 60;
        }

        public void SlamSpray()
        {
            for (int f = 0; f < 7; f++)
            {
                Vector2 velo = new Vector2(Main.rand.Next(-10, 10), -12);
                Projectile.NewProjectile(Entity.GetSource_FromThis(), NPC.Center, velo, ProjectileID.GoldenShowerHostile, 15, 4);
            }
            
            if (DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive)
            {
                //Opus.RadialSpreadProjectile(ModContent.ProjectileType<CrimsonSpike>(), 10, NPC.Center, 12, 2, 30, offset: 0);
            }
        }

        public void Spiral_BindPlayer(Player playerToBind, float radius)
        {
            if (playerToBind == null)
            {
                return;
            }

            Vector2 offset = playerToBind.Center - NPC.Center;
            float dist = offset.Length();

            if (dist > radius)
            {
                offset.Normalize();
                offset *= radius;
                playerToBind.Center = NPC.Center + offset;
            }
        }

        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<HaepienNodeCharm>(), 24, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrimalShards>(), 1, 4, 16));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrimalIdol>(), 1, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<IchorScroll>(), 1, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Scorn>(), 2, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DistendedPike>(), 2, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SavageSpray>(), 3, 1, 1));
            npcLoot.Add(ItemDropRule.ByCondition(new EternityDropRuleCondition(), ModContent.ItemType<GreedyGraze>(), 1, 1, 1));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Item_IchorNodeRelic>()));
            npcLoot.Add(ItemDropRule.Common(ItemID.FlaskofIchor, 3, 1, 9));
            npcLoot.Add(ItemDropRule.NotScalingWithLuck(ItemID.Ichor, 2, 20, 60));
            npcLoot.Add(ItemDropRule.Coins(1250, true));
        }
    }
}