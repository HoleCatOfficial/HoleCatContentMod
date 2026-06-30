
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.BossBar;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Boss;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Ichor;
using DestroyerTest.Content.Projectiles.Boss.VampireBoss;
using DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using GlowmaskHelper.Content;
using InnoVault.PRT;
using log4net.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Terraria;
using Terraria.Audio;
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
            Nodes,
            Enraged
        }

        public bool HasTriggeredNodes = false;

        public SoundStyle Roar = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/CorpseRoar1") with { PitchVariance = 1.0f, MaxInstances = 0  };
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

            NPC.damage = 0;
            NPC.defense = 65;
            NPC.lifeMax = 420000;

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
            spriteBatch.Draw(texture.Value, drawPos, NPC.frame, drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);
            spriteBatch.Draw(Glowtexture.Value, drawPos, NPC.frame, Color.White, NPC.rotation, origin, NPC.scale, effects, 0f);
            return false;
        }


        public attackType CurrentAttack = attackType.Follow;

        public bool SpawnFlag = false;

        public bool anyNodesAlive;
        public int nodeCount;

        public bool SoundFlag1 = false;

        SlotId DesperationLoopSlot;
        public SoundStyle Loop = new SoundStyle("DestroyerTest/Assets/Audio/AuraLoop/LaserLoop1") 
        { 
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };
        public float PitchVal = -2;

        public int AITimer = 0;

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

        public override bool? CanCollideWithPlayerMeleeAttack(Player player, Item item, Rectangle meleeAttackHitbox)
        {
            return base.CanCollideWithPlayerMeleeAttack(player, item, meleeAttackHitbox);
        }


        public Vector2 Center;
        public int DeathInterval = 10;

        public List<NPC> BodySegments = new List<NPC>();

        bool HasShedBlisters = false;
        public override void AI()
        {
            AITimer++;

            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
   

            Vector2 ToPlayer = NPC.Center - player.Center;

            Vector2 ToPlayerInverse = player.Center - NPC.Center;

            if (Frame == 1 && !HasShedBlisters)
            {
                for (int i = 0; i < BodySegments.Count(); i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), BodySegments[i].Center, Main.rand.NextVector2Circular(2, 2), ModContent.ProjectileType<IchorBlister>(), 50, 4);
                }
                HasShedBlisters = true;
            }

            Vector2 RandNearPlayer = player.Center + new Vector2(Main.rand.NextFloat(-200f, 200f), Main.rand.NextFloat(-200f, 200f));

            ModifyHitDustAmounts();
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

            List<NPC> ichorNodes = Main.npc.Where(n => n.active && n.type == ModContent.NPCType<IchorNode>()).ToList();
            anyNodesAlive = ichorNodes.Count > 0;
            nodeCount = ichorNodes.Count;
            float radius = Opus.Sine(200f, 240f);

            if (anyNodesAlive)
            {
                Vector2[] NodePositions = Opus.GetEquidistantOrbitVectors(nodeCount, NPC.Center, 0.02f, radius);

                for (int i = 0; i < nodeCount; i++)
                {
                    Vector2 IdealVel = ichorNodes[i].Center - NodePositions[i];
                    ichorNodes[i].SmoothMoveToPoint(NodePositions[i], 48);
                }

                NPC.dontTakeDamage = true;
                NPC.immortal = true;
                NPC.life += 2;
            }
            else if (!anyNodesAlive)
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

            if (NPC.life <= NPC.lifeMax * 0.75f)
            {
                if (HasTriggeredNodes == false)
                {
                    CurrentAttack = attackType.Nodes;

                    HasTriggeredNodes = true;

                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && SpawnFlag == false)
            {
                if (NPC.ai[0] == 0f)
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

                        int BodySegment = NPC.NewNPC(NPC.GetSource_FromAI(),
                            (int)(NPC.position.X + NPC.width / 2),
                            (int)(NPC.position.Y + NPC.height),
                            WyvBodyInt, NPC.whoAmI);

                        BodySegments.Add(Main.npc[BodySegment]);

                        Main.npc[BodySegment].ai[2] = NPC.whoAmI;
                        Main.npc[BodySegment].realLife = NPC.whoAmI;
                        Main.npc[BodySegment].ai[1] = Me;
                        Main.npc[Me].ai[0] = BodySegment;

                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, BodySegment);
                        Me = BodySegment;
                    }
                    NPC.netUpdate = true;
                    SpawnFlag = true;
                }
            }

            if (ModContent.GetInstance<DTConfig>().EnableDebugMessages && Main.GameUpdateCount % 120 == 0)
            {
                Mod.Logger.Info($"Current State: {CurrentAttack}");
            }

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/UnfinishedBoss");
            
        

            ImportantMathematics();

            switch (CurrentAttack)
            {
                case attackType.Follow:
                    {
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
                        AI_Organs();
                        CurrentAttack = attackType.Follow;
                        AITimer = 0;
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

        public void AI_Organs()
        {
            int Damage = (int)MathHelper.Lerp(5, 100, LifeProgress);
            Player player = Main.player[NPC.target];

            SoundEngine.PlaySound(Attack);
            for (int i = 0; i < BodySegments.Count; i++)
            {
                Vector2 toPlayer = player.Center - BodySegments[i].Center;
                toPlayer.Normalize();

                Projectile.NewProjectile(NPC.GetSource_FromAI(), BodySegments[i].Center, toPlayer * 2, ModContent.ProjectileType<OrganProjectile>(), Damage, 4, ai0: player.whoAmI);
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

            npcLoot.Add(ItemDropRule.Common(ItemID.SoulofFlight, 1, 5, 20));



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
}