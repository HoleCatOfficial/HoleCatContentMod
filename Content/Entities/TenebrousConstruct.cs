using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Boss;
using DestroyerTest.Content.Projectiles.Boss.TenebrousConstruct;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.SHADEMANAGEMENT;
using DestroyerTest.Content.Tools;
using InnoVault.PRT;
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

        SoundStyle Hit = new SoundStyle("DestroyerTest/Assets/Audio/TenebrousConstruct/Hit", 5)
        {
            PitchVariance = 0.2f,
            MaxInstances = 0
        };

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
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Utils.DrawBorderString(spriteBatch, InternalTimer.ToString(), (NPC.Center + new Vector2(0, -40)) - Main.screenPosition, Color.Red, 1f, 0.5f, 0.5f);

            Asset<Texture2D> WingLeft = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/TenebrousConstructWingLeft");
            Asset<Texture2D> WingRight = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/TenebrousConstructWingRight");

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
            LanceCross
        }

        public State CurrentState;
        public int InternalTimer = 0;
        public int LanceCount = 0;

        public bool RoseAlive;
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

            if (Main.rand.NextBool(12))
            {
                Dust.NewDust(NPC.Center, NPC.width, NPC.height, ModContent.DustType<TenebrisDarkmatterDust>(), 0, 0, 0, default, 1.0f);
            }

            RoseAlive = Main.npc.Any(n => n.active && n.type == ModContent.NPCType<NightmareRoseBoss>());
            if (RoseAlive)
            {
                NPC.dontTakeDamage = true;
            }
            else
            {
                NPC.dontTakeDamage = false;
                
            }

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Placeholder8");

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
                                CurrentState = State.LanceCross;
                            }
                        }
                        break;
                    }
                case State.LanceCross:
                    {
                        NPC.aiStyle = NPCAIStyleID.Flocko;
                        if (InternalTimer % 300 == 0)
                        {
                            LanceCount++;
                            SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot);
                            Opus.RingSpreadProjectile(ModContent.ProjectileType<TenebrisLance>(), 4, player.Center, 800, 40, 3, -24f, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                        }

                        if (LanceCount > 5)
                        {
                            LanceCount = 0;
                            InternalTimer = 0;
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

            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TintableDustLighted, NPC.velocity.X * 0.7f, NPC.velocity.Y * 0.7f, 0, ColorLib.TenebrisGradient, 1);
            int numProjectiles = 36;
            float rotationStep = MathHelper.TwoPi / numProjectiles;

            for (int i = 0; i < numProjectiles; i++)
            {
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