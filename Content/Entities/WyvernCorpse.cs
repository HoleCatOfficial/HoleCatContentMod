
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.BossBar;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss;
using DestroyerTest.Content.Projectiles.Boss.VampireBoss;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using InnoVault.PRT;
using log4net.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Animations;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using UtfUnknown.Core.Models.SingleByte.Finnish;
using GlowmaskHelper.Content;
using OpusLib;
using DestroyerTest.Content.Projectiles.Boss;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Ichor;
using ReLogic.Utilities;
using OpusLib.Content.Helpers;
using Terraria.GameContent;
using BreadLibrary.Core.Graphics.Particles;

namespace DestroyerTest.Content.Entities
{
    /// <summary>
    /// This is the code from Consolaria's Arch Wyvern. I do not own any of this except for the textures I paint over it. This code will be replaced in the future, when I am capable of modding something so advanced. (Trust me. I tried many times with the example worm. It did not go well.)
    /// </summary>
    [AutoloadBossHead]
    public class WyvernCorpseHead : ModNPC
    {
        public enum attackType
        {
            Follow,
            Dash,
            IchorRam,
            Circle,
            OrganBurst,
            SummonCrimsonMinions,
            SummonAxes,
            BloodShoot,
            FleshBombShoot,
            Clouds,
            TeleDash,
            HeartMatrix,
            Nodes,
            Desperation,
            Enraged
        }

        public SoundStyle Roar = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/CorpseRoar1") with { PitchVariance = 1.0f, MaxInstances = 0  };
        public SoundStyle Kill = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/Enrage") with { PitchVariance = 1.0f, Volume = 4 };
        public SoundStyle Teeth = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/ToothShoot") with { PitchVariance = 1.0f };
        public SoundStyle TeleportSetPosition = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/TeleportSetPosition") with { PitchVariance = 1.0f };
        public SoundStyle Attack = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/Attack", 10) with { PitchVariance = 1.0f, MaxInstances = 0 };
        public SoundStyle NodeSpawnSound = new SoundStyle("DestroyerTest/Infernum/Assets/Audio/WyvernCorpseIntroFinish") with { PitchVariance = 1f, MaxInstances = 0 };
        public SoundStyle Kill2 = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/DespRoar");
        public SoundStyle Desperation = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/Desperation");
        public static LocalizedText BestiaryText
        {
            get; private set;
        }

        public void immunities()
        {
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
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "DestroyerTest/Content/Entities/WyvernCorpseBestiary",

                PortraitPositionXOverride = -25f,
                PortraitPositionYOverride = 0f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);

            immunities();

            BestiaryText = this.GetLocalization("Bestiary");
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 36;

            NPC.aiStyle = NPCAIStyleID.Worm;

            NPC.damage = 0;
            NPC.defense = 65;
            NPC.lifeMax = 420000;

            NPC.noGravity = true;
            NPC.noTileCollide = true;

            NPC.HitSound = SoundID.Tink with { Pitch = -0.6f, PitchVariance = 0.4f };
            NPC.DeathSound = Kill;
            NPC.boss = true;

            NPC.knockBackResist = 0.0f;
            NPC.rarity = 5;
            NPC.npcSlots = 20f;

            NPC.netAlways = true;
            NPC.netUpdate = true;
            NPC.netID = ModContent.NPCType<WyvernCorpseHead>();
            NPC.BossBar = ModContent.GetInstance<CrimsonBossBar>();

