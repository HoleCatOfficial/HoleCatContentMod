using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.BossBar;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Boss;
using DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.CursedFlame;
using DestroyerTest.Content.Projectiles.Boss.VampireBoss;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.RogueItems;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using GlowmaskHelper.Content;
using InnoVault.PRT;
using log4net.Repository.Hierarchy;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using OpusLib;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
using rail;
using ReLogic.Content;
using ReLogic.Localization.IME;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Skies;
using Terraria.Graphics;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.UI;

namespace DestroyerTest.Content.Entities
{
    [AutoloadBossHead]
    public class NightmareRoseBoss : ModNPC
    {
        public override string BossHeadTexture => "DestroyerTest/Content/Entities/NightmareRoseBoss_Head_Boss";


        public void immunities()
        {
            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<ShimmeringFlames>()] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Bleeding] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Dazed] = true;
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

        public SoundStyle SpawnIdle = new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/RoseSpawnIdle") with { MaxInstances = 0 };
        public SoundStyle SpawnRoar = new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/RoseSpawnRoar") with { MaxInstances = 0 };
        public SoundStyle Kill = new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/NightmareRoseKill") with { Volume = 2, MaxInstances = 0 };
        public SoundStyle Fire = new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/CursedFlameShoot") with { Volume = 2, PitchVariance = 1f, MaxInstances = 0 };
        public SoundStyle ArenaDivide = new SoundStyle("DestroyerTest/Assets/Audio/Impacts/HellWeaponImpact") with { Volume = 2, PitchVariance = 1f, MaxInstances = 0 };
        public SoundStyle DespShootMine = new SoundStyle("DestroyerTest/Assets/Audio/Impacts/MetalImpactV1_", 3) with { Volume = 2, PitchVariance = 1f, MaxInstances = 0 };
        public SoundStyle NodeSpawnSound = new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/NodeSpawn") with { PitchVariance = 1f, MaxInstances = 0 };
        public SoundStyle Napalm = new SoundStyle("DestroyerTest/Assets/Audio/NodeAttackNapalm") with { PitchVariance = 1f, MaxInstances = 0 };
        public SoundStyle Desperation = new SoundStyle("DestroyerTest/Assets/Audio/RoseDesperation") with { MaxInstances = 0 };

        public SoundStyle WingDisable = new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/WingDisable") with { MaxInstances = 0 };
        public override void SetDefaults()
        {
            NPC.width = 144;
            NPC.height = 274;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 25;
            NPC.lifeMax = 342000;
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
            GeneralEternityChanges(DestroyerTestMod.EternityIsActive);

            if (DestroyerTestMod.MasochistIsActive)
            {
                SpawnRoar = new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/MasoSpawn") with { MaxInstances = 0 };
            }
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

        public static bool SecretSeed()
        {
            if (Main.zenithWorld || Main.getGoodWorld || Main.notTheBeesWorld)
            {
                return true;
            }
            else
            {
                return false;
            }
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
        public float BorderRad = 1200f;
        public bool BorderActive = false;
        public int IdleTimer;
        public int FlameTimer = 0;
        public int FlameInterval = 0;
        public int FlameStartTimer = 120;
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
        public int NapalmDelay = 120;

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

        void ShineHead()
        {
            SmallShine shine = new SmallShine();
            shine.Prepare(NPCHead, Vector2.Zero, Color.White, 2f);
            ParticleEngine.ShaderParticles.Add(shine);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            Texture2D Tex()
            {
                if (DestroyerTestMod.MasochistIsActive)
                {
                    return NPC.GetMasoTexture("DestroyerTest/Content/Entities/MasoMode", "NightmareRoseBoss").Value;
                }
                else
                {
                    return ModContent.Request<Texture2D>(Texture).Value;
                }
            }

            Texture2D GlowTex()
            {
                if (DestroyerTestMod.MasochistIsActive)
                {
                    return NPC.GetMasoGlowTexture("DestroyerTest/Content/Entities/MasoMode", "NightmareRoseBoss").Value;
                }
                else
                {
                    return ModContent.Request<Texture2D>(Texture + "_Glow").Value;
                }
            }


            Rectangle sourceRect = new Rectangle(
                0,
                frameIndex * NPC.height,
                NPC.width,
                NPC.height
            );

            if (anyNodesAlive)
            {
                Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

                if (!DestroyerTestMod.MasochistIsActive)
                {
                    Main.EntitySpriteDraw(DTAssetLib.CorruptSigil.Value, NPC.Center - Main.screenPosition, null, ColorLib.CursedFlames, 0f, DTAssetLib.CorruptSigil.Value.Size() / 2, Opus.Sine(1.7f, 2f), SpriteEffects.None, 0f);
                }
                else
                {
                    Main.EntitySpriteDraw(DTAssetLib.CorruptSigil.Value, NPC.Center - Main.screenPosition, null, ColorLib.TenebrisGradient, 0f, DTAssetLib.CorruptSigil.Value.Size() / 2, Opus.Sine(1.7f, 2f), SpriteEffects.None, 0f);
                }
                //Opus.DrawNPCShadowsRotating(NPC, NPC.frame, 6, ColorLib.CursedFlames, 0.2f);
                Opus.ReturnToDefaultDrawing(spriteBatch);
            }

            if (SecretSeed())
            {
                Main.EntitySpriteDraw(TextureAssets.Npc[NPC.type].Value, NPC.Center - Main.screenPosition, sourceRect, Main.DiscoColor, 180, sourceRect.Size() / 2, 1f, SpriteEffects.None, 0);
            }
            else
            {
                Main.EntitySpriteDraw(Tex(), NPC.Center - Main.screenPosition, sourceRect, drawColor, 0, sourceRect.Size() / 2, 1f, SpriteEffects.None, 0);

                Main.EntitySpriteDraw(GlowTex(), NPC.Center - Main.screenPosition, sourceRect, Color.White, 0, sourceRect.Size() / 2, 1f, SpriteEffects.None, 0);
            }

            return false;
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
            ProjectileID.SolarWhipSword,
            ProjectileID.LunarFlare,
            ProjectileID.MoonlordArrow,
            ProjectileID.MoonlordArrowTrail,
            ProjectileID.VortexBeaterRocket,
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
            /*
            ApplyAdaptiveReduction(ref modifiers);
            */
            if (currentState == AttackState.Desperation || anyNodesAlive)
            {
                NPC.immortal = true;
                modifiers.FinalDamage *= 0f;
            }
            if (ImmuneProjectiles.Contains(projectile.type) && Main.masterMode)
            {
                modifiers.FinalDamage *= 0.75f;
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
            /*
            ApplyAdaptiveReduction(ref modifiers);
            */
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
            if (Main.cloudAlpha < 0.6f)
            {
                Main.cloudAlpha += 0.003f;
            }

            // Restart rain with new settings
            Main.StopRain();
            Main.StartRain();
        }

        int TintCounter = 0;
        int MaxTintCount = 240;
        public void ModifyClouds()
        {
            Player player = Main.player[NPC.target];
            Main main = ModContent.GetInstance<Main>();
            //Main.cloudAlpha = 0.2f;

            if (TintCounter < MaxTintCount)
            {
                TintCounter++;

            }

            if (player.statLife <= 0)
            {
                SunlightModification.Reset();
            }
            //SunlightModification.Sunlight(1f, Color.Black, (float)TintCounter / (float)MaxTintCount);
        }

        public void ModifyMusic()
        {
            int tribID = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Tribulation");
            int eternID = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Placeholder4");
            int masoID = MusicLoader.GetMusicSlot(Mod, "Assets/Music/MasoEvils");
            int secretSeedID = MusicLoader.GetMusicSlot(Mod, "Assets/Music/EvilBossSecretSeed");
            int idleID = MusicLoader.GetMusicSlot(Mod, "Assets/Music/RoseIdle");

            void EnableMusic()
            {
                if (!SetVolume)
                {
                    Main.musicVolume = VolumeOnSpawn;
                    SetVolume = true;
                }
            }

            void StartPlay(IAudioTrack track)
            {
                if (Main.audioSystem is LegacyAudioSystem audioSystem)
                {
                    if (audioSystem.AudioTracks[idleID].IsPlaying)
                    {
                        audioSystem.AudioTracks[idleID].Stop(AudioStopOptions.Immediate);
                    }
                }

                if (!track.IsPlaying)
                {
                    
                    track.Stop(AudioStopOptions.Immediate);
                    track.Reuse();
                    track.Play();
                }
            }

            if (Main.dedServ)
                return;

            if (currentState == AttackState.SpawnIdle)
            {
                if (!RecordedVolume)
                {
                    VolumeOnSpawn = Main.musicVolume;
                    RecordedVolume = true;
                }

                Main.musicVolume -= 0.1f;
                Music = idleID;
                return;
            }

            if (Main.audioSystem is LegacyAudioSystem audioSystem)
            {
                var tribulation = audioSystem.AudioTracks[tribID];
                var eternity = audioSystem.AudioTracks[eternID];
                var masochist = audioSystem.AudioTracks[masoID];
                var secretSeed = audioSystem.AudioTracks[secretSeedID];

                IAudioTrack selectedTrack = tribulation;
                int selectedID = tribID;

                // Highest priority first
                if (Main.getGoodWorld)
                {
                    selectedTrack = secretSeed;
                    selectedID = secretSeedID;
                }
                else if (DestroyerTestMod.MasochistIsActive &&
                         DTMusicConfig.instance.EternityMusic)
                {
                    selectedTrack = masochist;
                    selectedID = masoID;
                }
                else if (DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive && DTMusicConfig.instance.EternityMusic)
                {
                    selectedTrack = eternity;
                    selectedID = eternID;
                }

                EnableMusic();

                StartPlay(selectedTrack);

                Main.musicFade[selectedID] = 1f;
            }
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
                NPC.defense = 40;
                //NPC.takenDamageMultiplier = 0.85f;
                NPC.lifeMax = 600000;
                NPC.life = NPC.lifeMax;
                HealAmount = 35;
            }
            else
            {
                NPC.defense = 25;
                NPC.lifeMax = 342000;
                NPC.life = NPC.lifeMax;
                HealAmount = 15;
            }
        }

        public int RoarWaveTimer = 0;
        public int HealAmount = 0;
        public int DeathInterval = 10;
        public int BorderDustType;
        public static bool ShouldCenterCameraOnNPC = false;
        public float VolumeOnSpawn = 0f;
        public bool RecordedVolume = false;
        public bool SetVolume = false;
        public bool Flag2 = false;

        public bool FireLR = false; //True = Right, False = Left
        public bool DartsLR = false; //True = Right, False = Left
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
                if (!DestroyerTestMod.MasochistIsActive)
                {
                    BorderCol = ColorLib.CursedFlames;
                    BorderDustType = DustID.CursedTorch;
                }
                else
                {
                    BorderCol = ColorLib.TenebrisGradient;
                    BorderDustType = DustID.TintableDustLighted;
                }
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

                if (DestroyerTestMod.EternityIsActive || DestroyerTestMod.MasochistIsActive && currentState != AttackState.SpawnIdle)
                {
                    ModifyClouds();
                }

                if (Main.masterMode && currentState != AttackState.SpawnIdle)
                {
                    //ModifyWeather();
                    Main.cloudAlpha = 0.6f;
                }
            }

            FlameRingAngleStep = MathHelper.TwoPi / FlameRingVectorCount;

            if (player.Distance(NPC.Center) >= BorderRad && BorderActive && BorderRad > 150)
            {
                player.Hurt(new PlayerDeathReason() { CustomReason = NetworkText.FromKey("Mods.DestroyerTest.NPCs.NightmareRose.ExitBarrierDeath", player.name) }, 90, 0, false, true, -1, false, 9, 9, 0);
            }
            if (player.Distance(NPC.Center) < BorderRad && BorderActive)
            {
                // Infinite wing time
                player.wingTime = player.wingTimeMax;

                // Reduce wing speed to half
                //player.moveSpeed *= 0.5f; // baseWingSpeed should be stored somewhere
                //player.GetModPlayer<ApplyArenaEffectsPlayer>().CurrentArenaBoss = ModContent.NPCType<NightmareRoseBoss>();
                //if (Main.masterMode)
                //{
                //player.AddBuff(ModContent.BuffType<ArenaEffects>(), 20);
                //}
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
                if (NPC.life < (NPC.lifeMax * 0.75f))
                {
                    NPC.life += HealAmount;
                }
                if (Main.rand.NextBool(26))
                {
                    RegenHeart Heart = new RegenHeart();
                    Heart.Initialize(Main.rand.NextVector2FromRectangle(NPC.Hitbox), new Vector2(Main.rand.NextFloat(-2, 2), -5), ColorLib.CursedFlames, 1.5f);
                    ParticleEngine.BehindProjectiles.Add(Heart);

                }
            }
            else
            {
                NPC.immortal = false;
                NPC.dontTakeDamage = false;
            }


            if (NPC.life <= NPC.lifeMax * 0.25f && !HasTriggeredNodes)
            {
                currentState = AttackState.Nodes;

            }

            if (player.active == false || player.dead == true)
            {
                if (NPC.Opacity > 0)
                {
                    NPC.Opacity -= 0.1f;
                }
                else
                {
                    NPC.active = false;
                }
            }

            Rotation--;

            PlayerCenter = player.Center;

            IdleFX();

            ModifyMusic();

            NPC.velocity = Vector2.Zero;

            int MinionSpawnType = Main.rand.Next(new int[]
                {
                    ModContent.NPCType<DarkGluttonHead>(),
                    ModContent.NPCType<DarkPredatorHead>(),
                    ModContent.NPCType<DarkArchmage>(),
                    ModContent.NPCType<TenebrousPhantasm>()
                });


            if (NodesAreIn)
            {
                NodeRadius = Opus.Sine(600f, 660f);
            }

            if (DTConfig.instance.EnableDebugMessages && Main.GameUpdateCount % 60 == 0)
            {
                Mod.Logger.Info($"Current State: {currentState}");
            }

            switch (currentState)
            {
                case AttackState.SpawnIdle:
                    {
                        NPC.Opacity = 0f;
                        NPC.dontTakeDamage = true;
                        ShouldCenterCameraOnNPC = true;
                        player.channel = false;
                        player.moveSpeed *= 0;
                        if (SpawnCount <= 0)
                        {
                            SoundEngine.PlaySound(SpawnIdle, NPCHead);
                        }
                        SpawnCount++;
                        if (SpawnCount < SpawnIdleRoarFlag)
                        {
                            VingetteScale *= 0.99f;
                        }

                        if (SpawnCount == (60 * 8) - 40)
                        {
                            if (DestroyerTestMod.MasochistIsActive)
                            {
                                SoundEngine.PlaySound(SpawnRoar, NPCHead);
                            }
                        }
                        if (SpawnCount >= SpawnIdleRoarFlag)
                        {
                            ScreenshakePlayer screenshake = ModContent.GetInstance<ScreenshakePlayer>();
                            screenshake.screenshakeMagnitude = 8;
                            screenshake.screenshakeTimer = 180;
                            RoarWaveTimer = 180;

                            FablesTitleCardSystem.RegisterFablesBossIntro(FablesTitleCardSystem.NightmareRoseTitle.Name, FablesTitleCardSystem.NightmareRoseTitle.Title, 180, true, ColorLib.WretchedGradient(), Color.White, Color.Red, Color.Red, FablesTitleCardSystem.NightmareRoseTitle.MusicTitle, FablesTitleCardSystem.NightmareRoseTitle.MusicArtist);
                            if (!DestroyerTestMod.MasochistIsActive)
                            {
                                SoundEngine.PlaySound(SpawnRoar, NPCHead);
                            }
                            SpawnDarknessAlpha = 0;
                            if (Main.masterMode)
                            {
                                Main.NewText("A torrent befalls the corruption...", ColorLib.CursedFlames);
                                //Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<NightmareeRoseBackgroundProj>(), 0, 0, ai0: 0);
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
                        if (NPC.Opacity < 1)
                        {
                            NPC.Opacity += 0.05f;
                        }

                        if (RoarWaveTimer > 0)
                        {
                            RoarWaveTimer--;
                            if (RoarWaveTimer % 20 == 0)
                            {
                                SoundwaveParticle soundwave = new();
                                soundwave.Initialize(NPCHead, Vector2.Zero, Color.White, 2f);
                                ParticleEngine.ShaderParticles.Add(soundwave);
                            }
                        }
                        else
                        {
                            ShouldCenterCameraOnNPC = false;
                        }
                        NPC.aiStyle = -1;
                        ShouldCenterCameraOnNPC = false;
                        if (VingetteScale < 10)
                        {
                            VingetteScale += 0.75f;
                        }

                        int IdleMax = -1;
                        if (!Main.expertMode && !Main.masterMode && !DestroyerTestMod.EternityIsActive)
                        {
                            IdleMax = 80;
                        }
                        if (Main.expertMode && !Main.masterMode && !DestroyerTestMod.EternityIsActive)
                        {
                            IdleMax = 70;
                        }
                        if (Main.masterMode || DestroyerTestMod.EternityIsActive)
                        {
                            IdleMax = 60;
                        }
                        if (SecretSeed())
                        {
                            IdleMax = 15;
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
                        NodeSpawn();
                        ShouldCenterCameraOnNPC = true;
                        if (NodesAreIn)
                        {
                            currentState = GetRandomState();
                            ShouldCenterCameraOnNPC = false;
                        }
                    }
                    break;
                case AttackState.CursedFlames:
                    {
                        if (!DestroyerTestMod.EternityIsActive)
                        {
                            if (FlameStartTimer >= 120)
                            {
                                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/CursedFlamesWarn2") with { Volume = 0.75f, PitchVariance = 0.4f });
                                ShineHead();
                            }
                            FlameStartTimer--;
                            if (FlameStartTimer > 0)
                            {

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
                        if (DestroyerTestMod.EternityIsActive)
                        {

                            if (FlameStartTimer >= 120)
                            {
                                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/CursedFlamesWarn2") with { Volume = 0.75f, PitchVariance = 0.4f });
                                ShineHead();
                                if (!SetDir1)
                                {
                                    FireLR = Main.rand.NextBool(2);
                                    SetDir1 = true;
                                }
                            }
                            FlameStartTimer--;
                            ContemptAttackRotationOffset += FireLR ? 0.01f : -0.01f;
                            ContemptAttackWarningOffset += FireLR ? 0.005f : -0.005f;
                            if (FlameStartTimer > 0)
                            {

                            }
                            if (FlameTimer < 240 && FlameStartTimer <= 0)
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
                        stateWeights[AttackState.DemoniteWhisper] = 0.1f;
                        stateWeights[AttackState.Lances] = 0.5f;
                        if (Divided)
                        {
                            ResetState();
                        }
                        if (DestroyerTestMod.EternityIsActive && !Divided)
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
                        stateWeights[AttackState.DemoniteWhisper] = 0f;
                        stateWeights[AttackState.CursedFlames] = 0.5f;

                        if (DestroyerTestMod.EternityIsActive)
                        {
                            if (FlameRingCount < 9 && Main.GameUpdateCount % 60 == 0)
                            {
                                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ChargeBreak") with { PitchVariance = 1f });

                                Opus.RingSpreadProjectile(ModContent.ProjectileType<TenebrisFlamesHostile>(), 5, player.Center, 300, 30, 2, 8);
                                FlameRingCount++;
                            }
                            if (FlameRingCount >= 9)
                            {
                                ResetState();
                            }
                        }
                        else
                        {


                            if (FlameRingCount < 9 && Main.GameUpdateCount % 60 == 0)
                            {
                                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/NodeAttackTS") with { PitchVariance = 1f, Volume = 3f });
                                float off = Main.rand.NextFloat(MathHelper.TwoPi);
                                Opus.RadialSpreadProjectile(ModContent.ProjectileType<NightmareRoseCursedCrystal>(), 9, NPC.Center, 16, 4, 12, ai1: 1, offset: off);
                                Opus.RadialSpreadProjectile(ModContent.ProjectileType<NightmareRoseCursedCrystal>(), 9, NPC.Center, 16, 4, 12, ai1: -1, offset: off);
                                FlameRingCount++;
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
                        stateWeights[AttackState.DemoniteWhisper] = 1f;
                        stateWeights[AttackState.CursedFlames] = 1f;
                        if (DestroyerTestMod.EternityIsActive)
                        {
                            int numProjectiles = 7;
                            float rotationStep = MathHelper.TwoPi / numProjectiles;

                            SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                            for (int i = 0; i < numProjectiles; i++)
                            {
                                Vector2 velocity = new Vector2(20f, 0f).RotatedBy(rotationStep * i);
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
                            Opus.RingSpreadProjectile(ModContent.ProjectileType<TormentedSoul2>(), 12, NPCHead, 30, 20, 1, 12);
                            Opus.RingSpreadProjectile(ModContent.ProjectileType<TormentedSoul2>(), 8, NPCHead, 15, 20, 1, 9, offset: 360f / 8f);

                            ResetState();
                        }
                        break;
                    }
                case AttackState.Napalm:
                    {
                        stateWeights[AttackState.DemoniteWhisper] = 1f;
                        stateWeights[AttackState.CursedFlames] = 1f;

                        VileThornCooldown++;
                        if (!DestroyerTestMod.MasochistIsActive)
                        {
                            player.wingTime = 0;
                            if (NapalmDelay >= 120)
                            {
                                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/NapalmWarn") with { Volume = 0.75f, PitchVariance = 0.4f });
                                SoundEngine.PlaySound(WingDisable);


                                ShineHead();

                                WingDisableParticle particle = new WingDisableParticle();
                                particle.Initialize(player.Center, Vector2.Zero, Color.White, 3f);
                                ParticleEngine.ShaderParticles.Add(particle);


                                player.velocity.Y += 100;
                            }
                            NapalmDelay--;
                            if (NapalmDelay > 0)
                            {

                            }
                            if (Main.GameUpdateCount % 30 == 0 && NapalmDelay <= 0)
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
                                            20,
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
                        if (DestroyerTestMod.MasochistIsActive)
                        {
                            if (Main.GameUpdateCount % 120 == 0)
                            {
                                Opus.RadialSpreadProjectileRandom(ModContent.ProjectileType<DarkOrb>(), 3, NPCHead, 30, 3, 8);
                                VileThornCount += 1;
                                //Main.NewText(VileThornCount.ToString(), Color.Blue);
                            }
                            if (VileThornCount >= 8)
                            {
                                ResetState();
                                currentState = GetRandomState();
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
                            bool eternity = DestroyerTestMod.EternityIsActive;
                            //Main.NewText("EternityIsActive returned: " + eternity);

                            if (!eternity)
                            {
                                Minion = NPC.NewNPCDirect(Entity.GetSource_FromThis(), NPC.Center, ModContent.NPCType<GigaCursedHammer>());
                            }
                            else if (eternity)
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
                            if (DestroyerTestMod.EternityIsActive)
                            {
                                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                                for (int f = 0; f < 2; f++)
                                {
                                    float rot = angle + MathHelper.Pi * f;
                                    Vector2 dir = rot.ToRotationVector2();
                                    Projectile.NewProjectile(Entity.GetSource_FromAI(), NPCHead, dir * 6f, ModContent.ProjectileType<BigSoul>(), 30, 7);
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
                        if (DestroyerTestMod.EternityIsActive)
                        {
                            if (Main.GameUpdateCount % 60 == 0)
                            {
                                VortexFire(player);
                            }

                            if (VortexFireCount >= 10)
                            {
                                ResetState();
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
                        BlossomMines(Main.rand.NextVector2FromRectangle(new Rectangle(0, 0, (int)BorderRad, (int)BorderRad)));
                        ResetState();
                    }
                    break;
                case AttackState.Desperation:
                    {
                        NPC.dontTakeDamage = true;
                        ShouldCenterCameraOnNPC = true;

                        LaserRotOffset += 0.03f;
                        if (LaserWarnOpacity > 0)
                        {
                            LaserWarnOpacity -= 0.05f;
                        }
                        if (cfg.EnableDebugMessages)
                        {
                            Mod.Logger.Info($"Desperation Timer: {DesperationTimer}");
                        }
                        if (DesperationTimer < 1200)
                        {

                            DesperationTimer++;
                            if (Main.GameUpdateCount % 110 == 0 && DesperationTimer < 900)
                            {
                                SoundEngine.PlaySound(DespShootMine);
                                SoulBombSpawn();
                            }

                            float progress = (float)DesperationTimer / 1200f;
                            BorderRad = MathHelper.Lerp(1200f, 0f, progress);
                            RingScale = MathHelper.Lerp(6.2f, 0, progress);

                        }
                        if (DesperationTimer >= 1200)
                        {
                            NPC.netUpdate = true;
                            currentState = AttackState.KillIdle;
                            BorderActive = false;
                            //Main.NewText("Get away from the Rose!!", ColorLib.Soul);
                            LerpingBloomRingSharp Ring = new();
                            Color[] P = new Color[4] { Color.White, ColorLib.Soul, ColorLib.Soul2, ColorLib.Soul3 };
                            Ring.Prepare(NPCHead, Vector2.Zero, P, 0.2f, 0.03f, 2f);
                            ParticleEngine.ShaderParticles.Add(Ring);
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
                            if (!DeathSoundFlag)
                            {
                                SoundEngine.PlaySound(Kill);
                                DeathSoundFlag = true;
                            }
                            GatherParticle();
                            DeathIdleTimer--;

                            float progress = (float)DeathIdleTimer / 120f;
                            OverlayAlpha = MathHelper.Lerp(0f, 1f, progress.Inverse());

                            if (DeathIdleTimer % 20 == 0)
                            {
                                Opus.RingSpreadDustRandom(DustID.FireworksRGB, 20, NPCHead, Main.rand.NextFloat(30f, 400f), 0, Color.White, 2f, 1f);
                            }
                        }
                        if (DeathIdleTimer <= 0)
                        {
                            //Projectile.NewProjectile(Entity.GetSource_FromThis(), NPCHead, Vector2.Zero, ModContent.ProjectileType<SoulExplosion>(), 200, 18);
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

        public void IdleFX()
        {
            if (currentState == AttackState.SpawnIdle || DTOptimizationsConfig.instance.OptimizeGame)
            {
                return;
            }

            if (currentState == AttackState.Desperation)
            {
                for (int i = 0; i < 4; i++)
                {
                    Dust spark = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<SoulDust>(), Main.rand.NextFloat(-0.02f, 0.02f), Main.rand.NextFloat(-3.5f, -2.5f), 40, default, 0.75f);
                    spark.noGravity = true;
                }

                Spark Spark = new Spark();

                Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(NPC.Hitbox), new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-10.5f, -2.5f)), 0f, ColorLib.Soul, 0.5f, false, 40, SparkDrawMode.Additive);
                ParticleEngine.ShaderParticles.Add(Spark);

                Lighting.AddLight(NPC.Center, ColorLib.Soul.ToVector3() * 0.5f);
            }
            else if (currentState == AttackState.KillIdle)
            {
                for (int i = 0; i < 4; i++)
                {
                    Dust spark = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<SoulDust>(), Main.rand.NextFloat(-0.02f, 0.02f), Main.rand.NextFloat(-3.5f, -2.5f), 40, default, 0.75f);
                    spark.noGravity = true;
                }

                Spark Spark = new Spark();

                Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(NPC.Hitbox), new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-10.5f, -2.5f)), 0f, ColorLib.Soul, 0.5f, false, 40, SparkDrawMode.Additive);
                ParticleEngine.ShaderParticles.Add(Spark);

                Lighting.AddLight(NPC.Center, ColorLib.Soul.ToVector3() * 0.5f);
            }
            else
            {
                if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    if (Main.rand.NextBool())
                    {
                        Dust fire = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.FireworksRGB, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-13.5f, -2.5f), 40, ColorLib.CursedFlames, 2.5f);
                        fire.noGravity = true;

                        Spark Spark = new Spark();

                        Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(NPC.Hitbox), new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-10.5f, -2.5f)), 0f, ColorLib.CursedFlames, 0.5f, false, 40, SparkDrawMode.Additive);
                        ParticleEngine.ShaderParticles.Add(Spark);
                    }
                    Lighting.AddLight(NPC.Center, ColorLib.CursedFlames.ToVector3() * 0.5f);
                }
                if (DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    if (Main.rand.NextBool())
                    {
                        Dust fire = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.FireworksRGB, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-13.5f, -2.5f), 40, ColorLib.CursedFlames, 2.5f);
                        fire.noGravity = true;

                        Spark Spark = new Spark();

                        Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(NPC.Hitbox), new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-10.5f, -2.5f)), 0f, ColorLib.CursedFlames, 0.5f, false, 40, SparkDrawMode.Additive);
                        ParticleEngine.ShaderParticles.Add(Spark);
                    }
                    Lighting.AddLight(NPC.Center, ColorLib.CursedFlames.ToVector3() * 0.5f);
                }
                if (DestroyerTestMod.MasochistIsActive)
                {
                    if (Main.rand.NextBool())
                    {
                        Dust fire = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.FireworksRGB, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-13.5f, -2.5f), 40, ColorLib.TenebrisGradient, 2.5f);
                        fire.noGravity = true;

                        Spark Spark = new Spark();

                        Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(NPC.Hitbox), new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-10.5f, -2.5f)), 0f, ColorLib.TenebrisGradient, 0.5f, false, 40, SparkDrawMode.Additive);
                        ParticleEngine.ShaderParticles.Add(Spark);
                    }
                    Lighting.AddLight(NPC.Center, ColorLib.TenebrisGradient.ToVector3() * 0.5f);
                }
            }
        }

        float RingScale = 6.2f;
        float Rotation = 0f;
        float OverlayAlpha = 0f;
        public Color BorderCol;
        public float VingetteScale = 2f;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            base.PostDraw(spriteBatch, screenPos, drawColor);



            if (LaserWarnTimer > 0)
            {
                Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
                Main.EntitySpriteDraw(DTAssetLib.BlessedNodeLaserTelegraph.Value, NPCHead - Main.screenPosition, null, ColorLib.TenebrisGradient * LaserWarnOpacity, LaserRotOffset - 12f, DTAssetLib.BlessedNodeLaserTelegraph.Value.Size() / 2, 1f, SpriteEffects.None);
                Main.EntitySpriteDraw(DTAssetLib.BlessedNodeLaserTelegraph.Value, NPCHead - Main.screenPosition, null, Color.White * LaserWarnOpacity, LaserRotOffset - 12f, DTAssetLib.BlessedNodeLaserTelegraph.Value.Size() / 2, 0.65f, SpriteEffects.None);
                Opus.ReturnToDefaultDrawing(spriteBatch);
            }

            if (FlameStartTimer < 120 && FlameStartTimer >= 0 && currentState == AttackState.CursedFlames)
            {
                Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
                if (!DestroyerTestMod.EternityIsActive)
                {
                    GlowConeWarning_CursedFlames();
                }
                if (DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    GlowConeWarning_CursedFlamesEternity();
                }
                Opus.ReturnToDefaultDrawing(spriteBatch);
                if (FlameStartTimer > 60)
                {
                    GlowConeScaling += 0.05f;
                }
                if (FlameStartTimer < 60)
                {
                    GlowConeScaling -= 0.05f;
                }
            }

            if (NapalmDelay < 120 && NapalmDelay >= 0 && currentState == AttackState.Napalm)
            {
                Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
                GlowConeWarning_Napalm();
                Opus.ReturnToDefaultDrawing(spriteBatch);
                if (NapalmDelay > 60)
                {
                    GlowConeScaling += 0.05f;
                }
                if (NapalmDelay < 60)
                {
                    GlowConeScaling -= 0.05f;
                }
            }


            DTUtils Utility = new DTUtils();
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            if (BorderActive)
            {
                Main.EntitySpriteDraw(DTAssetLib.NightmareRoseArenaBorder.Value, NPCHead - Main.screenPosition, null, BorderCol, Rotation, DTAssetLib.NightmareRoseArenaBorder.Value.Size() / 2, RingScale, SpriteEffects.None, 0);

                Main.EntitySpriteDraw(DTAssetLib.Vingette.Value, NPCHead - Main.screenPosition, null, BorderCol, Rotation, DTAssetLib.Vingette.Value.Size() / 2, RingScale, SpriteEffects.None, 0);
            }
            Opus.ReturnToDefaultDrawing(spriteBatch);

            string Maso = DestroyerTestMod.MasochistIsActive ? "_Maso" : "";
            Asset<Texture2D> White = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/NightmareRoseDeathFade" + Maso);
            if (currentState == AttackState.KillIdle)
            {
                Main.EntitySpriteDraw(White.Value, NPC.Center - Main.screenPosition, null, Color.White * OverlayAlpha, 0f, new Vector2(White.Value.Width / 2, White.Value.Height / 2), 1f, SpriteEffects.None, 0);
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
                FlameStartTimer = 120;
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
                NapalmDelay = 120;
                GlowConeScaling = 0.01f;
                VortexFireCount = 0;
                LaserWarnTimer = 120;

                HasBoosted = false;
                HasSpawnedSigil = false;
                HasSpawnedMines = false;
                SetDir1 = false;
                SetDir2 = false;
            }
        }

        public float GlowConeScaling = 0.01f;
        public void GlowConeWarning_CursedFlames()
        {
            Vector2 dir = DirectionToPlayerCenter;

            Main.EntitySpriteDraw(DTAssetLib.GlowCone.Value, NPCHead - Main.screenPosition, null, ColorLib.CursedFlames, dir.ToRotation(), DTAssetLib.GlowCone.Value.Size() / 2, GlowConeScaling, SpriteEffects.None, 0);
        }


        public void GlowConeWarning_CursedFlamesEternity()
        {
            var i = Opus.GetEquidistantOrbitVectorsAndRots(6, NPCHead, ContemptAttackWarningOffset, 40);

            foreach (var p in i)
            {
                Main.EntitySpriteDraw(DTAssetLib.GlowCone.Value, NPCHead - Main.screenPosition, null, ColorLib.CursedFlames, p.Rotation, DTAssetLib.GlowCone.Value.Size() / 2, GlowConeScaling, SpriteEffects.None, 0);
            }
        }

        public void GlowConeWarning_Napalm()
        {
            Main.EntitySpriteDraw(DTAssetLib.GlowCone.Value, NPCHead - Main.screenPosition, null, ColorLib.CursedFlames, -MathHelper.PiOver2, DTAssetLib.GlowCone.Value.Size() / 2, new Vector2(GlowConeScaling * 2, GlowConeScaling), SpriteEffects.None, 0);
        }

        public void GatherParticle()
        {
            for (int y = 0; y < 2; y++)
            {
                Vector2 Spawn = NPCHead + Main.rand.NextVector2CircularEdge(400, 400);
                Vector2 Inward = NPCHead - Spawn;

            }
        }

        public void SoulBombSpawn()
        {
            for (int j = 0; j < 2; j++)
            {
                Vector2 spawnPos = NPCHead;
                Vector2 targetPos = NPCHead + Main.rand.NextVector2CircularEdge(BorderRad, BorderRad);
                Vector2 direction = (targetPos - spawnPos).SafeNormalize(Vector2.Zero); // SafeNormalize prevents division by zero

                Projectile SB = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), spawnPos, direction * 6, ModContent.ProjectileType<SoulCrystalBomb>(), 0, 1);
                SB.timeLeft = 60;
            }
        }

        public float NodeRadius = 2400;
        public bool NodesAreIn = false;
        public void NodeSpawn()
        {
            int projectileCount = 6;

            if (!HasTriggeredNodes)
            {
                LerpingBloomRingSharp Ring = new();
                Ring.Prepare(NPCHead, Vector2.Zero, ColorLib.WretchedColorMap, 0.2f, 0.03f, 2f);
                ParticleEngine.ShaderParticles.Add(Ring);



                Main.NewText("The Nightmare Rose calls upon the Corruption for Help!", ColorLib.CursedFlames);

                SoundEngine.PlaySound(NodeSpawnSound);
                for (int i = 0; i < projectileCount; i++)
                {
                    // Get evenly spaced angle with rotation offset
                    float angle = MathHelper.TwoPi * i / projectileCount;
                    Vector2 spawnOffset = NodeRadius * angle.ToRotationVector2(); // position on the circle
                    Vector2 spawnPosition = NPC.Center + spawnOffset;

                    NPC.NewNPC(Entity.GetSource_FromThis(), (int)spawnPosition.X, (int)spawnPosition.Y, ModContent.NPCType<CursedFlameNode>());

                }
                HasTriggeredNodes = true;
            }

            if (HasTriggeredNodes)
            {
                if (NodeRadius > 600 && !NodesAreIn)
                {
                    NodeRadius -= 4;
                }

                if (NodeRadius <= 600 && !NodesAreIn)
                {
                    NodesAreIn = true;
                }
            }
        }

        public void ManageSigil(Vector2 SpawnPos)
        {
            NPC.NewNPC(Entity.GetSource_FromAI(), (int)SpawnPos.X, (int)SpawnPos.Y, ModContent.NPCType<CorruptSigil>(), 0);
        }

        public void SummonSouls()
        {
            if (NPC.HasValidTarget)
            {
                Player player = Main.player[NPC.target];
                SoundEngine.PlaySound(DTAssetLib.Impacts.Void with { MaxInstances = 0, PitchVariance = 0.5f, Volume = 0.6f });
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/SoulSummon") with { MaxInstances = 0, PitchVariance = 0.2f });
                player.GetModPlayer<ScreenshakePlayer>().screenshakeTimer = 10;
                player.GetModPlayer<ScreenshakePlayer>().screenshakeMagnitude = 8;
                if (!DestroyerTestMod.EternityIsActive)
                {
                    for (int a = 0; a < 10; a++)
                    {
                        Vector2 SpawnPoint = new Vector2(NPC.Center.X + Main.rand.NextFloat(-BorderRad, BorderRad), NPC.Center.Y + 800);
                        Projectile.NewProjectile(Entity.GetSource_FromThis(), SpawnPoint, new Vector2(0, -8), ModContent.ProjectileType<TormentedSoul>(), 25, 2);
                    }
                }
                if (DestroyerTestMod.EternityIsActive)
                {
                    for (int a = 0; a < 5; a++)
                    {
                        Vector2 SpawnPoint = new Vector2(NPC.Center.X + Main.rand.NextFloat(-BorderRad, BorderRad), NPC.Center.Y + 800);
                        Projectile.NewProjectile(Entity.GetSource_FromThis(), SpawnPoint, new Vector2(0, -16), ModContent.ProjectileType<TormentedSoul>(), 25, 2);
                    }
                    for (int a = 0; a < 5; a++)
                    {
                        bool Side = Main.rand.NextBool(2);
                        int S = 0;
                        Vector2 SpawnPoint;
                        Vector2 MoveDir;
                        if (Side)
                        {
                            MoveDir = new Vector2(-16, 0);
                            SpawnPoint = new Vector2(NPC.Center.X + 1000, NPC.Center.Y + Main.rand.NextFloat(-BorderRad, BorderRad));
                            S = 1;
                        }
                        else
                        {
                            MoveDir = new Vector2(16, 0);
                            SpawnPoint = new Vector2(NPC.Center.X - 1000, NPC.Center.Y + Main.rand.NextFloat(-BorderRad, BorderRad));
                            S = 2;
                        }

                        Projectile.NewProjectile(Entity.GetSource_FromThis(), SpawnPoint, MoveDir, ModContent.ProjectileType<TormentedSoul>(), 25, 2, ai2: S);
                    }
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

        public float ContemptAttackRotationOffset = 0f;
        public float ContemptAttackWarningOffset = 0f;
        public bool SetDir1 = false;

        public float LaserWarnOpacity = 0f;
        public float LaserRotOffset = 0;
        public int LaserWarnTimer = 120;
        Projectile[] LaserCol;
        public void ContemptAttack()
        {
            if (!SetDir1)
            {
                FireLR = Main.rand.NextBool(2);
                SetDir1 = true;
            }
            float radius = BorderRad;
            int projectileCount = 6;

            Projectile flame = null;

            if (FireLR)
            {
                LaserRotOffset -= 0.02f;
            }
            else
            {
                LaserRotOffset += 0.02f;
            }

            float rotationOffset = ContemptAttackRotationOffset;

            if (DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
            {

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

                    LerpingBloomRingSharp Ring = new();
                    Ring.Prepare(NPCHead, Vector2.Zero, ColorLib.WretchedColorMap, 0.2f, 0.03f, 2f);
                    ParticleEngine.ShaderParticles.Add(Ring);

                }
                if (flame != null && flame.Center == NPCHead)
                {
                    flame.Kill();
                }
            }
            if (DestroyerTestMod.MasochistIsActive)
            {
                if (LaserWarnTimer == 119)
                {
                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/BlessedNodeLasersCharge"), NPC.Center);
                }
                if (LaserWarnTimer > 0)
                {
                    float t = Utilities.Convert01To010((LaserWarnTimer / 120f));
                    LaserWarnOpacity = MathHelper.Lerp(0f, 1f, t);

                    //Opus.RadialSpreadDust(DustID.AncientLight, 6, NPC.Center, 0, Main.DiscoColor, 1f, 5f, offset: LaserRotOffset);
                    LaserWarnTimer--;

                    
                }
                else
                {


                    SunlightModification.Pulse(1f, ColorLib.TenebrisGradient, 0.8f);
                    
                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/TenebrisLasers"), NPCHead);

                    int Dir = FireLR ? -1 : 1;
                    LaserCol = Opus.RadialSpreadProjectile(ModContent.ProjectileType<TenebrisLaser>(), 6, NPCHead, 60, 1, 0.005f, ai1: Dir, offset: LaserRotOffset);
                    LaserWarnTimer = 120;
                }
            }
        }

        private float dartRotation;
        public bool SetDir2 = false;
        public void DartAttack()
        {
            if (!SetDir2)
            {
                DartsLR = Main.rand.NextBool(2);
                SetDir2 = true;
            }
            float radius = BorderRad;
            int projectileCount = 4;

            dartRotation += DartsLR ? 0.01f : -0.01f;

            float rotationOffset = dartRotation;


            Projectile Dart = null;

            if (Main.GameUpdateCount % 15 == 0)
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
                    Dart.timeLeft = 100;
                }
            }
            if (Dart != null && Dart.Center == NPCHead)
            {
                Dart.Kill();
            }

            if (Main.GameUpdateCount % 240 == 0)
            {
                int numProjectiles = 5;
                float rotationStep = MathHelper.TwoPi / numProjectiles;

                SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                for (int i = 0; i < numProjectiles; i++)
                {
                    Vector2 velocity = new Vector2(20f, 0f).RotatedBy(rotationStep * i);
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

        public bool VortexFireUD = false; //True = Up, False = Down
        public bool SetDir3 = false;
        public int VortexFireCount = 0;
        public void VortexFire(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item122);
            if (!SetDir3)
            {
                VortexFireUD = Main.rand.NextBool(2);
                SetDir3 = false;
            }
            if (DestroyerTestMod.EternityIsActive && !Main.masterMode)
            {
                for (int r = 0; r < 3; r++)
                {
                    Vector2 spawn = VortexFireUD ? NPCHead + new Vector2(Main.rand.NextFloat(-800f, 800f), -1000) : NPCHead + new Vector2(Main.rand.NextFloat(-800f, 800f), 1000);

                    float dirY = VortexFireUD ? 20 : -20;

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawn, new Vector2(0, dirY), ModContent.ProjectileType<CursedFlameVortex>(), 20, 5);
                }
            }
            if (DestroyerTestMod.MasochistIsActive)
            {
                for (int r = 0; r < 3; r++)
                {
                    Vector2 spawn = VortexFireUD ? NPCHead + new Vector2(Main.rand.NextFloat(-800f, 800f), -1000) : NPCHead + new Vector2(Main.rand.NextFloat(-800f, 800f), 1000);

                    float dirY = VortexFireUD ? 20 : -20;

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawn, new Vector2(0, dirY), ModContent.ProjectileType<ShimmeringVortex>(), 20, 5);
                }
            }
            VortexFireCount++;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Item_NightmareRoseTrophy>(), 10));
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

            npcLoot.Add(notExpertRule);

            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<NightmareRoseLootBag>()));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<NightmarePowder>()));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Item_NightmareRoseRelic>()));


            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HaepienNodeCharm>(), 20, 1, 1));
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
            SunlightModification.Reset();
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
            ScreenshakePlayer ScreenShake = Main.LocalPlayer.GetModPlayer<ScreenshakePlayer>();

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

            Vector2 shakeOffset = ScreenShake.GetShakeOffset();

            Main.screenPosition = camPos;
            camPos += shakeOffset;
            Main.screenPosition += shakeOffset;
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

    [AutoloadHead]
    [AutoloadGlowmask]
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
            {
                CustomTexturePath = "DestroyerTest/Content/Entities/NodesBestiary",
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

        public float trailOffset = 0f;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            trailOffset += 0.04f;

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(7).Value, NPC.OldCenter().ToList(), NPC.oldRot.ToList(), 24f, ColorLib.WretchedGradient(), trailOffset, 10);
            return true;
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
            float radius = modBoss.NodeRadius;
            float speed = 0.01f;
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

            // Final orbit target
            Vector2 targetOffset = new Vector2(MathF.Cos(myAngle), MathF.Sin(myAngle)) * radius;
            Vector2 targetCenter = OrbitCenter + targetOffset;

            // Smooth movement instead of instant snapping
            float lerpSpeed = 0.08f; // lower = slower, higher = snappier

            NPC.Center = Vector2.Lerp(
                NPC.Center,
                targetCenter,
                lerpSpeed
            );

        }
    }

    /*
    public class NightmareeRoseBackgroundProj : ModProjectile
    {
        public override string Texture => "DestroyerTest/Content/Extras/FadeLine";
        private Asset<Texture2D> WindTex;
        private Asset<Texture2D> RainTex;
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
            RainTex = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/RainOverlay", AssetRequestMode.AsyncLoad);
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

            if (Projectile.ai[0] == 0)
            {
                if (!optcfg.OptimizeGame)
                {
                    if (!optcfg.OptimizeGame)
                    {
                        //Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone);
                        
                        float time = (float)Main.GameUpdateCount / 60f;

                        int screenW = Main.screenWidth;
                        int screenH = Main.screenHeight;

                        // --- Layer 1 ---
                        float scrollSpeedX1 = 10f;
                        float scrollSpeedY1 = 70f;

                        int texW = RainTex.Value.Width;
                        int texH = RainTex.Value.Height;

                        int offsetX1 = (int)(time * scrollSpeedX1) % texW;
                        int offsetY1 = (int)(time * scrollSpeedY1) % texH;

                        Rectangle source1 = new Rectangle(
                            offsetX1,
                            offsetY1,
                            Math.Min(screenW, texW - offsetX1),
                            Math.Min(screenH, texH - offsetY1)
                        );

                        spriteBatch.Draw(
                            RainTex.Value,
                            Main.screenPosition,
                            source1,
                            Color.White * 0.7f,
                            0f,
                            Vector2.Zero,
                            Projectile.scale,
                            SpriteEffects.None,
                            0f
                        );

                        // --- Layer 2 ---
                        float scrollSpeedX2 = 20f;
                        float scrollSpeedY2 = 140f;

                        int offsetX2 = (int)(time * scrollSpeedX2) % texW;
                        int offsetY2 = (int)(time * scrollSpeedY2) % texH;

                        Rectangle source2 = new Rectangle(
                            offsetX2,
                            offsetY2,
                            Math.Min(screenW, texW - offsetX2),
                            Math.Min(screenH, texH - offsetY2)
                        );

                        spriteBatch.Draw(
                            WindTex.Value,
                            Main.screenPosition,
                            source2,
                            Color.White * 0.3f,
                            0f,
                            Vector2.Zero,
                            Projectile.scale,
                            SpriteEffects.None,
                            0f
                        );

                        Opus.ReturnToDefaultDrawing(spriteBatch);
                    }
                }
            }
            if (Projectile.ai[0] == 1)
            {
                float t = (float)Math.Sin(Main.GameUpdateCount / 60f) * 0.5f + 0.5f;
                Color drawColor = Color.Lerp(Color.Black, ColorLib.TenebrisGradient * 0.5f, t);

                if (!optcfg.OptimizeGame)
                {
                    Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

                    float time = (float)Main.GameUpdateCount / 60f;

                    // --- Layer 1 scroll parameters ---
                    float scrollSpeedX1 = 600f;
                    float scrollSpeedY1 = 30f;

                    float scrollOffsetX1 = (time * scrollSpeedX1) % WindTex.Value.Width * Projectile.scale;
                    float scrollOffsetY1 = (time * scrollSpeedY1) % WindTex.Value.Height * Projectile.scale;

                    int screenW = Main.screenWidth;
                    int screenH = Main.screenHeight;

                    // --- draw one tile beyond each edge ---
                    float startX = -WindTex.Value.Width * Projectile.scale;
                    float startY = -WindTex.Value.Height * Projectile.scale;
                    float endX = screenW + WindTex.Value.Width * Projectile.scale;
                    float endY = screenH + WindTex.Value.Height * Projectile.scale;

                    // --- Draw first layer ---
                    for (float x = -scrollOffsetX1 + startX; x < endX; x += WindTex.Value.Width)
                    {
                        for (float y = -scrollOffsetY1 + startY; y < endY; y += WindTex.Value.Height)
                        {
                            spriteBatch.Draw(WindTex.Value, new Vector2(x, y), null, drawColor, 0f, Vector2.Zero, 1f * Projectile.scale, SpriteEffects.None, 0f);
                        }
                    }

                    float scrollSpeedX2 = 250f;
                    float scrollSpeedY2 = -60f; // opposite direction for contrast

                    float scrollOffsetX2 = (time * scrollSpeedX2) % WindTex.Value.Width * Projectile.scale;
                    float scrollOffsetY2 = (time * scrollSpeedY2) % WindTex.Value.Height * Projectile.scale;

                    Color drawColor2 = drawColor * 0.8f; // slightly dimmer to layer properly

                    // --- Draw second layer ---
                    for (float x = -scrollOffsetX2 + startX; x < endX; x += WindTex.Value.Width)
                    {
                        for (float y = -scrollOffsetY2 + startY; y < endY; y += WindTex.Value.Height)
                        {
                            spriteBatch.Draw(WindTex.Value, new Vector2(x, y), null, drawColor2, 0f, Vector2.Zero, 1f * Projectile.scale, SpriteEffects.None, 0f);
                        }
                    }

                    Opus.ReturnToDefaultDrawing(spriteBatch);
                }
            }
            return false;
        }

        */



    

}
