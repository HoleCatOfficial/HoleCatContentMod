using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Boss;
using DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss;
using DestroyerTest.Content.Projectiles.Boss.TenebrousConstruct;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using DestroyerTest.Content.Projectiles.Weapon.Summon;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.SHADEMANAGEMENT;
using DestroyerTest.Content.Tools;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using OpusLib;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Cinematics;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using UtfUnknown.Core.Models.SingleByte.Finnish;

namespace DestroyerTest.Content.Entities
{
    [AutoloadBossHead]
    public class TenebrousConstruct : ModNPC
    {

        public override void SetStaticDefaults()
        {
            immunities();
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f,
                Direction = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            Main.npcFrameCount[Type] = 55;
        }
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

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("An inanimate creation of unknown origin. Despite being composed of shade matter, it is not related to anything in the shade world."),
                ModContent.GetInstance<ShadeWorldBestiary>().ModBiomeBestiaryInfoElement,
            });
        }

        SoundStyle Stun = new SoundStyle("DestroyerTest/Assets/Audio/TenebrousConstruct/Stun", 5)
        {
            PitchVariance = 0.2f,
            MaxInstances = 0,

        };

        SoundStyle Idle = new SoundStyle("DestroyerTest/Assets/Audio/TenebrousConstruct/Idle", 8)
        {
            PitchVariance = 0.2f,
            MaxInstances = 0,
        };

        SoundStyle Kill = new SoundStyle("DestroyerTest/Assets/Audio/TenebrousConstruct/Kill", 3)
        {
            PitchVariance = 0.2f,
            MaxInstances = 0
        };

        SoundStyle Hit = DTAssetLib.Impacts.Malevolence with { PitchVariance = 0.6f };

        public static List<string> FightDialogue;
        public readonly int NumFightDialogueLines = 21;

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 55;
            NPC.defense = 140;
            NPC.lifeMax = 80000;
            NPC.HitSound = Hit;
            NPC.DeathSound = Kill;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            // Sets the above
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0.0f;
            NPC.boss = true;

            FightDialogue = new List<string>();

            for (int i = 0; i < NumFightDialogueLines - 1; i++)
            {
                FightDialogue.Add(Language.GetTextValue($"Mods.DestroyerTest.NPCs.TenebrousConstruct.FightDialogue{i}"));
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            int startFrame = 0;
            int finalFrame = 54;
            int frameSpeed = 1;
            NPC.frameCounter += 0.5f;
            NPC.frameCounter += NPC.velocity.Length() / 10f;
            if (NPC.frameCounter > frameSpeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y > finalFrame * frameHeight)
                {
                    NPC.frame.Y = startFrame * frameHeight;
                }
            }
        }

        public float WingXScale = 1f;
        public float OrbitBarrierOpacity = 0f;

        float[] RingRotAmt = new float[16];
        float[] RingRot = new float[16];
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return true;
            }

            for(int i = 0; i < RingRotAmt.Length; i++)
            {
                if (RingRotAmt[i] == 0f)
                {
                    RingRotAmt[i] = Main.rand.NextFloat(-0.3f, 0.3f);
                }
            }


            for (int i2 = 0; i2 < RingRot.Length; i2++)
            {
                RingRot[i2] += RingRotAmt[i2];
            }


            Utils.DrawBorderString(spriteBatch, InternalTimer.ToString(), (NPC.Center + new Vector2(0, -40)) - Main.screenPosition, Color.Red, 1f, 0.5f, 0.5f);

            Asset<Texture2D> WingLeft = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/TenebrousConstructWingLeft");
            Asset<Texture2D> WingRight = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/TenebrousConstructWingRight");

            if (CurrentState == State.Orbit)
            {
                if (OrbitBarrierOpacity < 1f)
                {
                    OrbitBarrierOpacity += 0.025f;
                }
            }
            else
            {
                if (OrbitBarrierOpacity > 0f)
                {
                    OrbitBarrierOpacity -= 0.025f;
                }
            }

            spriteBatch.UseBlendState(BlendState.Additive);

            //Inner Ring
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient * 0.5f) * OrbitBarrierOpacity, RingRot[0], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(200f, 39), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient * 0.5f) * OrbitBarrierOpacity, RingRot[1], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(200f, 39), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient * 0.5f) * OrbitBarrierOpacity, RingRot[2], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(200f, 39), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient * 0.5f) * OrbitBarrierOpacity, RingRot[3], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(200f, 39), SpriteEffects.None, 0);

            //Second ring
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient * 0.5f) * OrbitBarrierOpacity, RingRot[4], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(500f, 39), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient * 0.5f) * OrbitBarrierOpacity, RingRot[5], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(500f, 39), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient * 0.5f) * OrbitBarrierOpacity, RingRot[6], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(500f, 39), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient * 0.5f) * OrbitBarrierOpacity, RingRot[7], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(500f, 39), SpriteEffects.None, 0);

            //third
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient * 0.5f) * OrbitBarrierOpacity, RingRot[8], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(800f, 39), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient * 0.5f) * OrbitBarrierOpacity, RingRot[9], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(800f, 39), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient * 0.5f) * OrbitBarrierOpacity, RingRot[10], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(800f, 39), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient * 0.5f) * OrbitBarrierOpacity, RingRot[11], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(800f, 39), SpriteEffects.None, 0);

            //outer
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient * 0.5f) * OrbitBarrierOpacity, RingRot[12], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(1100f, 39), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient * 0.5f) * OrbitBarrierOpacity, RingRot[13], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(1100f, 39), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient * 0.5f) * OrbitBarrierOpacity, RingRot[14], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(1100f, 39), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient * 0.5f) * OrbitBarrierOpacity, RingRot[15], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(1100f, 39), SpriteEffects.None, 0);

            Main.EntitySpriteDraw(DTAssetLib.BarrierRing(true).Value, NPC.Center - screenPos, null, Color.White * OrbitBarrierOpacity, 0f, DTAssetLib.BarrierRing(true).Value.Size() / 2, DTAssetLib.BarrierRing(true).Value.ScaleRingTextureToMatchRadius(1300f, 186), SpriteEffects.None, 0);
            spriteBatch.ResetToDefault();

            // Left wing: origin at RIGHT edge, middle vertically
            Vector2 originLeft = new Vector2(WingLeft.Width(), WingLeft.Height() / 2);
            Main.EntitySpriteDraw(
                WingLeft.Value,
                NPC.Center - screenPos + new Vector2(-30, -30),
                null,
                Color.White,
                0f,
                originLeft,
                new Vector2(WingXScale * 2, 2f),
                SpriteEffects.None,
                0
            );

            // Right wing: origin at LEFT edge, middle vertically
            Vector2 originRight = new Vector2(0, WingRight.Height() / 2);
            Main.EntitySpriteDraw(
                WingRight.Value,
                NPC.Center - screenPos + new Vector2(30, -30),
                null,
                Color.White,
                0f,
                originRight,
                new Vector2(WingXScale * 2, 2f),
                SpriteEffects.None,
                0
            );


            return true;
        }

        public enum State
        {
            IdleChase,
            LanceCross,
            Orbit,
            StarShoot
        }

        public State CurrentState;
        public int InternalTimer = 0;
        public int LanceCount = 0;
        public List<Vector2[]> Rings = new List<Vector2[]>();
        public List<Projectile[]> RingProjectiles = new List<Projectile[]>();

        bool[] DisplayedDialogue = new bool[21];
        int CurrentDialogue = 0;

        void ControlDialogue()
        {
            float Prog = ((float)NPC.life / (float)NPC.lifeMax).Inverse();
            Prog = Utils.Clamp(Prog, 0f, 1f);
            CurrentDialogue = (int)MathHelper.Lerp(0, NumFightDialogueLines - 1, Prog);
            CurrentDialogue = Utils.Clamp(CurrentDialogue, 0, 20);

            if (!DisplayedDialogue[CurrentDialogue])
            {
                Main.NewText(FightDialogue[CurrentDialogue]);
                DisplayedDialogue[CurrentDialogue] = true;
            }
        }

        public override void AI()
        {
            NPC.TargetClosest(faceTarget: true);
            Player player;
            player = Main.player[NPC.target];

            InternalTimer++;

            NPC.rotation = 0.05f * NPC.velocity.Length();
            Vector2 direction = player.Center - NPC.Center;
            direction.Normalize();

            WingXScale = Opus.Sine(0f, 0.8f, 0.08f);

            ControlDialogue();

            if (Main.rand.NextBool(12))
            {
                Dust.NewDust(NPC.Center, NPC.width, NPC.height, ModContent.DustType<TenebrisDarkmatterDust>(), 0, 0, 0, default, 1.0f);
            }


            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/UnfinishedBoss");

            switch (CurrentState)
            {
                case State.IdleChase:
                    {
                        {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, direction * 4f, 0.025f);

                            if (Main.rand.NextBool(32) && Main.GameUpdateCount % 60 == 0)
                            {
                                SoundEngine.PlaySound(Idle, NPC.Center);
                            }
                            
                            if (InternalTimer >= 300)
                            {
                                NPC.velocity = Vector2.Zero;
                                SoundEngine.PlaySound(Stun);
                                CurrentState = State.LanceCross;
                            }
                        }
                        break;
                    }
                case State.LanceCross:
                    {
                        NPC.aiStyle = NPCAIStyleID.DungeonSpirit;
                        if (InternalTimer % 300 == 0)
                        {
                            LanceCount++;
                            SoundEngine.PlaySound(SoundID.Item84);
                            Opus.RingSpreadProjectile(ModContent.ProjectileType<TenebrisLance>(), 4, player.Center, 800, 40, 3, -24f, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                        }

                        if (InternalTimer > 2000)
                        {
                            LanceCount = 0;
                            SoundEngine.PlaySound(Stun);
                            CurrentState = State.Orbit;
                        }
                        break;
                    }
                case State.Orbit:
                    {
                        NPC.SmoothMoveToPoint(player.Center, 1f);

                        if (player.Center.Distance(NPC.Center) > 1300)
                        {
                            player.Center = NPC.Center + new Vector2(1200, 0).RotatedBy(player.DirectionFrom(NPC.Center).ToRotation());
                        }

                        if (Rings.Count == 0)
                        {
                            Rings.Add(Opus.GetEquidistantOrbitVectors(6, NPC.Center, 0.04f, 200));
                            RingProjectiles.Add(new Projectile[6]);

                            Rings.Add(Opus.GetEquidistantOrbitVectors(12, NPC.Center, 0.02f, 500));
                            RingProjectiles.Add(new Projectile[12]);

                            Rings.Add(Opus.GetEquidistantOrbitVectors(24, NPC.Center, 0.01f, 800));
                            RingProjectiles.Add(new Projectile[24]);

                            Rings.Add(Opus.GetEquidistantOrbitVectors(48, NPC.Center, 0.005f, 1100));
                            RingProjectiles.Add(new Projectile[48]);
                        }
                        else
                        {

                            Rings[0] = Opus.GetEquidistantOrbitVectors(6, NPC.Center, 0.04f, 200);
                            Rings[1] = Opus.GetEquidistantOrbitVectors(12, NPC.Center, 0.02f, 500);
                            Rings[2] = Opus.GetEquidistantOrbitVectors(24, NPC.Center, 0.01f, 800);
                            Rings[3] = Opus.GetEquidistantOrbitVectors(48, NPC.Center, 0.005f, 1100);
                        }


                        for (int o = 0; o < 4; o++)
                        {

                            Vector2[] ringPositions = Rings[o];
                            Projectile[] projectiles = RingProjectiles[o];

                            for (int i = 0; i < ringPositions.Length; i++)
                            {
                                if (projectiles[i] == null || !projectiles[i].active)
                                {
                                    projectiles[i] = Projectile.NewProjectileDirect(
                                        NPC.GetSource_FromAI(),
                                        ringPositions[i],
                                        Vector2.Zero,
                                        ModContent.ProjectileType<DarkEnergyOrb>(),
                                        40,
                                        3
                                    );
                                }
                                else
                                {
                                    projectiles[i].Center = ringPositions[i];
                                    projectiles[i].timeLeft = 60;
                                }
                            }
                        }

                        if (InternalTimer % 300 == 0)
                        {
                            Projectile Mine = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Main.rand.NextVector2Circular(10, 10), ModContent.ProjectileType<DarkLaserMine>(), 100, 2);
                            Mine.timeLeft = 180;
                        }

                        if (InternalTimer > 5000)
                        {

                            for (int i = 0; i < RingProjectiles.Count; i++)
                            {
                                for (int j = 0; j < RingProjectiles[i].Length; j++)
                                {
                                    RingProjectiles[i][j].Kill();
                                }
                            }

                            for (int i = 0; i < Rings.Count; i++)
                            {
                                for (int j = 0; j < Rings[i].Length; j++)
                                {
                                    Rings[i][j] = NPC.Center;
                                }
                            }

                            RingProjectiles.Clear();
                            Rings.Clear();
                            SoundEngine.PlaySound(Stun);
                            CurrentState = State.StarShoot;
                        }

                        break;
                    }
                case State.StarShoot:
                    {
                        Vector2[] PossibleShootPositions = Opus.GetEquidistantVectors(12, NPC.Center, 50f);  
                        
                        if (InternalTimer % 4 == 0 && InternalTimer > 5060)
                        {
                            Vector2 ShootPosition = PossibleShootPositions[Main.rand.Next(PossibleShootPositions.Length)];
                            
                            Vector2 PlayerPrediction = player.Center + ( player.velocity * 20);
                            Dust.NewDustPerfect(PlayerPrediction, DustID.RedTorch).noGravity = true;
                            Vector2 ToPlayer = ShootPosition.DirectionTo(PlayerPrediction);
                            ToPlayer.Normalize();

                            SoundEngine.PlaySound(SoundID.Item28 with { MaxInstances = 0 }, ShootPosition);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPosition, ToPlayer * 18f, ModContent.ProjectileType<TenebrisStarHostile_NoHoming>(), 20, 5);

                            if (Main.rand.NextBool(60))
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPosition, ToPlayer * 18f, ModContent.ProjectileType<TenebrisLance>(), 20, 5);
                            }
                        }

                        if (InternalTimer > 5660)
                        {
                            InternalTimer = 0;
                            SoundEngine.PlaySound(Stun);
                            CurrentState = State.IdleChase;
                        }
                        break;
                    }
               
            }

        }



        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 120, true, false);
        }

        List<int> FiftyPercentDamageProjectiles = new()
        {
            ProjectileID.LastPrismLaser,
            ProjectileID.RainbowWhip,
            ModContent.ProjectileType<WyvernTailProjectile>(),
        };

        List<int> TwentyFivePercentDamageProjectiles = new()
        {
            ProjectileID.EmpressBlade,
        };

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (FiftyPercentDamageProjectiles.Contains(projectile.type))
            {
                hit.Damage = (int)(hit.Damage * 0.5f);
            }
            if (TwentyFivePercentDamageProjectiles.Contains(projectile.type))
            {
                hit.Damage = (int)(hit.Damage * 0.25f);
            }

            if (damageDone > 700 && Main.rand.NextBool(50) )
            {
                AdvancedPopupRequest T = new AdvancedPopupRequest() with { Text = "Ouch!!!", Color = Color.Red, DurationInFrames = 180, Velocity = new Vector2(0, -4) };
                PopupText.NewText(T, NPC.Center);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadeParticle>(), 3, 24, 36));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShimmeringShards>(), 3, 13, 23));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShimmeringSludge>(), 4, 2, 9));
        }

        public override void OnKill()
        {
            int Gore1 = Mod.Find<ModGore>("TenebrousConstructGore1").Type;
            int Gore2 = Mod.Find<ModGore>("TenebrousConstructGore2").Type;
            int Gore3 = Mod.Find<ModGore>("TenebrousConstructGore3").Type;
            int Gore4 = Mod.Find<ModGore>("TenebrousConstructGore4").Type;
            int Gore5 = Mod.Find<ModGore>("TenebrousConstructGore5").Type;

            var entitySource = NPC.GetSource_Death();
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(0, 10)), Gore1);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(0, 10)), Gore2);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(0, 10)), Gore3);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(0, 10)), Gore4);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(0, 10)), Gore5);

            AdvancedPopupRequest T = new AdvancedPopupRequest() with { Text = "YOU BRAT!!!", Color = Color.Red, DurationInFrames = 180, Velocity = new Vector2(0, -4) };
            PopupText.NewText(T, NPC.Center);

           
            int numProjectiles = 36;
            float rotationStep = MathHelper.TwoPi / numProjectiles;

            for (int i = 0; i < numProjectiles; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TintableDustLighted, NPC.velocity.X * 0.7f, NPC.velocity.Y * 0.7f, 0, ColorLib.TenebrisGradient, 1);

                Vector2 velocity = new Vector2(1f, 0f).RotatedBy(rotationStep * i);
                Projectile.NewProjectile(
                    Entity.GetSource_FromThis(),
                    NPC.Center,
                    velocity,
                    ModContent.ProjectileType<TenebrisDart>(),
                    30,
                    6
                );
            }
        }
    }
}