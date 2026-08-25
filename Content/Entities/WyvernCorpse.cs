
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.BossBar;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Boss;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Ichor;
using DestroyerTest.Content.Projectiles.Boss.VampireBoss;
using DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RogueItems;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.UI;
using FargowiltasSouls;
using GlowmaskHelper.Content;
 
using log4net.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.Cinematics;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Animations;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using UtfUnknown.Core.Models.SingleByte.Finnish;
using static System.Net.Mime.MediaTypeNames;
using static BreadLibrary.Core.SoftBodySim.SoftbodySim;

namespace DestroyerTest.Content.Entities
{
    [AutoloadBossHead]
    public class WyvernCorpseHead : ModNPC
    {
        public enum attackType
        {
            Follow,
            BloodBombs,
            Organs,
            CrystalBombMatrix,
            ChargeLaserOrb,
            MagicTeeth,
            OrganCircle,
            Nodes,
            Enraged
        }

        public bool HasTriggeredNodes = false;

        public SoundStyle Roar = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/CorpseRoar1") with { PitchVariance = 1.0f, MaxInstances = 0 };
        public SoundStyle Kill = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/Enrage") with { PitchVariance = 1.0f, Volume = 4 };
        public SoundStyle Teeth = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/ToothShoot") with { PitchVariance = 1.0f };
        public SoundStyle TeleportSetPosition = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/TeleportSetPosition") with { PitchVariance = 1.0f };
        public SoundStyle Attack = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/Attack", 10) with { PitchVariance = 1.0f, MaxInstances = 0 };
        public SoundStyle NodeSpawnSound = new SoundStyle("DestroyerTest/Infernum/Assets/Audio/WyvernCorpseIntroFinish") with { PitchVariance = 1f, MaxInstances = 0 };
        public SoundStyle Kill2 = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/DespRoar");
        public SoundStyle Desperation = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/Desperation");

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

            Main.npcFrameCount[Type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 36;

            NPC.aiStyle = NPCAIStyleID.Worm;

            NPC.damage = 70;
            NPC.defense = 65;
            NPC.lifeMax = 420000;

            if (DTUtils.CalamityBossRushActive())
            {
                NPC.lifeMax = 1000000;
                NPC.defense = 90;
            }

            NPC.noGravity = true;
            NPC.noTileCollide = true;


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

            HasShedBlisters = false;
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


        public void NoDamageEffects()
        {
            if (shouldBeInvisible)
            {
                //Fade out
                if (NPC.Opacity > 0)
                {
                    NPC.Opacity -= 0.05f;
                }

            }
            else
            {
                //Fade in
                if (NPC.Opacity < 1)
                {
                    NPC.Opacity += 0.05f;
                }
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1f;
            return new bool?();
        }

        int Frame = 0;
        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return;
            }

            float Progress = (float)NPC.life / (float)NPC.lifeMax;
            Frame = (int)MathHelper.Lerp(5, 0, Progress);


            NPC.frame.Y = Frame * frameHeight;
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
                    /*
                    texture = NPC.GetMasoTexture("DestroyerTest/Content/Entities/MasoMode", "WyvernCorpseHead");
                    Glowtexture = NPC.GetMasoTexture("DestroyerTest/Content/Entities/MasoMode", "WyvernCorpseHead");
                    */

                    texture = TextureAssets.Npc[Type];
                    Glowtexture = ModContent.Request<Texture2D>($"{Texture}_Glow", AssetRequestMode.AsyncLoad);
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
            if (NPC.IsABestiaryIconDummy)
            {
                return false;
            }

            SetTex();

            Vector2 origin = NPC.frame.Size() * 0.5f;

            Vector2 drawPos = new Vector2(NPC.position.X - Main.screenPosition.X + (NPC.width / 2) - texture.Value.Width * NPC.scale / 2f + origin.X * NPC.scale, NPC.position.Y - Main.screenPosition.Y + NPC.height - (texture.Value.Height / 6) * NPC.scale + 4f + origin.Y * NPC.scale + 56f);

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
            spriteBatch.Draw(texture.Value, drawPos, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, origin, NPC.scale, effects, 0f);
            spriteBatch.Draw(Glowtexture.Value, drawPos, NPC.frame, Color.White * NPC.Opacity, NPC.rotation, origin, NPC.scale, effects, 0f);
            return false;
        }