            NPC.hide = true;
            NPC.value = Item.buyPrice(platinum: 1, gold: 15, silver: 75);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                new FlavorTextBestiaryInfoElement(DTUtils.GetModNPCLocalizationEntry(this, 1)),
                new FlavorTextBestiaryInfoElement(DTUtils.GetModNPCLocalizationEntry(this, 2)),
            });
        }
        public override bool CheckActive()
        {
            return false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.1f;
            return new bool?();
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

        // Multiplayer-synced fields
        public attackType CurrentAttack = attackType.Follow;

        public bool SpawnFlag = false;
        public int FollowTime = 0;

        public bool IsDashing = false;
        public int DashCount = 0;
        public int DashTime = 80;
        public int DashOverrideTimer = 1800;

        public int SpitTime = 0;

        public float circleradius = 800f;
        public float circlerotspeed = 0.05f;
        public int CircleTime = 0;
        public int CircleLanceCount = 0;

        public int OrganBurstIntervalTimer = 0;
        public int OrganBurstCount = 0;

        public bool HasSummoned40PercentMinions = false;

        public int BloodTimer = 0;
        public int BloodInterval = 0;

        public int MinionSpawnTimer = 0;
        public int MinionSpawnCount = 0;
        public int MinionSpawnType = 0;

        public int ToothCount = 5;
        public bool flag1 = false;
        public Vector2 ToothCenter;

        public int BombSpawnTimer = 0;
        public int BombSpawnCount = 0;

        public int TeleDashCount = 0;
        public bool TeleDashGetTelePosition = false;
        public int TeleDashWaitTime = 240;
        public Vector2 TelePos;
        public Vector2 DashDirection;
        public Vector2 Outer;

        public bool HeartMatrixGetPositions = false;
        public Vector2 HeartPos;


        public bool HasTriggeredNodes = false;

        public Vector2 DesperationOrbitCenter;
        public int DesperationTimer = 0;
        public float DesperationVingetteScale = 15;
        public byte DesperationVingetteAlpha = 255;

        public float IchorSpiralRotationOffset = 0;
        public float TextureRotationOffset = 0;
        public bool anyNodesAlive;
        public int nodeCount;

        public bool SoundFlag1 = false;
        public bool HasShotTeeth = false;

        SlotId DesperationLoopSlot;
        public SoundStyle Loop = new SoundStyle("DestroyerTest/Assets/Audio/AuraLoop/LaserLoop1") 
        { 
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };
        public float PitchVal = -2;

        

        // Write extra AI fields for multiplayer sync
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)CurrentAttack);
            writer.Write(FollowTime);
            writer.Write(IsDashing);
            writer.Write(DashCount);
            writer.Write(DashTime);
            writer.Write(SpitTime);
            writer.Write(circleradius);
            writer.Write(circlerotspeed);
            writer.Write(CircleTime);
            writer.Write(OrganBurstIntervalTimer);
            writer.Write(OrganBurstCount);
            writer.Write(HasSummoned40PercentMinions);
            writer.Write(BloodTimer);
            writer.Write(BloodInterval);
            writer.Write(MinionSpawnTimer);
            writer.Write(MinionSpawnCount);
            writer.Write(MinionSpawnType);
            writer.Write(BombSpawnTimer);
            writer.Write(BombSpawnCount);
            writer.Write(HasTriggeredNodes);
            writer.WriteVector2(DesperationOrbitCenter);
            writer.Write(DesperationTimer);
            writer.Write(anyNodesAlive);
        }

        // Read extra AI fields for multiplayer sync
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            CurrentAttack = (attackType)reader.ReadInt32();
            FollowTime = reader.ReadInt32();
            IsDashing = reader.ReadBoolean();
            DashCount = reader.ReadInt32();
            DashTime = reader.ReadInt32();
            SpitTime = reader.ReadInt32();
            circleradius = reader.ReadSingle();
            circlerotspeed = reader.ReadSingle();
            CircleTime = reader.ReadInt32();
            OrganBurstIntervalTimer = reader.ReadInt32();
            OrganBurstCount = reader.ReadInt32();
            HasSummoned40PercentMinions = reader.ReadBoolean();
            BloodTimer = reader.ReadInt32();
            BloodInterval = reader.ReadInt32();
            MinionSpawnTimer = reader.ReadInt32();
            MinionSpawnCount = reader.ReadInt32();
            MinionSpawnType = reader.ReadInt32();
            BombSpawnTimer = reader.ReadInt32();
            BombSpawnCount = reader.ReadInt32();
            HasTriggeredNodes = reader.ReadBoolean();
            DesperationOrbitCenter = reader.ReadVector2();
            DesperationTimer = reader.ReadInt32();
            anyNodesAlive = reader.ReadBoolean();
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }


        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (CurrentAttack == attackType.Desperation || anyNodesAlive)
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

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (CurrentAttack == attackType.Desperation || anyNodesAlive)
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
            if (CurrentAttack == attackType.Desperation || anyNodesAlive)
                return false;

            return base.CanBeHitByItem(player, item);
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (CurrentAttack == attackType.Desperation || anyNodesAlive)
            {
                NPC.immortal = true;
                modifiers.FinalDamage *= 0f;
            }
        }

        public override bool? CanCollideWithPlayerMeleeAttack(Player player, Item item, Rectangle meleeAttackHitbox)
        {
            return base.CanCollideWithPlayerMeleeAttack(player, item, meleeAttackHitbox);
        }

        public Vector2 Center;
        public int DeathInterval = 10;
        public override void AI()
        {
            TextureRotationOffset -= 0.25f;
            DTConfig cfg = ModContent.GetInstance<DTConfig>();
            DTMusicConfig muscfg = ModContent.GetInstance<DTMusicConfig>();
            DTOptimizationsConfig optcfg = ModContent.GetInstance<DTOptimizationsConfig>();

            NPC.TargetClosest();
            Player player = Main.player[NPC.target];

            if (Main.netMode == NetmodeID.Server && Main.GameUpdateCount % 1200 == 0)
            {
                var alivePlayers = Main.player.Where(p => p.active && !p.dead).ToList();
                if (alivePlayers.Count > 0)
                {
                    var newTarget = Main.rand.Next(alivePlayers.Count);
                    NPC.target = alivePlayers[newTarget].whoAmI;
                    NPC.netUpdate = true;
                }
            }

            Center = NPC.Center;

            float circleangle = Main.GameUpdateCount * circlerotspeed;

            Vector2 offset = new Vector2(MathF.Cos(circleangle), MathF.Sin(circleangle)) * circleradius;

            Vector2 offsetDes = new Vector2(MathF.Cos(circleangle), MathF.Sin(circleangle)) * 400;

            Vector2 ToPlayer = NPC.Center - player.Center;

            Vector2 ToPlayerInverse = player.Center - NPC.Center;

            DashDirection = (player.Center - TelePos);
            Outer = TelePos + DashDirection * 120;


            Vector2 RandNearPlayer = player.Center + new Vector2(Main.rand.NextFloat(-200f, 200f), Main.rand.NextFloat(-200f, 200f));

            if (NPC.target < 0 || NPC.target == 250 || player.dead) NPC.TargetClosest(true);
            if (player.dead && NPC.timeLeft > 300) NPC.timeLeft = 300;

            if (!(player.ZoneCrimson && (player.ZoneOverworldHeight || player.ZoneSkyHeight)) && Main.masterMode)
            {
                NPC.dontTakeDamage = true;
            }
            else
            {
                NPC.dontTakeDamage = false;
            }

            // Assuming this is inside your boss NPC code
            anyNodesAlive = Main.npc.Any(n => n.active && n.type == ModContent.NPCType<IchorNode>());
            nodeCount = Main.npc.Count(n => n.active && n.type == ModContent.NPCType<IchorNode>());

            if (anyNodesAlive)
            {
                NPC.dontTakeDamage = true;
                NPC.immortal = true;
                NPC.life += 2;
            }
            else if (!anyNodesAlive && CurrentAttack == attackType.Desperation)
            {
                NPC.immortal = true;
                NPC.dontTakeDamage = true;
            }
            else
            {
                NPC.dontTakeDamage = false;
                NPC.immortal = false;
            }

            if (player.dead)
            {
                DeathInterval--;
                if (DeathInterval <= 0)
                {
                    NPC.active = false;
                }
            }

            if (NPC.life >= NPC.lifeMax * 0.24f && NPC.life <= NPC.lifeMax * 0.25f)
            {
                if (HasTriggeredNodes == false)
                {
                    CurrentAttack = attackType.Nodes;

                    HasTriggeredNodes = true;

                }
            }

            if (NPC.life <= NPC.lifeMax * 0.05f)
            {
                CurrentAttack = attackType.Desperation;
            }

            if (CurrentAttack == attackType.Desperation)
            {
                NPC.dontTakeDamage = true;
                NPC.immortal = true;
            }

            if (EternityIsActive())
            {
                SunlightModification._SunColorBrightness = 1f;
                //Main.eclipseLight = 1;
                //Main.ColorOfTheSkies = Color.Black;
            }
            else
            {
                SunlightModification.Reset();
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && SpawnFlag == false)
            {
                if (NPC.ai[0] == 0f)
                {
                    NPC.ai[2] = NPC.whoAmI;
                    NPC.realLife = NPC.whoAmI;

                    int num96 = NPC.whoAmI;
                    for (int num97 = 0; num97 < 60; num97++)
                    {
                        int WyvBodyInt = ModContent.NPCType<WyvernCorpseBody1>();
                        if (num97 == 4 || num97 == 16 || num97 == 32 || num97 == 48)
                            WyvBodyInt = ModContent.NPCType<WyvernCorpseLegs>();
                        else if (num97 == 57)
                            WyvBodyInt = ModContent.NPCType<WyvernCorpseBody2>();
                        else if (num97 == 58)
                            WyvBodyInt = ModContent.NPCType<WyvernCorpseBody3>();
                        else if (num97 == 59)
                            WyvBodyInt = ModContent.NPCType<WyvernCorpseTail>();

                        int num99 = NPC.NewNPC(NPC.GetSource_FromAI(),
                            (int)(NPC.position.X + NPC.width / 2),
                            (int)(NPC.position.Y + NPC.height),
                            WyvBodyInt, NPC.whoAmI);

                        Main.npc[num99].ai[2] = NPC.whoAmI;
                        Main.npc[num99].realLife = NPC.whoAmI;
                        Main.npc[num99].ai[1] = num96;
                        Main.npc[num96].ai[0] = num99;

                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num99);
                        num96 = num99;
                    }
                    NPC.netUpdate = true;
                    SpawnFlag = true;
                }
            }

            if (ModContent.GetInstance<DTConfig>().EnableDebugMessages && Main.GameUpdateCount % 120 == 0)
            {
                Mod.Logger.Info($"Current State: {CurrentAttack}");
            }
            

            if (!Main.dedServ && !EternityIsActive())
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Tribulation");
            }
            if (!Main.dedServ && EternityIsActive() && !muscfg.EternityMusic)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Tribulation");
            }
            if (!Main.dedServ && EternityIsActive() && muscfg.EternityMusic)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Placeholder4");
            }
            if (!Main.dedServ && EternityIsActive() && Main.masterMode && muscfg.EternityMusic)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/MasoEvils");
            }

            ImportantMathematics();

            switch (CurrentAttack)
            {
                case attackType.Follow:
                    {
                        FollowTime++;
                        if (FollowTime == 360)
                        {
                            CurrentAttack = attackType.Dash;
                        }
                    }
                    break;
                case attackType.Nodes:
                    if (NPC.type == ModContent.NPCType<WyvernCorpseHead>())
                    {
                        NPC.aiStyle = -1;
                        NodeSpawn();

                        CurrentAttack = attackType.Dash;
                        ResetStats();
                    }
                    break;
                case attackType.Dash:
                    {
                        DashOverrideTimer--;
                        if (DashOverrideTimer > 0)
                        {

                            if (!IsDashing && NPC.Distance(player.Center) < 350 && DashTime > 0)
                            {
                                SoundEngine.PlaySound(Roar, NPC.Center);
                                Projectile FleshBomb = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<FleshBomb>(), 20, 1);
                                NPC.velocity *= 3f;
                                IsDashing = true;
                                DashCount++;
                            }

                            if (IsDashing)
                            {
                                DashTime--; // tick down every frame while dashing
                                DashParticle();

                                Vector2 FlankLeft = NPC.velocity.RotatedBy(MathHelper.PiOver2);
                                Vector2 FlankRight = NPC.velocity.RotatedBy(-MathHelper.PiOver2);

                                if (Main.GameUpdateCount % 5 == 0 && NPC.velocity.Length() > 2)
                                {
                                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/HopeScabbardTele") with { MaxInstances = 0, PitchVariance = 0.3f, Volume = 0.15f }, NPC.Center);
                                    Projectile.NewProjectile(Entity.GetSource_FromAI(), NPC.Center, FlankLeft * 0.02f, ModContent.ProjectileType<LightDart>(), 15, 3);
                                    Projectile.NewProjectile(Entity.GetSource_FromAI(), NPC.Center, FlankRight * 0.02f, ModContent.ProjectileType<LightDart>(), 15, 3);
                                }

                                if (DashTime <= 0)
                                {
                                    NPC.velocity /= 5;
                                    DashTime = 80;
                                    Projectile.NewProjectile(Entity.GetSource_FromAI(), NPC.Center, FlankLeft * 0.5f, ModContent.ProjectileType<LightLance>(), 15, 3);
                                    Projectile.NewProjectile(Entity.GetSource_FromAI(), NPC.Center, FlankRight * 0.5f, ModContent.ProjectileType<LightLance>(), 15, 3);
                                    IsDashing = false;
                                }
                            }

                            if (DashCount >= 5)
                            {
                                CurrentAttack = attackType.IchorRam;
                                ResetStats();
                            }
                        }
                        if (DashOverrideTimer <= 0)
                        {
                            CurrentAttack = attackType.IchorRam;
                            ResetStats();
                        }
                    }
                    break;
                case attackType.IchorRam:
                    {
                        SpitTime++;
                        int interval = 10;
                        if (EternityIsActive())
                        {
                            interval = 20;
                        }
                        if (!EternityIsActive())
                        {
                            interval = 30;
                        }
                        if (!EternityIsActive())
                        {
                            if (SpitTime % interval == 0)
                            {
                                Projectile IchorSpit = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), NPC.Center, NPC.velocity * 1.5f, ProjectileID.GoldenShowerHostile, 40, 2);
                            }
                        }
                        if (EternityIsActive())
                        {
                            if (!Main.masterMode)
                            {
                                float Sine = (float)Math.Sin(SpitTime * 0.1f) * 0.01f;
                                if (SpitTime % interval == 0)
                                {
                                    Projectile.NewProjectile(Entity.GetSource_FromThis(), NPC.Center, NPC.velocity * 1.5f, ModContent.ProjectileType<PrimalIchor>(), 40, 2, ai0: 1, ai1: Sine);
                                    Projectile.NewProjectile(Entity.GetSource_FromThis(), NPC.Center, NPC.velocity * 1.5f, ModContent.ProjectileType<PrimalIchor>(), 40, 2, ai0: 0, ai1: Sine);
                                }
                            }
                            else
                            {
                                if (SpitTime % interval == 0)
                                {
                                    Projectile.NewProjectile(Entity.GetSource_FromThis(), NPC.Center, NPC.velocity * 1.5f, ModContent.ProjectileType<SoulSpark_NoHoming>(), 40, 2);
                                }
                            }
                        }
                        if (SpitTime >= 480)
                        {
                            CurrentAttack = attackType.OrganBurst;
                            ResetStats();
                        }
                    }
                    break;
                case attackType.Circle:
                    {
                        //NPC.velocity += player.Center + offset - new Vector2(NPC.width / 2, NPC.height / 2);
                        Vector2 targetPos = player.Center + offset;
                        Vector2 toTarget = targetPos - NPC.Center;
                        NPC.velocity = Vector2.Lerp(NPC.velocity, toTarget * 0.1f, 0.6f);

                        if (!EternityIsActive())
                        {
                            if (Main.GameUpdateCount % 20 == 0)
                            {
                                SoundEngine.PlaySound(Attack);

                                float radius = 700f; // distance from player for spawning
                                circleradius = 700f;

                                if (!flag1)
                                {
                                    ToothCenter = player.Center;
                                    flag1 = true;
                                }
                                for (int i = 0; i < ToothCount; i++)
                                {
                                    float angle = MathHelper.TwoPi * i / ToothCount;
                                    Vector2 spawnPos = ToothCenter + radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                                    Vector2 velocity = (ToothCenter - spawnPos).SafeNormalize(Vector2.UnitY) * 8f;

                                    Projectile Tooth = Projectile.NewProjectileDirect(
                                        NPC.GetSource_FromAI(),
                                        spawnPos,
                                        velocity,
                                        ModContent.ProjectileType<Tooth>(),
                                        30,
                                        0f
                                    );
                                    Tooth.tileCollide = false;

                                    if (Tooth.Center.Distance(ToothCenter) < 20)
                                    {
                                        Tooth.Kill();
                                    }
                                }

                                ToothCount--;
                            }

                            if (ToothCount < 2)
                                {
                                    CurrentAttack = attackType.SummonCrimsonMinions;
                                    circleradius = 880f;
                                    ResetStats();
                                }
                        }
                        if (EternityIsActive())
                        {
                            if (Main.GameUpdateCount % 300 == 0)
                            {
                                SoundEngine.PlaySound(Attack);
                                for (int i = 0; i < 5; i++)
                                {
                                    float angle = MathHelper.TwoPi * i / 5;
                                    Vector2 spawnPos = player.Center + 900 * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                                    Vector2 velocity = (player.Center - spawnPos).SafeNormalize(Vector2.UnitY) * 10f;

                                    Projectile Lance = Projectile.NewProjectileDirect(
                                        NPC.GetSource_FromAI(),
                                        spawnPos,
                                        velocity,
                                        ModContent.ProjectileType<LightLance>(),
                                        18,
                                        0f
                                    );
                                    Lance.timeLeft = 80;
                                }
                                CircleLanceCount++;
                            }
                            if (Main.GameUpdateCount % 60 == 0)
                            {
                                //Opus.RadialProjectileRandomDir(ModContent.ProjectileType<TenebrisStarHostile>(), 3, NPC.Center, 8, 5, 2);
                            }
                        }
                        if (CircleLanceCount >= 6 && NPC.life < NPC.lifeMax * 0.4f)
                        {
                            circleradius = 880f;
                            CurrentAttack = attackType.SummonCrimsonMinions;
                            ResetStats();
                        }
                        if (CircleLanceCount >= 6 && NPC.life > NPC.lifeMax * 0.4f)
                        {
                            circleradius = 880f;
                            NPC.velocity *= 0.3f;
                            CurrentAttack = attackType.IchorRam;
                            ResetStats();
                        }
                    }
                    break;
                case attackType.HeartMatrix:
                    {
                        if (EternityIsActive())
                        {
                            if (NPC.life > NPC.lifeMax * 0.4f)
                            {
                                CurrentAttack = attackType.IchorRam;
                            }
                            if (NPC.life <= NPC.lifeMax * 0.4f)
                            {
                                CurrentAttack = attackType.SummonAxes;
                            }
                        }
                        else
                        {
                            CurrentAttack = attackType.Circle;
                        }
                        ResetStats();
                        /*
                        if (EternityIsActive())
                        {
                            if (!HeartMatrixGetPositions)
                            {
                                SoundEngine.PlaySound(Teeth);
                                for (int f = 0; f < 6; f++)
                                {
                                    Vector2 HeartPosoffset = Main.rand.NextVector2Circular(1200f, 1200f);
                                    HeartPos = player.Center + HeartPosoffset;
                                    PRTLoader.NewParticle(PRTLoader.GetParticleID<CrimsonBloodRuneParticle>(), HeartPos, Vector2.Zero, Color.White, 3);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), HeartPos, Vector2.Zero, ModContent.ProjectileType<HeartNode>(), 20, 3);
                                }
                                HeartMatrixGetPositions = true;

                                if (HeartMatrixGetPositions == true && NPC.life > NPC.lifeMax * 0.4f)
                                {
                                    CurrentAttack = attackType.IchorRam;
                                }
                                if (HeartMatrixGetPositions == true && NPC.life <= NPC.lifeMax * 0.4f)
                                {
                                    CurrentAttack = attackType.SummonAxes;
                                }
                                ResetStats();
                            }
                        }
                        if (!EternityIsActive())
                        {
                            CurrentAttack = attackType.Circle;
                            ResetStats();
                        }
                        */
                        break;
                    }
                case attackType.OrganBurst:
                    {
                        float numberProjectiles = 5 + Main.rand.Next(3); // 3, 4, or 5 shots
                        float rotation = MathHelper.ToRadians(45);
                        Vector2 position = NPC.Center;
                        Vector2 velocity = NPC.velocity;

                        position += Vector2.Normalize(velocity) * 45f;

                        if (!EternityIsActive())
                        {
                            OrganBurstIntervalTimer++;
                            if (OrganBurstIntervalTimer == 20)
                            {
                                SoundEngine.PlaySound(Attack, NPC.Center);
                                for (int i = 0; i < numberProjectiles; i++)
                                {
                                    Vector2 perturbedSpeed = NPC.velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1)));
                                    Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), position, perturbedSpeed, ModContent.ProjectileType<OrganProjectile>(), 44, 2);
                                }

                                OrganBurstIntervalTimer = 0;
                                OrganBurstCount++;
                            }
                            if (OrganBurstCount >= 10 && NPC.life > NPC.lifeMax * 0.4f)
                            {
                                CurrentAttack = attackType.Circle;
                                ResetStats();
                            }
                            if (OrganBurstCount > 10 && NPC.life <= NPC.lifeMax * 0.4f)
                            {
                                CurrentAttack = attackType.SummonAxes;
                                ResetStats();
                            }
                        }
                        if (EternityIsActive())
                        {
                            if (Main.GameUpdateCount % 20 == 0)
                            {
                                SoundEngine.PlaySound(Attack, NPC.Center);
                                for (int i = 0; i < 2; i++)
                                {
                                    Vector2 perturbedSpeed = NPC.velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1)));
                                    Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), position, perturbedSpeed, ModContent.ProjectileType<SoulCrystalBomb>(), 44, 2);
                                }
                                OrganBurstCount++;
                            }
                            if (OrganBurstCount >= 10 && NPC.life > NPC.lifeMax * 0.4f)
                            {
                                CurrentAttack = attackType.Circle;
                                ResetStats();
                            }
                            if (OrganBurstCount > 10 && NPC.life <= NPC.lifeMax * 0.4f)
                            {
                                CurrentAttack = attackType.HeartMatrix;
                                ResetStats();
                            }
                        }
                    }
                    break;
                case attackType.SummonCrimsonMinions:
                    {
                        /*
                        SoundEngine.PlaySound(Roar);
                        for (int e = 0; e < 6; e++)
                        {
                            Vector2 MinionPosition = Main.rand.NextVector2FromRectangle(
                            new Rectangle(
                                (int)Main.LocalPlayer.Center.X - Main.screenWidth / 2,
                                (int)Main.LocalPlayer.Center.Y - Main.screenHeight / 2,
                                Main.screenWidth,
                                Main.screenHeight
                                )
                            );
                            NPC.NewNPC(Entity.GetSource_FromThis(), (int)MinionPosition.X, (int)MinionPosition.Y, MinionSpawnType[Main.rand.Next(MinionSpawnType.Length)], 0);
                        }
                        */
                        CurrentAttack = attackType.BloodShoot;
                        ResetStats();
                    }
                    break;
                case attackType.TeleDash:
                    {
                        if (EternityIsActive())
                        {
                            Rectangle Screen = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
                            DTUtils Utility = new DTUtils();
                            
                            if (!TeleDashGetTelePosition)
                            {
                                Vector2 Teleoffset = Main.rand.NextVector2Circular(1200f, 1200f);
                                TelePos = player.Center + Teleoffset;
                                PRTLoader.NewParticle(PRTLoader.GetParticleID<SmallShine>(), TelePos, Vector2.Zero, Color.White, 10);
                                TeleDashGetTelePosition = true;
                            }

                            if (TeleDashGetTelePosition)
                            {
                                Dust.NewDust(new Vector2(TelePos.X - 20, TelePos.Y - 20), 40, 40, DustID.Ichor, Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-2, 2), 0, default, 2f);
                            }

                            if (TeleDashWaitTime > 0 && TeleDashGetTelePosition)
                            {
                                TeleDashWaitTime--;
                            }

                            if (TeleDashGetTelePosition && TeleDashWaitTime <= 0)
                            {
                                SoundEngine.PlaySound(Attack);
                                TeleDashCount++;
                                NPC.Center = TelePos;
                                for (int n = 0; n < Main.maxNPCs; n++)
                                {
                                    if (Main.npc[n].active && Main.npc[n].realLife == NPC.whoAmI)
                                    {
                                        float segOffset = (n % 2 != 0) ? -MathHelper.PiOver2 : MathHelper.PiOver2;
                                        Main.npc[n].Center = TelePos;
                                    }
                                }
                                float DashDir = (player.Center - NPC.Center).ToRotation();
                                if (TeleDashCount < 9)
                                {
                                    NPC.velocity = DashDir.ToRotationVector2() * 85;
                                }
                                else
                                {
                                    NPC.velocity = DashDir.ToRotationVector2() * 25;
                                }
                                DashParticle();
                                Opus.RadialSpreadProjectile(ModContent.ProjectileType<OrganProjectile>(), Main.rand.Next(4, 14), TelePos, 5, 4, Main.rand.Next(4, 13), RandomOffset: true);
                                TeleDashWaitTime = 100;
                                TeleDashGetTelePosition = false;
                            }

                            if (TeleDashCount >= 9)
                            {
                                CurrentAttack = attackType.BloodShoot;
                                ResetStats();
                            }
                        }
                        else
                        {
                            CurrentAttack = attackType.BloodShoot;
                            ResetStats();
                        }
                        break;
                    }

                case attackType.SummonAxes:
                    {
                        if (NPC.life <= NPC.lifeMax * 0.4f && HasSummoned40PercentMinions == false)
                        {
                            SoundEngine.PlaySound(Roar, NPC.Center);

                            if (!EternityIsActive())
                            {
                                NPC.NewNPC(Entity.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<TheGreatFlayer>());
                            }
       

                            HasSummoned40PercentMinions = true;
                        }
                        else
                        {
                            CurrentAttack = attackType.TeleDash;
                            ResetStats();
                        }
                    }
                    break;
                case attackType.BloodShoot:
                    {

                        if (HasShotTeeth == false)
                        {
                            SoundEngine.PlaySound(Teeth);
                            float RotOffset = MathHelper.PiOver2;
                            // Helper method to spawn teeth from an origin point
                            void ShootTeeth(Vector2 origin, float Offset)
                            {
                                Vector2 velocity = NPC.velocity.RotatedBy(Offset);

                                if (!EternityIsActive())
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(
                                            NPC.GetSource_FromAI(),
                                            origin,
                                            velocity,
                                            ModContent.ProjectileType<Tooth>(),
                                            40,
                                            1
                                        );
                                    }
                                }
                                if (EternityIsActive())
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(
                                            NPC.GetSource_FromAI(),
                                            origin,
                                            velocity,
                                            ModContent.ProjectileType<SoulSpark>(),
                                            40,
                                            1,
                                            ai2: 2
                                        );
                                    }
                                }
                            }

                            // Shoot from the head
                            ShootTeeth(NPC.Center, RotOffset);

                            // Shoot from all body segments that belong to this NPC
                            for (int n = 0; n < Main.maxNPCs; n++)
                            {
                                if (Main.npc[n].active && Main.npc[n].realLife == NPC.whoAmI)
                                {
                                    float segOffset = (n % 2 != 0) ? -MathHelper.PiOver2 : MathHelper.PiOver2;
                                    ShootTeeth(Main.npc[n].Center, segOffset);
                                }
                            }

                            HasShotTeeth = true;
                        }

                        if (HasShotTeeth == true)
                        {
                            CurrentAttack = attackType.FleshBombShoot;
                            ResetStats();
                        }
                    }
                    break;

                case attackType.FleshBombShoot:
                    {
                        if (BombSpawnTimer == 0 && BombSpawnCount == 0)
                        {
                            DesperationOrbitCenter = player.Center;
                        }
                        BombSpawnTimer++;
                        if (!EternityIsActive())
                        {
                            if (Main.GameUpdateCount % 40 == 0)
                            {
                                Projectile FleshBomb = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CrystalBomb>(), 20, 1);
                                BombSpawnCount += 1;
                            }
                        }
                        if (EternityIsActive())
                        {
                            if (Main.GameUpdateCount % 40 == 0)
                            {
                                SoundEngine.PlaySound(Attack, NPC.Center);
                                int numProjectiles = Main.rand.Next(2, 5);
                                float rotationStep = MathHelper.TwoPi / numProjectiles;

                                for (int i = 0; i < numProjectiles; i++)
                                {
                                    Vector2 velocity = new Vector2(6f, 0f).RotatedBy(rotationStep * i);
                                    Projectile.NewProjectile(
                                        Entity.GetSource_FromThis(),
                                        NPC.Center,
                                        velocity,
                                        ModContent.ProjectileType<CrystalBomb>(),
                                        50,
                                        4
                                    );
                                }
                                BombSpawnCount += 1;
                            }
                        }
                        if (BombSpawnCount >= 8)
                            {
                                CurrentAttack = attackType.Follow;
                                ResetStats();
                            }
                    }
                    break;

                case attackType.Desperation:
                    {
                        DesperationTimer++;
                        PitchVal += 0.0025f;
                        //DesperationVingetteScale--;
                        DesperationVingetteAlpha = (byte)MathHelper.Clamp(
                            255f * (DesperationTimer / 1200f),
                            0f,
                            255f
                        );
                        MathHelper.Clamp(DesperationVingetteScale, 5, 10);
                        if (cfg.EnableDebugMessages)
                        {
                            Mod.Logger.Debug($"{DesperationTimer}");
                        }


                        int shake = 8;
                        NPC.dontTakeDamage = true;
                        NPC.immortal = true;
                        NPC.aiStyle = -1;
                        NPC.rotation = NPC.velocity.ToRotation();
                        circlerotspeed = 0.15f;
                        DesperationOrbitCenter = Vector2.Lerp(DesperationOrbitCenter, player.Center, 0.01f);
                        Vector2 targetPos = DesperationOrbitCenter + offset;
                        Vector2 toTarget = targetPos - NPC.Center;
                        NPC.velocity = Vector2.Lerp(NPC.velocity, toTarget * 0.1f, 0.6f);
                        if (SoundFlag1 == false)
                        {
                            Main.NewText("The Wyvern is channeling its soul energy!", ColorLib.Soul);
                            SoundEngine.PlaySound(Kill2);
                            SoundFlag1 = true;
                        }

                        if (!SoundEngine.TryGetActiveSound(DesperationLoopSlot, out var activeSound)) {
                            var tracker = new NPCAudioTracker(NPC);
                            DesperationLoopSlot = SoundEngine.PlaySound(Loop, DesperationOrbitCenter, soundInstance => {
                                soundInstance.Position = DesperationOrbitCenter;
                                soundInstance.Pitch = PitchVal;
                                return tracker.IsActiveAndInGame();
                            });
                        }
                        else
                        {
                            activeSound.Position = DesperationOrbitCenter;
                            activeSound.Pitch = PitchVal;
                        }

                        player.GetModPlayer<ScreenshakePlayer>().screenshakeTimer = 40;
                        player.GetModPlayer<ScreenshakePlayer>().screenshakeMagnitude = shake;
                        if (player.Distance(DesperationOrbitCenter) > 800 || player.Distance(DesperationOrbitCenter) < 100)
                        {
                            player.AddBuff(ModContent.BuffType<SoulInferno>(), 600);
                        }
                        
                        circleradius--;
                        //Projectile FinalRain = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), NPC.Center, new Vector2(Main.rand.NextFloat(-2, 2), 15), ProjectileID.GoldenShowerHostile, 25, 1);

                        if (DesperationTimer % 10 == 0)
                        {
                            shake += 2;
                            for (int i = 0; i < 6; i++)
                            {
                                var angle = IchorSpiralRotationOffset + (i * MathHelper.TwoPi / 6f);
                                var launchVelocity = new Vector2(10, 0).RotatedBy(angle);
                                Projectile.NewProjectile(Entity.GetSource_FromThis(), DesperationOrbitCenter, launchVelocity, ModContent.ProjectileType<SoulCrystal>(), 25, 4);
                            }
                        }
                        IchorSpiralRotationOffset += 0.45f;

                        if (DesperationTimer >= 1200)
                        {
                            if (!DownedBossSystem.downedWyvernCorpseBoss)
                            {
                                Item.NewItem(Item.GetSource_None(), DesperationOrbitCenter, ModContent.ItemType<WyvernSoul>(), 1, true, 0, false, false);
                            }
                            NPC.dontTakeDamage = false;
                            NPC.life = 0;
                            NPC.HitEffect();
                            NPC.checkDead();
                            NPC.active = false;
                        }
                    }
                    break;
                case attackType.Enraged:
                    {
                        NPC.dontTakeDamage = true;
                        NPC.damage = 300;
                        break;
                    }
            }


            if (CurrentAttack != attackType.Desperation)
            {
                NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;
            }
        }



        public void ResetStats()
        {
            FollowTime = 0;
            IsDashing = false;
            DashCount = 0;
            DashTime = 80;
            DashOverrideTimer = 300;
            SpitTime = 0;
            CircleTime = 0;
            circleradius = 1500f;
            OrganBurstIntervalTimer = 0;
            OrganBurstCount = 0;
            BloodTimer = 0;
            MinionSpawnTimer = 0;
            MinionSpawnCount = 0;
            BombSpawnTimer = 0;
            BombSpawnCount = 0;
            CircleLanceCount = 0;
            TeleDashCount = 0;
            ToothCount = 10;
            HeartMatrixGetPositions = false;
            HasShotTeeth = false;
            flag1 = false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            base.PostDraw(spriteBatch, screenPos, drawColor);
            if (CurrentAttack == attackType.Desperation)
            {
                DrawCrystalCore(spriteBatch, DesperationOrbitCenter);
                DrawVingette(spriteBatch, DesperationOrbitCenter, 1.7f);
            }
        }

        public void DrawVingette(SpriteBatch spriteBatch, Vector2 Center, float Scale)
        {
            Main.spriteBatch.Draw(
                    DTAssetLib.Vingette.Value,
                    Center - Main.screenPosition,
                    null,
                    DTColorUtils.WithAlpha(Color.Black, DesperationVingetteAlpha),
                    TextureRotationOffset,
                    new Vector2(DTAssetLib.Vingette.Value.Width / 2f, DTAssetLib.Vingette.Value.Height / 2f),
                    Scale,
                    SpriteEffects.None,
                    1f
                );
        }
        public void DrawCrystalCore(SpriteBatch spriteBatch, Vector2 Center)
        {
            DTUtils Utility = new DTUtils();
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            Main.spriteBatch.Draw(
                DTAssetLib.Cyclone(2).Value,
                Center - Main.screenPosition,
                null,
                ColorLib.Soul,
                TextureRotationOffset,
                new Vector2(DTAssetLib.Cyclone(2).Value.Width / 2f, DTAssetLib.Cyclone(2).Value.Height / 2f),
                0.5f,
                SpriteEffects.None,
                1f
            );

            Main.spriteBatch.Draw(
                DTAssetLib.FeatheredCircle.Value,
                Center - Main.screenPosition,
                null,
                Color.White,
                0f,
                new Vector2(DTAssetLib.FeatheredCircle.Value.Width / 2f, DTAssetLib.FeatheredCircle.Value.Height / 2f),
                1f,
                SpriteEffects.None,
                1f
            );


            Main.spriteBatch.Draw(
                DTAssetLib.Cyclone(2).Value,
                Center - Main.screenPosition,
                null,
                ColorLib.Soul,
                TextureRotationOffset,
                new Vector2(DTAssetLib.Cyclone(2).Value.Width / 2f, DTAssetLib.Cyclone(2).Value.Height / 2f),
                4.7f,
                SpriteEffects.None,
                1f
            );



            Opus.ReturnToDefaultDrawing(spriteBatch);
        }

        int TGscroll = 0;

        public void DrawTelegraph(Vector2 start, Vector2 end, Texture2D texture)
        {
            TGscroll--;
            Vector2 direction = end - start;

            DTUtils.instance.ScrollingTextureSpine(new Line(start, end), DTAssetLib.Streak(13), ColorLib.IchorCrystalGradient, Main.spriteBatch, BlendState.Additive, TGscroll, 1f);
        }


        public void ImportantMathematics()
        {
            Player player = Main.player[NPC.target];
            int num107 = (int)(NPC.position.X / 16f) - 1;
            int num108 = (int)((NPC.position.X + NPC.width) / 16f) + 2;
            int num109 = (int)(NPC.position.Y / 16f) - 1;
            int num110 = (int)((NPC.position.Y + NPC.height) / 16f) + 2;

            if (num107 < 0) num107 = 0;
            if (num108 > Main.maxTilesX) num108 = Main.maxTilesX;
            if (num109 < 0) num109 = 0;
            if (num110 > Main.maxTilesY) num110 = Main.maxTilesY;
            if (NPC.velocity.X < 0f) NPC.spriteDirection = 1;
            if (NPC.velocity.X > 0f) NPC.spriteDirection = -1;

            float num115 = 16f;
            float num116 = 0.4f;

            Vector2 vector14 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
            float num118 = Main.rand.Next(-500, 501) + player.position.X + (player.width / 2);
            float num119 = Main.rand.Next(-500, 501) + player.position.Y + (player.height / 2);
            num118 = ((int)(num118 / 16f) * 16);
            num119 = ((int)(num119 / 16f) * 16);
            vector14.X = ((int)(vector14.X / 16f) * 16);
            vector14.Y = ((int)(vector14.Y / 16f) * 16);
            num118 -= vector14.X;
            num119 -= vector14.Y;
            float num120 = (float)Math.Sqrt((num118 * num118 + num119 * num119));

            float num123 = Math.Abs(num118);
            float num124 = Math.Abs(num119);
            float num125 = num115 / num120;
            num118 *= num125;
            num119 *= num125;

            bool flag14 = false;
            if (((NPC.velocity.X > 0f && num118 < 0f) || (NPC.velocity.X < 0f && num118 > 0f) || (NPC.velocity.Y > 0f && num119 < 0f) || (NPC.velocity.Y < 0f && num119 > 0f)) && Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) > num116 / 2f && num120 < 300f)
            {
                flag14 = true;
                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < num115) NPC.velocity *= 1.1f;
            }
            if (NPC.position.Y > player.position.Y || (player.position.Y / 16f) > Main.worldSurface || player.dead)
            {
                flag14 = true;
                if (Math.Abs(NPC.velocity.X) < num115 / 2f)
                {
                    if (NPC.velocity.X == 0f) NPC.velocity.X = NPC.velocity.X - NPC.direction;
                    NPC.velocity.X = NPC.velocity.X * 1.1f;
                }
                else
                {
                    if (NPC.velocity.Y > -num115) NPC.velocity.Y = NPC.velocity.Y - num116;
                }
            }
            if (!flag14)
            {
                if ((NPC.velocity.X > 0f && num118 > 0f) || (NPC.velocity.X < 0f && num118 < 0f) || (NPC.velocity.Y > 0f && num119 > 0f) || (NPC.velocity.Y < 0f && num119 < 0f))
                {
                    if (NPC.velocity.X < num118) NPC.velocity.X = NPC.velocity.X + num116;
                    else
                    {
                        if (NPC.velocity.X > num118) NPC.velocity.X = NPC.velocity.X - num116;
                    }
                    if (NPC.velocity.Y < num119) NPC.velocity.Y = NPC.velocity.Y + num116;
                    else
                    {
                        if (NPC.velocity.Y > num119) NPC.velocity.Y = NPC.velocity.Y - num116;
                    }
                    if (Math.Abs(num119) < num115 * 0.2 && ((NPC.velocity.X > 0f && num118 < 0f) || (NPC.velocity.X < 0f && num118 > 0f)))
                    {
                        if (NPC.velocity.Y > 0f) NPC.velocity.Y = NPC.velocity.Y + num116 * 2f;
                        else NPC.velocity.Y = NPC.velocity.Y - num116 * 2f;
                    }
                    if (Math.Abs(num118) < num115 * 0.2 && ((NPC.velocity.Y > 0f && num119 < 0f) || (NPC.velocity.Y < 0f && num119 > 0f)))
                    {
                        if (NPC.velocity.X > 0f) NPC.velocity.X = NPC.velocity.X + num116 * 2f;
                        else NPC.velocity.X = NPC.velocity.X - num116 * 2f;
                    }
                }
                else
                {
                    if (num123 > num124)
                    {
                        if (NPC.velocity.X < num118) NPC.velocity.X = NPC.velocity.X + num116 * 1.1f;
                        else
                        {
                            if (NPC.velocity.X > num118) NPC.velocity.X = NPC.velocity.X - num116 * 1.1f;
                        }
                        if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < num115 * 0.5)
                        {
                            if (NPC.velocity.Y > 0f) NPC.velocity.Y = NPC.velocity.Y + num116;
                            else NPC.velocity.Y = NPC.velocity.Y - num116;
                        }
                    }
                    else
                    {
                        if (NPC.velocity.Y < num119) NPC.velocity.Y = NPC.velocity.Y + num116 * 1.1f;
                        else
                        {
                            if (NPC.velocity.Y > num119) NPC.velocity.Y = NPC.velocity.Y - num116 * 1.1f;
                        }
                        if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < num115 * 0.5)
                        {
                            if (NPC.velocity.X > 0f) NPC.velocity.X = NPC.velocity.X + num116;
                            else NPC.velocity.X = NPC.velocity.X - num116;
                        }
                    }
                }
            }
        }

        public static void DrawHealingShadow(NPC npc, Vector2 Center, Vector2 offset, Color color, float rotationOffset = 0f)
        {
            Texture2D value = TextureAssets.Npc[npc.type].Value;
            Vector2 origin = value.Size() / 2f;
            SpriteEffects effects = SpriteEffects.None;
            if (npc.spriteDirection == 1) 
            {
                effects = SpriteEffects.FlipHorizontally;
            }
            Main.EntitySpriteDraw(value, Center + offset.RotatedBy(rotationOffset) - Main.screenPosition, new Rectangle?(npc.frame), color * 0.5f, npc.rotation, value.Size() / 2f, npc.scale, effects);
        }

        public bool flag = false;

        public Asset<Texture2D> texture;
        public Asset<Texture2D> Glowtexture;
        public void SetTex()
        {
            if (!flag)
            {
                if (DestroyerTestMod.MasochistIsActive)
                {
                    texture = NPC.GetMasoTexture("DestroyerTest/Content/Entities/MasoMode", "WyvernCorpseHead");
                    Glowtexture = NPC.GetMasoTexture("DestroyerTest/Content/Entities/MasoMode", "WyvernCorpseHead");
                }
                else
                {
                    texture = TextureAssets.Npc[Type];
                    Glowtexture = ModContent.Request<Texture2D>($"{Texture}_Glow", AssetRequestMode.AsyncLoad);
                }
                flag = true;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SetTex();

            Vector2 origin = texture.Size() / 2f;
            Vector2 drawPos = new Vector2(NPC.position.X - Main.screenPosition.X + (NPC.width / 2) - texture.Value.Width * NPC.scale / 2f + origin.X * NPC.scale, NPC.position.Y - Main.screenPosition.Y + NPC.height - texture.Value.Height * NPC.scale + 4f + origin.Y * NPC.scale + 56f);

            if (CurrentAttack == attackType.TeleDash)
            {
                DrawDashTelegraph(TelePos, Outer, DTAssetLib.ArrowTelegraphCont.Value);
                DrawTelePoint(spriteBatch, TelePos);
            }
            if (anyNodesAlive)
            {
                //Opus.DrawNPCShadowsRotating(NPC, 6, ColorLib.Ichor);

                float rotationOffset = 0.3f * (float)NPC.direction;
                DrawHealingShadow(NPC, new Vector2(0f, 6), drawPos, ColorLib.Ichor, rotationOffset);
                DrawHealingShadow(NPC, new Vector2(0f, 0f - 6), drawPos, ColorLib.Ichor, rotationOffset);
                DrawHealingShadow(NPC, new Vector2(6, 0f), drawPos, ColorLib.Ichor, rotationOffset);
                DrawHealingShadow(NPC, new Vector2(0f - 6, 0f), drawPos, ColorLib.Ichor, rotationOffset);
            }

            
            SpriteEffects effects = SpriteEffects.None;
            if (NPC.spriteDirection == 1) effects = SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture.Value, drawPos, new Rectangle?(NPC.frame), drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);
            spriteBatch.Draw(Glowtexture.Value, drawPos, new Rectangle?(NPC.frame), Color.White, NPC.rotation, origin, NPC.scale, effects, 0f);
            return false;
        }

        int DashTGscroll = 0;
        public void DrawDashTelegraph(Vector2 start, Vector2 end, Texture2D texture)
        {
            DashTGscroll -= 10;
            // Compute direction and total length
            Vector2 direction = end - start;
   
            DTUtils.instance.ScrollingTextureSpine(new Line(start, end), DTAssetLib.ArrowTelegraphCont, ColorLib.IchorCrystalGradient, Main.spriteBatch, BlendState.Additive, DashTGscroll, 1f);
        }

        float tOffset = 0f;
        public void DrawTelePoint(SpriteBatch spriteBatch, Vector2 Center)
        {
            tOffset += 0.1f;
            DTUtils Utility = new DTUtils();
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            DTUtils.DrawCrystalCore(spriteBatch, Center, Color.White, ColorLib.Ichor, tOffset, 2f);

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }

        

        public void DashParticle()
        {
            Spark Spark1 = new Spark();

            Spark1.PrepareSpark(NPC.Center + new Vector2(NPC.width / 2, (NPC.height / 2) - NPC.height / 2).RotatedBy(NPC.rotation), new Vector2(10, 80).RotatedBy(NPC.rotation), 0f, ColorLib.Ichor, 1f, false, 40, SparkDrawMode.Additive);
            ParticleEngine.ShaderParticles.Add(Spark1);

            Spark Spark2 = new Spark();

            Spark2.PrepareSpark(NPC.Center + new Vector2(NPC.width / 2, (NPC.height / 2) - NPC.height / 2).RotatedBy(NPC.rotation), new Vector2(10, -80).RotatedBy(NPC.rotation), 0f, ColorLib.Ichor, 1f, false, 40, SparkDrawMode.Additive);
            ParticleEngine.ShaderParticles.Add(Spark2);

        }

        public void NodeSpawn()
        {
            float radius = 200;
            int projectileCount = 6;


            SoundEngine.PlaySound(NodeSpawnSound);

            for (int i = 0; i < projectileCount; i++)
            {
                // Get evenly spaced angle with rotation offset
                float angle = MathHelper.TwoPi * i / projectileCount;
                Vector2 spawnOffset = radius * angle.ToRotationVector2(); // position on the circle
                Vector2 spawnPosition = NPC.Center + spawnOffset;

                NPC.NewNPC(Entity.GetSource_FromThis(), (int)spawnPosition.X, (int)spawnPosition.Y, ModContent.NPCType<IchorNode>());

                PRTLoader.NewParticle(PRTLoader.GetParticleID<BloomRingSharp>(), NPC.Center, Vector2.Zero, ColorLib.Ichor, 0.001f, 1);

            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            DTOptimizationsConfig optcfg = ModContent.GetInstance<DTOptimizationsConfig>();
            if (!optcfg.DisableExcessDusts)
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDust(Main.rand.NextVector2FromRectangle(NPC.Hitbox), 20, 20, DustID.Blood, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1), 0, default, 2);
                }
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 4; i++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, Vector2.Zero, Main.rand.Next(61, 64), 1f);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {

            npcLoot.Add(ItemDropRule.Common(ItemID.SoulofFlight, 1, 5, 20));



        }

        public override void OnKill()
        {
            SunlightModification.Reset();
            //SoundEngine.StopTrackedSounds();
        }

        public override void OnSpawn(IEntitySource source)
        {
            DTOptimizationsConfig optcfg = ModContent.GetInstance<DTOptimizationsConfig>();
            if (EternityIsActive() && !optcfg.OptimizeGame)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<WyvernCorpseBackgroundProj>(), 0, 0f);
            }
        }


    }

    [AutoloadGlowmask]
    public class IchorNode : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.CanHitPastShimmer[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.TrailCacheLength[Type] = 20;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            // To hide from bestiary, override SetBestiary and leave it empty.
        }

        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 100;
            NPC.aiStyle = -1;
            NPC.damage = 25;
            NPC.defense = 50;
            NPC.lifeMax = 17000;
            NPC.HitSound = new SoundStyle("DestroyerTest/Assets/Audio/NodeHit");
            NPC.DeathSound = new SoundStyle("DestroyerTest/Assets/Audio/NodeExplode");
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.timeLeft = 150000;
            //NPC.boss = true;
            NPC.npcSlots = 12f;
            NPC.netID = ModContent.NPCType<IchorNode>();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => new bool?(false);

        public override void SetBestiary(Terraria.GameContent.Bestiary.BestiaryDatabase database, Terraria.GameContent.Bestiary.BestiaryEntry bestiaryEntry)
        {
            // Do not add any info elements to hide from bestiary
        }
        public override bool CheckActive()
        {
            return false;
        }

        public enum AIState
        {
            Idle,
            Slam,
            CrystalX,
            Pikes
        }

        public AIState currentState;
        public List<NPC> allNodes = new List<NPC>();
        public bool InPositionToSlam = false;
        public bool HasSlammed = false;
        public int ScreenIntervals;
        public int AwakenTimer = 1200;
        public Vector2 OrbitCenter;
        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];

            // Find the boss once per tick
            NPC bossNPC = Main.npc.FirstOrDefault(n =>
                n.active && n.type == ModContent.NPCType<WyvernCorpseHead>());

            if (bossNPC == null)
            {
                NPC.active = false; // despawn if parent is gone
                return;
            }

            // cache orbit center


            // rebuild node list fresh each tick
            allNodes.Clear();
            for (int i = 0; i < Main.maxNPCs; i++)
                if (Main.npc[i].active && Main.npc[i].type == Type)
                    allNodes.Add(Main.npc[i]);

            ScreenIntervals = allNodes.Count;

            // --- state machine ---
            switch (currentState)
            {
                case AIState.Idle:
                    OrbitCenter = bossNPC.Center;
                    Orbit(300f, 0.05f, OrbitCenter);
                    if (DestroyerTestMod.EternityIsActive)
                    {
                        if (--AwakenTimer <= 0)
                        {
                            if (Main.rand.NextBool(5))
                            {
                                AwakenTimer = 1200;
                                currentState = AIState.CrystalX;
                            }
                        }
                    }
                    break;

                case AIState.CrystalX:
                    OrbitCenter = player.Center;
                    Cross();
                    break;

                case AIState.Slam:
                    OrbitCenter = bossNPC.Center;
                    Slam();
                    break;
                case AIState.Pikes:
                    {
                        OrbitCenter = player.Center;
                        Pikes(300f, 0.05f, OrbitCenter);
                        break;
                    }
            }
        }


        public int CrossCount = 0;
        public void Cross()
        {
            // drift slowly toward the player instead of orbiting
            Player player = Main.player[NPC.target];
            Vector2 direction = player.Center - NPC.Center;
            if (direction != Vector2.Zero)
                direction.Normalize();

            // gentle glide speed
            NPC.velocity = direction * 2f;

            // fire crystals every 3 seconds
            if (Main.GameUpdateCount % 180 == 0)
            {
                Opus.RadialSpreadProjectile(
                    ModContent.ProjectileType<IchorNodeCrystal2>(),
                    4, NPC.Center, 20, 4, 8, offset: 0f);
                CrossCount++;
            }

            // after a few volleys, switch to Slam
            if (CrossCount > 4)
            {
                CrossCount = 0;
                if (Main.rand.NextBool(8))
                {
                    currentState = AIState.Slam;
                }
                else
                {
                    currentState = AIState.Pikes;
                }
                NPC.velocity = Vector2.Zero; // stop drifting
            }
        }

        public void Orbit(float radius, float speed, Vector2 center)
        {
            float angle = Main.GameUpdateCount * speed;

            // Sort nodes to get stable order across clients
            allNodes.Sort((a, b) => a.whoAmI.CompareTo(b.whoAmI));
            int index = allNodes.IndexOf(NPC);
            int total = Math.Max(allNodes.Count, 1);

            float spacing = MathHelper.TwoPi / total;
            float myAngle = angle + index * spacing;

            Vector2 offset = new Vector2(MathF.Cos(myAngle), MathF.Sin(myAngle)) * radius;
            NPC.Center = center + offset - new Vector2(NPC.width / 2, NPC.height / 2);
        }

        public void Slam()
        {

            Vector2 targetPos = Main.player[NPC.target].Center + new Vector2(0, -120f);

            // slide horizontally toward your random target
            NPC.Center = Vector2.Lerp(NPC.Center, targetPos, 0.1f);

            // once close, drop straight down
            if (!HasSlammed && Vector2.Distance(NPC.Center, targetPos) < 120f)
            {
                HasSlammed = true;
                NPC.velocity += new Vector2(0f, 30f);
            }

            // impact check
            if (HasSlammed)
            {

                int tileX = (int)(NPC.Center.X / 16f);
                int tileY = (int)((NPC.Center.Y + NPC.height / 2) / 16f);
                if (WorldGen.SolidTile(tileX, tileY))
                {
                    Opus.RadialSpreadProjectile(
                        ProjectileID.GoldenShowerHostile, 5,
                        NPC.Bottom, 15, 3, 10, RandomOffset: false);

                    SoundEngine.PlaySound(
                        new SoundStyle("DestroyerTest/Assets/Audio/StarHammerThrow"),
                        NPC.Center);

                    NPC.velocity = Vector2.Zero;
                    HasSlammed = false;
                    currentState = AIState.Idle;
                    AwakenTimer = 1200; // reset for next cycle
                }
                else
                {
                    Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y + NPC.height / 2), 2, 2, DustID.Ichor, 2f, -1.5f, 0, ColorLib.Ichor, 2f);
                    Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y + NPC.height / 2), 2, 2, DustID.Ichor, -2f, -1.5f, 0, ColorLib.Ichor, 2f);
                    PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), new Vector2(NPC.Center.X, NPC.Center.Y + NPC.height / 2), new Vector2(2, 1.5f), ColorLib.Ichor, 1.0f);
                    PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), new Vector2(NPC.Center.X, NPC.Center.Y + NPC.height / 2), new Vector2(-2, 1.5f), ColorLib.Ichor, 1.0f);
                    NPC.velocity += new Vector2(0f, 30f);
                }
            }
        }

        public int PikeCount = 0;
        public void Pikes(float radius, float speed, Vector2 center)
        {
            Player player = Main.player[NPC.target];
            Vector2 direction = player.Center - NPC.Center;
            if (direction != Vector2.Zero)
                direction.Normalize();

            // gentle glide speed
            NPC.velocity = direction * 6f;

            if (Main.GameUpdateCount % 180 == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Scholar/ShieldActivate", 3) with { PitchVariance = 0.4f });
                Opus.RadialProjectileRandomDir(ModContent.ProjectileType<NodeBossDistendedPike>(), Main.rand.Next(2, 5), NPC.Center, NPC.damage / 2, 7, 20);
                PikeCount++;
            }

            if (PikeCount >= 4)
            {
                currentState = AIState.Slam;
                PikeCount = 0;
            }
        }

         
    }

    public class WyvernCorpseBackgroundProj : ModProjectile
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
            Projectile.scale = 4;
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

            Color drawColor = Opus.Sine(Color.Black, ColorLib.Soul3);

            if (!optcfg.OptimizeGame)
            {
                Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

                float time = (float)Main.GameUpdateCount / 60f;

                // --- Layer 1 scroll parameters ---
                float scrollSpeedX1 = 200f;
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
                        spriteBatch.Draw(WindTex.Value, new Vector2(x, y), null, drawColor * 0.5f, 0f, Vector2.Zero, 1f * Projectile.scale, SpriteEffects.None, 0f);
                    }
                }

                float scrollSpeedX2 = 150f;
                float scrollSpeedY2 = -60f; // opposite direction for contrast

                float scrollOffsetX2 = (time * scrollSpeedX2) % WindTex.Value.Width * Projectile.scale;
                float scrollOffsetY2 = (time * scrollSpeedY2) % WindTex.Value.Height * Projectile.scale;

                Color drawColor2 = drawColor * 0.8f; // slightly dimmer to layer properly

                // --- Draw second layer ---
                for (float x = -scrollOffsetX2 + startX; x < endX; x += WindTex.Value.Width)
                {
                    for (float y = -scrollOffsetY2 + startY; y < endY; y += WindTex.Value.Height)
                    {
                        spriteBatch.Draw(WindTex.Value, new Vector2(x, y), null, drawColor2 * 0.5f, 0f, Vector2.Zero, 1f * Projectile.scale, SpriteEffects.None, 0f);
                    }
                }

                Opus.ReturnToDefaultDrawing(spriteBatch);
            }
            return false;
        }



    }
}