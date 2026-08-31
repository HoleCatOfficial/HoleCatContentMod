using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
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
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.RogueItems;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.UI;
using GlowmaskHelper.Content;
 
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
using Terraria.WorldBuilding;
using Conditions = Terraria.GameContent.ItemDropRules.Conditions;

namespace DestroyerTest.Content.Entities
{
    [AutoloadBossHead]
    public class NightmareRoseBoss : ModNPC, IDrawPixelated
    {
        public override string BossHeadTexture => "DestroyerTest/Content/Entities/NightmareRoseBoss_Head_Boss";


        public void immunities()
        {
            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<ShimmeringFlames>()] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<Defilement>()] = true;
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

        public SoundStyle IdleCroak = new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/Ambient/AmbientCroak", 5) with { MaxInstances = 0 };

        public override void SetDefaults()
        {
            NPC.width = 144;
            NPC.height = 274;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.lifeMax = 342000;
            NPC.defense = 25;
            if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.DeathIsActive)
            {
                NPC.lifeMax = 342000;
                NPC.defense = 25;
            }
            if ((DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive) && !DestroyerTestMod.MasochistIsActive)
            {
                NPC.lifeMax = 420000;
                NPC.defense = 30;
            }
            if (DestroyerTestMod.MasochistIsActive)
            {
                NPC.lifeMax = 700000;
                NPC.defense = 35;
            }
            if (DTUtils.CalamityBossRushActive())
            {
                NPC.lifeMax = 1000000;
                NPC.defense = 60;
            }

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

