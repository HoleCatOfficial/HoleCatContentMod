
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
            Nodes,
            Desperation,
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

        // Multiplayer-synced fields
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

        

        // Write extra AI fields for multiplayer sync
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)CurrentAttack);
            writer.Write(anyNodesAlive);
        }

        // Read extra AI fields for multiplayer sync
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            CurrentAttack = (attackType)reader.ReadInt32();
            anyNodesAlive = reader.ReadBoolean();
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

        public override void HitEffect(NPC.HitInfo hit)
        {
            float Progress = (float)NPC.life / (float)NPC.lifeMax;

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
   

            Vector2 ToPlayer = NPC.Center - player.Center;

            Vector2 ToPlayerInverse = player.Center - NPC.Center;

         

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

            if (NPC.life <= NPC.lifeMax * 0.75f)
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
            

            if (!Main.dedServ && !DestroyerTestMod.EternityIsActive)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Tribulation");
            }
            if (!Main.dedServ && DestroyerTestMod.EternityIsActive && !muscfg.EternityMusic)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Tribulation");
            }
            if (!Main.dedServ && DestroyerTestMod.EternityIsActive && muscfg.EternityMusic)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Placeholder4");
            }
            if (!Main.dedServ && DestroyerTestMod.EternityIsActive && DestroyerTestMod.MasochistIsActive && muscfg.EternityMusic)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/MasoEvils");
            }

            ImportantMathematics();

            switch (CurrentAttack)
            {
                case attackType.Follow:
                    {
                       
                    }
                    break;
                case attackType.Nodes:
                    if (NPC.type == ModContent.NPCType<WyvernCorpseHead>())
                    {
                        NPC.aiStyle = -1;
                        NodeSpawn();

                        CurrentAttack = attackType.Follow;
                        ResetStats();
                    }
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



        public void ResetStats()
        {

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