        public attackType CurrentAttack = attackType.Follow;

        public bool SpawnFlag = false;

        public bool anyNodesAlive;
        public int nodeCount;

        public bool invulnerableFromNodes => anyNodesAlive;
        public bool invulnerableFromAttack => CurrentAttack == attackType.CrystalBombMatrix;

        public bool shouldBeInvisible => invulnerableFromAttack;




        public bool SoundFlag1 = false;



        public int AITimer = 0;

        bool UsesRegularAI = true;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)CurrentAttack);
            writer.Write(anyNodesAlive);
            writer.Write7BitEncodedInt(AITimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            CurrentAttack = (attackType)reader.ReadInt32();
            anyNodesAlive = reader.ReadBoolean();
            AITimer = reader.Read7BitEncodedInt();
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }

        int NumCrimstoneDusts = 0;
        int NumSoulParticles = 0;
        int NumBoneDusts = 0;

        void ModifyHitDustAmounts()
        {
            float Progress = (float)NPC.life / (float)NPC.lifeMax;

            float FirstQuarterProgress = (float)NPC.life / (float)NPC.lifeMax / 4;
            float LastQuarterProgress = (float)NPC.life / (float)NPC.lifeMax * 0.75f;

            //Crimstone dusts stop appearing below 25% health.
            if (Progress > 0.25f)
            {
                NumCrimstoneDusts = (int)MathHelper.Lerp(3, 0, FirstQuarterProgress);
            }
            else
            {
                NumSoulParticles = (int)MathHelper.Lerp(8, 0, LastQuarterProgress);
            }

            //Bone dusts increase throughout the fight.
            NumBoneDusts = (int)MathHelper.Lerp(1, 3, Progress);

        }

        public float LifeProgress => (float)NPC.life / (float)NPC.lifeMax;
        public override void HitEffect(NPC.HitInfo hit)
        {
            float Progress = LifeProgress;

            if (Progress > 0.5f)
            {
                SoundEngine.PlaySound(SoundID.Tink with { Pitch = -0.6f, PitchVariance = 0.4f }, NPC.Center);
                if (!DTOptimizationsConfig.instance.DisableExcessDusts)
                {
                    for (int i = 0; i < NumCrimstoneDusts; i++)
                    {
                        Dust.NewDust(Main.rand.NextVector2FromRectangle(NPC.Hitbox), 20, 20, DustID.Crimstone, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1), 0, default, 2);
                    }
                }
            }
            if (Progress < 0.5f && Progress > 0.25f)
            {
                SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt with { Pitch = 0.6f, PitchVariance = 0.2f }, NPC.Center);
                if (!DTOptimizationsConfig.instance.DisableExcessDusts)
                {
                    for (int i = 0; i < NumBoneDusts; i++)
                    {
                        Dust.NewDust(Main.rand.NextVector2FromRectangle(NPC.Hitbox), 20, 20, DustID.Bone, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1), 0, default, 2);
                    }
                }
            }
            if (Progress < 0.25f)
            {
                SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot with { Pitch = 0.6f, PitchVariance = 0.2f }, NPC.Center);

                if (!DTOptimizationsConfig.instance.DisableExcessDusts)
                {

                    for (int i = 0; i < NumSoulParticles; i++)
                    {
                        PointGlowPreMultiplied SoulParticle = new();
                        SoulParticle.Initialize(Main.rand.NextVector2FromRectangle(NPC.Hitbox), new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)), ColorLib.Soul, 1f, 120);
                        ParticleEngine.Particles.Add(SoulParticle);
                    }
                }
            }


            if (Progress <= 0.001f)
            {
                SoundEngine.PlaySound(DTAssetLib.Impacts.DreamHit, NPC.Center);


                for (int i = 0; i < 10; i++)
                {
                    PointGlowPreMultiplied SoulParticle = new();
                    SoulParticle.Initialize(Main.rand.NextVector2FromRectangle(NPC.Hitbox), new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)), ColorLib.Soul, 1f, 120);
                    ParticleEngine.Particles.Add(SoulParticle);
                }
            }
        }


        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (anyNodesAlive)
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
            if (anyNodesAlive)
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
            if (anyNodesAlive)
                return false;

            return base.CanBeHitByItem(player, item);
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (anyNodesAlive)
            {
                NPC.immortal = true;
                modifiers.FinalDamage *= 0f;
            }
        }

        public bool ShouldHit = false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return ShouldHit;
        }

        public override bool? CanCollideWithPlayerMeleeAttack(Player player, Item item, Rectangle meleeAttackHitbox)
        {
            return base.CanCollideWithPlayerMeleeAttack(player, item, meleeAttackHitbox);
        }


        public Vector2 Center;
        public int DeathInterval = 10;

        public List<NPC> BodySegments = new List<NPC>();
        public int bodySegmentHolder = 0;

        public List<NPC> iNodes;
        public int[] NodeShakeTimers;
        public float[] NodeShakeMaxX;
        public float[] NodeShakeMaxY;

        bool HasShedBlisters = false;
        bool HasSpawnedOrb = false;

        int PostCrystalWaitTime = 0;
        int ToothRoundCount = 0;
        float OrganSpinRotOff = 0f;
        bool OrganSpinRecordPlayer = false;

        Vector2 OrganSpinCenter;
        int OrganSpinSpawnCount = 0;

        Player player => Main.player[NPC.target];
        public override void AI()
        {
            AITimer++;

            NPC.TargetClosest();

            if (Main.netMode != NetmodeID.MultiplayerClient && SpawnFlag == false)
            {
                if (bodySegmentHolder == 0)
                {
                    NPC.ai[2] = NPC.whoAmI;
                    NPC.realLife = NPC.whoAmI;

                    int Me = NPC.whoAmI;
                    for (int i = 0; i < 60; i++)
                    {
                        int WyvBodyInt = ModContent.NPCType<WyvernCorpseBody1>();
                        if (i == 4 || i == 16 || i == 32 || i == 48)
                            WyvBodyInt = ModContent.NPCType<WyvernCorpseLegs>();
                        else if (i == 57)
                            WyvBodyInt = ModContent.NPCType<WyvernCorpseBody2>();
                        else if (i == 58)
                            WyvBodyInt = ModContent.NPCType<WyvernCorpseBody3>();
                        else if (i == 59)
                            WyvBodyInt = ModContent.NPCType<WyvernCorpseTail>();

                        int BodySegment = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.position.X + NPC.width / 2), (int)(NPC.position.Y + NPC.height), WyvBodyInt, NPC.whoAmI);

                        BodySegments.Add(Main.npc[BodySegment]);

                        Main.npc[BodySegment].ai[2] = NPC.whoAmI;
                        Main.npc[BodySegment].realLife = NPC.whoAmI;
                        Main.npc[BodySegment].ai[1] = Me;
                        Main.npc[BodySegment].ai[3] = i + 1;

                        // THIS is missing from your second implementation.
                        Main.npc[Me].ai[0] = BodySegment;

                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, BodySegment);

                        Me = BodySegment;
                    }
                    NPC.netUpdate = true;
                    SpawnFlag = true;
                }
            }

            NPC.dontTakeDamage = invulnerableFromNodes || invulnerableFromAttack;

            Vector2 ToPlayer = NPC.Center - player.Center;

            Vector2 ToPlayerInverse = player.Center - NPC.Center;

            if (Frame == 1 && !HasShedBlisters)
            {
                for (int i = 0; i < BodySegments.Count(); i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), BodySegments[i].Center, Main.rand.NextVector2Circular(2, 2), ModContent.ProjectileType<IchorBlister>(), 50, 4, ai0: player.whoAmI);
                }
                HasShedBlisters = true;
            }

            Vector2 RandNearPlayer = player.Center + new Vector2(Main.rand.NextFloat(-200f, 200f), Main.rand.NextFloat(-200f, 200f));

            ModifyHitDustAmounts();
            if (NPC.target < 0 || NPC.target == 250 || player.dead) NPC.TargetClosest(true);
            if (player.dead && NPC.timeLeft > 300) NPC.timeLeft = 300;

            iNodes = Main.npc.Where(n => n.active && n.type == ModContent.NPCType<IchorNode>()).ToList();

            foreach (NPC node in iNodes)
            {
                if (node != null && node.active)
                {
                    anyNodesAlive = true;
                }
            }



            nodeCount = iNodes.Count;

            if (nodeCount <= 0)
            {
                anyNodesAlive = false;
            }

            float radius = Opus.Sine(200f, 240f);

            if (anyNodesAlive)
            {
                Vector2[] NodePositions = Opus.GetEquidistantOrbitVectors(nodeCount, NPC.Center, 0.02f, radius);

                for (int i = 0; i < nodeCount; i++)
                {
                    Vector2 IdealVel = iNodes[i].Center - NodePositions[i];
                    float MSpeed = CurrentAttack == attackType.OrganCircle ? 200 : 48;
                    iNodes[i].SmoothMoveToPoint(NodePositions[i], MSpeed);
                }

                NPC.dontTakeDamage = true;
            }

            if (player.dead)
            {
                DeathInterval--;
                if (DeathInterval <= 0)
                {
                    NPC.active = false;
                }
            }

            NoDamageEffects();

            if (NPC.life <= NPC.lifeMax * 0.75f)
            {
                if (HasTriggeredNodes == false)
                {
                    CurrentAttack = attackType.Nodes;

                    HasTriggeredNodes = true;

                }
            }



            if (ModContent.GetInstance<DTConfig>().EnableDebugMessages && Main.GameUpdateCount % 120 == 0)
            {
                Mod.Logger.Info($"Current State: {CurrentAttack}");
            }



            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/UnfinishedBoss");

            ManageShakeTimers();

            ImportantMathematics();


            switch (CurrentAttack)
            {
                case attackType.Follow:
                    {
                        NumBombs = 0;
                        PostCrystalWaitTime = 0;
                        HasSpawnedOrb = false;

                        if (AITimer >= 240)
                        {
                            CurrentAttack = attackType.BloodBombs;
                        }
                        break;
                    }

                case attackType.BloodBombs:
                    {
                        AI_BloodBombs();

                        if (AITimer >= 600)
                        {
                            CurrentAttack = attackType.Organs;
                        }
                        break;
                    }
                case attackType.Organs:
                    {
                        if (AI_Organs())
                        {
                            CurrentAttack = attackType.CrystalBombMatrix;
                        }
                        break;
                    }
                case attackType.CrystalBombMatrix:
                    {
                        AI_CrystalBombMatrix();

                        Vector2 Above = player.Center + new Vector2(0, -400);

                        NPC.SmoothMoveToPoint(Above, 40f);

                        NPC.dontTakeDamage = true;
                        ShouldHit = false;

                        for (int i = 0; i < iNodes.Count; i++)
                        {
                            iNodes[i].dontTakeDamage = true;
                        }

                        if (NumBombs >= 5)
                        {
                            if (PostCrystalWaitTime < 120)
                            {
                                PostCrystalWaitTime++;
                            }
                            else
                            {
                                CurrentAttack = attackType.ChargeLaserOrb;
                                NPC.dontTakeDamage = false;
                                ShouldHit = true;
                                for (int i = 0; i < iNodes.Count; i++)
                                {
                                    iNodes[i].dontTakeDamage = false;
                                }
                                NPC.damage = 70;
                            }
                        }
                    }
                    break;
                case attackType.ChargeLaserOrb:
                    {
                        bool Active = Main.npc.Any(n => n.active && n.type == ModContent.NPCType<SoulOrb>());

                        if (Active)
                        {

                        }
                        else
                        {
                            if (!HasSpawnedOrb)
                            {
                                SoundEngine.PlaySound(Kill);
                                NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<SoulOrb>(), ai0: player.whoAmI);
                                HasSpawnedOrb = true;
                            }
                            else
                            {
                                CurrentAttack = attackType.MagicTeeth;
                            }
                        }
                    }
                    break;
                case attackType.MagicTeeth:
                    {
                        AI_ToothRounds();

                        if (ToothRoundCount >= 4)
                        {
                            CurrentAttack = attackType.OrganCircle;
                            ToothRoundCount = 0;

                        }
                        break;
                    }
                case attackType.OrganCircle:
                    {
                        OrganSpinRotOff += 0.1f;

                        if (!OrganSpinRecordPlayer)
                        {
                            OrganSpinCenter = player.MountedCenter;
                            OrganSpinRecordPlayer = true;
                        }
                        else
                        {
                            if (OrganSpinSpawnCount < 20)
                            {
                                player.wingTime = player.wingTimeMax;

                                float orbitradius = anyNodesAlive ? 1700 : 1000;
                                Vector2 targetPoint = OrganSpinCenter + new Vector2(orbitradius, 0).RotatedBy(OrganSpinRotOff);

                                NPC.SmoothMoveToPoint(targetPoint, 160);

                                if (player.Distance(OrganSpinCenter) > 990)
                                {
                                    player.Center = OrganSpinCenter + new Vector2(950, 0).RotatedBy(OrganSpinCenter.DirectionTo(player.Center).ToRotation());
                                }

                                if (AITimer % 120 == 0)
                                {
                                    SoundEngine.PlaySound(Attack);
                                    for (int i = 0; i < 2; i++)
                                    {
                                        Vector2 sp = BodySegments[Main.rand.Next(BodySegments.Count)].Center;
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), sp, sp.DirectionTo(OrganSpinCenter) * 8f, ModContent.ProjectileType<OrganProjectile>(), 50, 6, ai0: player.whoAmI);
                                    }
                                    OrganSpinSpawnCount++;
                                }
                            }
                            else
                            {
                                CurrentAttack = attackType.Follow;
                                NPC.velocity *= 0.05f;
                                OrganSpinRecordPlayer = false;
                                OrganSpinSpawnCount = 0;
                                AITimer = 0;
                            }
                        }
                        break;
                    }
                case attackType.Nodes:

                    NPC.aiStyle = -1;
                    NodeSpawn();

                    CurrentAttack = attackType.Follow;

                    break;
                case attackType.Enraged:
                    {
                        NPC.dontTakeDamage = true;
                        NPC.damage = 300;
                        break;
                    }
            }



            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

        }

        void ManageShakeTimers()
        {
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
                if (WyvernCorpseHealthBar.NodeLockShake == null || WyvernCorpseHealthBar.NodeLockShake.Length != nodeCount)
                {
                    WyvernCorpseHealthBar.NodeLockShake = new Vector2[nodeCount];
                }

                // Update each node's timer and max offsets, then drive the UI shake positions
                for (int i = 0; i < nodeCount; i++)
                {
                    if (NodeShakeTimers[i] < 120)
                        NodeShakeTimers[i]++;

                    NodeShakeMaxX[i] = MathHelper.Lerp(10f, 0f, (float)NodeShakeTimers[i] / 120f);
                    NodeShakeMaxY[i] = NodeShakeMaxX[i];

                    if (iNodes[i].active && iNodes[i].life > 0)
                    {
                        // random offset in both directions
                        WyvernCorpseHealthBar.NodeLockShake[i] = new Vector2(
                            Main.rand.NextFloat(-NodeShakeMaxX[i], NodeShakeMaxX[i]),
                            Main.rand.NextFloat(-NodeShakeMaxY[i], NodeShakeMaxY[i])
                        );
                    }
                    else
                    {
                        WyvernCorpseHealthBar.NodeLockShake[i] = Vector2.Zero;
                    }
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            base.PostDraw(spriteBatch, screenPos, drawColor);

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


        public void AI_BloodBombs()
        {
            int Damage = (int)MathHelper.Lerp(5, 100, LifeProgress);
            int Interval = (int)MathHelper.Lerp(12, 4, LifeProgress);
            if (AITimer % Interval == 0)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Main.rand.NextVector2Circular(10, 10), ModContent.ProjectileType<FleshBomb>(), Damage, 4);
            }
        }

        private int organSegmentIndex = 0;

        public bool AI_Organs()
        {
            int damage = (int)MathHelper.Lerp(5, 100, LifeProgress);
            Player player = Main.player[NPC.target];

            Vector2 toPlayer = player.Center - BodySegments[organSegmentIndex].Center;
            toPlayer.Normalize();

            if (AITimer % 6 == 0)
            {
                if (organSegmentIndex >= BodySegments.Count - 1)
                {
                    organSegmentIndex = 0;
                    return true;
                }

                SoundEngine.PlaySound(Attack);



                Projectile organ = Projectile.NewProjectileDirect(
                    NPC.GetSource_FromAI(),
                    BodySegments[organSegmentIndex].Center,
                    toPlayer * 6,
                    ModContent.ProjectileType<OrganProjectile>(),
                    damage,
                    4,
                    ai0: player.whoAmI);

                organSegmentIndex++;
            }

            return false;
        }

        int NumBombs = 0;
        public void AI_CrystalBombMatrix()
        {
            int Damage = (int)MathHelper.Lerp(5, 100, LifeProgress);
            int Interval = DTUtils.CalamityBossRushActive() ? 240 : (int)MathHelper.Lerp(600, 240, LifeProgress);

            Vector2[] Positions = Opus.GetEquidistantVectors(4, player.Center, 700, MathHelper.PiOver4);

            int type = ModContent.ProjectileType<CrystalBomb>();
            if (Frame == 4 || Frame == 5)
            {
                type = ModContent.ProjectileType<SoulCrystalBomb>();
            }

            if (AITimer % Interval == 0)
            {
                SoundEngine.PlaySound(Attack);
                for (int i = 0; i < Positions.Length; i++)
                {
                    Projectile B = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), Positions[i], Vector2.Zero, type, Damage, 0);
                    B.timeLeft = 180;
                }

                if (Main.masterMode)
                {
                    for (int i = 0; i < Positions.Length; i++)
                    {
                        Projectile B2 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), Positions[i], Main.rand.NextVector2Circular(3, 3), ModContent.ProjectileType<IchorBlister>(), (int)(Damage * 0.75f), 0);
                        B2.timeLeft = 180;
                    }
                }
                NumBombs++;
            }
        }

        public void AI_ToothRounds()
        {
            Vector2[] P = Opus.GetEquidistantVectors(6, Main.player[NPC.target].Center, 300, Main.rand.NextFloat(MathHelper.TwoPi));
            if (ToothRoundCount < 4 && AITimer % 240 == 0)
            {
                SoundEngine.PlaySound(Roar);
                for (int i = 0; i < P.Length; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.Center.DirectionTo(P[i]) * 20f, ModContent.ProjectileType<PossessedTooth>(), 50, 6);
                }
                ToothRoundCount++;
            }
        }


        public void NodeSpawn()
        {
            float radius = 200;
            int projectileCount = 3;


            SoundEngine.PlaySound(NodeSpawnSound);

            for (int i = 0; i < projectileCount; i++)
            {
                // Get evenly spaced angle with rotation offset
                float angle = MathHelper.TwoPi * i / projectileCount;
                Vector2 spawnOffset = radius * angle.ToRotationVector2(); // position on the circle
                Vector2 spawnPosition = NPC.Center + spawnOffset;

                NPC.NewNPC(Entity.GetSource_FromThis(), (int)spawnPosition.X, (int)spawnPosition.Y, ModContent.NPCType<IchorNode>());

            }
        }



        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {


            // Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

            // The order in which you add loot will appear as such in the Bestiary. To mirror vanilla boss order:
            // 1. Trophy
            // 2. Classic Mode ("not expert")
            // 3. Expert Mode (usually just the treasure bag)
            // 4. Master Mode (relic first, pet last, everything else in between)



            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Item_WyvernCorpseTrophy>(), 10));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PhantasmalRemnant>(), 1, 10, 22));
            npcLoot.Add(ItemDropRule.Common(ItemID.SoulofFlight, 1, 5, 20));

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<RibChainsaw>(), 2, 1, 1));
            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<WyvernTail>(), 2, 1, 1));
            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<GreatFlayer>(), 2, 1, 1));
            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<WyvernSkull>(), 5, 1, 1));

            npcLoot.Add(notExpertRule);

            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<WyvernCorpseLootBag>()));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<DivineVessel>()));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Item_WyvernCorpseRelic>()));


            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HaepienNodeCharm>(), 20, 1, 1));


        }

        public override void OnKill()
        {
            SunlightModification.Reset();
            //SoundEngine.StopTrackedSounds();



        }

        public override void OnSpawn(IEntitySource source)
        {
            FablesTitleCardSystem.RegisterFablesBossIntro(FablesTitleCardSystem.WyvernCorpseTitle.Name, FablesTitleCardSystem.WyvernCorpseTitle.Title, 180, true, ColorLib.IchorCrystalGradient, ColorLib.IchorCrystalGradient, ColorLib.Soul, ColorLib.Soul, FablesTitleCardSystem.WyvernCorpseTitle.MusicTitle, FablesTitleCardSystem.WyvernCorpseTitle.MusicArtist);
        
            
        }




    }

    [AutoloadHead]
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
            if (DTUtils.CalamityBossRushActive())
            {
                NPC.lifeMax = 500000;
                NPC.defense = 60;
            }

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

        public void NoDamageEffects()
        {
            if (NPC.dontTakeDamage)
            {
                //Fade out
                if (NPC.Opacity > 0)
                {
                    NPC.Opacity -= 0.05f;
                }

            }
            else
            {
                //Fade in
                if (NPC.Opacity < 1)
                {
                    NPC.Opacity += 0.05f;
                }
            }
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

            NoDamageEffects();

            // Find the boss once per tick
            NPC bossNPC = Main.npc.FirstOrDefault(n =>
                n.active && n.type == ModContent.NPCType<WyvernCorpseHead>());

            if (bossNPC == null)
            {
                NPC.active = false; // despawn if parent is gone
                return;
            }

            


            ScreenIntervals = allNodes.Count;

            // --- state machine ---
            switch (currentState)
            {
                case AIState.Idle:
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
                case AIState.Pikes:
                    {
                        OrbitCenter = player.Center;
                        Pikes(300f, 0.05f, OrbitCenter);
                        break;
                    }
            }
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            var boss = Main.npc.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<WyvernCorpseHead>());
            if (boss?.ModNPC is WyvernCorpseHead Head)
            {
                int idx = Head.iNodes.FindIndex(n => n.whoAmI == NPC.whoAmI);
                if (idx >= 0 && Head.NodeShakeTimers != null && idx < Head.NodeShakeTimers.Length)
                    Head.NodeShakeTimers[idx] = 0;
            }
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            var boss = Main.npc.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<WyvernCorpseHead>());
            if (boss?.ModNPC is WyvernCorpseHead Head)
            {
                int idx = Head.iNodes.FindIndex(n => n.whoAmI == NPC.whoAmI);
                if (idx >= 0 && Head.NodeShakeTimers != null && idx < Head.NodeShakeTimers.Length)
                    Head.NodeShakeTimers[idx] = 0;
            }
        }

        public override void OnKill()
        {
            var boss = Main.npc.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<WyvernCorpseHead>());
            if (boss?.ModNPC is WyvernCorpseHead Head)
            {
                int idx = Head.iNodes.FindIndex(n => n.whoAmI == NPC.whoAmI);
                Head.iNodes.RemoveAt(idx);
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
                Opus.RadialSpreadProjectileRandom(ModContent.ProjectileType<NodeBossDistendedPike>(), Main.rand.Next(2, 5), NPC.Center, NPC.damage / 2, 7, 20);
                PikeCount++;
            }

            if (PikeCount >= 4)
            {
                currentState = AIState.Slam;
                PikeCount = 0;
            }
        }
    }

    public class WyvernCorpseDeathCutscene : ModSystem
    {
        public bool CutsceneActive = false;
        public static int CutsceneTime = 2400;
        public int CutsceneTimer = 0;

        void OnCutsceneEnd()
        {

        }

        public override void PreUpdatePlayers()
        {
            if (CutsceneActive)
            {
                foreach (Player player in Main.player)
                {
                    if (player != null && player.active)
                    {
                        player.SetCCed();
                        player.noItems = true;
                    }
                }



                if (CutsceneTimer < 0)
                {
                    CutsceneTimer--;
                }
                else
                {
                    OnCutsceneEnd();
                    CutsceneActive = false;
                }
            }
        }
    }
}