            if (DestroyerTestMod.MasochistIsActive)
            {
                SpawnRoar = new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/MasoSpawn") with { MaxInstances = 0 };
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            position = new Vector2(0, 0);
            return false;
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
        private void SetChances(AttackState State)
        {
            var weights = stateWeights;
            bool Eternity = (DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive) && !DestroyerTestMod.MasochistIsActive;
            bool Masochist = DestroyerTestMod.MasochistIsActive;

            switch (State)
            {
                case AttackState.SpawnIdle:
                    {

                        break;
                    }
                case AttackState.Idle:
                    {
                        
                        break;
                    }
                case AttackState.CursedFlames:
                    {
                        weights[AttackState.CursedFlames] = 0.5f;
                        weights[AttackState.Napalm] = 1f;
                        weights[AttackState.Lances] = 1f;
                        break;
                    }
                case AttackState.Napalm:
                    {
                        weights[AttackState.Napalm] = 0f;
                        weights[AttackState.Lances] = 0.0f;
                        if (Masochist)
                        {
                            weights[AttackState.BlossomMine] = 0f;
                            weights[AttackState.CursedFlames] = 0.2f;
                        }
                        break;
                    }
                case AttackState.Minions:
                    {
                        break;
                    }
                case AttackState.RottenPetals:
                    {
                        break;
                    }
                case AttackState.OvergrownHammer:
                    {
                        break;
                    }
                case AttackState.DemoniteWhisper:
                    {
                        weights[AttackState.BlossomMine] = 1f;
                        weights[AttackState.CursedFlames] = 1f;
                        weights[AttackState.WallDarts] = 1f;
                        weights[AttackState.Lances] = 1f;
                        weights[AttackState.Napalm] = 1f;
                        break;
                    }
                case AttackState.CorruptSigil:
                    {
                        break;
                    }
                case AttackState.ArenaDivide:
                    {
                        weights[AttackState.Lances] = 0.5f;
                        weights[AttackState.DemoniteWhisper] = 1f;
                        break;
                    }
                case AttackState.BlossomMine:
                    {
                        weights[AttackState.FlameRing] = 0f;
                        weights[AttackState.ArenaDivide] = 1f;
                        weights[AttackState.CursedFlames] = 0f;

                        if (Masochist)
                        {
                            weights[AttackState.DemoniteWhisper] = 0f;
                            
                            weights[AttackState.WallDarts] = 0f;
                        }
                        break;
                    }
                case AttackState.Desperation:
                    {
                        break;
                    }
                case AttackState.Nodes:
                    {
                        weights[AttackState.FlameRing] = 0f;
                        break;
                    }
                case AttackState.FlameRing:
                    {
                        weights[AttackState.BlossomMine] = 1f;
                        break;
                    }
                case AttackState.Lances:
                    {
                        weights[AttackState.ArenaDivide] = 0.1f;
                        weights[AttackState.Napalm] = 0.0f;
                        break;
                    }
                case AttackState.WallDarts:
                    {
                        weights[AttackState.DemoniteWhisper] = 1f;
                        weights[AttackState.FlameRing] = 1f;
                        weights[AttackState.BlossomMine] = 1f;
                        break;
                    }
                case AttackState.KillIdle:
                    {
                        break;
                    }
            }
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
            BorderRad = reader.ReadSingle();
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

            if (DTConfig.instance.EnableDebugMessages)
            {
                Mod.Logger.Info($"[NightmareRose.ReceiveExtraAI] state:{currentState} BorderRad:{BorderRad} BorderActive:{BorderActive} FlameTimer:{FlameTimer}");
            }
        }

        #endregion

        bool ShouldCheckForTilesOnSpawn = true;
        public override void OnSpawn(IEntitySource source)
        {
            ShouldCheckForTilesOnSpawn = !DTUtils.CalamityBossRushActive() && !DestroyerTestMod.EternityIsActive && !DestroyerTestMod.DeathIsActive;

            if (ShouldCheckForTilesOnSpawn)
            {
                Point resultPoint;
                Point resultPoint2;

                Rectangle SearchArea = Utils.CenteredRectangle(NPC.Bottom.ToTileCoordinates().ToWorldCoordinates() + new Vector2(0, 5 * 16), new Vector2(125 * 16, 8 * 16));
                Rectangle SearchArea2 = Utils.CenteredRectangle(NPC.Bottom.ToTileCoordinates().ToWorldCoordinates() + new Vector2(0, -50 * 16), new Vector2(125 * 16, 100 * 16));
                //Visualize
                Dust.DrawDebugBox(SearchArea);
                Dust.DrawDebugBox(SearchArea2);

                bool TileCheck = WorldUtils.Find(
                    SearchArea.TopLeft().ToTileCoordinates(),
                    Searches.Chain(new Searches.Rectangle(1, 1),
                    new GenCondition[]
                    {
                    new Terraria.WorldBuilding.Conditions.IsSolid().AreaAnd(125, 8),
                    }),
                    out resultPoint);

                bool AirCheck = WorldUtils.Find(
                    SearchArea.TopLeft().ToTileCoordinates(),
                    Searches.Chain(new Searches.Rectangle(1, 1),
                    new GenCondition[]
                    {
                    new Terraria.WorldBuilding.Conditions.IsSolid().AreaAnd(125, 100),
                    }),
                    out resultPoint2);

                if (TileCheck && AirCheck)
                {
                    Main.NewText(Language.GetTextValue("Mods.DestroyerTest.NPCs.NightmareRoseBoss.AirClutterDespawn"), Color.Red);
                }

                if (!TileCheck && AirCheck)
                {
                    Main.NewText(Language.GetTextValue("Mods.DestroyerTest.NPCs.NightmareRoseBoss.GroundHolesDespawn"), Color.Red);
                }

                if (!TileCheck && !AirCheck)
                {
                    Main.NewText(Language.GetTextValue("Mods.DestroyerTest.NPCs.NightmareRoseBoss.GroundHolesAndAirClutterDespawn"), Color.Red);
                }

                if (TileCheck && !AirCheck)
                {

                    NPC.life = NPC.lifeMax;
                    BorderActive = true;
                    currentState = AttackState.SpawnIdle;
                    NPCHead = NPC.Center + new Vector2(0, -79);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<SpawnSoul>(), 0, 0);

                    SunlightModification.Reset();
                }
                else
                {
                    NPC.active = false;
                }
            }
            else
            {
                NPC.life = NPC.lifeMax;
                BorderActive = true;
                currentState = AttackState.SpawnIdle;
                NPCHead = NPC.Center + new Vector2(0, -79);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<SpawnSoul>(), 0, 0);

                SunlightModification.Reset();
            }
        }

        void ShineHead()
        {
            SmallShine shine = new SmallShine();
            shine.Prepare(NPCHead, Vector2.Zero, Color.White, 3.7f);
            ParticleEngine.ShaderParticles.Add(shine);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return true;
            }
            
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
                    Main.EntitySpriteDraw(DTAssetLib.ShadeSigil.Value, NPC.Center - Main.screenPosition, null, ColorLib.TenebrisGradient, 0f, DTAssetLib.ShadeSigil.Value.Size() / 2, Opus.Sine(0.7f, 1f), SpriteEffects.None, 0f);
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

        float RingScale = 1f;
        float Rotation = 0f;
        float OverlayAlpha = 0f;
        public Color BorderCol;
        public float VingetteScale = 2f;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            base.PostDraw(spriteBatch, screenPos, drawColor);

            if (NPC.IsABestiaryIconDummy)
            {
                return;
            }




            DTUtils Utility = new DTUtils();


            AttackState[] states = Enum.GetValues<AttackState>();

            if (DTConfig.instance.EnableDebugMessages)
            {
                for (int i = 0; i < states.Length; i++)
                {
                    float X = NPC.Center.X + 80f;
                    float Y = NPC.Top.Y + (20f * i);

                    AttackState state = states[i];

                    if (stateWeights.TryGetValue(state, out float weight))
                    {
                        Utils.DrawBorderString(
                            spriteBatch,
                            $"{state}: {weight}",
                            new Vector2(X, Y) - screenPos,
                            Color.Red,
                            0.75f,
                            0f,
                            0.5f
                        );
                    }
                }
            }

            string Maso = DestroyerTestMod.MasochistIsActive ? "_Maso" : "";
            Asset<Texture2D> White = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/NightmareRoseDeathFade" + Maso);
            if (currentState == AttackState.KillIdle)
            {
                Main.EntitySpriteDraw(White.Value, NPC.Center - Main.screenPosition, null, Color.White * OverlayAlpha, 0f, new Vector2(White.Value.Width / 2, White.Value.Height / 2), 1f, SpriteEffects.None, 0);
            }

        }

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveNPCs;
        bool IDrawPixelated.ShouldDrawPixelated => true;
        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return;
            }

            var Cap = spriteBatch.Capture();
            spriteBatch.End();

            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            spriteBatch.Begin(Cap);

            if (cfNodes != null)
            {
                if (cfNodes.Count > 0)
                {
                    Main.EntitySpriteDraw(DTAssetLib.Star(3).Value, NPCHead - Main.screenPosition, null, ColorLib.CursedFlames with { A = 0 }, nodeHealShineRot, DTAssetLib.Star(3).Value.Size() / 2, 2.4f, SpriteEffects.None);
                    Main.EntitySpriteDraw(DTAssetLib.Star(3).Value, NPCHead - Main.screenPosition, null, Color.White with { A = 0 }, nodeHealShineRot, DTAssetLib.Star(3).Value.Size() / 2, 1.8f, SpriteEffects.None);

                    for (int i = 0; i < cfNodes.Count; i++)
                    {
                        Line L = new Line(cfNodes[i].Center, NPCHead);

                        Main.EntitySpriteDraw(DTAssetLib.Star(3).Value, cfNodes[i].Center - Main.screenPosition, null, ColorLib.CursedFlames with { A = 0 }, nodeHealShineRot, DTAssetLib.Star(3).Value.Size() / 2, 2f, SpriteEffects.None);
                        Main.EntitySpriteDraw(DTAssetLib.Star(3).Value, cfNodes[i].Center - Main.screenPosition, null, Color.White with { A = 0 }, nodeHealShineRot, DTAssetLib.Star(3).Value.Size() / 2, 1.3f, SpriteEffects.None);

                        DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(1, true), ColorLib.WretchedGradient() with { A = 0 }, Main.spriteBatch, BlendState.Additive, NodeHealLineScroll, 0.3f, 1f);
                        var Cap2 = spriteBatch.Capture();
                        spriteBatch.End();

                        Cap2.TransformMatrix = PixelationSystem.PixelationMatrix;

                        spriteBatch.Begin(Cap2);

                    }
                }
            }

            if (ShouldDrawLaserWarning)
            {
                spriteBatch.UseBlendState(BlendState.Additive);
                Main.EntitySpriteDraw(DTAssetLib.BlessedNodeLaserTelegraph.Value, NPCHead - Main.screenPosition, null, ColorLib.TenebrisGradient * LaserWarnOpacity, LaserRotOffset - 12f, DTAssetLib.BlessedNodeLaserTelegraph.Value.Size() / 2, 1f, SpriteEffects.None);
                Main.EntitySpriteDraw(DTAssetLib.BlessedNodeLaserTelegraph.Value, NPCHead - Main.screenPosition, null, Color.White * LaserWarnOpacity, LaserRotOffset - 12f, DTAssetLib.BlessedNodeLaserTelegraph.Value.Size() / 2, 0.65f, SpriteEffects.None);
                spriteBatch.UseBlendState(BlendState.AlphaBlend);
            }

            if (FlameStartTimer < 120 && FlameStartTimer >= 0 && currentState == AttackState.CursedFlames)
            {
                spriteBatch.UseBlendState(BlendState.Additive);
                if (!DestroyerTestMod.EternityIsActive)
                {
                    GlowConeWarning_CursedFlames();
                }
                if ((DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive) && !DestroyerTestMod.MasochistIsActive)
                {
                    GlowConeWarning_CursedFlamesEternity();
                }
                spriteBatch.UseBlendState(BlendState.AlphaBlend);
                if (FlameStartTimer > 60)
                {
                    GlowConeScaling += 0.25f;
                }
                if (FlameStartTimer < 60)
                {
                    GlowConeScaling -= 0.25f;
                }
            }

            if (NapalmDelay < 120 && NapalmDelay >= 0 && currentState == AttackState.Napalm)
            {
                spriteBatch.UseBlendState(BlendState.Additive);
                GlowConeWarning_Napalm();
                spriteBatch.UseBlendState(BlendState.AlphaBlend);
                if (NapalmDelay > 60)
                {
                    GlowConeScaling += 0.25f;
                }
                if (NapalmDelay < 60)
                {
                    GlowConeScaling -= 0.25f;
                }
            }


            if (BorderActive)
            {
                Main.EntitySpriteDraw(DTAssetLib.NightmareRoseArenaBorder.Value, NPCHead - Main.screenPosition, null, BorderCol with { A = 0 }, Rotation, DTAssetLib.NightmareRoseArenaBorder.Value.Size() / 2, RingScale, SpriteEffects.FlipHorizontally, 0);
                Main.EntitySpriteDraw(DTAssetLib.NightmareRoseArenaBorder.Value, NPCHead - Main.screenPosition, null, OpusColorUtils.Pastel(BorderCol, 0.75f) with { A = 0 }, Rotation, DTAssetLib.NightmareRoseArenaBorder.Value.Size() / 2, RingScale, SpriteEffects.FlipHorizontally, 0);

                Main.EntitySpriteDraw(DTAssetLib.Vingette.Value, NPCHead - Main.screenPosition, null, BorderCol, Rotation, DTAssetLib.Vingette.Value.Size() / 2, 2.7f, SpriteEffects.None, 0);
            }
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

            
            //SunlightModification.Sunlight(1f, Color.Black, (float)TintCounter / (float)MaxTintCount);
        }

        public void ModifyMusic()
        {
            int tribID = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Tribulation");
            int eternID = MusicLoader.GetMusicSlot(Mod, "Assets/Music/EternityEvils");
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
                else if (DestroyerTestMod.MasochistIsActive && DTMusicConfig.instance.EternityMusic)
                {
                    selectedTrack = masochist;
                    selectedID = masoID;
                }
                else if ((DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive) && !DestroyerTestMod.MasochistIsActive && DTMusicConfig.instance.EternityMusic)
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

        public void FablesTitleCard()
        {
            if (!DTCrossMod.FablesIsLoaded)
            {
                return;
            }

            FablesTitleCardSystem.RegisterFablesBossIntro(new FablesTitleCardSystem.NightmareRoseTitle());
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
        int NodeHealLineScroll = 0;
        float nodeHealShineRot = 0f;
        bool SpawnAuraOnNodes1 = false;
        bool SpawnAuraOnNodes2 = false;

        public List<NPC> cfNodes;
        // Per-node shake state (one entry per node in cfNodes)
        public int[] NodeShakeTimers;
        public float[] NodeShakeMaxX;
        public float[] NodeShakeMaxY;

        public bool FireLR = false; //True = Right, False = Left
        public bool DartsLR = false; //True = Right, False = Left

        int LastChoice;
        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            DTConfig cfg = ModContent.GetInstance<DTConfig>();
            DTMusicConfig muscfg = ModContent.GetInstance<DTMusicConfig>();
            DTOptimizationsConfig optcfg = ModContent.GetInstance<DTOptimizationsConfig>();

            NPCHead = NPC.Center + new Vector2(0, -79);
            DirectionToPlayerCenter = (player.MountedCenter - NPCHead).SafeNormalize(Vector2.UnitY);


            RingScale = DTAssetLib.NightmareRoseArenaBorder.Value.ScaleRingTextureToMatchRadius(BorderRad, 1327);

            SetChances(currentState);

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
                

                if (!DestroyerTestMod.MasochistIsActive)
                {
                    BorderCol = ColorLib.Soul;
                    BorderDustType = ModContent.DustType<SoulDust>();
                }
                else
                {
                    BorderCol = Color.White;
                    BorderDustType = DustID.TintableDustLighted;
                }
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

                    //Dust Border = Dust.NewDustPerfect(Pos, BorderDustType, Vector2.Zero, 0, BorderCol, 1f);
                    //Border.noGravity = true;
                    //Border.fadeIn = 1f;
                    //Border.scale = Main.rand.NextFloat(0.2f, 4.0f);

                    PointGlowPreMultiplied Border = new();
                    Border.Initialize(Pos, Pos.DirectionFrom(NPCHead).RotatedBy(Main.rand.NextFloat(-1.5f, -1.1f)) * Main.rand.NextFloat(1f, 9f), BorderCol, 0.6f);
                    ParticleEngine.BehindProjectiles.Add(Border);
                }

                if (DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive || DestroyerTestMod.MasochistIsActive && currentState != AttackState.SpawnIdle)
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
                player.Hurt(new PlayerDeathReason() { CustomReason = NetworkText.FromKey("Mods.DestroyerTest.NPCs.NightmareRoseBossBossBoss.ExitBarrierDeath", player.name) }, 90, 0, false, true, -1, false, 9, 9, 0);
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

            if (player.statLife <= 0)
            {
                SunlightModification.Reset();
                DeathInterval--;
                if (DeathInterval <= 0)
                {
                    SunlightModification.Reset();
                    NPC.active = false;
                }
            }


            cfNodes = Main.npc.Where(n => n.active && n.type == ModContent.NPCType<CursedFlameNode>()).ToList();
            anyNodesAlive = cfNodes.Count > 0;
            nodeCount = cfNodes.Count;

            // Ensure per-node shake arrays are sized to the current node count and preserve previous values
            if (nodeCount > 0)
            {
                if (NodeShakeTimers == null || NodeShakeTimers.Length != nodeCount)
                {
                    int oldLen = NodeShakeTimers?.Length ?? 0;
                    int[] newTimers = new int[nodeCount];
                    float[] newMaxX = new float[nodeCount];
                    float[] newMaxY = new float[nodeCount];
                    if (oldLen > 0)
                    {
                        int copy = Math.Min(oldLen, nodeCount);
                        Array.Copy(NodeShakeTimers, newTimers, copy);
                        if (NodeShakeMaxX != null) Array.Copy(NodeShakeMaxX, newMaxX, Math.Min(NodeShakeMaxX.Length, copy));
                        if (NodeShakeMaxY != null) Array.Copy(NodeShakeMaxY, newMaxY, Math.Min(NodeShakeMaxY.Length, copy));
                    }
                    NodeShakeTimers = newTimers;
                    NodeShakeMaxX = newMaxX;
                    NodeShakeMaxY = newMaxY;
                }

                // Ensure UI array exists
                if (NightmareRoseHealthBar.NodeLockShake == null || NightmareRoseHealthBar.NodeLockShake.Length != nodeCount)
                {
                    NightmareRoseHealthBar.NodeLockShake = new Vector2[nodeCount];
                }

                // Update each node's timer and max offsets, then drive the UI shake positions
                for (int i = 0; i < nodeCount; i++)
                {
                    if (NodeShakeTimers[i] < 120)
                        NodeShakeTimers[i]++;

                    NodeShakeMaxX[i] = MathHelper.Lerp(10f, 0f, (float)NodeShakeTimers[i] / 120f);
                    NodeShakeMaxY[i] = NodeShakeMaxX[i];

                    if (cfNodes[i].active && cfNodes[i].life > 0)
                    {
                        // random offset in both directions
                        NightmareRoseHealthBar.NodeLockShake[i] = new Vector2(
                            Main.rand.NextFloat(-NodeShakeMaxX[i], NodeShakeMaxX[i]),
                            Main.rand.NextFloat(-NodeShakeMaxY[i], NodeShakeMaxY[i])
                        );
                    }
                    else
                    {
                        NightmareRoseHealthBar.NodeLockShake[i] = Vector2.Zero;
                    }
                }
            }

            if (DTConfig.instance.EnableDebugMessages)
            {
                //string nodeList = string.Join(",", cfNodes.Select(n => n.whoAmI.ToString()).ToArray());
                //Mod.Logger.Info($"[NightmareRose] cfNodes.Count={cfNodes.Count} cfNodes=[{nodeList}] anyNodesAlive={anyNodesAlive}");
            }

            if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.DeathIsActive)
            {
                HealAmount = 15;
            }
            if ((DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive) && !DestroyerTestMod.MasochistIsActive)
            {
                HealAmount = 50;
            }
            if (DestroyerTestMod.MasochistIsActive)
            {
                HealAmount = 80;
            }

            if (anyNodesAlive)
            {
                NPC.dontTakeDamage = true;
                NPC.immortal = true;

                var NodePositions = Opus.GetEquidistantOrbitVectors(nodeCount, NPCHead, 0.003f, NodeRadius);

                NodeHealLineScroll -= 8;
                nodeHealShineRot -= 0.1f;
                bool AnyAuras = Main.projectile.Any(n => n.active && n.type == ModContent.ProjectileType<NodeDefilementAura>());

                if (Main.GameUpdateCount % 150 == 0 && (DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive))
                {
                    int choice = Main.rand.Next(nodeCount);
                    choice = choice == LastChoice ? Main.rand.Next(nodeCount) : choice;
                    LastChoice = choice;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), cfNodes[choice].Center, Vector2.Zero, ModContent.ProjectileType<NodeDefilementAura>(), 10, 0, ai2: cfNodes[choice].whoAmI);
                }



                for (int i = 0; i < cfNodes.Count; i++)
                {
                    Line L = new Line(NPCHead, cfNodes[i].Center);
                    //DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(1, true), ColorLib.WretchedGradient(), Main.spriteBatch, BlendState.Additive, NodeHealLineScroll, 0.5f, 1.75f);

                    cfNodes[i].SmoothMoveToPoint(NodePositions[i], 24f);


                    
         
                }

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
                SpawnAuraOnNodes1 = false;
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
                    NPC.Opacity -= 0.02f;
                }
                else
                {
                    NPC.active = false;
                }
            }

            Rotation--;

            PlayerCenter = player.MountedCenter;

            IdleFX();

            if (!DTUtils.CalamityBossRushActive())
            {
                ModifyMusic();
            }

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
                            ScreenshakePlayer screenshake = player.GetModPlayer<ScreenshakePlayer>();
                            screenshake.screenshakeMagnitude = 6;
                            screenshake.screenshakeTimer = 180;
                            RoarWaveTimer = 180;

                            FablesTitleCard();
                            
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
                            IdleMax = 100;
                        }
                        if (Main.expertMode && !Main.masterMode && !DestroyerTestMod.EternityIsActive)
                        {
                            IdleMax = 80;
                        }
                        if (Main.masterMode || DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive)
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
                        if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.DeathIsActive)
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
                                if (FlameTimer % 20 == 0)
                                {
                                    Vector2 targ = player.Center + player.velocity * 4f;
                                    Projectile.NewProjectile(Entity.GetSource_FromThis(), NPCHead, NPCHead.DirectionTo(targ) * 15f, ModContent.ProjectileType<BloomTurret>(), 40, 0, ai0: player.whoAmI);
                                }

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
                        if (DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive)
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
                            ContemptAttackWarningOffset += FireLR ? 0.00001f : -0.00001f;
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
                        if (Divided)
                        {
                            ResetState();
                        }
                        if ((DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive) && !Divided)
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

                        if (DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive)
                        {
                            if (FlameRingCount < 9 && Main.GameUpdateCount % 60 == 0)
                            {
                                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ChargeBreak") with { PitchVariance = 1f });

                                int amount = anyNodesAlive ? 3 : 5;
                                var P = Opus.GetEquidistantVectors(amount, player.MountedCenter, 300, 0);
                                for (int i = 0; i < P.Length; i++)
                                {
                                    SmallShine shine = new SmallShine();
                                    shine.Prepare(P[i], Vector2.Zero, Color.White, 2f);
                                    ParticleEngine.ShaderParticles.Add(shine);
                                }

                                Opus.RingSpreadProjectile(ModContent.ProjectileType<TenebrisFlamesHostile>(), amount, player.MountedCenter, 300, 30, 2, 8);
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
                                Opus.RadialSpreadProjectile(ModContent.ProjectileType<NightmareRoseCursedCrystal>(), 9, NPC.Center, 40, 4, 12, ai1: 1, offset: off);
                                Opus.RadialSpreadProjectile(ModContent.ProjectileType<NightmareRoseCursedCrystal>(), 9, NPC.Center, 40, 4, 12, ai1: -1, offset: off);
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
                        if (DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive)
                        {
                            int numProjectiles = anyNodesAlive ? 4 : 7;
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
                            Opus.RingSpreadProjectile(ModContent.ProjectileType<BloomTurret2>(), 12, NPCHead, 30, 20, 1, 8, ai0: player.whoAmI);

                            Opus.RadialSpreadProjectile(ModContent.ProjectileType<VileSpike>(), 8, NPCHead, 20, 1, 18);

                            ResetState();
                        }
                        break;
                    }
                case AttackState.Napalm:
                    {

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
                                particle.Initialize(player.MountedCenter, Vector2.Zero, Color.White, 9f);
                                ParticleEngine.ShaderParticles.Add(particle);

                                for (int i = 0; i < 7; i++)
                                {
                                    Vector2 Pos = new Vector2(NPC.Bottom.X + Main.rand.NextFloat(-1600, 1600), NPC.Bottom.Y + 20);
                                    Projectile proj =  Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), Pos, new Vector2(Main.rand.NextFloat(-3f, 3f), -6f), ModContent.ProjectileType<VileSpike>(), 30, 4);
                                    proj.timeLeft = 480;
                                }

                                Projectile p1 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPCHead, new Vector2(-10f, 0f), ModContent.ProjectileType<VileSpike>(), 30, 4);
                                p1.timeLeft = 480;

                                Projectile p2 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPCHead, new Vector2(10f, 0f), ModContent.ProjectileType<VileSpike>(), 30, 4);
                                p2.timeLeft = 480;


                                if (!player.mount.Active)
                                {
                                    player.velocity.Y += 100;
                                }
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
                            int amount = anyNodesAlive ? 1 : 3;
                            if (Main.GameUpdateCount % 120 == 0)
                            {
                                Opus.RadialSpreadProjectileRandom(ModContent.ProjectileType<DarkOrb>(), amount, NPCHead, 30, 3, 8);
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
                        ResetState();
                        NPC Minion;
                        if (NPC.life < NPC.lifeMax * 0.4f && !HammerActive)
                        {
                            bool eternity = DestroyerTestMod.EternityIsActive;
                            //Main.NewText("EternityIsActive returned: " + eternity);
                            /*
                            if (!eternity)
                            {
                                Minion = NPC.NewNPCDirect(Entity.GetSource_FromThis(), NPC.Center, ModContent.NPCType<GigaCursedHammer>());
                            }
                            else if (eternity)
                            {
                                Minion = NPC.NewNPCDirect(Entity.GetSource_FromThis(), NPC.Center, ModContent.NPCType<TenebrousConstruct>());
                            }
                            
                            HammerActive = true;
                            */
                        }

                        else
                        {
                            ResetState();
                        }
                    }
                    break;
                case AttackState.DemoniteWhisper:
                    {
                        int getinterval()
                        {
                            if (DTUtils.ClassicMode())
                            {
                                return 180;
                            }
                            if (Main.expertMode && !Main.masterMode)
                            {
                                return 120;
                            }
                            if (Main.masterMode)
                            {
                                return 90;
                            }
                            return 180;
                        }

                        int interval = (DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive) ? 180 : getinterval();
                        if (Main.GameUpdateCount % interval == 0)
                        {
                            SummonSouls();
                            SoulSpawnCount++;
                            if (DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive)
                            {
                                Opus.RadialSpreadProjectile(ModContent.ProjectileType<BigSoul>(), DestroyerTestMod.MasochistIsActive ? 3 : 2, NPCHead, 30, 7, 6f, offset: Main.rand.NextFloat(MathHelper.TwoPi));
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
                        if (DestroyerTestMod.EternityIsActive|| DestroyerTestMod.DeathIsActive)
                        {
                            int interval = DestroyerTestMod.MasochistIsActive ? 15 : 60;
                            if (Main.GameUpdateCount % interval == 0)
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
                        int CountBlossomMines = Main.projectile.Where(n => n.active && n.type == ModContent.ProjectileType<BlossomMine>()).Count();
                        int CountDarkMines = Main.projectile.Where(n => n.active && n.type == ModContent.ProjectileType<DarkLaserMine>()).Count();

                        if (CountBlossomMines < 6 && CountDarkMines < 6)
                        {
                            BlossomMines(Main.rand.NextVector2FromRectangle(new Rectangle(0, 0, (int)BorderRad, (int)BorderRad)));
                        }
                        else
                        {
                            ResetState();
                        }
                        
                    }
                    break;
                case AttackState.Desperation:
                    {
                        NPC.dontTakeDamage = true;
                        ShouldCenterCameraOnNPC = true;
                        
                        for (int i = 0; i < NPC.maxBuffs; i++)
                        {
                            NPC.DelBuff(i);
                        }

                        if (DTCrossMod.CalamityIsLoaded)
                        {
                            DTCrossMod.CalamityMod.Call("SetShouldCloseBossHealthBar", NPC, false);
                        }

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
                            //RingScale = MathHelper.Lerp(DTAssetLib.NightmareRoseArenaBorder.Value.ScaleRingTextureToMatchRadius(BorderRad, 1327), 0, progress);

                        }
                        if (DesperationTimer >= 1200)
                        {
                            NPC.netUpdate = true;
                            currentState = AttackState.KillIdle;
                            BorderActive = false;
                            //Main.NewText("Get away from the Rose!!", ColorLib.Soul);
                            LerpingBloomRingSharp Ring = new();
                            Color[] P = new Color[4] { Color.White, DestroyerTestMod.MasochistIsActive ? Color.White : ColorLib.Soul, DestroyerTestMod.MasochistIsActive ? Color.White : ColorLib.Soul2, DestroyerTestMod.MasochistIsActive ? Color.White : ColorLib.Soul3 };
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
                                foreach (Dust dust in Opus.RingSpreadDustRandom(DustID.FireworksRGB, 20, NPCHead, Main.rand.NextFloat(30f, 400f), 0, Color.White, -10f, 1f))
                                {
                                    dust.noGravity = true;
                                }
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

                            NPC.dontTakeDamage = false;
                            NPC.StrikeInstantKill();

                        }
                        else
                        {
                            NPC.dontTakeDamage = true;
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

                Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(NPC.Hitbox), new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-10.5f, -2.5f)), 0f, ColorLib.Soul, 0.5f, false, 40, SparkDrawMode.Additive, 2f);
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

                Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(NPC.Hitbox), new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-10.5f, -2.5f)), 0f, ColorLib.Soul, 0.5f, false, 40, SparkDrawMode.Additive, 2f);
                ParticleEngine.ShaderParticles.Add(Spark);

                Lighting.AddLight(NPC.Center, ColorLib.Soul.ToVector3() * 0.5f);
            }
            else
            {
                if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    if (Main.rand.NextBool(8))
                    {
                        Dust fire = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.FireworksRGB, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-13.5f, -2.5f), 40, ColorLib.CursedFlames, 2.5f);
                        fire.noGravity = true;

                        Spark Spark = new Spark();

                        Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(NPC.Hitbox), new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-10.5f, -2.5f)), 0f, ColorLib.CursedFlames, 0.5f, false, 40, SparkDrawMode.Additive, 2f);
                        ParticleEngine.ShaderParticles.Add(Spark);
                    }
                    Lighting.AddLight(NPC.Center, ColorLib.CursedFlames.ToVector3() * 0.5f);
                }
                if (DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    if (Main.rand.NextBool(8))
                    {
                        Dust fire = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.FireworksRGB, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-13.5f, -2.5f), 40, ColorLib.CursedFlames, 2.5f);
                        fire.noGravity = true;

                        Spark Spark = new Spark();

                        Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(NPC.Hitbox), new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-10.5f, -2.5f)), 0f, ColorLib.CursedFlames, 0.5f, false, 40, SparkDrawMode.Additive, 2f);
                        ParticleEngine.ShaderParticles.Add(Spark);
                    }
                    Lighting.AddLight(NPC.Center, ColorLib.CursedFlames.ToVector3() * 0.5f);
                }
                if (DestroyerTestMod.MasochistIsActive)
                {
                    if (Main.rand.NextBool(8))
                    {
                        Dust fire = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.FireworksRGB, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-13.5f, -2.5f), 40, ColorLib.TenebrisGradient, 2.5f);
                        fire.noGravity = true;

                        Spark Spark = new Spark();

                        Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(NPC.Hitbox), new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-10.5f, -2.5f)), 0f, ColorLib.TenebrisGradient, 0.5f, false, 40, SparkDrawMode.Additive, 2f);
                        ParticleEngine.ShaderParticles.Add(Spark);
                    }
                    Lighting.AddLight(NPC.Center, ColorLib.TenebrisGradient.ToVector3() * 0.5f);
                }
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

            Main.EntitySpriteDraw(DTAssetLib.GlowCone.Value, NPCHead - Main.screenPosition, null, ColorLib.CursedFlames, dir.ToRotation() + MathHelper.PiOver4, DTAssetLib.GlowCone.Value.Size() / 2, GlowConeScaling, SpriteEffects.None, 0);

            
        }


        public void GlowConeWarning_CursedFlamesEternity()
        {
            var i = Opus.GetEquidistantOrbitVectorsAndRots(6, NPCHead, ContemptAttackWarningOffset, 40);

            foreach (var p in i)
            {
                Main.EntitySpriteDraw(DTAssetLib.GlowCone.Value, NPCHead - Main.screenPosition, null, ColorLib.CursedFlames, p.Rotation + MathHelper.PiOver4, DTAssetLib.GlowCone.Value.Size() / 2, GlowConeScaling, SpriteEffects.None, 0);
            }
        }

        public void GlowConeWarning_Napalm()
        {
            Main.EntitySpriteDraw(DTAssetLib.GlowCone.Value, NPCHead - Main.screenPosition, null, ColorLib.CursedFlames, -MathHelper.PiOver4, DTAssetLib.GlowCone.Value.Size() / 2, new Vector2(GlowConeScaling * 2, GlowConeScaling), SpriteEffects.None, 0);
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
            if (!DestroyerTestMod.MasochistIsActive)
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
            else
            {
                for (int j = 0; j < 2; j++)
                {
                    Vector2 spawnPos = NPCHead;
                    Vector2 targetPos = NPCHead + Main.rand.NextVector2CircularEdge(BorderRad, BorderRad);
                    Vector2 direction = (targetPos - spawnPos).SafeNormalize(Vector2.Zero); // SafeNormalize prevents division by zero

                    Projectile SB = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), spawnPos, direction * 6, ModContent.ProjectileType<DarkLaserMine>(), 0, 1);
                    SB.timeLeft = 60;
                }
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
                if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.DeathIsActive)
                {
                    for (int a = 0; a < 10; a++)
                    {
                        Vector2 SpawnPoint = new Vector2(NPC.Center.X + Main.rand.NextFloat(-BorderRad, BorderRad), NPC.Center.Y + 800);
                        Projectile.NewProjectile(Entity.GetSource_FromThis(), SpawnPoint, new Vector2(0, -8), ModContent.ProjectileType<TormentedSoul>(), 25, 2);
                    }
                }
                if (DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive)
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
                            SpawnPoint = new Vector2(NPC.Center.X + 1700, NPC.Center.Y + Main.rand.NextFloat(-BorderRad, BorderRad));
                            S = 1;
                        }
                        else
                        {
                            MoveDir = new Vector2(16, 0);
                            SpawnPoint = new Vector2(NPC.Center.X - 1700, NPC.Center.Y + Main.rand.NextFloat(-BorderRad, BorderRad));
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
            if (!DestroyerTestMod.MasochistIsActive)
            {
                for (int e = 0; e < 6; e++)
                {
                    Vector2 minePosition = Main.rand.NextVector2FromRectangle(
                    new Rectangle(
                        (int)Main.LocalPlayer.MountedCenter.X - Main.screenWidth / 2,
                        (int)Main.LocalPlayer.MountedCenter.Y - Main.screenHeight / 2,
                        Main.screenWidth,
                        Main.screenHeight
                        )
                    );

                    Projectile.NewProjectile(
                        Entity.GetSource_FromThis(),
                        minePosition,
                        Vector2.Zero,
                        ModContent.ProjectileType<BlossomMine>(),
                        50,
                        0f
                    );
                }
            }
            else
            {
                for (int e = 0; e < 6; e++)
                {
                    Vector2 minePosition = Main.rand.NextVector2FromRectangle(
                    new Rectangle(
                        (int)Main.LocalPlayer.MountedCenter.X - Main.screenWidth / 2,
                        (int)Main.LocalPlayer.MountedCenter.Y - Main.screenHeight / 2,
                        Main.screenWidth,
                        Main.screenHeight
                        )
                    );

                    Projectile.NewProjectile(
                        Entity.GetSource_FromThis(),
                        minePosition,
                        Vector2.Zero,
                        ModContent.ProjectileType<DarkLaserMine>(),
                        100,
                        0f
                    );
                }
            }
        }

        public float ContemptAttackRotationOffset = 0f;
        public float ContemptAttackWarningOffset = 0f;
        public bool SetDir1 = false;

        public float LaserWarnOpacity = 0f;
        public float LaserRotOffset = 0;
        public int LaserWarnTimer = 120;
        bool Flag4 = false;
        Projectile[] LaserCol;

        bool ShouldDrawLaserWarning = false;
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

            if ((DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive) && !DestroyerTestMod.MasochistIsActive)
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

                    ShouldDrawLaserWarning = true;
                    Flag4 = false;
                }
                else
                {

                    if (!Flag4)
                    {
                        SunlightModification.Pulse(1f, ColorLib.TenebrisGradient, 0.8f);
                        foreach (Projectile P in Opus.RadialSpreadProjectile(ModContent.ProjectileType<DarkLaserMine>(), 10, NPCHead, 70, 8, 10))
                        {
                            P.timeLeft = 300;
                        }
                        Flag4 = true;
                    }
                    
                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/TenebrisLasers"), NPCHead);

                    int Dir = FireLR ? -1 : 1;
                    LaserCol = Opus.RadialSpreadProjectile(ModContent.ProjectileType<TenebrisLaser>(), 6, NPCHead, 60, 1, 0.005f, ai1: Dir, offset: LaserRotOffset);
                    LaserWarnTimer = 120;
                    LaserWarnOpacity = 0f;
                    ShouldDrawLaserWarning = false;
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

            int DartDamage()
            {
                if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.DeathIsActive)
                {
                    return 40;
                }
                if ((DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive) && !DestroyerTestMod.MasochistIsActive)
                {
                    return 50;
                }
                if (DestroyerTestMod.MasochistIsActive)
                {
                    return 60;
                }

                return 1;
            }


            Projectile Dart = null;

            int interval = anyNodesAlive ? 20 : 15;

            if (Main.GameUpdateCount % interval == 0)
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
                        DartDamage(),
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
                int numProjectiles = anyNodesAlive ? 3 : 5;
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
                        DartDamage() * 2,
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



            if ((DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive) && !DestroyerTestMod.MasochistIsActive)
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

            // Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

            // The order in which you add loot will appear as such in the Bestiary. To mirror vanilla boss order:
            // 1. Trophy
            // 2. Classic Mode ("not expert")
            // 3. Expert Mode (usually just the treasure bag)
            // 4. Master Mode (relic first, pet last, everything else in between)



            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Item_NightmareRoseTrophy>(), 10));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PhantasmalRemnant>(), 1, 4, 9));

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Contempt>(), 2, 1, 1));
            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CursedHammer>(), 2, 1, 1));
            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<DeadlyBlossom>(), 2, 1, 1));
            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<BlossomBeater>(), 2, 1, 1));
            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<ForsakenMaelstrom>(), 4, 1, 1));
            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<HaepienNodeCharm>(), 6, 1, 1));

            npcLoot.Add(notExpertRule);

            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<NightmareRoseLootBag>()));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<NightmarePowder>()));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Item_NightmareRoseRelic>()));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Item_NightmareRoseMemoryPedistal>()));


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
            Vector2 playerCenter = Main.LocalPlayer.MountedCenter;

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

            if (DTUtils.CalamityBossRushActive())
            {
                NPC.lifeMax = 400000;
                NPC.defense = 60;
            }
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

            NPC.ai[0]++;

            if (NPC.ai[0] % 60 == 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastAuraPulse, NPC.Center);
                LerpingBloomRingSharp Ring = new();
                Ring.Prepare(NPC.Center, Vector2.Zero, ColorLib.WretchedColorMap, 0.2f, 0.01f, 3f);
                ParticleEngine.Particles.Add(Ring);
            }

            // This node's per-node shake state is handled by the NightmareRoseBoss instance.
            // The node itself does not maintain arrays for all nodes.
            // Keep other node AI behaviors above; nothing else required here for UI shake.
            var NR = Main.npc.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<NightmareRoseBoss>());
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            var boss = Main.npc.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<NightmareRoseBoss>());
            if (boss?.ModNPC is NightmareRoseBoss nr)
            {
                int idx = nr.cfNodes.FindIndex(n => n.whoAmI == NPC.whoAmI);
                if (idx >= 0 && nr.NodeShakeTimers != null && idx < nr.NodeShakeTimers.Length)
                    nr.NodeShakeTimers[idx] = 0;
            }
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            var boss = Main.npc.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<NightmareRoseBoss>());
            if (boss?.ModNPC is NightmareRoseBoss nr)
            {
                int idx = nr.cfNodes.FindIndex(n => n.whoAmI == NPC.whoAmI);
                if (idx >= 0 && nr.NodeShakeTimers != null && idx < nr.NodeShakeTimers.Length)
                    nr.NodeShakeTimers[idx] = 0;
            }
        }


        public override void OnKill()
        {
            var boss = Main.npc.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<NightmareRoseBoss>());
            if (boss?.ModNPC is NightmareRoseBoss nr)
            {
                int idx = nr.cfNodes.FindIndex(n => n.whoAmI == NPC.whoAmI);
                nr.cfNodes.RemoveAt(idx);
            }
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
            Projectile.Center = Main.Localplayer.MountedCenter;
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
