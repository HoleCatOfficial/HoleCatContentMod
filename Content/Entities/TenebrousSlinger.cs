using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.Tools;
using InnoVault.PRT;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
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
    public class TenebrousSlinger : ModNPC
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
                new FlavorTextBestiaryInfoElement("A massive bow said to have been used by the acient peoples to put the stars in the sky. After its time in the shade world, it has taken on sentience."),
                ModContent.GetInstance<ShadeWorldBestiary>().ModBiomeBestiaryInfoElement,
            });
        }

        SoundStyle Shot = new SoundStyle("DestroyerTest/Assets/Audio/TenebrisSlinger/TenebrisSlingerShoot", 3)
        {
            PitchVariance = 0.2f,
            MaxInstances = 0,
        };

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
            NPC.width = 92;
            NPC.height = 130;
            NPC.damage = 55;
            NPC.defense = 140;
            NPC.lifeMax = 20000;
            NPC.HitSound = Hit;
            NPC.DeathSound = Kill;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            // Sets the above
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0.0f;
        }

        public override bool CheckActive()
        {
            return false;
        }
        public enum State
        {
            IdleChase,
            DartCross,
            Stunned,
            RetaliatoryLance
        }

        public State CurrentState;
        public bool Stunned = false;
        public int StunTimer = 1200;
        public bool ShootFlag1 = false;
        public int OrbCount = 0;
        public int MinimumIdle = 600;
        public override void AI()
        {
            NPC.TargetClosest(faceTarget: true);
            Player player;
            player = Main.player[NPC.target];

            Vector2 direction = player.Center - NPC.Center;
            direction.Normalize();
            NPC.rotation = NPC.velocity.ToRotation();

            if (Main.rand.NextBool(12))
            {
                Dust.NewDust(NPC.Center, NPC.width, NPC.height, ModContent.DustType<TenebrisDarkmatterDust>(), 0, 0, 0, default, 1.0f);
            }

            bool CorpseAlive = Main.npc.Any(n => n.active && n.type == ModContent.NPCType<WyvernCorpseHead>());
            if (CorpseAlive)
            {
                NPC.dontTakeDamage = true;
            }
            else
            {
                NPC.dontTakeDamage = false;
            }

            Lighting.AddLight(NPC.Center, ColorLib.TenebrisGradient.ToVector3());

            

            switch (CurrentState)
            {
                case State.IdleChase:
                    {
                        {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, direction * 4f, 0.05f);

                            if (Main.GameUpdateCount % 60 == 0)
                            {
                                Vector2 Shoot = new Vector2(50, 0).RotatedBy(NPC.rotation);
                                SoundEngine.PlaySound(Shot, NPC.Center);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Shoot * 0.75f, ModContent.ProjectileType<TenebrisArrowProjectile>(), 30, 1, ai0: 2);
                            }

                            if (Main.rand.NextBool(6) && Main.GameUpdateCount % 60 == 0)
                            {
                                SoundEngine.PlaySound(Idle, NPC.Center);
                            }

                            if (player.HeldItem.type == ModContent.ItemType<ShiningObelisk>() && player.itemAnimation == player.itemAnimationMax - 10)
                            {
                                ScreenFlashSystem.FlashIntensity = 1.0f;
                                SoundEngine.PlaySound(Stun, NPC.Center);
                                CurrentState = State.Stunned;
                                StunTimer = 1200;
                                NPC.netUpdate = true;
                            }
                            if (MinimumIdle > 0)
                            {
                                MinimumIdle--;
                            }
                            if (MinimumIdle <= 0)
                            {
                                if (Main.rand.NextBool(600))
                                {
                                    CurrentState = State.DartCross;
                                    MinimumIdle = 600;
                                }
                            }
                        }
                        break;
                    }
                case State.DartCross:
                    {
                        NPC.velocity = Vector2.Lerp(NPC.velocity, direction * 3f, 0.05f);
                        if (player.HeldItem.type == ModContent.ItemType<ShiningObelisk>() && player.itemAnimation == player.itemAnimationMax - 10)
                        {
                            ScreenFlashSystem.FlashIntensity = 1.0f;
                            SoundEngine.PlaySound(Stun, NPC.Center);
                            CurrentState = State.Stunned;
                            StunTimer = 1200;
                            NPC.netUpdate = true;
                        }
                        if (Main.rand.NextBool(6) && Main.GameUpdateCount % 60 == 0)
                        {
                            SoundEngine.PlaySound(Idle, NPC.Center);
                        }
                        if (Main.GameUpdateCount % 240 == 0)
                        {
                            for (int e = 0; e < 4; e++)
                            {
                                Vector2 Shoot = new Vector2(10, 0).RotatedBy(NPC.rotation + Main.rand.NextFloat(-0.5f, 0.5f));
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Shoot, ModContent.ProjectileType<TenebrisMine>(), 30, 1);
                            }
                            OrbCount++;
                        }
                        if (OrbCount >= 10)
                        {
                            CurrentState = State.IdleChase;
                            OrbCount = 0;
                        }
                        break;
                    }
                case State.Stunned:
                    {
                        {
                            NPC.rotation = direction.ToRotation();
                            if (StunTimer > 0)
                            {
                                NPC.velocity *= 0.7f;
                                if (Main.rand.NextBool(4))
                                {
                                    NPC.Center += new Vector2(Main.rand.Next(-2, 2), Main.rand.Next(-2, 2));
                                }
                                StunTimer--;
                            }

                            if (StunTimer <= 0)
                            {
                                CurrentState = State.RetaliatoryLance;
                                StunTimer = 1200;
                                NPC.netUpdate = true;
                            }
                        }
                        break;
                    }
                case State.RetaliatoryLance:
                    {
                        {
                            if (!ShootFlag1)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * 20, ModContent.ProjectileType<TenebrisLance>(), 30, 1);
                                ShootFlag1 = true;
                            }
                            if (ShootFlag1)
                            {
                                CurrentState = State.IdleChase;
                                ShootFlag1 = false;
                            }
                        }
                        break;
                    }
            }

        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> GlowMask = ModContent.Request<Texture2D>($"{Texture}_GlowMask");
            Main.EntitySpriteDraw(GlowMask.Value, NPC.Center - Main.screenPosition, null, Color.White, NPC.rotation, new Vector2(NPC.width / 2, NPC.height / 2), NPC.scale, SpriteEffects.None, 0);
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
            PRTLoader.NewParticle<Boom5>(NPC.Center, Vector2.Zero, ColorLib.TenebrisGradient, 1f);
            PRTLoader.NewParticle<BloomRing>(NPC.Center, Vector2.Zero, ColorLib.TenebrisGradient, 1f);
            int Gore1 = Mod.Find<ModGore>("TenebrousSlingerGore1").Type;
            int Gore2 = Mod.Find<ModGore>("TenebrousSlingerGore2").Type;
            int Gore3 = Mod.Find<ModGore>("TenebrousSlingerGore3").Type;

            var entitySource = NPC.GetSource_Death();
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(0, 10)), Gore1);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(0, 10)), Gore2);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(0, 10)), Gore3);

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