
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using BreadLibrary.Core.Graphics.Particles;
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
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Ichor;
using GlowmaskHelper.Content;
 
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            DTUtils.instance.ScrollingTextureSpine(R, DTAssetLib.ArrowTelegraphCont, ColorLib.Ichor with { A = 0 } * Opa, spriteBatch, BlendState.Additive, Roff, 0.5f, 1f);

            
            return true;
        }

        public float ShieldOpacity = 0f;
        public float ShieldScale = 1f;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            var v = DTAssetLib.BloomRingSharp.Value;

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(v, NPC.Center - Main.screenPosition, null, ColorLib.IchorCrystalGradient * ShieldOpacity, 0f, v.Size() / 2, ShieldScale, SpriteEffects.None);
            Utils.DrawBorderString(spriteBatch, $"{SentinelKillTally} / {SentinelKillRequirement}", (NPC.Center + new Vector2(0, -90) - Main.screenPosition), ColorLib.IchorCrystalGradient * ShieldOpacity, 3f, 0.5f, 0.5f);
            Opus.ReturnToDefaultDrawing(spriteBatch);

            if (CurrentAttack == AttackState.Dormant)
            {
                DTUtils.DrawChargeBar(2f, (NPC.Center + new Vector2(0, 100)) - Main.screenPosition, (float)SentinelKillTally / (float)SentinelKillRequirement, ColorLib.IchorCrystalGradient);
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
            BloodRain,
            IchorSpiral,
            ToothBombs,
            GroundSlam,
            None
        }

        public AttackState CurrentAttack;

        public int DormantPulseTimer = 60;
        public int SentinelKillTally = 0;
        public const int SentinelKillRequirement = 30;


        public int IdleTimer = 60;
        public int BloodRainSpawnTimer = 180;
        public int BloodRainWaitTimer = 240;
        public int IchorSpiralWarnTimer = 180;
        public bool IchorSpiralWarnParticleFlag = false;
        public int IchorSpiralTimer = 240;
        public int IchorSpiralCooldownTimer = 120;
        public float IchorSpiralRotationOffset = 0;
        public int MineInterval = 0;
        public int MineCount = 0;
        public int MineCooldown = 240;
        public int SlamCharge = 120;
        public int SlamCount = 0;
        public int WaveTimer = 0;
        public int WaveIndex = 0;
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
                        if (IdleTimer > 0)
                        {
                            IdleTimer--;
                        }

                        if (IdleTimer <= 0)
                        {
                            CurrentAttack = AttackState.BloodRain;
                            IdleTimer = 60;
                        }
                        break;
                    }
                case AttackState.BloodRain:
                    {
                        
                        BloodRainAI(player);
                        break;
                    }
                case AttackState.IchorSpiral:
                    {
                        NPC.velocity = Vector2.Zero;

                        if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.DeathIsActive)
                        {
                            if (IchorSpiralWarnTimer > 0)
                            {
                                IchorSpiralWarnTimer--;
                                if (!IchorSpiralWarnParticleFlag)
                                {
                                    SoundEngine.PlaySound(Spiralwarn);

                                    BloomRingSharp Ring = new();
                                    Ring.Prepare(NPC.Center, Vector2.Zero, Color.Red, 0.2f, 0.05f, 3.75f, BlendState.Additive);
                                    ParticleEngine.BehindProjectiles.Add(Ring);

                                    IchorSpiralWarnParticleFlag = true;
                                }
                            }
                            if (IchorSpiralWarnTimer <= 0)
                            {
                                Spiral_BindPlayer(player, 500);
                                Opus.RingSpreadDust(DustID.TintableDustLighted, 30, NPC.Center, 500f, 0, ColorLib.IchorCrystalGradient, 1.5f, 3, Main.rand.NextFloat(MathHelper.TwoPi));
                                if (IchorSpiralTimer > 0)
                                {

                                    IchorSpiralRotationOffset += 1f;
                                    //var launchVelocity = new Vector2(-8, 0);
                                    NPC.rotation = IchorSpiralRotationOffset;

                                    if (IchorSpiralTimer % 4 == 0)
                                    {
                                        SoundEngine.PlaySound(SoundID.Item156, NPC.Center);



                                        for (int i = 0; i < 6; i++)
                                        {
                                            var angle = IchorSpiralRotationOffset + (i * MathHelper.TwoPi / 6f);
                                            var launchVelocity = new Vector2(8, 0).RotatedBy(angle);
                                            Projectile Crys = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), NPC.Center, launchVelocity, ModContent.ProjectileType<IchorNodeCrystal2>(), 15, 4);
                                            Crys.timeLeft = 120;
                                        }

                                        IchorSpiralRotationOffset += 0.75f; // spiral effect
                                    }
                                    IchorSpiralTimer--;
                                }
                                if (IchorSpiralTimer <= 0 && IchorSpiralCooldownTimer > 0)
                                {
                                    IchorSpiralCooldownTimer--;
                                }
                                if (IchorSpiralTimer <= 0 && IchorSpiralCooldownTimer <= 0)
                                {
                                    CurrentAttack = AttackState.ToothBombs;
                                    IchorSpiralWarnTimer = 180;
                                    IchorSpiralWarnParticleFlag = false;
                                    IchorSpiralTimer = 240;
                                    IchorSpiralCooldownTimer = 120;
                                    Flag3 = false;
                                    NPC.rotation = 0f;
                                }
                            }
                        }
                        else
                        {
                            if (!Flag3)
                            {
                                Opus.RadialSpreadProjectile(ModContent.ProjectileType<BloodCloudBall>(), 18, NPC.Center, 40, 6, 10f);
                                Opus.RadialSpreadProjectile(ModContent.ProjectileType<BloodCloudBall>(), 12, NPC.Center, 40, 6, 6f);
                                Opus.RadialSpreadProjectile(ModContent.ProjectileType<BloodCloudBall>(), 8, NPC.Center, 40, 6, 4f);
                                Flag3 = true;
                            }
                            else
                            {
                                if (IS_Timer < 120)
                                {
                                    IS_Timer++;
                                }
                                else
                                {
                                    CurrentAttack = AttackState.ToothBombs;
                                    IchorSpiralWarnTimer = 180;
                                    IchorSpiralWarnParticleFlag = false;
                                    IchorSpiralTimer = 240;
                                    IchorSpiralCooldownTimer = 120;
                                    Flag3 = false;
                                    NPC.rotation = 0f;
                                    IS_Timer = 0;
                                }
                                
                            }
                        }
                        break;
                    }
                case AttackState.ToothBombs:
                    {
                        KeepToPlayer(player.Center + new Vector2(0, -200));
                        MineAI();
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

            for (int i = 0; i < P.Length; i++)
            {
                PointGlowPreMultiplied Glow = new PointGlowPreMultiplied();
                Glow.Initialize(P[i], Vector2.Zero, ColorLib.IchorCrystalGradient, 1f);
                ParticleEngine.BehindProjectiles.Add(Glow);
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

        int BR_EternityTimer = 0;
        public void BloodRainAI(Player player)
        {
            if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.DeathIsActive)
            {
                KeepToPlayer(player.Center + new Vector2(0, -200));

                if (BloodRainSpawnTimer > 0)
                {
                    if (BloodRainSpawnTimer % 8 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item66, NPC.Center);
                        Vector2 Position = new Vector2(
                            player.Center.X + Main.rand.Next(-200, 200),
                            player.Center.Y - 400f // blanket above
                        );
                        Projectile.NewProjectile(Entity.GetSource_FromThis(), Position, Vector2.Zero, ModContent.ProjectileType<BloodCloud>(), 16, 8);
                    }
                    BloodRainSpawnTimer--;
                }
                if (BloodRainSpawnTimer <= 0 && BloodRainWaitTimer > 0)
                {
                    BloodRainWaitTimer--;
                }
                if (BloodRainSpawnTimer <= 0 && BloodRainWaitTimer <= 0)
                {
                    CurrentAttack = AttackState.IchorSpiral;
                    BloodRainSpawnTimer = 180;
                    BloodRainWaitTimer = 240;
                }
            }
            else
            {
                NPC.velocity *= 0;
                BR_EternityTimer++;

                

                if (BR_EternityTimer % 60 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item66, NPC.Center);

                    Opus.RingSpreadProjectile(ModContent.ProjectileType<BloodCloudBall>(), 18, NPC.Center, 1200f, 40, 6, -5f, offset: 0.05f * BR_EternityTimer);

                }

                if (BR_EternityTimer >= 1200)
                {
                    CurrentAttack = AttackState.IchorSpiral;
                    BR_EternityTimer = 0;
                }
            }
        }

        public void MineAI()
        {
            if (MineInterval > 0)
            {
                MineInterval--;
            }

            if (MineInterval <= 0)
            {
                for (int q = 0; q < 6; q++)
                {
                    Vector2 Position = NPC.Center + new Vector2(Main.rand.Next(-400, 400), Main.rand.Next(-400, 400));
                    Vector2 Velocity = Position - NPC.Center;
                    Projectile Mine = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), NPC.Center, Velocity * 0.03f, ModContent.ProjectileType<CrystalBomb>(), 30, 5);
                    Mine.timeLeft = 120;
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
                CurrentAttack = AttackState.GroundSlam;
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
                Vector2 toTarget = new Vector2(player.Center.X, player.Center.Y - 300f) - NPC.Center;
                float speed = 10f;
                NPC.velocity = toTarget.SafeNormalize(Vector2.Zero) * speed;
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

                NPC.noTileCollide = false;
                NPC.velocity.Y = 0f;
                NPC.velocity.Y = 24f;
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
            int left = (int)(NPC.position.X / 16);
            int right = (int)((NPC.position.X + NPC.width) / 16);
            int bottom = (int)((NPC.position.Y + NPC.height) / 16);

            WaveTimer++;
            if (WaveTimer % 10 == 0)
            {
                int x = left + WaveIndex;
                Tile tile = Framing.GetTileSafely(x, bottom);
                bool isGround =
                    tile.HasUnactuatedTile &&
                    Main.tileSolid[tile.TileType] &&
                    !Main.tileSolidTop[tile.TileType];

                if (x <= right && isGround)
                {
                    Vector2 spawnPos = new Vector2(x * 16 + 8, bottom * 16);
                    Projectile.NewProjectile(
                        Entity.GetSource_FromThis(),
                        spawnPos,
                        Vector2.Zero,
                        ModContent.ProjectileType<NodeSlam>(),
                        25,
                        4f
                    );
                }


                WaveIndex++;

                if (x > right)
                {
                    // finished wave
                    WaveIndex = 0;
                    WaveTimer = 0;
                }
            }
        }

        public void SlamSpray()
        {
            for (int f = 0; f < 7; f++)
            {
                Vector2 velo = new Vector2(Main.rand.Next(-10, 10), -12);
                Projectile.NewProjectile(Entity.GetSource_FromThis(), NPC.Center, velo, ProjectileID.GoldenShowerHostile, 15, 4);
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
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<IchorNodeLootBag>()));
        }
    }
}