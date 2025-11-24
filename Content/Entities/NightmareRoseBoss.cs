using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.ConstitutionBoss;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.Buffs;
using log4net.Repository.Hierarchy;
using Microsoft.Build.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using System;
using System.Collections;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using System.Collections.Generic;
using static DestroyerTest.Content.Entities.ConstitutionClone;
using System.Linq;
using ReLogic.Content;
using DestroyerTest.Content.Projectiles.VampireBoss;
using DestroyerTest.Content.Projectiles.NightmareRose;
using Terraria.Graphics;
using DestroyerTest.Content.Projectiles;
using rail;
using System.Security.Policy;
using DestroyerTest.Common.Systems;
using Terraria.Localization;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.RogueItems;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.BossBar;
using ReLogic.Localization.IME;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Terraria.Graphics.CameraModifiers;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Dusts;
using GlowmaskHelper.Content;
using OpusLib;

namespace DestroyerTest.Content.Entities
{
    [AutoloadBossHead]
    [AutoloadGlowmask]
    public class NightmareRoseBoss : ModNPC
    {
        public override string BossHeadTexture => "DestroyerTest/Content/Entities/NightmareRoseBoss_Head_Boss";

        public void immunities()
        {
            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<ShimmeringFlames>()] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<HaepiensBlizzard>()] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<HaepiensInferno>()] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Bleeding] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Dazed] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Electrified] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frozen] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Oiled] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowFlame] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Slimed] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.SoulDrain] = true;
        }
        public override void SetStaticDefaults()
        {
            NPCID.Sets.CanHitPastShimmer[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            immunities();
            NPCID.Sets.TrailCacheLength[Type] = 20;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            { // Influences how the NPC looks in the Bestiary
                CustomTexturePath = "DestroyerTest/Content/Entities/NightmareRoseBossBestiary", // If the NPC is multiple parts like a worm, a custom texture for the Bestiary is encouraged.
                Position = Vector2.Zero,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
            Main.npcFrameCount[NPC.type] = 12;
        }

        public SoundStyle Kill = new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/NightmareRoseKill") with { Volume = 2, MaxInstances = 0 };
        public SoundStyle Fire = new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/CursedFlameShoot") with { Volume = 2, PitchVariance = 1f, MaxInstances = 0 };
        public SoundStyle ArenaDivide = new SoundStyle("DestroyerTest/Assets/Audio/Impacts/HellWeaponImpact") with { Volume = 2, PitchVariance = 1f, MaxInstances = 0 };
        public SoundStyle DespShootMine = new SoundStyle("DestroyerTest/Assets/Audio/Impacts/MetalImpactV1_", 3) with { Volume = 2, PitchVariance = 1f, MaxInstances = 0 };
        public SoundStyle NodeSpawnSound = new SoundStyle("DestroyerTest/Infernum/Assets/Audio/NightmareRoseIntroFinish") with { PitchVariance = 1f, MaxInstances = 0 };
        public SoundStyle Napalm = new SoundStyle("DestroyerTest/Assets/Audio/NodeAttackNapalm") with { PitchVariance = 1f, MaxInstances = 0 };
        public SoundStyle Desperation = new SoundStyle("DestroyerTest/Assets/Audio/RoseDesperation") with { MaxInstances = 0 };

        public override void SetDefaults()
        {
            NPC.width = 144;
            NPC.height = 274;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 45;
            NPC.lifeMax = 368000;
            NPC.HitSound = SoundID.DD2_MonkStaffGroundImpact;
            NPC.noGravity = false;
            NPC.lavaImmune = true;
            NPC.noTileCollide = false;
            NPC.knockBackResist = 0f;
            NPC.timeLeft = 150000;
            NPC.boss = true;
            NPC.npcSlots = 90f;
            NPC.netID = ModContent.NPCType<NightmareRoseBoss>();
            NPC.BossBar = ModContent.GetInstance<CorruptBossBar>();
            GeneralEternityChanges(EternityIsActive());
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.DestroyerTest.NPCs.NightmareRoseBoss.BestiaryEntry1")),
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.DestroyerTest.NPCs.NightmareRoseBoss.BestiaryEntry2")),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption
            });
        }

        public override bool CheckActive()
        {
            return false;
        }

        private int frameIndex;

        public override void FindFrame(int frameHeight)
        {
            if (currentState != AttackState.SpawnIdle)
            {
                if (currentState == AttackState.Desperation || currentState == AttackState.KillIdle)
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 10) // slower desperation animation
                    {
                        NPC.frameCounter = 0;
                        frameIndex++;
                        if (frameIndex > 10) // clamp at frame 10
                            frameIndex = 9;
                    }
                }
                else
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 5) // faster normal animation
                    {
                        NPC.frameCounter = 0;
                        frameIndex++;
                        if (frameIndex > 8) // loop back
                            frameIndex = 0;
                    }
                }
            }
            else
            {
                frameIndex = 11;
            }

            NPC.frame.Y = frameIndex * frameHeight;
        }



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

        public enum AttackState
        {
            SpawnIdle,
            Idle,
            CursedFlames,
            Napalm,
            Minions,
            RottenPetals,
            OvergrownHammer,
            DemoniteWhisper,
            CorruptSigil,
            ArenaDivide,
            BlossomMine,
            Desperation,
            Nodes,
            FlameRing,
            Lances,
            WallDarts,
            KillIdle
        }

        #region Vars

        public AttackState currentState = AttackState.Idle;
        public Vector2 PlayerCenter = Vector2.Zero;
        public Vector2 DirectionToPlayerCenter = Vector2.Zero;
        public Vector2 NPCHead;
        public int BorderRad = 1200;
        public bool BorderActive = false;
        public int IdleTimer;
        public int FlameTimer = 0;
        public int FlameInterval = 0;
        public int FlameStartTimer = 60;
        public int VileThornCooldown = 0;
        public int VileThornCount = 0;
        public int MinionSpawnTimer = 0;
        public int MinionSpawnCount = 0;
        /// <summary>
        /// hammeractive is not affected by state resets.
        /// </summary>
        public bool HammerActive = false;
        public int MinionFailsafe = 0;
        public bool HasBoosted = false;
        public int SigilTimer = 600;
        public int DartTimer = 0;
        public int SoulInterval = 0;
        public int SoulSpawnCount = 0;
        public bool HasSpawnedSigil = false;
        public bool HasSpawnedMines = false;
        public bool Divided = false;
        public int DivisionCooldown = 300;
        public int CooldownAccountedForWallLifetime = -1;
        public int ProjSpawnTimer = 0;
        public int DesperationTimer = 0;
        public bool HasTriggeredNodes = false;
        public bool anyNodesAlive;
        public int nodeCount = 0;
        public int FlameRingCount = 0;
        public int FlameRingVectorCount = Main.rand.Next(8, 23);
        public float FlameRingAngleStep;
        public float FlameRingBaseAngle = 0f;
        public int FlameRingStartRad = 22;
        public float FlameRingRotSpeed = Main.rand.NextFloat(-16f, -8f);
        public int MineType = -1;
        public int DeathIdleTimer = 120;
        public bool DeathSoundFlag = false;
        public int SpawnIdleTimer = 60 * 16;
        public int SpawnIdleRoarFlag = 60 * 8;
        public byte SpawnDarknessAlpha = 0;
        public int SpawnCount = 0;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)currentState);
            writer.WriteVector2(PlayerCenter);
            writer.WriteVector2(DirectionToPlayerCenter);
            writer.WriteVector2(NPCHead);
            writer.Write(BorderRad);
            writer.Write(BorderActive);
            writer.Write(FlameTimer);
            writer.Write(FlameInterval);
            writer.Write(VileThornCooldown);
            writer.Write(VileThornCount);
            writer.Write(MinionSpawnTimer);
            writer.Write(MinionSpawnCount);
            writer.Write(HammerActive);
            writer.Write(MinionFailsafe);
            writer.Write(HasBoosted);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            currentState = (AttackState)reader.ReadInt32();
            PlayerCenter = reader.ReadVector2();
            DirectionToPlayerCenter = reader.ReadVector2();
            NPCHead = reader.ReadVector2();
            BorderRad = reader.ReadInt32();
            BorderActive = reader.ReadBoolean();
            FlameTimer = reader.ReadInt32();
            FlameInterval = reader.ReadInt32();
            VileThornCooldown = reader.ReadInt32();
            VileThornCount = reader.ReadInt32();
            MinionSpawnTimer = reader.ReadInt32();
            MinionSpawnCount = reader.ReadInt32();
            HammerActive = reader.ReadBoolean();
            MinionFailsafe = reader.ReadInt32();
            HasBoosted = reader.ReadBoolean();
        }

        #endregion

        public override void OnSpawn(IEntitySource source)
        {
            BorderActive = true;
            currentState = AttackState.SpawnIdle;
            NPCHead = NPC.Center + new Vector2(0, -79);
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<SpawnSoul>(), 0, 0);
        }


        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (currentState == AttackState.Desperation || anyNodesAlive)
                return false;

            return base.CanBeHitByProjectile(projectile);
        }

        public List<int> ImmuneProjectiles = new List<int>()
        {
            ProjectileID.LastPrismLaser,
            ProjectileID.Meowmere,
            ProjectileID.SolarWhipSword,
            ProjectileID.SolarWhipSwordExplosion,
            ProjectileID.PhantasmArrow,
            ProjectileID.LunarFlare,
            ProjectileID.MoonlordArrow,
            ProjectileID.MoonlordArrowTrail,
            ProjectileID.VortexBeaterRocket,
            ProjectileID.StardustCellMinion,
            ProjectileID.StardustGuardian,
            ProjectileID.EmpressBlade,
            ProjectileID.NebulaArcanum,
            ProjectileID.NebulaArcanumExplosionShot,
            ProjectileID.NebulaArcanumExplosionShotShard,
            ProjectileID.NebulaArcanumSubshot,
            ProjectileID.NebulaBlaze1,
            ProjectileID.NebulaBlaze2,
        };

        public int HitCount = 0;
        public int MaxHits = 20;
        public float decayTimer = 10f;

        private void ApplyAdaptiveReduction(ref NPC.HitModifiers modifiers)
        {
            //Since this runs each time the npc is hit, increment the count and reset the decay timer.
            if (HitCount < MaxHits)
            {
                HitCount++;
            }
            decayTimer = 10;

            float reductionFactor = 1f - 0.02f * HitCount; // 2% per hit
            reductionFactor = MathHelper.Clamp(reductionFactor, 0.2f, 1f);
            modifiers.FinalDamage.Base *= reductionFactor;
        }


        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            ApplyAdaptiveReduction(ref modifiers);
            if (currentState == AttackState.Desperation || anyNodesAlive)
            {
                NPC.immortal = true;
                modifiers.FinalDamage *= 0f;
            }
            if (ImmuneProjectiles.Contains(projectile.type))
            {
                modifiers.FinalDamage *= 0.5f;
            }
        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            if (currentState == AttackState.Desperation || anyNodesAlive)
                return false;

            return base.CanBeHitByItem(player, item);
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            ApplyAdaptiveReduction(ref modifiers);
            if (currentState == AttackState.Desperation || anyNodesAlive)
            {
                NPC.immortal = true;
                modifiers.FinalDamage *= 0f;
            }
        }



        public void ModifyWeather()
        {
            // Enable rain
            Main.raining = true;
            Main.maxRaining = 1f; // max intensity
            Main.rainTime = 60000; // how long it lasts (ticks)

            // Wind settings (safe range: -1.2f to 1.2f)
            Main.windSpeedCurrent = 1.2f;
            Main.windSpeedTarget = 1.2f;

            // Darker skies / storm clouds (0f = clear, 1f = fully stormy)
            Main.cloudAlpha = 0.6f;

            // Restart rain with new settings
            Main.StopRain();
            Main.StartRain();
        }

        public void ModifyClouds()
        {
            Main main = ModContent.GetInstance<Main>();
            Main.cloudAlpha = 0.2f;
            Main.eclipseLight = 1;
            Main.ColorOfTheSkies = Color.Black;
        }

        public void KillSentry()
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p.active && p.sentry)
                {
                    p.Kill();
                }
            }
        }

        public void GeneralEternityChanges(bool Active)
        {
            if (Active)
            {
                NPC.defense = 245;
                //NPC.takenDamageMultiplier = 0.85f;
                NPC.lifeMax = 736000;
                NPC.life = NPC.lifeMax;
                HealAmount = 35;
            }
            else
            {
                NPC.defense = 85;
                NPC.lifeMax = 368000;
                NPC.life = NPC.lifeMax;
                HealAmount = 20;
            }
        }

        public int HealAmount = 0;
        public int DeathInterval = 10;
        public int BorderDustType;
        public static bool ShouldCenterCameraOnNPC = false;
        public float VolumeOnSpawn = 0f;
        public bool RecordedVolume = false;
        public bool SetVolume = false;
        public bool Flag2 = false;
        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            DTConfig cfg = ModContent.GetInstance<DTConfig>();
            DTMusicConfig muscfg = ModContent.GetInstance<DTMusicConfig>();
            DTOptimizationsConfig optcfg = ModContent.GetInstance<DTOptimizationsConfig>();

            NPCHead = NPC.Center + new Vector2(0, -79);
            DirectionToPlayerCenter = (player.Center - NPCHead).SafeNormalize(Vector2.UnitY);

            if (currentState != AttackState.Desperation && currentState != AttackState.KillIdle)
            {
                BorderCol = ColorLib.CursedFlames;
                BorderDustType = DustID.CursedTorch;
            }
            else
            {
                BorderCol = ColorLib.Soul;
                BorderDustType = ModContent.DustType<SoulDust>();
            }

            BorderActive = true; // This happens **after** the loop finishes
            KillSentry();
            if (BorderActive)
            {

                int DustAmount = 90;

                for (int i = 0; i < DustAmount; i++)
                {
                    // Base evenly spaced angle
                    float angle = MathHelper.TwoPi * i / DustAmount;

                    // Add randomness (small jitter)
                    angle += Main.rand.NextFloat(-0.05f, 0.05f); // adjust range for more/less distortion

                    // Keep them on the same circumference
                    Vector2 Pos = NPCHead + Main.rand.NextVector2CircularEdge(BorderRad, BorderRad);

                    Dust Border = Dust.NewDustPerfect(Pos, BorderDustType, Vector2.Zero, 0, BorderCol, 1f);
                    Border.noGravity = true;
                    Border.fadeIn = 1f;
                    Border.scale = Main.rand.NextFloat(0.2f, 4.0f);
                }

                if (!Main.masterMode && (Main.expertMode || EternityIsActive()) && currentState != AttackState.SpawnIdle)
                {
                    ModifyClouds();
                }

                if (Main.masterMode && currentState != AttackState.SpawnIdle)
                {
                    ModifyWeather();
                }
            }

            FlameRingAngleStep = MathHelper.TwoPi / FlameRingVectorCount;

            if (player.Distance(NPC.Center) >= BorderRad && BorderActive && BorderRad > 150)
            {
                if (!EternityIsActive())
                {
                    player.Hurt(new PlayerDeathReason() { CustomReason = NetworkText.FromKey("Mods.DestroyerTest.NPCs.NightmareRose.ExitBarrierDeath", player.name) }, 90, 0, false, true, -1, false, 9, 9, 0);
                }
                else
                {
                    player.KillMe(new PlayerDeathReason() { CustomReason = NetworkText.FromKey("Mods.DestroyerTest.NPCs.NightmareRose.EternityBarrierDeath", player.name) }, 28000, 0, false);
                }
            }
            if (player.Distance(NPC.Center) < BorderRad && BorderActive)
            {
                // Infinite wing time
                player.wingTime = player.wingTimeMax;

                // Reduce wing speed to half
                player.moveSpeed *= 0.5f; // baseWingSpeed should be stored somewhere
                player.GetModPlayer<ApplyArenaEffectsPlayer>().CurrentArenaBoss = ModContent.NPCType<NightmareRoseBoss>();
                if (Main.masterMode)
                {
                    player.AddBuff(ModContent.BuffType<ArenaEffects>(), 20);
                }
            }

            if (HitCount > 0)
            {
                if (decayTimer > 0)
                {
                    decayTimer--;
                }

                if (decayTimer <= 0)
                {
                    HitCount--;
                    decayTimer = 10;
                }
            }


            if (NPC.life <= NPC.lifeMax * 0.05f && currentState != AttackState.Desperation && currentState != AttackState.KillIdle)
            {
                foreach (NPC Const in Main.npc)
                {
                    if (Const.active && (Const.type == ModContent.NPCType<TenebrousConstruct>() || Const.type == ModContent.NPCType<GigaCursedHammer>()))
                    {
                        Const.StrikeInstantKill();
                    }
                }
                SoundEngine.PlaySound(Desperation);
                currentState = AttackState.Desperation;
                DesperationTimer = 0; // reset on entry
            }

            if (player.dead)
            {
                DeathInterval--;
                if (DeathInterval <= 0)
                {
                    NPC.active = false;
                }
            }

            // Assuming this is inside your boss NPC code
            anyNodesAlive = Main.npc.Any(n => n.active && n.type == ModContent.NPCType<CursedFlameNode>());
            nodeCount = Main.npc.Count(n => n.active && n.type == ModContent.NPCType<CursedFlameNode>());

            if (anyNodesAlive)
            {
                NPC.dontTakeDamage = true;
                NPC.immortal = true;
                NPC.life += HealAmount;
                if (Main.rand.NextBool(26))
                {
                    PRTLoader.NewParticle(
                        PRTLoader.GetParticleID<RegenHeart>(),
                        Main.rand.NextVector2FromRectangle(NPC.Hitbox),
                        new Vector2(Main.rand.NextFloat(-2, 2), -5),
                        ColorLib.CursedFlames,
                        1.5f
                    );
                }
            }
            else
            {
                NPC.immortal = false;
                NPC.dontTakeDamage = false;
            }


            if (NPC.life >= NPC.lifeMax * 0.24f && NPC.life <= NPC.lifeMax * 0.25f)
            {
                if (HasTriggeredNodes == false)
                {
                    currentState = AttackState.Nodes;
                    HasTriggeredNodes = true;
                }
            }

            if (player.active == false || player.dead == true)
            {
                OnKill();
            }

            if (Divided)
            {
                stateWeights[AttackState.WallDarts] = 0.00f;
                stateWeights[AttackState.CursedFlames] = 0.00f;
                if (CooldownAccountedForWallLifetime <= 0)
                {
                    CooldownAccountedForWallLifetime = DivisionCooldown + 1200;
                }

                CooldownAccountedForWallLifetime--;

                if (CooldownAccountedForWallLifetime <= DivisionCooldown)
                {
                    Divided = false;
                }

            }

            Rotation--;

            PlayerCenter = player.Center;


            if (!Main.dedServ && currentState == AttackState.SpawnIdle)
            {
                if (!RecordedVolume)
                {
                    VolumeOnSpawn = Main.musicVolume;
                    RecordedVolume = true;
                }
                Main.musicVolume -= 0.1f;
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/RoseIdle");
            }
            if (!Main.dedServ && !EternityIsActive() && currentState != AttackState.SpawnIdle)
            {
                if (!SetVolume)
                {
                    Main.musicFade[Music] = 1;
                    Main.musicVolume = VolumeOnSpawn;
                    SetVolume = true;
                }
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Tribulation");
            }
            if (!Main.dedServ && EternityIsActive() && !muscfg.EternityMusic && currentState != AttackState.SpawnIdle)
            {
                if (!SetVolume)
                {
                    Main.musicFade[Music] = 1;
                    Main.musicVolume = VolumeOnSpawn;
                    SetVolume = true;
                }
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Tribulation");
            }
            if (!Main.dedServ && EternityIsActive() && muscfg.EternityMusic && currentState != AttackState.SpawnIdle)
            {
                if (!SetVolume)
                {
                    Main.musicFade[Music] = 1;
                    Main.musicVolume = VolumeOnSpawn;
                    SetVolume = true;
                }
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Placeholder4");
            }

            NPC.velocity = Vector2.Zero;

            int MinionSpawnType = Main.rand.Next(new int[]
                {
                    ModContent.NPCType<DarkGluttonHead>(),
                    ModContent.NPCType<DarkPredatorHead>(),
                    ModContent.NPCType<DarkArchmage>(),
                    ModContent.NPCType<TenebrousPhantasm>()
                });





            Mod.Logger.Info($"Current State: {currentState}");

            switch (currentState)
            {
                case AttackState.SpawnIdle:
                    {
                        NPC.dontTakeDamage = true;
                        ShouldCenterCameraOnNPC = true;
                        player.channel = false;
                        player.moveSpeed *= 0;
                        if (SpawnCount <= 0)
                        {
                            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/RoseSpawnIdle"));
                        }
                        SpawnCount++;
                        if (SpawnCount < SpawnIdleRoarFlag)
                        {
                            VingetteScale *= 0.99f;
                        }
                        if (SpawnCount >= SpawnIdleRoarFlag)
                        {
                            SpawnDarknessAlpha = 0;
                            if (EternityIsActive())
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<NightmareeRoseBackgroundProj>(), 0, 0f);
                            }
                            currentState = AttackState.Idle;
                            NPC.dontTakeDamage = false;
                            SpawnCount = 0;
                        }
                        break;
                    }
                case AttackState.Idle:
                    if (NPC.type == ModContent.NPCType<NightmareRoseBoss>())
                    {
                        NPC.aiStyle = -1;
                        ShouldCenterCameraOnNPC = false;
                        if (VingetteScale < 10)
                        {
                            VingetteScale += 0.75f;
                        }


                        int IdleMax = -1;
                        if (!Main.expertMode && !Main.masterMode && !EternityIsActive())
                        {
                            IdleMax = 100;
                        }
                        if (Main.expertMode && !Main.masterMode && !EternityIsActive())
                        {
                            IdleMax = 80;
                        }
                        if (Main.masterMode || EternityIsActive())
                        {
                            IdleMax = 60;
                        }

                        if (IdleTimer < IdleMax)
                        {
                            IdleTimer++;
                        }

                        if (IdleTimer >= IdleMax)
                        {
                            currentState = GetRandomState();
                            IdleTimer = 0;

                        }
                    }
                    break;
                case AttackState.Nodes:
                    if (NPC.type == ModContent.NPCType<NightmareRoseBoss>())
                    {
                        NPC.aiStyle = -1;
                        SoundEngine.PlaySound(NodeSpawnSound);
                        NodeSpawn();
                        currentState = GetRandomState();
                    }
                    break;
                case AttackState.CursedFlames:
                    {
                        if (Divided)
                        {
                            ResetState();
                        }
                        if (!EternityIsActive() && !Divided)
                        {
                            if (FlameStartTimer >= 60)
                            {
                                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/CursedFlamesWarn") with { Volume = 1.5f });
                            }
                            FlameStartTimer--;
                            if (HasBoosted == false)
                            {
                                player.velocity += new Vector2(0, -15);
                                HasBoosted = true;
                            }
                            if (FlameTimer < 240 && FlameStartTimer <= 0)
                            {
                                FlameTimer++;
                                FlameInterval++;
                                Vector2 velocity = DirectionToPlayerCenter.SafeNormalize(Vector2.UnitY);
                                if (FlameInterval >= 10)
                                {
                                    SoundEngine.PlaySound(Fire);
                                    Projectile.NewProjectile(Entity.GetSource_FromThis(), NPCHead, velocity * 20f, ModContent.ProjectileType<CursedFlameProj>(), 15, 0);
                                    FlameInterval = 0;
                                }
                            }
                            if (FlameTimer >= 240)
                            {
                                ResetState();
                            }
                        }
                        if (EternityIsActive() && !Divided)
                        {
                            if (FlameTimer < 240)
                            {
                                FlameTimer++;
                                ContemptAttack();
                            }
                            if (FlameTimer >= 240)
                            {
                                ResetState();
                            }
                        }
                    }
                    break;
                case AttackState.WallDarts:
                    {
                        if (Divided)
                        {
                            ResetState();
                        }
                        if (EternityIsActive() && !Divided)
                        {
                            if (DartTimer < 800)
                            {
                                DartTimer++;
                                DartAttack();
                            }
                            if (DartTimer >= 800)
                            {
                                ResetState();
                            }
                        }
                        else
                        {
                            ResetState();
                        }
                        break;
                    }
                case AttackState.FlameRing:
                    {
                        if (EternityIsActive())
                        {
                            if (FlameRingCount < 9 && Main.GameUpdateCount % 60 == 0)
                            {
                                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ChargeBreak") with { PitchVariance = 1f });

                                Opus.RingProjectileOutwardRandomDir(ModContent.ProjectileType<TenebrisFlames>(), 7, player.Center, 300, 25, 1, 8, AI2: 2);
                                FlameRingCount++;
                            }
                            if (FlameRingCount >= 9)
                            {
                                ResetState();
                            }
                        }
                        else
                        {
                            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Constitution_Jab") with { PitchVariance = 1f, Volume = 3f });

                            if (FlameRingCount < 9 && Main.GameUpdateCount % 60 == 0)
                            {
                                for (int i = 0; i < FlameRingVectorCount; i++)
                                {
                                    float randomOffset = Main.rand.NextFloat(-0.4f, 0.4f);
                                    float angle = FlameRingBaseAngle + i * FlameRingAngleStep + randomOffset;

                                    float radius = FlameRingStartRad;
                                    float curvedAngle = angle - FlameRingRotSpeed * MathHelper.PiOver2;

                                    Vector2 startPos = NPC.Center + radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                                    Vector2 outwardVel = new Vector2((float)Math.Cos(curvedAngle), (float)Math.Sin(curvedAngle)) * 0.5f; // outward speed
                                    Vector2 spinVel = outwardVel.RotatedBy(MathHelper.PiOver2) * 0.8f; // tangential spin

                                    Vector2 finalVel = (outwardVel + spinVel).SafeNormalize(Vector2.UnitY) * FlameRingRotSpeed;

                                    Projectile.NewProjectile(Entity.GetSource_FromThis(), startPos, finalVel, ModContent.ProjectileType<CursedNodeCrystal>(), 15, 5, ai2: 2);
                                }
                            }
                            if (FlameRingCount >= 9)
                            {
                                ResetState();
                            }
                        }
                        break;
                    }
                case AttackState.Lances:
                    {
                        if (EternityIsActive())
                        {
                            int numProjectiles = 10;
                            float rotationStep = MathHelper.TwoPi / numProjectiles;

                            SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                            for (int i = 0; i < numProjectiles; i++)
                            {
                                Vector2 velocity = new Vector2(12f, 0f).RotatedBy(rotationStep * i);
                                Projectile.NewProjectile(
                                    Entity.GetSource_FromThis(),
                                    NPCHead,
                                    velocity,
                                    ModContent.ProjectileType<TenebrisLance>(),
                                    15,
                                    6
                                );
                            }
                            ResetState();
                        }
                        else
                        {
                            for (int i = 0; i < FlameRingVectorCount; i++)
                            {
                                float randomOffset = Main.rand.NextFloat(-0.4f, 0.4f);
                                float angle = FlameRingBaseAngle + i * FlameRingAngleStep + randomOffset;

                                float radius = FlameRingStartRad;
                                float curvedAngle = angle - FlameRingRotSpeed * MathHelper.PiOver2;

                                Vector2 startPos = NPC.Center + radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                                Vector2 outwardVel = new Vector2((float)Math.Cos(curvedAngle), (float)Math.Sin(curvedAngle)) * 1.5f; // outward speed
                                Vector2 spinVel = outwardVel.RotatedBy(MathHelper.PiOver2) * 0.8f; // tangential spin

                                Vector2 finalVel = (outwardVel + spinVel).SafeNormalize(Vector2.UnitY) * FlameRingRotSpeed;

                                Projectile.NewProjectile(Entity.GetSource_FromThis(), startPos, finalVel, ModContent.ProjectileType<CorruptPetalHostile>(), 16, 5, ai2: 2);
                            }
                            ResetState();
                        }
                        break;
                    }
                case AttackState.Napalm:
                    {
                        player.wingTime = 0;
                        VileThornCooldown++;
                        if (Main.GameUpdateCount % 30 == 0)
                        {
                            VileThornCount += 1;

                            for (int e = 0; e < 6; e++)
                            {
                                SoundEngine.PlaySound(Napalm);
                                Projectile proj = Projectile.NewProjectileDirect(
                                        Entity.GetSource_FromThis(),
                                        NPCHead,
                                        new Vector2(Main.rand.NextFloat(-5, 6), -15),
                                        ModContent.ProjectileType<CursedFlameNapalm>(),
                                        18,
                                        2
                                    );
                                proj.tileCollide = true;
                                proj.hostile = true;
                                proj.friendly = false;
                                proj.timeLeft = 480;
                            }

                            if (VileThornCount >= 8)
                            {
                                ResetState();
                            }
                        }
                    }
                    break;

                case AttackState.Minions:
                    {
                        /*
                        MinionFailsafe++;
                        MinionSpawnTimer++;

                        if (MinionSpawnTimer == 10)
                        {
                            NPC Minion = NPC.NewNPCDirect(Entity.GetSource_FromThis(), NPCHead, MinionSpawnType);
                            Minion.damage = 30;
                            Minion.lifeMax = 400;
                            Minion.life = 400;
                            Minion.noGravity = true;

                            MinionSpawnCount += 1;
                            MinionSpawnTimer = 0;
                        }
                        if (MinionSpawnCount >= 6)
                        {
                            currentState = AttackState.Idle;
                            ResetState();
                        }
                        if (MinionFailsafe >= 1200)
                        {
                            currentState = AttackState.Idle;
                            ResetState();
                        }*/
                    }

                    ResetState();
                    break;
                case AttackState.RottenPetals:
                    {
                        // Will sprite later.
                        ResetState();
                    }
                    break;


                case AttackState.OvergrownHammer:
                    {
                        NPC Minion;
                        if (NPC.life < NPC.lifeMax * 0.4f && !HammerActive)
                        {
                            bool eternity = EternityIsActive();
                            //Main.NewText("EternityIsActive returned: " + eternity);

                            if (!Main.masterMode && !eternity)
                            {
                                Minion = NPC.NewNPCDirect(Entity.GetSource_FromThis(), NPC.Center, ModContent.NPCType<GigaCursedHammer>());
                            }
                            else if (Main.masterMode || eternity)
                            {
                                Minion = NPC.NewNPCDirect(Entity.GetSource_FromThis(), NPC.Center, ModContent.NPCType<TenebrousConstruct>());
                            }
                            HammerActive = true;
                        }

                        else
                        {
                            ResetState();
                        }
                    }
                    break;
                case AttackState.DemoniteWhisper:
                    {
                        if (Main.GameUpdateCount % 180 == 0)
                        {
                            SummonSouls();
                            SoulSpawnCount++;
                            if (EternityIsActive())
                            {
                                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                                for (int f = 0; f < 2; f++)
                                {
                                    float rot = angle + MathHelper.Pi * f;
                                    Vector2 dir = rot.ToRotationVector2();
                                    Projectile.NewProjectile(Entity.GetSource_FromAI(), NPCHead, dir * 6f, ModContent.ProjectileType<BigSoul>(), 18, 7);
                                }
                            }
                        }
                        if (SoulSpawnCount >= 4)
                        {
                            ResetState();

                        }
                    }
                    break;
                case AttackState.ArenaDivide:
                    {
                        if (EternityIsActive())
                        {
                            if (!Divided && CooldownAccountedForWallLifetime <= 0)
                            {
                                if (Main.rand.NextBool(3))
                                {
                                    ArenaDivision();
                                }
                                else
                                {
                                    currentState = AttackState.Idle;
                                }
                            }
                        }
                        else
                        {
                            currentState = AttackState.Idle;
                        }
                        break;
                    }
                case AttackState.CorruptSigil:
                    {
                        ManageSigil(Main.rand.NextVector2Circular(BorderRad, BorderRad));
                        ResetState();
                    }
                    break;
                case AttackState.BlossomMine:
                    {
                        BlossomMines(Main.rand.NextVector2FromRectangle(new Rectangle(0, 0, BorderRad, BorderRad)));
                        ResetState();
                    }
                    break;
                case AttackState.Desperation:
                    {
                        NPC.dontTakeDamage = true;
                        ShouldCenterCameraOnNPC = true;
                        if (cfg.EnableDebugMessages)
                        {
                            Mod.Logger.Info($"Desperation Timer: {DesperationTimer}");
                        }
                        if (DesperationTimer < 1200)
                        {

                            DesperationTimer++;
                            if (Main.GameUpdateCount % 80 == 0 && DesperationTimer < 900)
                            {
                                SoundEngine.PlaySound(DespShootMine);
                                SoulBombSpawn();
                            }
                            BorderRad = (int)MathHelper.SmoothStep(BorderRad, 0, 0.001f);
                            RingScale = MathHelper.SmoothStep(RingScale, 0, 0.00165f);
                        }
                        if (DesperationTimer >= 1200)
                        {
                            NPC.netUpdate = true;
                            currentState = AttackState.KillIdle;
                            BorderActive = false;
                            Main.NewText("Get away from the Rose!!", ColorLib.Soul);
                            PRTLoader.NewParticle(PRTLoader.GetParticleID<BloomRingSharp>(), NPC.Center, Vector2.Zero, Color.White, 0.001f, 1);
                        }
                    }
                    break;
                case AttackState.KillIdle:
                    {
                        ShouldCenterCameraOnNPC = false;
                        if (cfg.EnableDebugMessages)
                        {
                            Mod.Logger.Info($"Death Timer: {DeathIdleTimer}");
                        }

                        if (DeathIdleTimer > 0)
                        {
                            OverlayAlpha = (byte)MathHelper.Clamp(
                                255f * (1f - (DeathIdleTimer / 120f)),
                                0f,
                                255f
                            );
                            if (!DeathSoundFlag)
                            {
                                SoundEngine.PlaySound(Kill);
                                DeathSoundFlag = true;
                            }
                            GatherParticle();
                            BorderRad = (int)MathHelper.SmoothStep(BorderRad, 0, 0.001f);
                            RingScale = MathHelper.SmoothStep(RingScale, 0, 0.0016f);
                            DeathIdleTimer--;
                        }
                        if (DeathIdleTimer <= 0)
                        {
                            Projectile.NewProjectile(Entity.GetSource_FromThis(), NPCHead, Vector2.Zero, ModContent.ProjectileType<SoulExplosion>(), 200, 18);
                            if (cfg.EnableDebugMessages)
                            {
                                Mod.Logger.Info($"Attempted Kill. Death Timer: {DeathIdleTimer}");
                            }
                            if (!DownedBossSystem.downedNightmareRoseBoss)
                            {
                                Item.NewItem(Item.GetSource_None(), NPCHead, ModContent.ItemType<RoseSoul>(), 1, true, 0, false, false);
                            }
                            NPC.immortal = false;
                            NPC.StrikeInstantKill();

                        }
                    }
                    break;


            }
        }

        float RingScale = 6.2f;
        float Rotation = 0f;
        byte OverlayAlpha = 0;
        public Color BorderCol;
        public float VingetteScale = 2f;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            base.PostDraw(spriteBatch, screenPos, drawColor);
            if (FlameTimer < 240 && FlameTimer >= 0 && currentState == AttackState.CursedFlames && !EternityIsActive())
            {
                DrawTelegraph(NPCHead, PlayerCenter, DTAssetLib.FlameTelegraph.Value);
            }


            DTUtils Utility = new DTUtils();
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            if (BorderActive)
            {
                Main.EntitySpriteDraw(DTAssetLib.NightmareRoseArenaBorder.Value, NPCHead - Main.screenPosition, null, BorderCol, Rotation, new Vector2(DTAssetLib.NightmareRoseArenaBorder.Value.Width / 2, DTAssetLib.NightmareRoseArenaBorder.Value.Height / 2), RingScale, SpriteEffects.None, 0);
            }
            Opus.ReturnToDefaultDrawing(spriteBatch);

            Asset<Texture2D> White = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/NightmareRoseDeathFade");
            if (currentState == AttackState.KillIdle)
            {
                Main.EntitySpriteDraw(White.Value, NPC.Center - Main.screenPosition, null, DTColorUtils.WithAlpha(Color.White, OverlayAlpha), 0f, new Vector2(White.Value.Width / 2, White.Value.Height / 2), 1f, SpriteEffects.None, 0);
            }

        }


        private Dictionary<AttackState, float> stateWeights = new()
        {
            { AttackState.Idle, 1.0f },
            { AttackState.CursedFlames, 1.0f },
            { AttackState.DemoniteWhisper, 1.0f },
            { AttackState.Minions, 1.0f },
            { AttackState.OvergrownHammer, 1.0f },
            { AttackState.RottenPetals, 1.0f },
            { AttackState.ArenaDivide, 1.0f },
            { AttackState.Napalm, 1.0f },
            { AttackState.FlameRing, 1.0f },
            { AttackState.WallDarts, 1.0f },
            { AttackState.Lances, 1.0f },
            { AttackState.CorruptSigil, 1.0f },
            { AttackState.BlossomMine, 0.65f },
        };

        public AttackState? lastAttack = null;


        private AttackState GetRandomState()
        {
            if (lastAttack == AttackState.DemoniteWhisper)
            {
                stateWeights[AttackState.CursedFlames] = 0f;
                stateWeights[AttackState.WallDarts] = 0f;
                stateWeights[AttackState.Lances] = 0f;
            }
            else
            {
                ResetWeights();
            }

            if (Divided)
            {
                stateWeights[AttackState.CursedFlames] = 0f;
                stateWeights[AttackState.WallDarts] = 0f;
            }
            else
            {
                ResetWeights();
            }

            // Exclude the current state
            var validStates = stateWeights
                .Where(pair => pair.Key != currentState && pair.Value > 0)
                .ToList();

            float totalWeight = validStates.Sum(pair => pair.Value);
            float roll = Main.rand.NextFloat() * totalWeight;

            float cumulative = 0f;
            foreach (var pair in validStates)
            {
                cumulative += pair.Value;
                if (roll <= cumulative)
                    return pair.Key;
            }



            // Fallback (should never happen unless all weights are 0)
            return currentState;
        }

        private void ResetWeights()
        {
            stateWeights[AttackState.Idle] = 1f;
            stateWeights[AttackState.CursedFlames] = 1f;
            stateWeights[AttackState.DemoniteWhisper] = 1f;
            stateWeights[AttackState.Minions] = 1f;
            stateWeights[AttackState.OvergrownHammer] = 1f;
            stateWeights[AttackState.RottenPetals] = 1f;
            stateWeights[AttackState.ArenaDivide] = 1f;
            stateWeights[AttackState.Napalm] = 1f;
            stateWeights[AttackState.FlameRing] = 1f;
            stateWeights[AttackState.WallDarts] = 1f;
            stateWeights[AttackState.Lances] = 1f;
            stateWeights[AttackState.CorruptSigil] = 1f;
            stateWeights[AttackState.BlossomMine] = 1f;
        }


        private void ResetState()
        {
            if (NPC.type == ModContent.NPCType<NightmareRoseBoss>())
            {
                NPC.netUpdate = true;
                currentState = AttackState.Idle;
                NPC.ai[0] = NPC.ai[1] = NPC.ai[2] = NPC.ai[3] = 0;

                FlameTimer = 0;
                FlameInterval = 0;
                FlameStartTimer = 60;
                VileThornCooldown = 0;
                VileThornCount = 0;
                MinionSpawnTimer = 0;
                MinionSpawnCount = 0;
                MinionFailsafe = 0;
                SigilTimer = 600;
                DartTimer = 0;
                SoulInterval = 0;
                SoulSpawnCount = 0;
                DivisionCooldown = 300;
                ProjSpawnTimer = 0;
                DesperationTimer = 0;
                nodeCount = 0;
                FlameRingCount = 0;
                FlameRingVectorCount = Main.rand.Next(8, 23);
                FlameRingAngleStep = 0f;
                FlameRingBaseAngle = 0f;
                FlameRingStartRad = 22;
                FlameRingRotSpeed = Main.rand.NextFloat(-16f, -8f);
                MineType = -1;
                DeathIdleTimer = 120;
                SpawnIdleTimer = 60 * 16;
                SpawnIdleRoarFlag = 60 * 8;
                SpawnDarknessAlpha = 0;
                SpawnCount = 0;

                HasBoosted = false;
                HasSpawnedSigil = false;
                HasSpawnedMines = false;
            }
        }


        public void DrawTelegraph(Vector2 start, Vector2 end, Texture2D texture)
        {
            Vector2 direction = end - start;
            float length = direction.Length();
            direction.Normalize();
            texture ??= ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/CursedFlamesTelegraph").Value;
            SpriteBatch spriteBatch = Main.spriteBatch;

            float rotation = direction.ToRotation();

            // Assuming your texture is a chain segment, like 16px long
            float segmentLength = texture.Height * 0.75f; // or Width, depending on the texture orientation
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (float i = 0; i < length; i += segmentLength)
            {
                Vector2 position = start + direction * i;

                Main.spriteBatch.Draw(
                    texture,
                    position - Main.screenPosition,
                    null,
                    new Color(179, 252, 0) * 0.2f,
                    rotation + MathHelper.PiOver2, // Adjust if your texture points upward
                    new Vector2(texture.Width / 2f, texture.Height / 2f), // Origin at center
                    1f, // Scale
                    SpriteEffects.None,
                    0f
                );
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public bool Flag3 = false;
        public void ArenaDivision()
        {
            if (!Flag3)
            {
                SoundEngine.PlaySound(ArenaDivide);
                Projectile Divider1 = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), NPCHead, Vector2.Zero, ModContent.ProjectileType<CursedFlameWallVertical>(), 30, 3);

                Divider1.timeLeft = 1200;
                Flag3 = true;
            }
        }

        public void GatherParticle()
        {
            for (int y = 0; y < 2; y++)
            {
                Vector2 Spawn = NPCHead + Main.rand.NextVector2CircularEdge(400, 400);
                Vector2 Inward = NPCHead - Spawn;
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Spawn, Inward * 0.025f, ColorLib.Soul, 5f);
            }
        }

        public void SoulBombSpawn()
        {
            for (int j = 0; j < 3; j++)
            {
                Vector2 spawnPos = NPCHead;
                Vector2 targetPos = NPCHead + Main.rand.NextVector2CircularEdge(BorderRad, BorderRad);
                Vector2 direction = (targetPos - spawnPos).SafeNormalize(Vector2.Zero); // SafeNormalize prevents division by zero

                Projectile SB = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), spawnPos, direction * 6, ModContent.ProjectileType<SoulCrystalBomb>(), 20, 1);
                SB.timeLeft = 60;
            }
        }

        public void NodeSpawn()
        {
            float radius = 200;
            int projectileCount = 6;


            SoundEngine.PlaySound(SoundID.Item20);

            for (int i = 0; i < projectileCount; i++)
            {
                // Get evenly spaced angle with rotation offset
                float angle = MathHelper.TwoPi * i / projectileCount;
                Vector2 spawnOffset = radius * angle.ToRotationVector2(); // position on the circle
                Vector2 spawnPosition = NPC.Center + spawnOffset;

                NPC.NewNPC(Entity.GetSource_FromThis(), (int)spawnPosition.X, (int)spawnPosition.Y, ModContent.NPCType<CursedFlameNode>());

                PRTLoader.NewParticle(PRTLoader.GetParticleID<BloomRingSharp>(), NPC.Center, Vector2.Zero, ColorLib.CursedFlames, 0.001f, 1);

            }
            Main.NewText("The Nightmare Rose calls upon the Corruption for Help!", ColorLib.CursedFlames);
        }

        public void ManageSigil(Vector2 SpawnPos)
        {
            NPC.NewNPC(Entity.GetSource_FromAI(), (int)SpawnPos.X, (int)SpawnPos.Y, ModContent.NPCType<CorruptSigil>(), 0);
        }

        public void SummonSouls()
        {
            Player player = Main.LocalPlayer;
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/SoulSummon"));
            player.GetModPlayer<ScreenshakePlayer>().screenshakeTimer = 10;
            player.GetModPlayer<ScreenshakePlayer>().screenshakeMagnitude = 8;
            if (!EternityIsActive())
            {
                for (int a = 0; a < 10; a++)
                {
                    Vector2 SpawnPoint = new Vector2(NPC.Center.X + Main.rand.Next(-BorderRad, BorderRad), NPC.Center.Y + 1800);
                    Projectile.NewProjectile(Entity.GetSource_FromThis(), SpawnPoint, new Vector2(0, -8), ModContent.ProjectileType<TormentedSoul>(), 10, 2);
                }
            }
            if (EternityIsActive())
            {
                for (int a = 0; a < 5; a++)
                {
                    Vector2 SpawnPoint = new Vector2(NPC.Center.X + Main.rand.Next(-BorderRad, BorderRad), NPC.Center.Y + 1800);
                    Projectile.NewProjectile(Entity.GetSource_FromThis(), SpawnPoint, new Vector2(0, -16), ModContent.ProjectileType<TormentedSoul>(), 10, 2);
                }
                for (int a = 0; a < 5; a++)
                {
                    bool Side = Main.rand.NextBool(2);
                    Vector2 SpawnPoint;
                    Vector2 MoveDir;
                    if (Side)
                    {
                        MoveDir = new Vector2(-16, 0);
                        SpawnPoint = new Vector2(NPC.Center.X + 1800, NPC.Center.Y + Main.rand.Next(-BorderRad, BorderRad));
                    }
                    else
                    {
                        MoveDir = new Vector2(16, 0);
                        SpawnPoint = new Vector2(NPC.Center.X - 1800, NPC.Center.Y + Main.rand.Next(-BorderRad, BorderRad));
                    }

                    Projectile.NewProjectile(Entity.GetSource_FromThis(), SpawnPoint, MoveDir, ModContent.ProjectileType<TormentedSoul>(), 10, 2);
                }
            }
        }

        public void BlossomMines(Vector2 SpawnPos)
        {
            SoundEngine.PlaySound(SoundID.Item163);
            for (int e = 0; e < 6; e++)
            {
                Vector2 minePosition = Main.rand.NextVector2FromRectangle(
                new Rectangle(
                    (int)Main.LocalPlayer.Center.X - Main.screenWidth / 2,
                    (int)Main.LocalPlayer.Center.Y - Main.screenHeight / 2,
                    Main.screenWidth,
                    Main.screenHeight
                    )
                );

                Projectile.NewProjectile(
                    Entity.GetSource_FromThis(),
                    minePosition,
                    Vector2.Zero,
                    ModContent.ProjectileType<BlossomMine>(),
                    10,
                    0f
                );
            }
        }

        public void ContemptAttack()
        {
            float radius = BorderRad;
            int projectileCount = 6;
            float rotationOffset = (float)(Main.GameUpdateCount % 360) * MathHelper.ToRadians(0.5f);
            Projectile flame = null;

            if (Main.GameUpdateCount % 10 == 0)
            {
                SoundEngine.PlaySound(Fire);

                for (int i = 0; i < projectileCount; i++)
                {
                    // Get evenly spaced angle with rotation offset
                    float angle = MathHelper.TwoPi * i / projectileCount + rotationOffset;
                    Vector2 spawnOffset = radius * angle.ToRotationVector2(); // position on the circle
                    Vector2 spawnPosition = NPCHead + spawnOffset;

                    Vector2 toOrigin = NPCHead - spawnPosition;
                    toOrigin = toOrigin.SafeNormalize(Vector2.UnitY);

                    flame = Projectile.NewProjectileDirect(
                        Entity.GetSource_FromThis(),
                        spawnPosition,
                        toOrigin * 20f,
                        ModContent.ProjectileType<CursedFlameProj>(),
                        15,
                        2,
                        Main.LocalPlayer.whoAmI
                    );
                    flame.timeLeft = 60;


                }

                PRTLoader.NewParticle(PRTLoader.GetParticleID<BloomRingSharp>(), NPCHead, Vector2.Zero, ColorLib.CursedFlames, 0.001f, 1);
            }
            if (flame != null && flame.Center == NPCHead)
            {
                flame.Kill();
            }
        }

        public void DartAttack()
        {
            float radius = BorderRad;
            int projectileCount = 4;
            float rotationOffset = (float)(Main.GameUpdateCount % 360) * MathHelper.ToRadians(0.5f);
            Projectile Dart = null;

            if (Main.GameUpdateCount % 5 == 0)
            {

                for (int i = 0; i < projectileCount; i++)
                {
                    // Get evenly spaced angle with rotation offset
                    float angle = MathHelper.TwoPi * i / projectileCount + rotationOffset;
                    Vector2 spawnOffset = radius * angle.ToRotationVector2(); // position on the circle
                    Vector2 spawnPosition = NPCHead + spawnOffset;

                    Vector2 toOrigin = NPCHead - spawnPosition;
                    toOrigin = toOrigin.SafeNormalize(Vector2.UnitY);

                    Dart = Projectile.NewProjectileDirect(
                        Entity.GetSource_FromThis(),
                        spawnPosition,
                        toOrigin * 2f,
                        ModContent.ProjectileType<TenebrisDart>(),
                        10,
                        2
                    );
                }
            }
            if (Dart != null && Dart.Center == NPCHead)
            {
                Dart.Kill();
            }

            if (Main.GameUpdateCount % 240 == 0)
            {
                int numProjectiles = 6;
                float rotationStep = MathHelper.TwoPi / numProjectiles;

                SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                for (int i = 0; i < numProjectiles; i++)
                {
                    Vector2 velocity = new Vector2(12f, 0f).RotatedBy(rotationStep * i);
                    Projectile.NewProjectile(
                        Entity.GetSource_FromThis(),
                        NPCHead,
                        velocity,
                        ModContent.ProjectileType<TenebrisLance>(),
                        10,
                        6
                    );
                }
            }
        }


        public override void OnKill()
        {
            int Gore1 = Mod.Find<ModGore>("NightmareRoseGore1").Type;
            int Gore2 = Mod.Find<ModGore>("NightmareRoseGore2").Type;
            int Gore3 = Mod.Find<ModGore>("NightmareRoseGore3").Type;
            int Gore4 = Mod.Find<ModGore>("NightmareRoseGore4").Type;

            var entitySource = NPC.GetSource_Death();
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-14, 14), Main.rand.Next(0, 10)), Gore1);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-14, 14), Main.rand.Next(0, 10)), Gore2);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-14, 14), Main.rand.Next(0, 10)), Gore3);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-14, 14), Main.rand.Next(0, 10)), Gore4);
        }

    }

    public class NightmareRoseCameraModification : ModSystem
    {
        private Vector2 camPos;
        private bool hasCamPos = false;

        public override void ModifyScreenPosition()
        {
            if (Main.dedServ)
                return;

            DTConfig cfg = ModContent.GetInstance<DTConfig>();
            if (!cfg.DragCamera)
                return;

            if (!hasCamPos)
            {
                camPos = Main.screenPosition;
                hasCamPos = true;
            }

            Vector2 screenHalf = new Vector2(Main.screenWidth * 0.5f, Main.screenHeight * 0.5f);
            Vector2 playerCenter = Main.LocalPlayer.Center;

            bool anyBossActive = IsAnyBossActive();

            // initialize to default to silence compiler warnings
            NPC nightmareBoss = default!;
            bool hasNightmareBoss = NightmareRoseBoss.ShouldCenterCameraOnNPC && TryGetNightmareRoseBoss(out nightmareBoss);

            Vector2 target = hasNightmareBoss
                ? nightmareBoss.Center - screenHalf
                : playerCenter - screenHalf;

            float distToPlayer = Vector2.Distance(camPos + screenHalf, playerCenter);

            if (!anyBossActive && distToPlayer > Main.screenWidth)
            {
                // snap if too far and no boss is active
                camPos = playerCenter - screenHalf;
            }
            else
            {
                // smooth follow
                float lerpFactor = anyBossActive ? 0.08f : 0.12f;
                camPos = Vector2.Lerp(camPos, target, lerpFactor);
            }

            Main.screenPosition = camPos;
        }

        private bool IsAnyBossActive()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];
                if (n != null && n.active && n.boss)
                    return true;
            }
            return false;
        }

        private bool TryGetNightmareRoseBoss(out NPC boss)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];
                if (n.active && n.type == ModContent.NPCType<NightmareRoseBoss>())
                {
                    boss = n;
                    return true;
                }
            }

            boss = default;
            return false;
        }
    }






    public class NightmareRoseBCL : ModSystem
    {
        public override void PostSetupContent()
        {
            // Most often, mods require you to use the PostSetupContent hook to call their methods. This guarantees various data is initialized and set up properly

            // Boss Checklist shows comprehensive information about bosses in its own UI. We can customize it:
            // https://forums.terraria.org/index.php?threads/.50668/
            DoBossChecklistIntegration();

            // We can integrate with other mods here by following the same pattern. Some modders may prefer a ModSystem for each mod they integrate with, or some other design.
        }

        private void DoBossChecklistIntegration()
        {
            // The mods homepage links to its own wiki where the calls are explained: https://github.com/JavidPack/BossChecklist/wiki/%5B1.4.4%5D-Boss-Log-Entry-Mod-Call
            // If we navigate the wiki, we can find the "LogBoss" method, which we want in this case
            // A feature of the call is that it will create an entry in the localization file of the specified NPC type for its spawn info, so make sure to visit the localization file after your mod runs once to edit it

            if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod))
            {
                return;
            }

            // For some messages, mods might not have them at release, so we need to verify when the last iteration of the method variation was first added to the mod, in this case 1.6
            // Usually mods either provide that information themselves in some way, or it's found on the GitHub through commit history/blame
            if (bossChecklistMod.Version < new Version(1, 6))
            {
                return;
            }

            // The "LogBoss" method requires many parameters, defined separately below:

            // Your entry key can be used by other developers to submit mod-collaborative data to your entry. It should not be changed once defined
            string internalName = "NightmareRose";

            // Value inferred from boss progression, see the wiki for details
            float weight = 18.35f;

            // Used for tracking checklist progress
            Func<bool> downed = () => DownedBossSystem.downedNightmareRoseBoss;

            LocalizedText Hint = Language.GetText("Mods.DestroyerTest.BossChecklist.NightmareRose.Hint");

            LocalizedText Despawn = Language.GetText("Mods.DestroyerTest.NPCs.NightmareRose.DespawnMessage");

            // The NPC type of the boss
            int bossType = ModContent.NPCType<NightmareRoseBoss>();

            // The item used to summon the boss with (if available)
            int spawnItem = ModContent.ItemType<TheBotanistsCurse>();

            // "collectibles" like relic, trophy, mask, pet
            List<int> collectibles = new List<int>()
            {
                ModContent.ItemType<Contempt>(),
                ModContent.ItemType<GigaCursedHammerWeapon>(),
                ModContent.ItemType<PossessedDartRifleItem>(),
                ModContent.ItemType<DeadlyBlossom>(),
                ItemID.CursedFlame,
                ModContent.ItemType<Item_NightmareRoseRelic>(),
                ModContent.ItemType<Item_NightmareRoseTrophy>()
            };

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            var customPortrait = (SpriteBatch sb, Rectangle rect, Color color) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/NightmareRoseBossBossChecklist").Value;
                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                sb.Draw(texture, centered, color);
            };

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = spawnItem,
                    ["collectibles"] = collectibles,
                    ["customPortrait"] = customPortrait,
                    ["spawnInfo"] = Hint,
                    ["despawnMessage"] = Despawn,

                    // Other optional arguments as needed are inferred from the wiki
                }
            );


            // Other bosses or additional Mod.Call can be made here.
        }
    }

    public class CursedFlameNode : ModNPC
    {
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
            NPC.defense = 16;
            NPC.lifeMax = 16000;
            NPC.HitSound = new SoundStyle("DestroyerTest/Assets/Audio/NodeHit");
            NPC.DeathSound = new SoundStyle("DestroyerTest/Assets/Audio/NodeExplode");
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.timeLeft = 150000;
            //NPC.boss = true;
            NPC.npcSlots = 12f;
            NPC.netID = ModContent.NPCType<CursedFlameNode>();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => new bool?(false);

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("Elemental Constructs that strengthen the potency of Cursed Flames and Ichor."),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface
            });
        }

        public override bool CheckActive()
        {
            return false;
        }



        public override void AI()
        {
            NPC bossNPC = null;
            DTConfig cfg = ModContent.GetInstance<DTConfig>();
            DTMusicConfig muscfg = ModContent.GetInstance<DTMusicConfig>();
            DTOptimizationsConfig optcfg = ModContent.GetInstance<DTOptimizationsConfig>();

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];
                if (n.active && n.type == ModContent.NPCType<NightmareRoseBoss>())
                {
                    bossNPC = n;
                    break;
                }
            }

            if (bossNPC == null)
            {
                NPC.active = false;
                return;
            }
            else
            {
                NPC.active = true;
            }

            // Access ModNPC safely
            NightmareRoseBoss modBoss = bossNPC.ModNPC as NightmareRoseBoss;

            // If NPCHead is a custom property
            Vector2 OrbitCenter = modBoss != null ? modBoss.NPCHead : bossNPC.Center;

            if (Main.rand.NextBool(3) && optcfg.DisableExcessDusts == false)
            {
                int dustCount = 12;
                Vector2 start = NPC.Center;
                Vector2 end = OrbitCenter;
                Vector2 direction = (end - start).SafeNormalize(Vector2.Zero);
                float length = Vector2.Distance(start, end);

                for (int i = 0; i < dustCount; i++)
                {
                    float t = i / (float)(dustCount - 1);
                    Vector2 pos = Vector2.Lerp(start, end, t);

                    Dust d = Dust.NewDustPerfect(pos, DustID.CursedTorch, direction, 0, default, 1.2f);
                    d.noGravity = true;
                    d.fadeIn = 1f;
                }
            }

            bool ParentAlive = Main.npc.Any(n => n.active && n.type == ModContent.NPCType<NightmareRoseBoss>());

            if (ParentAlive)
            {
                NPC.active = true;
            }
            else
            {
                NPC.active = false;
            }

            // Orbit settings
            float radius = 600f;
            float speed = 0.05f;
            float angle = Main.GameUpdateCount * speed;

            // Get a list of all active CursedFlameNode NPCs
            List<NPC> allNodes = new List<NPC>();

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC node = Main.npc[i];
                if (node.active && node.type == ModContent.NPCType<CursedFlameNode>())
                {
                    allNodes.Add(node);
                }
            }

            // Sort the list by whoAmI to ensure consistent order across clients and frames
            allNodes.Sort((a, b) => a.whoAmI.CompareTo(b.whoAmI));

            int index = allNodes.IndexOf(NPC);
            int total = allNodes.Count;

            // Calculate spacing
            float spacing = MathHelper.TwoPi / (total == 0 ? 1 : total);
            float myAngle = angle + index * spacing;

            // Final orbit
            Vector2 offset = new Vector2(MathF.Cos(myAngle), MathF.Sin(myAngle)) * radius;
            NPC.Center = OrbitCenter + offset - new Vector2(NPC.width / 2, NPC.height / 2);

        }
    }

    public class NightmareeRoseBackgroundProj : ModProjectile
    {
        public override string Texture => "DestroyerTest/Content/Extras/FadeLine";
        private Asset<Texture2D> WindTex;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 248000;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            WindTex = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/EvilBossWind", AssetRequestMode.AsyncLoad);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override void AI()
        {
            bool ParentAlive = Main.npc.Any(n => n.active && (n.type == ModContent.NPCType<NightmareRoseBoss>() || n.type == ModContent.NPCType<WyvernCorpseHead>()));
            if (ParentAlive)
            {
                Projectile.active = true;
            }
            else
            {
                Projectile.active = false;
            }
            Projectile.Center = Main.LocalPlayer.Center;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();
            DTOptimizationsConfig optcfg = ModContent.GetInstance<DTOptimizationsConfig>();

            float t = (float)Math.Sin(Main.GameUpdateCount / 60f) * 0.5f + 0.5f;
            Color drawColor = Color.Lerp(Color.Black, ColorLib.TenebrisGradient * 0.5f, t);

            if (!optcfg.OptimizeGame)
            {
                Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

                float time = (float)Main.GameUpdateCount / 60f;

                // --- Layer 1 scroll parameters ---
                float scrollSpeedX1 = 600f;
                float scrollSpeedY1 = 30f;

                float scrollOffsetX1 = (time * scrollSpeedX1) % WindTex.Value.Width;
                float scrollOffsetY1 = (time * scrollSpeedY1) % WindTex.Value.Height;

                int screenW = Main.screenWidth;
                int screenH = Main.screenHeight;

                // --- draw one tile beyond each edge ---
                float startX = -WindTex.Value.Width;
                float startY = -WindTex.Value.Height;
                float endX = screenW + WindTex.Value.Width;
                float endY = screenH + WindTex.Value.Height;

                // --- Draw first layer ---
                for (float x = -scrollOffsetX1 + startX; x < endX; x += WindTex.Value.Width)
                {
                    for (float y = -scrollOffsetY1 + startY; y < endY; y += WindTex.Value.Height)
                    {
                        spriteBatch.Draw(WindTex.Value, new Vector2(x, y), null, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                }

                float scrollSpeedX2 = 250f;
                float scrollSpeedY2 = -60f; // opposite direction for contrast

                float scrollOffsetX2 = (time * scrollSpeedX2) % WindTex.Value.Width;
                float scrollOffsetY2 = (time * scrollSpeedY2) % WindTex.Value.Height;

                Color drawColor2 = drawColor * 0.8f; // slightly dimmer to layer properly

                // --- Draw second layer ---
                for (float x = -scrollOffsetX2 + startX; x < endX; x += WindTex.Value.Width)
                {
                    for (float y = -scrollOffsetY2 + startY; y < endY; y += WindTex.Value.Height)
                    {
                        spriteBatch.Draw(WindTex.Value, new Vector2(x, y), null, drawColor2, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                }

                Opus.ReturnToDefaultDrawing(spriteBatch);
            }
            return false;
        }



    }

}
