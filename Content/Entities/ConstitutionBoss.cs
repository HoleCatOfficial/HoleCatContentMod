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
using System.Linq;
using ReLogic.Content;
using DestroyerTest.Common.Systems;
using Terraria.Localization;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.Tiles;
using Terraria.GameContent.ItemDropRules;
using DestroyerTest.Content.Resources;
using Humanizer.Localisation.DateToOrdinalWords;

namespace DestroyerTest.Content.Entities
{
    [AutoloadBossHead]
    public class ConstitutionBoss : ModNPC
    {
        public override string BossHeadTexture => "DestroyerTest/Content/Entities/ConstitutionBoss_Head_Boss";
        public override void SetStaticDefaults()
        {
            NPCID.Sets.CanHitPastShimmer[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Burning] = false;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Ichor] = false;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Oiled] = false;
            NPCID.Sets.TrailCacheLength[Type] = 20;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            { // Influences how the NPC looks in the Bestiary
                CustomTexturePath = "DestroyerTest/Content/Entities/ConstitutionBestiary", // If the NPC is multiple parts like a worm, a custom texture for the Bestiary is encouraged.
                Position = Vector2.Zero,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        public override void SetDefaults()
        {
            NPC.width = 70;
            NPC.height = 64;
            NPC.aiStyle = -1;
            NPC.damage = 24;
            NPC.defense = 24;
            NPC.lifeMax = 4500;
            NPC.HitSound = new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionBoss/ConstitutionBossHit") with { PitchVariance = 1, MaxInstances = 100 };
            NPC.DeathSound = new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionBoss/ConstitutionBossKill") with { PitchVariance = 1, MaxInstances = 1, Volume = 8 };
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.timeLeft = 150000;
            NPC.boss = true;
            NPC.npcSlots = 90f;
            NPC.netUpdate = true;
            NPC.netID = ModContent.NPCType<ConstitutionBoss>();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("A blade hailing from the heavens, built to maim, but not to kill."),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface
            });
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
            {
                Texture2D texture = TextureAssets.Npc[Type].Value;
                Texture2D WhiteOutline = ModContent.Request<Texture2D>("DestroyerTest/Content/Entities/ConstitutionBossOutline").Value;

                Vector2 drawOrigin = new(texture.Width * 0.5f, NPC.height * 0.5f);
                //Effect shader = ModContent.Request<Effect>("DestroyerTest/Assets/HSHLShaders/SlashTrans", AssetRequestMode.ImmediateLoad).Value;

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
                {
                    float outlineRotation = NPC.rotation;
                    SpriteEffects effects = SpriteEffects.None;
                    if (NPC.direction == -1)
                    {
                        outlineRotation += MathHelper.Pi + MathHelper.ToRadians(180);
                        effects = SpriteEffects.FlipHorizontally;
                    }
                    Main.EntitySpriteDraw(WhiteOutline, NPC.Center, null, ColorLib.StellarColor, outlineRotation, new Vector2(NPC.width / 2, NPC.height / 2), NPC.scale * 1.2f, effects, 0);
                }
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                {
                    Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY);

                    float outlineRotation = NPC.rotation;
                    SpriteEffects effects = SpriteEffects.None;
                    if (NPC.direction == -1)
                    {
                        outlineRotation += MathHelper.Pi + MathHelper.ToRadians(180);
                        effects = SpriteEffects.FlipHorizontally;
                    }

                    Main.EntitySpriteDraw(WhiteOutline, drawPos, null, NPC.GetAlpha(ColorLib.StellarColor) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length), outlineRotation, new Vector2(NPC.width / 2, NPC.height / 2), NPC.scale * 1.2f, effects, 0);
                }
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            }
            return true;
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
            idlefloat,
            idlespin,
            Jab,
            ShootStars,
            ShootClone,
            ShootStars2,
            ShootStars3,
            ShootSwords,
            RainStars,
            Lightning,
            LightBird,
            Minefield,
            TeleStars,
            StellarFlame
        }
        public int RotTime = 60;
        public int JabTime = 20;
        public int StarsCount1 = 3;
        public int StarsCount2 = 16;
        public int StarsCount3 = 2;
        public int SwordCount = 4;
        public Vector2 Chargedir;
        public float TeleRadius = 600f;
        public int JabCount = 4;
        public int CloneRepeat = 2;
        public float AmountofCloning = 0;
        public float shootStarsTimer1 = 0f;
        public float starsShotCount1 = 0f;
        public float CloneTimer = 0f;
        public float shootStarsTimer2 = 0f;
        public float starsShotCount2 = 0f;
        public bool HasCharged = false;
        public bool HasShotStars1 = false;
        public bool HasCloned = false;
        public bool HasShotStars2 = false;
        public bool HasShotStars3 = false;
        public float shootStarsTimer3 = 0f;
        public float starsShotCount3 = 0f;
        public float shootSwordsTimer = 0f;
        public float swordsShotCount = 0f;
        public bool HasShotSwords = false;
        public bool Phase2 = false;
        public int Chance = 4;
        public float lanceradius = 500f;
        public float lancerotspeed = 0.05f;
        public bool HasPlayedPhase2Roar = false;
        public int StarIndex = 10;
        public int LightningCount = 4;
        public float lightningTimer = 0;
        public bool HasSpawnedLighting = false;
        public float lightbirdTimer = 0;
        public bool HasSpawnedLightbird = false;
        public int LightbirdCount = 0;
        public int LightbirdCountMax = 8;
        public float OrbitTimer1 = 0;
        public float OrbitTimer2 = 0;
        public float OrbitTimer3 = 0;
        public bool WarnedSound = false;
        public bool boomedSound = false;
        public bool HasBlownMinefield = false;
        public Vector2 MineSpot;
        public int MineCount = 100;
        public List<Vector2> MineSpots = new List<Vector2>();
        public int TeleStarCount = 0;
        public int TeleStarMax = 12;
        public bool HasTeleportedandShotStars = false;
        public int MaxFlameCount = 900;
        public int FlameTimer = 0;
        public bool HasPlayedFlameSound = false;

        public int AttackIntervalDefault = 20;
        public AttackState currentState = AttackState.idlefloat;
        // Sync AI variables across the network
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)currentState);
            writer.Write(RotTime);
            writer.Write(JabTime);
            writer.Write(StarsCount1);
            writer.Write(StarsCount2);
            writer.Write(StarsCount3);
            writer.Write(SwordCount);
            writer.WriteVector2(Chargedir);
            writer.Write(TeleRadius);

            writer.Write(JabCount);
            writer.Write(CloneRepeat);
            writer.Write(AmountofCloning);
            writer.Write(shootStarsTimer1);
            writer.Write(starsShotCount1);
            writer.Write(CloneTimer);
            writer.Write(shootStarsTimer2);
            writer.Write(starsShotCount2);
            writer.Write(HasCharged);
            writer.Write(HasShotStars1);
            writer.Write(HasCloned);
            writer.Write(HasShotStars2);
            writer.Write(HasShotStars3);
            writer.Write(shootStarsTimer3);
            writer.Write(starsShotCount3);
            writer.Write(shootSwordsTimer);
            writer.Write(swordsShotCount);
            writer.Write(HasShotSwords);
            writer.Write(Phase2);
            writer.Write(Chance);
            writer.Write(lanceradius);
            writer.Write(lancerotspeed);
            writer.Write(HasPlayedPhase2Roar);
            writer.Write(StarIndex);
            writer.Write(LightningCount);
            writer.Write(lightningTimer);
            writer.Write(HasSpawnedLighting);
            writer.Write(lightbirdTimer);
            writer.Write(HasSpawnedLightbird);
            writer.Write(LightbirdCount);
            writer.Write(LightbirdCountMax);
            writer.Write(OrbitTimer1);
            writer.Write(OrbitTimer2);
            writer.Write(OrbitTimer3);

            writer.Write(WarnedSound);
            writer.Write(boomedSound);
            writer.Write(HasBlownMinefield);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            currentState = (AttackState)reader.ReadInt32();
            RotTime = reader.ReadInt32();
            JabTime = reader.ReadInt32();
            StarsCount1 = reader.ReadInt32();
            StarsCount2 = reader.ReadInt32();
            StarsCount3 = reader.ReadInt32();
            SwordCount = reader.ReadInt32();
            Chargedir = reader.ReadVector2();
            TeleRadius = reader.ReadSingle();

            JabCount = reader.ReadInt32();
            CloneRepeat = reader.ReadInt32();
            AmountofCloning = reader.ReadSingle();
            shootStarsTimer1 = reader.ReadSingle();
            starsShotCount1 = reader.ReadSingle();
            CloneTimer = reader.ReadSingle();
            shootStarsTimer2 = reader.ReadSingle();
            starsShotCount2 = reader.ReadSingle();
            HasCharged = reader.ReadBoolean();
            HasShotStars1 = reader.ReadBoolean();
            HasCloned = reader.ReadBoolean();
            HasShotStars2 = reader.ReadBoolean();
            HasShotStars3 = reader.ReadBoolean();
            shootStarsTimer3 = reader.ReadSingle();
            starsShotCount3 = reader.ReadSingle();
            shootSwordsTimer = reader.ReadSingle();
            swordsShotCount = reader.ReadSingle();
            HasShotSwords = reader.ReadBoolean();
            Phase2 = reader.ReadBoolean();
            Chance = reader.ReadInt32();
            lanceradius = reader.ReadSingle();
            lancerotspeed = reader.ReadSingle();
            HasPlayedPhase2Roar = reader.ReadBoolean();
            StarIndex = reader.ReadInt32();
            LightningCount = reader.ReadInt32();
            lightningTimer = reader.ReadSingle();
            HasSpawnedLighting = reader.ReadBoolean();
            lightbirdTimer = reader.ReadSingle();
            HasSpawnedLightbird = reader.ReadBoolean();
            LightbirdCount = reader.ReadInt32();
            LightbirdCountMax = reader.ReadInt32();
            OrbitTimer1 = reader.ReadSingle();
            OrbitTimer2 = reader.ReadSingle();
            OrbitTimer3 = reader.ReadSingle();

            WarnedSound = reader.ReadBoolean();
            boomedSound = reader.ReadBoolean();
            HasBlownMinefield = reader.ReadBoolean();
        }

        public override void OnSpawn(IEntitySource source)
        {
            Mod.Logger.Info("Constitution Spawned!");
            NPC.velocity = Chargedir;
        }

        public int DeathInterval = 10;
        public override void AI()
        {

            Player player = Main.LocalPlayer;
            DTUtils Utility = new DTUtils();

            Chargedir = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);

            float TeleAngle = Main.rand.NextFloat(0, MathHelper.TwoPi);

            Vector2 TeleCircumferencePoint = player.Center + TeleRadius * new Vector2((float)Math.Cos(TeleAngle), (float)Math.Sin(TeleAngle));

            float lanceangle = Main.GameUpdateCount * lancerotspeed;

            Vector2 offset = new Vector2(MathF.Cos(lanceangle), MathF.Sin(lanceangle)) * lanceradius;

            Vector2 lancecenter = Main.LocalPlayer.Center;

            Vector2 DesperationPos = new Vector2(player.Center.X, player.Center.Y - 1000);


            if (player.dead)
            {
                DeathInterval--;
                if (DeathInterval <= 0)
                {
                    NPC.active = false;
                }
            }

            if (NPC.direction == -1)
            {
                NPC.rotation += MathHelper.Pi + MathHelper.ToRadians(180);
            }

            if (EternityIsActive())
            {
                AttackIntervalDefault = 8;
            }



            /*
                if (NPC.Center.DistanceSQ(player.Center) > 40000)
                {
                    TeleManager(player, ref TeleCircumferencePoint);
                }
                */

                NPC.rotation = (Chargedir * 4f).ToRotation() + MathHelper.PiOver4;


            if (NPC.life <= 0.50f * NPC.lifeMax && !HasPlayedPhase2Roar)
            {
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionBoss/ConstitutionBossKill") with { PitchVariance = 1, MaxInstances = 1, Volume = 8 });
                //SoundEngine.PlaySound(SoundID.Roar);
                MoonlordDeathDrama.RequestLight(1.0f, Main.LocalPlayer.Center);
                Phase2 = true;
                HasPlayedPhase2Roar = true;
            }

            if (Phase2)
            {
                StarsCount1 = 12;
                Chance = 2;
                ColorGradientOverlaySystem.ColorVisibility = 0.7f;
            }

            if (NPC.life < NPC.lifeMax * 0.15f)
            {
                stateWeights[AttackState.Jab] = 0.0f;
                stateWeights[AttackState.LightBird] = 0.0f;
                stateWeights[AttackState.Lightning] = 0.0f;
                stateWeights[AttackState.Minefield] = 0.5f;
                stateWeights[AttackState.RainStars] = 1.0f;
                stateWeights[AttackState.ShootClone] = 0.0f;
                stateWeights[AttackState.ShootStars] = 0.3f;
                stateWeights[AttackState.ShootStars2] = 0.0f;
                stateWeights[AttackState.ShootStars3] = 1.0f;
                stateWeights[AttackState.ShootSwords] = 0.0f;
                stateWeights[AttackState.StellarFlame] = 1.0f;
                stateWeights[AttackState.TeleStars] = 0.5f;
            }

            Mod.Logger.Info($"Current State: {currentState}");

            switch (currentState)
            {
                case AttackState.idlefloat:
                    if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
                    {
                        NPC.aiStyle = 10;

                        if (Main.rand.NextBool(Chance))
                        {
                            currentState = GetRandomState();
                        }

                    }
                    break;
                case AttackState.idlespin:
                    if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
                    {
                        NPC.aiStyle = 5;
                        NPC.rotation += 0.4f * NPC.direction;
                        currentState = GetRandomState();
                    }
                    break;
                case AttackState.Jab:
                    if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
                    {

                        if (NPC.ai[3] < JabCount)
                        {
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 80)
                            {
                                TeleManager(player, ref TeleCircumferencePoint);
                                Chargedir = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionBoss/ConstitutionBossJab") with { PitchVariance = 1, MaxInstances = 1, Volume = 9 });
                                NPC.velocity = Chargedir * 24f;
                                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
                                NPC.ai[2] = 0;
                                NPC.ai[3]++;
                            }
                            if (EternityIsActive() && Main.GameUpdateCount % (AttackIntervalDefault + 4) == 0)
                            {
                                Vector2 velocity = new Vector2(5, 0f).RotatedByRandom(MathHelper.TwoPi);
                                Projectile.NewProjectile(Projectile.GetSource_None(), NPC.Center, velocity, ModContent.ProjectileType<ConstitutionStar>(), 15, 2f, ai2: 2);
                            }
                        }
                        else
                        {
                            stateWeights[AttackState.ShootClone] = 0.1f;
                            stateWeights[AttackState.Lightning] = 1.0f;
                            ResetState();
                        }
                    }
                    break;
                case AttackState.ShootStars:
                    if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
                    {
                        NPC.aiStyle = 10;
                        if (!HasShotStars1)
                        {
                            if (Main.GameUpdateCount % AttackIntervalDefault == 0)
                            {
                                ShootStar1(player);
                                starsShotCount1++; // you might want to track count in separate var
                            }

                            if (starsShotCount1 >= StarsCount1)
                            {
                                HasShotStars1 = true;
                                ResetState();
                            }
                        }
                    }
                    break;
                case AttackState.ShootClone:
                    if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
                    {
                        NPC.aiStyle = 10;
                        if (!HasCloned)
                        {
                            CloneTimer++;
                            if (CloneTimer == 60)
                            {
                                CloneMe();
                                Utility.RadialSpreadProjectile(ModContent.ProjectileType<StellarFlameHostile>(), 9, NPC.Center, 17, 8, 10);
                                HasCloned = true;
                            }
                            if (HasCloned == true)
                            {
                                stateWeights[AttackState.Minefield] = 0.2f;
                                ResetState();
                            }
                        }
                    }
                    break;

                case AttackState.ShootStars2:
                    if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
                    {
                        NPC.aiStyle = 10;
                        NPC.position = lancecenter + offset - new Vector2(NPC.width / 2, NPC.height / 2);
                        OrbitTimer1++;
                        if (!HasShotStars2)
                        {
                            if (!EternityIsActive())
                            {
                                if (Main.GameUpdateCount % AttackIntervalDefault == 0)
                                {
                                    LanceBurst();
                                    starsShotCount2++;
                                }

                                if (starsShotCount2 >= StarsCount2)
                                {
                                    HasShotStars2 = true;
                                    ResetState();
                                }
                                if (OrbitTimer1 >= 600)
                                {
                                    ResetState();
                                }
                            }
                            if (EternityIsActive())
                            {
                                if (Main.GameUpdateCount % AttackIntervalDefault == 0)
                                {
                                    LanceCross(player, Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                                    LanceCrossSpawnFlag = false;
                                    starsShotCount2++;
                                }

                                if (starsShotCount2 >= StarsCount2)
                                {
                                    HasShotStars2 = true;
                                    ResetState();
                                }
                                if (OrbitTimer1 >= 600)
                                {
                                    ResetState();
                                }
                            }
                        }
                    }
                    break;
                case AttackState.ShootStars3:

                    if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
                    {
                        NPC.aiStyle = 10;
                        if (Phase2 == false)
                        {
                            currentState = AttackState.idlefloat;
                        }
                        if (Phase2 == true)
                        {
                            StarSwarm(player);

                            if (!HasShotStars3)
                            {
                                if (Main.GameUpdateCount % AttackIntervalDefault == 0)
                                {
                                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionBoss/ConstitutionBossShootStars3") with { PitchVariance = 1, MaxInstances = 1, Volume = 3 });
                                    StarSwarm(player);
                                    starsShotCount3++; // you might want to track count in separate var
                                }

                                if (starsShotCount3 >= StarsCount3)
                                {

                                    HasShotStars3 = true;
                                    ResetState();
                                }
                            }
                        }
                    }
                    break;
                case AttackState.ShootSwords:
                    if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
                    {
                        NPC.aiStyle = 10;
                        if (Phase2 == false)
                        {
                            currentState = AttackState.idlefloat;
                        }
                        if (Phase2 == true)
                        {
                            if (!HasShotSwords)
                            {
                                if (Main.GameUpdateCount % AttackIntervalDefault == 0)
                                {
                                    ShootSwords();
                                    swordsShotCount++;
                                }

                                if (swordsShotCount >= SwordCount)
                                {
                                    stateWeights[AttackState.Minefield] = 1.0f;
                                    HasShotSwords = true;
                                    ResetState();
                                }
                            }
                        }
                    }
                    break;
                case AttackState.RainStars:
                    if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
                    {
                        NPC.Center = DesperationPos;
                        StarIndex += 10;
                        SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionBoss/ConstitutionBossShootStars3") with { PitchVariance = 1, MaxInstances = 1, Volume = 3 });
                        RainStars(player, velocity: new Vector2(0, 16));
                        if (Main.rand.NextBool(3) && EternityIsActive())
                        {
                            StarFury(player, 12);
                        }
                        currentState = GetRandomState();
                    }
                    break;
                case AttackState.Lightning:
                    if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
                    {
                        NPC.aiStyle = 10;
                        if (!HasShotStars1)
                        {
                            if (Main.GameUpdateCount % AttackIntervalDefault == 0)
                            {
                                StarFury(player, Main.rand.Next(10, 15));
                                LightningCount++;
                            }

                            if (LightningCount >= 4)
                            {
                                HasSpawnedLighting = true;
                                ResetState();
                            }
                        }
                    }
                    break;
                case AttackState.LightBird:
                    if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
                    {
                        NPC.aiStyle = 10;
                        NPC.position = lancecenter + offset - new Vector2(NPC.width / 2, NPC.height / 2);
                        OrbitTimer2++;
                        if (!HasSpawnedLightbird)
                        {
                            if (Main.GameUpdateCount % AttackIntervalDefault + 10 == 0)
                            {
                                ShootLightBird();
                                LightbirdCount++;
                            }

                            if (LightbirdCount >= LightbirdCountMax)
                            {
                                HasSpawnedLightbird = true;
                                ResetState();
                            }
                            if (OrbitTimer2 >= 480)
                            {
                                ResetState();
                            }
                        }
                    }
                    break;
                case AttackState.Minefield:
                    if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
                    {
                        NPC.aiStyle = 10;

                        if (!EternityIsActive())
                        {

                            if (!WarnedSound)
                            {
                                SoundEngine.PlaySound(SoundID.Item90);
                                WarnedSound = true;
                            }

                            if (!HasBlownMinefield)
                            {
                                // Generate unique positions for each mine
                                for (int e = 0; e < MineCount; e++)
                                {
                                    Vector2 minePosition = Main.rand.NextVector2FromRectangle(
                                        new Rectangle(
                                            (int)Main.LocalPlayer.Center.X - Main.screenWidth / 2,
                                            (int)Main.LocalPlayer.Center.Y - Main.screenHeight / 2,
                                            Main.screenWidth,
                                            Main.screenHeight
                                        )
                                    );
                                    MineSpots.Add(minePosition);
                                }

                                Minefield(player);
                                HasBlownMinefield = true;
                            }

                            if (HasBlownMinefield)
                            {
                                stateWeights[AttackState.Minefield] = 0.2f;
                                ResetState();
                            }
                        }
                        if (EternityIsActive())
                        {
                            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/NodeSpawn") with { PitchVariance = 0.4f, MaxInstances = 0 });
                            Utility.RadialSpreadProjectile(ModContent.ProjectileType<StellarBomb>(), 5, NPC.Center, 20, 6, 6);
                            StarFury2(10);
                            ResetState();
                        }
                    }
                    break;
                case AttackState.TeleStars:
                    if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
                    {
                        NPC.aiStyle = 10;
                        if (TeleStarCount < TeleStarMax)
                        {
                            TeleManager(player, ref TeleCircumferencePoint);
                            ShootStar1(player);
                            TeleStarCount++;
                        }
                        if (TeleStarCount >= TeleStarMax)
                        {
                            HasTeleportedandShotStars = true;
                        }

                        if (HasTeleportedandShotStars)
                        {
                            stateWeights[AttackState.Minefield] = 0.2f;
                            ResetState();
                        }

                    }
                    break;
                case AttackState.StellarFlame:
                    if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
                    {
                        if (NPC.life < NPC.lifeMax * 0.15f)
                        {
                            NPC.aiStyle = 10;
                            FlameTimer++;
                            if (HasPlayedFlameSound == false)
                            {
                                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionBoss/FlameShoot") with { Volume = 2, MaxInstances = 1, PitchVariance = 1 });
                                HasPlayedFlameSound = true;
                            }
                            if (FlameTimer < 600)
                            {
                                StellarFlame(player);
                            }
                            if (FlameTimer > 600)
                            {
                                ResetState();
                            }
                        }
                        else
                        {
                            ResetState();
                        }
                    }
                    break;

            }

            if (!Main.dedServ && NPC.life > NPC.lifeMax * 0.15f && !EternityIsActive())
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/ConstitutionBoss");
            }
            if (!Main.dedServ && NPC.life > NPC.lifeMax * 0.15f && EternityIsActive())
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Placeholder5");
            }
            if (!Main.dedServ && NPC.life <= NPC.lifeMax * 0.15f && !EternityIsActive())
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/ConstitutionDespiration");
            }


            int DustAmount = 3;
            if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
            {
                if (Main.rand.NextBool(3))
                {
                    for (int g = 0; g < DustAmount; g++)
                    {
                        Dust.NewDust(NPC.Center, NPC.width + 10, NPC.height + 10, DustID.Enchanted_Pink, 0, 0, 0, default, 1f);
                        Dust.NewDust(NPC.Center, NPC.width + 10, NPC.height + 10, DustID.Enchanted_Gold, 0, 0, 0, default, 1f);
                    }
                }
            }
        }

        private Dictionary<AttackState, float> stateWeights = new()
        {
            { AttackState.idlefloat, 1.0f },
            { AttackState.idlespin, 1.0f },
            { AttackState.Jab, 1.0f },
            { AttackState.LightBird, 1.0f },
            { AttackState.Lightning, 1.0f },
            { AttackState.Minefield, 1.0f },
            { AttackState.RainStars, 1.0f },
            { AttackState.ShootClone, 1.0f },
            { AttackState.ShootStars, 1.0f },
            { AttackState.ShootStars2, 1.0f },
            { AttackState.ShootStars3, 1.0f },
            { AttackState.ShootSwords, 1.0f },
            { AttackState.TeleStars, 0.7f },
            { AttackState.StellarFlame, 1.0f }
            // Add more states as needed
        };


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


        private void ResetState()
        {
            if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
            {
                currentState = GetRandomState();
                NPC.ai[0] = NPC.ai[1] = NPC.ai[2] = NPC.ai[3] = 0;
                shootStarsTimer1 = shootStarsTimer2 = shootStarsTimer3 =
                starsShotCount1 = starsShotCount2 = starsShotCount3 =
                CloneTimer = AmountofCloning = swordsShotCount =
                shootSwordsTimer = FlameTimer = 0;
                MineSpots.Clear();
                HasCharged = false;
                HasShotStars1 = false;
                HasCloned = false;
                HasShotStars2 = false;
                HasShotStars3 = false;
                HasShotSwords = false;
                HasBlownMinefield = false;
                WarnedSound = false;
                HasSpawnedLightbird = false;
                HasSpawnedLighting = false;
                HasTeleportedandShotStars = false;
                HasPlayedFlameSound = false;
                LanceCrossGetPlayerCenterFlag = false;
                LanceCrossSpawnFlag = false;
            }
        }

        #region Attack Methods

        /// <summary>
        /// Teleports near the player.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="TeleCircumferencePoint"></param>
        public void TeleManager(Player player, ref Vector2 TeleCircumferencePoint)
        {
            if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
            {
                NPC.ai[3] += 1;
                int DustAmount = 8;
                SoundEngine.PlaySound(SoundID.Item29, NPC.position);
                ParticleOrchestrator.RequestParticleSpawn(false, ParticleOrchestraType.RainbowRodHit, new ParticleOrchestraSettings { PositionInWorld = TeleCircumferencePoint }, player.whoAmI);
                for (int g = 0; g < DustAmount; g++)
                {
                    Dust.NewDust(NPC.Center, NPC.width + 10, NPC.height + 10, DustID.Enchanted_Pink, 0, 0, 0, default, 1f);
                    Dust.NewDust(NPC.Center, NPC.width + 10, NPC.height + 10, DustID.Enchanted_Gold, 0, 0, 0, default, 1f);
                }
                NPC.Center = TeleCircumferencePoint;
                Chargedir = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            }
        }
        
        /// <summary>
        /// Shoots Stars directly towards the player.
        /// </summary>
        /// <param name="player"></param>

        public void ShootStar1(Player player)
        {
            if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
            {
                SoundEngine.PlaySound(SoundID.Item29, NPC.position);
                int StarAmount = StarsCount3;
                float arcRadius = 200f; // Distance behind the player
                float arcAngle = MathHelper.ToRadians(60); // Total arc angle (e.g., 60 degrees)
                Vector2 directionToTarget = (NPC.Center - player.Center).SafeNormalize(Vector2.UnitY);
                for (int i = 0; i < StarAmount; i++)
                {
                    float t = (StarAmount == 1) ? 0.5f : (float)i / (StarAmount - 1);
                    float angle = MathHelper.Lerp(-arcAngle / 2, arcAngle / 2, t);
                    Vector2 spawnOffset = directionToTarget.RotatedBy(MathHelper.Pi + angle) * arcRadius;
                    Vector2 spawnPosition = NPC.Center + spawnOffset;
                    Vector2 velocity = (player.Center - spawnPosition).SafeNormalize(Vector2.UnitY) * 10f;

                    Projectile harmStar = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), spawnPosition, velocity, ProjectileID.StarWrath, 8, 2);
                    harmStar.timeLeft = 600;
                    harmStar.friendly = false;
                    harmStar.hostile = true;


                }
            }
        }


        /// <summary>
        /// Spawns a clone to chase the player.
        /// </summary>
        public void CloneMe()
        {
            if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
            {
                int DustAmount = 12;
                SoundEngine.PlaySound(SoundID.Item109, NPC.Center);

                // Create the projectile directly and modify it immediately
                Projectile cloneProj = Projectile.NewProjectileDirect(
                    Entity.GetSource_FromThis(),
                    NPC.position,
                    NPC.velocity * 1.2f,
                    ModContent.ProjectileType<ConstitutionClone>(),
                    4,
                    2

                );

                // Make it explicitly hostile

                cloneProj.friendly = false;
                cloneProj.hostile = true;


                for (int g = 0; g < DustAmount; g++)
                {
                    Dust.NewDust(NPC.Center, NPC.width + 10, NPC.height + 10, DustID.Enchanted_Pink, 0, 0, 0, default, 1f);
                    Dust.NewDust(NPC.Center, NPC.width + 10, NPC.height + 10, DustID.Enchanted_Gold, 0, 0, 0, default, 1f);
                }
            }
        }


        /// <summary>
        /// Shoots a radial spread of Galantine Lances. The Count is randomized between 4 and 13.
        /// </summary>
        public void LanceBurst()
        {
            if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
            {
                SoundEngine.PlaySound(SoundID.Item125, NPC.Center);
                DTUtils Utility = new DTUtils();
                Utility.RadialSpreadProjectile(ModContent.ProjectileType<GalantineLance>(), Main.rand.Next(4, 14), NPC.Center, 15, 4, 10);
            }
        }

        public bool LanceCrossGetPlayerCenterFlag = false;
        public bool LanceCrossSpawnFlag = false;
        public Vector2 StaticPlayerCenter = Vector2.Zero;
        /// <summary>
        /// Spawns four lances offscreen. The lances converge at a 90 degree angle, creating a momentary box around the player.
        /// <para/> Eternity Exclusive.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="Rotation"></param>
        /// <param name="Speed"></param>
        public void LanceCross(Player player, float Rotation, float Speed = 30f)
        {
            if (!LanceCrossGetPlayerCenterFlag)
            {
                StaticPlayerCenter = player.Center;
                LanceCrossGetPlayerCenterFlag = true;
            }

            float HalfWidth = 80;
            float DistFromCenter = 4000;
            //Set Spawn positions. Lances spawn 160 units apart with the player in the middle of the two. They spawn and travel parallel to one another.
            Vector2[] spawnPoints = new Vector2[]
            {
                StaticPlayerCenter + new Vector2(DistFromCenter, -HalfWidth),
                StaticPlayerCenter + new Vector2(DistFromCenter, HalfWidth),
                StaticPlayerCenter + new Vector2(HalfWidth, DistFromCenter),
                StaticPlayerCenter + new Vector2(-HalfWidth, DistFromCenter)
            };


            //For these its quite easy, since  we used double negative quadrants, we can just use negative speed for them.
            //The screen area is rectangular, but the lances need to cross at the same time to create the ideal circle effect, so they are spaced the same distance away, and use the same velocities.
            Vector2[] velocities = new Vector2[]
            {
                new Vector2(-Speed, 0),
                new Vector2(-Speed, 0),
                new Vector2(0, -Speed),
                new Vector2(0, -Speed)
            };

            if (LanceCrossSpawnFlag == false)
            {
                int type = ModContent.ProjectileType<GalantineLance>();
                //We need to account for our rotation here. It would be boring if the crosses were always vertical and horizontal to a tee.
                //Rotating our spawn points and velocities should allow us to have functional crossings no matter the rotation.

                for (int i = 0; i < spawnPoints.Length; i++)
                {
                    Vector2 rotatedSpawn = StaticPlayerCenter + (spawnPoints[i] - StaticPlayerCenter).RotatedBy(Rotation);
                    Vector2 rotatedVelocity = velocities[i].RotatedBy(Rotation);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), rotatedSpawn, rotatedVelocity, type, 30, 3f, player.whoAmI);
                }

                LanceCrossSpawnFlag = true;
                LanceCrossGetPlayerCenterFlag = false;
            }
        }

        /// <summary>
        /// Summons multiple Mini-comet projectiles from above the screen down towards the player.
        /// </summary>
        /// <param name="player"></param>
        public void StarSwarm(Player player)
        {
            if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
            {
                int StarAmount = StarsCount3;
                float arcRadius = 200f; // Distance behind the player
                float arcAngle = MathHelper.ToRadians(60); // Total arc angle (e.g., 60 degrees)
                Vector2 directionToTarget = (NPC.Center - player.Center).SafeNormalize(Vector2.UnitY);
                for (int i = 0; i < StarAmount; i++)
                {
                    float t = (StarAmount == 1) ? 0.5f : (float)i / (StarAmount - 1);
                    float angle = MathHelper.Lerp(-arcAngle / 2, arcAngle / 2, t);
                    Vector2 spawnOffset = directionToTarget.RotatedBy(MathHelper.Pi + angle) * arcRadius;
                    Vector2 spawnPosition = NPC.Center + spawnOffset;
                    Vector2 velocity = (player.Center - spawnPosition).SafeNormalize(Vector2.UnitY) * 10f;
                    Projectile.NewProjectile(Entity.GetSource_FromThis(), spawnPosition, velocity * 0.4f, ModContent.ProjectileType<MiniComet>(), 10, 2, ai2: 2);
                }
            }
        }

        /// <summary>
        /// Fires a radial spread of small phantom blades.
        /// <para/> Note from HoleCat: Find a more interesting implementation for this. This is just the lances all over again.
        /// </summary>
        public void ShootSwords()
        {
            if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
            {
                SoundEngine.PlaySound(SoundID.Item125, NPC.Center);
                DTUtils Utility = new DTUtils();
                Utility.RadialSpreadProjectile(ModContent.ProjectileType<GalantineLance>(), Main.rand.Next(4, 14), NPC.Center, 15, 4, 10);
                ParticleOrchestrator.RequestParticleSpawn(false, ParticleOrchestraType.TrueExcalibur, new ParticleOrchestraSettings { PositionInWorld = NPC.Center });;
            }
        }

        /// <summary>
        /// Summons a massive amount of stars from the sky to rain down on the player.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="velocity"></param>
        public void RainStars(Player player, Vector2 velocity)
        {
            //Code taken from ExampleShootingSword.

            Vector2 target = player.Center;
            float ceilingLimit = target.Y;
            if (ceilingLimit > player.Center.Y - 200f)
            {
                ceilingLimit = player.Center.Y - 200f;
            }
            for (int i = 0; i < 6; i++)
            {
                Vector2 position = player.Center - new Vector2(Main.rand.NextFloat(401) * player.direction, 600f);
                position.Y -= 100 * i;
                Vector2 heading = target - position;

                if (heading.Y < 0f)
                {
                    heading.Y *= -1f;
                }

                if (heading.Y < 20f)
                {
                    heading.Y = 20f;
                }

                heading.Normalize();
                heading *= velocity.Length();
                heading.Y += Main.rand.Next(-40, 41) * 0.02f;
                Projectile Rain = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), position, heading, ProjectileID.StarWrath, 16, 4);
                Rain.timeLeft = 600;
                Rain.hostile = true;
                Rain.friendly = false;
            }
        }

        /// <summary>
        /// Throws out multiple Starfury Clones, which slow down before dashing to the player.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="Amount"></param>
        public void StarFury(Player player, int Amount)
        {
            if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
            {
                SoundEngine.PlaySound(SoundID.Item90, NPC.position);
                DTUtils Utility = new DTUtils();
                Utility.RadialProjectileRandomDir(ModContent.ProjectileType<StarfuryClone>(), Main.rand.Next(10, 15), NPC.Center, 15, 4, 10);
            }
        }

        /// <summary>
        /// Selects random positions from the screen and spawns bursts of StarFury clones from them.
        /// </summary>
        /// <param name="Amount"></param>
        public void StarFury2(int Amount)
        {
            Rectangle Screen = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
            DTUtils Utility = new DTUtils();
            for (int t = 0; t < Amount; t++)
            {
                Vector2 SpawnPos = Main.rand.NextVector2FromRectangle(Screen);
                Utility.RadialSpreadProjectile(ModContent.ProjectileType<StarfuryClone>(), Main.rand.Next(4, 14), SpawnPos, 5, 4, Main.rand.Next(10, 25));
            }
        }

        /// <summary>
        /// Shoots a radial spread of light brids.
        /// </summary>
        public void ShootLightBird()
        {
            if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
            {
                SoundEngine.PlaySound(SoundID.Item125, NPC.Center);
                DTUtils Utility = new DTUtils();
                Utility.RadialSpreadProjectile(ModContent.ProjectileType<TrailBlazer>(), 8, NPC.Center, 15, 4, 10);
                ParticleOrchestrator.RequestParticleSpawn(false, ParticleOrchestraType.TrueExcalibur, new ParticleOrchestraSettings { PositionInWorld = NPC.Center }); ;
            }
        }

        /// <summary>
        /// Spawns a randomized field of mines. Is unused in eternity mode.
        /// </summary>
        /// <param name="player"></param>
        public void Minefield(Player player)
        {
            foreach (var minePosition in MineSpots)
            {
                Projectile mine = Projectile.NewProjectileDirect(
                    Projectile.InheritSource(NPC),
                    minePosition, Vector2.Zero,
                    ModContent.ProjectileType<StarMine>(), 0, 0
                );
            }
        }

        /// <summary>
        /// Constitution's Desperation Attack. A stand in for a laser if anything.
        /// </summary>
        /// <param name="player"></param>
        public void StellarFlame(Player player)
        {
            if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
            {
                Vector2 spawnPosition = NPC.Center;
                Vector2 velocity = (player.Center - spawnPosition).SafeNormalize(Vector2.UnitY);
                Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), spawnPosition, velocity * 20, ModContent.ProjectileType<StellarFlameHostile>(), 8, 2);
            }
        }

        #endregion



        public override void OnKill()
        {

            Phase2 = false;
            ColorGradientOverlaySystem.ColorVisibility = 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StellarMatter>(), 2, 4, 35));
        }

    }



    public class ConstitutionClone : ModProjectile
    {
        public override string Texture => "DestroyerTest/Content/Entities/ConstitutionBossClone";
        public override string GlowTexture => "DestroyerTest/Content/Entities/ConstitutionBossClone";



        public ref float DelayTimer => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 64;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 480;
        }

        private Player HomingTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.player[(int)Projectile.ai[0] - 1];
            set => Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
        }




        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            float maxDetectRadius = 2000f; // The maximum radius at which a projectile can detect a target

            if (DelayTimer < 20)
            {
                DelayTimer += 1;
                return;
            }
            // First, we find a homing target if we don't have one
            if (HomingTarget == null)
            {
                HomingTarget = FindTarget(maxDetectRadius);
            }

            // If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
            if (HomingTarget != null && !IsValidTarget(HomingTarget))
            {
                HomingTarget = null;
            }

            // If we don't have a target, don't adjust trajectory
            if (HomingTarget == null)
                return;

            // If found, we rotate the projectile velocity in the direction of the target.
            // We only rotate by 3 degrees an update to give it a smooth trajectory. Increase the rotation speed here to make tighter turns
            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(15)).ToRotationVector2() * length;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;


            Lighting.AddLight(Projectile.Center, 105, 68, 186);

            if (Main.rand.NextBool(3))
            {
                int DustAmount = 8;
                for (int g = 0; g < DustAmount; g++)
                {
                    Dust.NewDust(Projectile.Center, Projectile.width - 10, Projectile.height - 10, DustID.Enchanted_Pink, 0, 0, 0, default, 1f);
                    Dust.NewDust(Projectile.Center, Projectile.width - 10, Projectile.height - 10, DustID.Enchanted_Gold, 0, 0, 0, default, 1f);
                }
            }

        }

        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null
        public Player FindTarget(float maxDetectDistance)
        {
            Player ClosestTarget = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs
            foreach (var target in Main.player)
            {
                // Check if NPC able to be targeted. 
                if (IsValidTarget(target))
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        ClosestTarget = target;
                    }
                }
            }
            return ClosestTarget;
        }

        public bool IsValidTarget(Player target)
        {
            return target.active && target.Distance(Projectile.Center) < 2000 && target.statLife > 1 && !target.invis;
        }

        /*
        public class ConstitutionSubprojectiles : GlobalProjectile
        {
            public override void OnSpawn(Projectile projectile, IEntitySource source)
            {
                if (source is EntitySource_Parent parentSource && parentSource.Entity is NPC npc && npc.type == ModContent.NPCType<ConstitutionBoss>())
                {
                    return;
                }
            }
        }
        */

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<GalantineBurn>(), 240);
            Projectile boom = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CloneCollisionBoom>(), 0, 0);
            Projectile.Kill();
        }

    }

    public class StarMine : ModProjectile
    {


        public override string Texture => "DestroyerTest/Content/Particles/ParticleDrawEntity";
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            // Increase size over time

            if (Projectile.timeLeft == 1)
            {
                Projectile mine = Projectile.NewProjectileDirect(
                   Projectile.GetSource_FromThis(),
                   Projectile.Center, Vector2.Zero,
                   ProjectileID.PrincessWeapon, 20, 1
               );
                mine.friendly = false;
                mine.hostile = true;
                mine.Name = "Stellar Explosion";
            }


        }


        public override bool PreDraw(ref Color lightColor)
        {
            // Set base color and adjust transparency based on time left
            lightColor = ColorLib.StellarColor;

            // Prepare for sprite drawing
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D glowTexture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/WarningTriangle").Value;

            // End previous batch before starting a new one
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            // Draw the expanding glow ring
            Main.EntitySpriteDraw(glowTexture, Projectile.Center - Main.screenPosition, null, lightColor, 0f, glowTexture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            // Restore default sprite batch
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item103);
        }
    }




    public class ConstitutionBossfight : ModSceneEffect
    {
        private readonly int constitutionBossType = ModContent.NPCType<ConstitutionBoss>();
        public override bool IsSceneEffectActive(Player player)
        {
            int npcIndex = NPC.FindFirstNPC(constitutionBossType);
            return npcIndex != -1 && player.Distance(Main.npc[npcIndex].Center) < 3000;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (IsSceneEffectActive(player))
            {
                Main.SceneMetrics.ShimmerMonolithState = 1;
            }
        }

        ///public override int Music
        //=> MusicLoader.GetMusicSlot(Mod, "Assets/Music/ConstitutionBoss");

        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;

    }

    public class CloneCollisionBoom : ModProjectile
    {
        public override string Texture => "DestroyerTest/Content/Particles/ParticleDrawEntity"; // Path to the texture for the projectile
        private const float MaxSizeMultiplier = 2.5f; // Maximum scale increase
        private const int FadeOutStartTime = 10; // Time left when fading starts
        private const int MaxLifetime = 30; // Total lifetime of the ring effect



        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = MaxLifetime;
            Projectile.scale = 0.1f; // Start small
        }

        float randomRotation = Main.rand.NextFloat(0f, MathHelper.TwoPi); // Random rotation

        bool backupCheck = false;

        public override void OnSpawn(IEntitySource source)
        {
            if (backupCheck == false)
            {
                Projectile.rotation = randomRotation;
                backupCheck = true;
            }
        }


        public override void AI()
        {

            // Increase size over time
            float lifeRatio = (MaxLifetime - Projectile.timeLeft) / (float)MaxLifetime;
            Projectile.scale = MathHelper.Lerp(0.1f, MaxSizeMultiplier, lifeRatio);



            if (Projectile.scale > 3.0f)
            {
                Projectile.Kill();
            }
        }



        public override bool PreDraw(ref Color lightColor)
        {

            lightColor = ColorLib.StellarColor;


            if (Projectile.timeLeft < FadeOutStartTime)
            {
                float fadeFactor = Projectile.timeLeft / (float)FadeOutStartTime;
                lightColor *= fadeFactor; // Fade out as time ends
            }

            // Prepare for sprite drawing
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D glowTexture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/Boom1").Value;

            // End previous batch before starting a new one
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            // Draw the expanding glow ring

            Main.EntitySpriteDraw(glowTexture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, glowTexture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            // Restore default sprite batch
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }
    }



    public class DummyPlayerProjectileHelper : ModSystem
    {
        private static Player dummyPlayer;

        public override void Load()
        {
            EnsureDummyPlayerExists();
        }

        // Initializes the dummy player if it does not exist.
        private static void EnsureDummyPlayerExists()
        {
            if (dummyPlayer == null)
            {
                dummyPlayer = new Player();
                dummyPlayer.name = "DummyPlayer";
                dummyPlayer.active = true;
                dummyPlayer.whoAmI = Main.maxPlayers - 1; // Use the last player index as a safe dummy
            }
        }

        // Assigns the projectile a dummy player owner.
        public static void AssignDummyPlayerOwner(Projectile projectile)
        {
            EnsureDummyPlayerExists();

            // Only assign dummy owner if the current owner is invalid
            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
            {
                projectile.owner = dummyPlayer.whoAmI;
            }
        }

        // Checks if a projectile is owned by the dummy player.
        public static bool IsDummyOwned(Projectile projectile)
        {
            EnsureDummyPlayerExists();
            return projectile.owner == dummyPlayer.whoAmI;
        }

        // Ensure the dummy player exists in the world
        public override void OnWorldLoad()
        {
            EnsureDummyPlayerExists();
        }
    }

    public class ConstitutionBCL : ModSystem
    {
        public override void PostSetupContent() {
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
			string internalName = "Constitution";

			// Value inferred from boss progression, see the wiki for details
			float weight = 6.8f;

			// Used for tracking checklist progress
			Func<bool> downed = () => DownedBossSystem.downedConstitutionBoss;

			LocalizedText Hint = Language.GetText("Mods.DestroyerTest.BossChecklist.Constitution.Hint");

			// The NPC type of the boss
			int bossType = ModContent.NPCType<ConstitutionBoss>();

			// The item used to summon the boss with (if available)
			int spawnItem = ModContent.ItemType<CursedStar>();


			// "collectibles" like relic, trophy, mask, pet
            List<int> collectibles = new List<int>()
            {
                ModContent.ItemType<StellarTintedGoggles>(),
                ModContent.ItemType<Constitution>(),
                ModContent.ItemType<StellarBow>(),
                ModContent.ItemType<StellarFlames>(),
                ModContent.ItemType<Item_ConstitutionRelic>(),
                ModContent.ItemType<Item_ConstitutionTrophy>()
			};

			// By default, it draws the first frame of the boss, omit if you don't need custom drawing
			// But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
			var customPortrait = (SpriteBatch sb, Rectangle rect, Color color) =>
			{
				Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/ConstitutionBossChecklist").Value;
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
                    ["despawnMessage"] = (Func<NPC, LocalizedText>)(npc =>
                        Language.GetText("Mods.DestroyerTest.NPCs.ConstitutionBoss.DespawnMessage").WithFormatArgs(npc.FullName)
                    ),

					// Other optional arguments as needed are inferred from the wiki
                }
			);
			

			// Other bosses or additional Mod.Call can be made here.
		}
    }

    }

