using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Stellar;
using DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.Tiles;
using Humanizer.Localisation.DateToOrdinalWords;
using log4net.Repository.Hierarchy;
using Microsoft.Build.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Social.Base;
using Terraria.Utilities.Terraria.Utilities;

namespace DestroyerTest.Content.Entities
{
    public class ConstitutionDamageValues
    {
        public static int BeamDamage()
        {
            if (DTUtils.ClassicMode())
            {
                return 12;
            }
            if (Main.expertMode && !Main.masterMode)
            {
                return 16;
            }
            if (Main.masterMode)
            {
                return 20;
            }

            if (DTUtils.CalamityBossRushActive())
            {
                return 70;
            }

            return 12;
        }

        public static int WallSweepDamage()
        {
            if (DTUtils.ClassicMode())
            {
                return 10;
            }
            if (Main.expertMode && !Main.masterMode)
            {
                return 12;
            }
            if (Main.masterMode)
            {
                return 18;
            }

            if (DTUtils.CalamityBossRushActive())
            {
                return 55;
            }

            return 10;
        }

        public static int DashSlashDamage()
        {
            if (DTUtils.ClassicMode())
            {
                return 12;
            }
            if (Main.expertMode && !Main.masterMode)
            {
                return 18;
            }
            if (Main.masterMode)
            {
                return 22;
            }

            if (DTUtils.CalamityBossRushActive())
            {
                return 130;
            }

            return 12;
        }

        public static int StellarVolleyDamage()
        {
            if (DTUtils.ClassicMode())
            {
                return 10;
            }
            if (Main.expertMode && !Main.masterMode)
            {
                return 16;
            }
            if (Main.masterMode)
            {
                return 8;
            }

            if (DTUtils.CalamityBossRushActive())
            {
                return 90;
            }

            return 16;
        }

        public static int EternityStellarBombDamage()
        {
            if (Main.masterMode)
            {
                return 40;
            }
            return 20;
        }

        public static int EternityStarfuryCloneDamage()
        {
            if (Main.masterMode)
            {
                return 30;
            }
            return 15;
        }

        public static int EternityLanceDamage()
        {
            if (Main.masterMode)
            {
                return 16;
            }
            return 8;
        }
    }

    public class ConstitutionSounds
    {
        public static SoundStyle Shoot1 = new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionBoss/ConstitutionBossShootStars3");
        public static SoundStyle StellarVolley = new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionBoss/StellarVolley");
        public static SoundStyle WallWarn = new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/CursedFlamesWarn");
        public static SoundStyle Teleport = DTAssetLib.SwordSounds.ConSwing;
        public static SoundStyle Dash = DTAssetLib.SwordSounds.MagicSwing;
    
    }

    [AutoloadBossHead]
    public class ConstitutionBoss : ModNPC, IDrawPixelated
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
            NPC.width = 52;
            NPC.height = 50;
            NPC.aiStyle = -1;
            NPC.damage = 24;
            NPC.defense = 24;

            if (!DestroyerTestMod.EternityIsActive)
            {
                NPC.lifeMax = 6000;
                NPC.defense = 20;
            }
            if (DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
            {
                NPC.lifeMax = 8000;
                NPC.defense = 25;
            }
            if (DestroyerTestMod.MasochistIsActive)
            {
                NPC.lifeMax = 10000;
                NPC.defense = 30;
            }
            if (DTUtils.CalamityBossRushActive())
            {
                NPC.lifeMax = 380000;
                NPC.defense = 70;
            }

     
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
                new FlavorTextBestiaryInfoElement(DTUtils.GetModNPCLocalizationEntry(this, 1)),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface
            });
        }

        public override bool CheckActive()
        {
            return false;
        }

        public Vector2 ArenaCTR;
        public bool Flag1 = false;

        public override void OnSpawn(IEntitySource source)
        {
            if (DTCrossMod.FablesIsLoaded)
            {
                FablesTitleCardSystem.RegisterFablesBossIntro(new FablesTitleCardSystem.ConstitutionTitle());
            }
            
        }

        public Line topSide = new Line(ArenaRect.TopRight(), ArenaRect.TopLeft());
        public Line bottomSide = new Line(ArenaRect.BottomLeft(), ArenaRect.BottomRight());
        public Line leftSide = new Line(ArenaRect.TopLeft(), ArenaRect.BottomLeft());
        public Line rightSide = new Line(ArenaRect.BottomRight(), ArenaRect.TopRight());
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            
        }

        public int WO1 = 0;
        public int WO2 = 0;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            

            if (LancesEternity)
            {
                spriteBatch.Draw(DTAssetLib.ConstitutionLanceWarning.Value, ArenaCTR - Main.screenPosition, null, ColorLib.StellarFireGradientLooping() * LanceWarningOpacity, 0f, DTAssetLib.ConstitutionLanceWarning.Value.Size() / 2, 1f, SpriteEffects.None, 0f);
            }
            DTConfig cfg = ModContent.GetInstance<DTConfig>();
            if (cfg.EnableDebugMessages)
            {
                Utils.DrawBorderString(spriteBatch, AITimer.ToString(), (NPC.Center - new Vector2(0, 40)) - Main.screenPosition, Color.Red, 1f);
            }


            return true;
        }

        public void ScrollingTextureSpine(Line line, Asset<Texture2D> texture, Color drawColor, SpriteBatch spriteBatch, int TexOffset, float Width = 1f)
        {

            if (texture == null)
            {
                Main.NewText("ScrollingTextureSpine: Texture is null. Aborted draw.", Color.Red);
                return;
            }


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, PixelationSystem.PixelationMatrix);

            spriteBatch.Draw(texture.Value, line.Start - Main.screenPosition, new Rectangle(TexOffset, 0, (int)line.GetLineLength, texture.Value.Height), drawColor with { A = 0 }, line.GetLineRotation, new Vector2(0, texture.Value.Height) / 2, new Vector2(1, Width), SpriteEffects.None, 0);

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }

        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            WO1 += 16;
            WO2 += 6;

            float OuterWidth = Opus.Sine(1f, 0.6f);
            ScrollingTextureSpine(topSide, DTAssetLib.Streak(2, true), ColorLib.StellarFireGradientLooping() * 0.75f, spriteBatch, WO1, OuterWidth);
            ScrollingTextureSpine(bottomSide, DTAssetLib.Streak(2, true), ColorLib.StellarFireGradientLooping() * 0.75f, spriteBatch, WO1, OuterWidth);
            ScrollingTextureSpine(leftSide, DTAssetLib.Streak(2, true), ColorLib.StellarFireGradientLooping() * 0.75f, spriteBatch, WO1, OuterWidth);
            ScrollingTextureSpine(rightSide, DTAssetLib.Streak(2, true), ColorLib.StellarFireGradientLooping() * 0.75f, spriteBatch, WO1, OuterWidth);

            ScrollingTextureSpine(topSide, DTAssetLib.Streak(1, true), DTColorUtils.Pastel(ColorLib.StellarFireGradientLooping(), 0.9f), spriteBatch, WO2, 0.2f);
            ScrollingTextureSpine(bottomSide, DTAssetLib.Streak(1, true), DTColorUtils.Pastel(ColorLib.StellarFireGradientLooping(), 0.9f), spriteBatch, WO2, 0.2f);
            ScrollingTextureSpine(leftSide, DTAssetLib.Streak(1, true), DTColorUtils.Pastel(ColorLib.StellarFireGradientLooping(), 0.9f), spriteBatch, WO2, 0.2f);
            ScrollingTextureSpine(rightSide, DTAssetLib.Streak(1, true), DTColorUtils.Pastel(ColorLib.StellarFireGradientLooping(), 0.9f), spriteBatch, WO2, 0.2f);

        }


        public int AITimer = 0;

        public Player player => Main.player[NPC.target];

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveTiles;

        public override void AI()
        {

            if (NPC.HasValidTarget)
            {
                if (!Flag1)
                {
                    ArenaCTR = player.Center;
                    Flag1 = true;
                }
            }
            else
            {
                NPC.TargetClosest();
            }

            if (player.statLife <= 0)
            {
                HandleDeath();
            }
            else
            {
                Arena();
            }

            

            AITimer++;

            if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.DeathIsActive)
            {
                NormalAI();
                Music = MusicLoader.GetMusicSlot("DestroyerTest/Assets/Music/ConstitutionBoss");
            }

            if (DestroyerTestMod.EternityIsActive && !DestroyerTestMod.DeathIsActive)
            {
                EternityAI();
                Music = MusicLoader.GetMusicSlot("DestroyerTest/Assets/Music/ConstEternityPlaceholder");
            }

            if (!DestroyerTestMod.EternityIsActive && DestroyerTestMod.DeathIsActive)
            {
                EternityAI();
                Music = MusicLoader.GetMusicSlot("DestroyerTest/Assets/Music/ConstEternityPlaceholder");
            }

            if (DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive)
            {
                EternityAI();
                Music = MusicLoader.GetMusicSlot("DestroyerTest/Assets/Music/ConstEternityPlaceholder");
            }

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;

            
        }

        public void NormalAI()
        {
            if (AITimer < 300 && AITimer >= 0)
            {
                Side = Main.rand.NextBool() ? 1 : -1;
                VolleyTele = false;
                IdleAI();
            }
            if (AITimer < 1200 && AITimer >= 300)
            {
                int interval1 = DTUtils.CalamityBossRushActive() ? 40 : 80;
                if (AITimer % interval1 == 0)
                {
                    BeamBoomAI();
                }
            }
            if (AITimer < 1740 && AITimer >= 1200)
            {
                WallShootAI();
            }
            if (AITimer < 3200 && AITimer >= 1740)
            {
                DashAI();
            }
            if (AITimer > 3500 && AITimer >= 3200)
            {
                IdleAI();
                WallShootCount = 0;
                DashCount = 0;
            }
            if (AITimer > 4200 && AITimer >= 3500)
            {
                VolleyAI();
            }
            if (AITimer >= 4800)
            {
                AITimer = 0;
            }
        }

        public float LanceWarningOpacity = 0f;
        public bool SpawnedHomingSlash = false;

        List<Projectile> OrbitingMines = new();
        
        public void EternityAI()
        {
            Vector2[] P = Opus.GetEquidistantOrbitVectors(Main.masterMode ? 9 : 7, ArenaCTR, 0.01f, 500);

            for (int i = OrbitingMines.Count - 1; i >= 0; i--)
            {
                if (!OrbitingMines[i].active)
                {
                    OrbitingMines.RemoveAt(i);
                    continue;
                }

                if (i < P.Length)
                    OrbitingMines[i].Center = P[i];
            }

            if (AITimer < 300 && AITimer >= 0)
            {
                Side = Main.rand.NextBool() ? 1 : -1;
                VolleyTele = false;
                IdleAI();
            }
            if (AITimer < 1200 && AITimer >= 300)
            {
                if (AITimer % 120 == 0)
                {
                    SoundEngine.PlaySound(ConstitutionSounds.Shoot1);
                    if (Main.masterMode)
                    {
                        EternityMineAI(P);
                    }
                    else
                    {
                        EternityMineAI(P);
                    }
                }

                if (NPC.life <= NPC.lifeMax / 2)
                {
                    if (AITimer % 10 == 0)
                    {
                        Vector2 Starspawn = ArenaCTR + new Vector2(Main.rand.NextFloat(-10, 10), -((ArenaRect.Height / 2) + 80));
                        Projectile G = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), Starspawn, new Vector2(0, 20f), ModContent.ProjectileType<ConstitutionStarHostile_NoHoming>(), ConstitutionDamageValues.StellarVolleyDamage(), 8);
                        G.timeLeft = 120;
                    }
                }
            }
            if (AITimer < 1300 && AITimer >= 1200)
            {
                OrbitingMines.Clear();
                IdleAI();
            }
            if (AITimer < 2400 && AITimer >= 1300)
            {
                EternArenaMod = true;
                //if (AITimer == 1300) { Main.NewText("HIT 1300"); }
                if (ArenaRect.Width > 140)
                {
                    int Width = (int)MathHelper.Lerp(ArenaRect.Width, 140, 0.1f);
                    if (Math.Abs(Width - 140) < 1)
                    {
                        Width = 140;
                    }
                    EternArenaWidth = Width;
                    ArenaRect = Utils.CenteredRectangle(ArenaCTR, new Vector2(Width, 1500));
                }
                else
                {
                    if (NPC.HasValidTarget)
                    {
                        NPC.Center = new Vector2(ArenaCTR.X + 560, player.Center.Y);
                    }
                    if (AITimer % 60 == 0)
                    {
                        SoundEngine.PlaySound(DTAssetLib.ScholarShieldSounds.Activate);
                        Opus.RadialSpreadProjectile(ModContent.ProjectileType<StarfuryClone>(), 8, NPC.Center, ConstitutionDamageValues.EternityStarfuryCloneDamage(), 3, 6, offset: NPC.rotation);
                    }
                }
            }
            if (AITimer < 3600 && AITimer >= 2400)
            {
                if (ArenaRect.Width < 1500)
                {
                    int Width = (int)MathHelper.Lerp(ArenaRect.Width, 1500, 0.1f);
                    if (Math.Abs(Width - 1500) < 1)
                    {
                        Width = 1500;
                    }
                    EternArenaWidth = Width;
                    ArenaRect = Utils.CenteredRectangle(ArenaCTR, new Vector2(Width, 1500));
                }
                else
                {
                    EternArenaMod = false;

                    
                }

                EternityLanceAI();

                if (ShotLance)
                {
                    if (AITimer % 120 == 0)
                    {
                        ShotLance = false;
                    }
                }
            }
            if (AITimer < 4800 && AITimer >= 3600)
            {
                SpawnedHomingSlash = false;
                
            }
            if (AITimer > 3600)
            {
                EternityOrbitBursts();
            }
            if (AITimer > 4200)
            {
                AITimer = 0;
            }
        }

        public bool EternArenaMod = false;
        public int EternArenaWidth = 0;

        public static Rectangle ArenaRect;
        public List<Projectile>Corners = new List<Projectile>();
        public void Arena()
        {
            Player player = Main.player[NPC.target];
            if (!EternArenaMod)
            {
                ArenaRect = Utils.CenteredRectangle(ArenaCTR, new Vector2(1500, 1500));
            }
            float HalfWidth = 750f;
            float HalfHeight = 750f;
            if (EternArenaMod)
            {
                HalfWidth = EternArenaWidth / 2;
            }

            Vector2 arenaCenter = ArenaCTR;

            float left   = arenaCenter.X - HalfWidth;
            float right  = arenaCenter.X + HalfWidth;
            float top    = arenaCenter.Y - HalfHeight;
            float bottom = arenaCenter.Y + HalfHeight;

            // X bounds
            if (player.position.X < left)
            {
                player.position.X = left;
                if (player.velocity.X < 0)
                    player.velocity.X = 0;
            }
            else if (player.position.X + player.width > right)
            {
                player.position.X = right - player.width;
                if (player.velocity.X > 0)
                    player.velocity.X = 0;
            }

            // Y bounds
            if (player.position.Y < top)
            {
                player.position.Y = top;
                if (player.velocity.Y < 0)
                    player.velocity.Y = 0;
            }
            else if (player.position.Y + player.height > bottom)
            {
                player.position.Y = bottom - player.height;
                if (player.velocity.Y > 0)
                    player.velocity.Y = 0;
            }

            topSide    = new Line(ArenaRect.TopRight(), ArenaRect.TopLeft());
            bottomSide = new Line(ArenaRect.BottomLeft(), ArenaRect.BottomRight());
            leftSide   = new Line(ArenaRect.TopLeft(), ArenaRect.BottomLeft());
            rightSide  = new Line(ArenaRect.BottomRight(), ArenaRect.TopRight());

            
            int cornerID = ModContent.ProjectileType<ConstitutionArenaCorner>();

            if (Corners.Count < 4)
            {
                Projectile corner = Projectile.NewProjectileDirect(NPC.GetSource_Misc("CornerSpawn"), NPC.Center, Vector2.Zero, cornerID, 0, 0);
                Corners.Add(corner);
            }
            else
            {
                Corners[0].Center = ArenaRect.TopLeft();
                Corners[1].Center = ArenaRect.TopRight();
                Corners[2].Center = ArenaRect.BottomLeft();
                Corners[3].Center = ArenaRect.BottomRight();
            }

            //Opus.RectDustRandom(DustID.TintableDustLighted, ArenaRect,  ColorLib.StellarFireGradientLooping(), 1f, 20);
            
            if (Main.rand.NextBool(3))
            {
                ConstitutionParticle Ambience = new();
                Ambience.Initialize(Main.rand.NextVector2FromRectangle(ArenaRect), new Vector2(0, Main.rand.NextFloat(-1.5f, -0.1f)), 2.5f, 100);
                ParticleEngine.BehindProjectiles.Add(Ambience);
            }
        }

        public int DeathTimer = 0;
        int MaxDeathTimer = 120;

        bool HasRecorded = false;

        int RecordedWidth;
        int RecordedHeight;

        float arenaWidth;
        float arenaHeight;

        void RecordArenaDims(out int width, out int height)
        {
            if (!HasRecorded)
            {
                RecordedWidth = ArenaRect.Width;
                RecordedHeight = ArenaRect.Height;
                HasRecorded = true;
            }

            width = RecordedWidth;
            height = RecordedHeight;
        }

        public void HandleDeath()
        {
            DeathTimer++;
            RecordArenaDims(out int W, out int H);

            float Progress = (float)DeathTimer / (float)MaxDeathTimer;
            Progress = MathHelper.Clamp(Progress, 0f, 1f);

            arenaWidth = MathHelper.Lerp(W, 0f, Progress);
            arenaHeight = MathHelper.Lerp(H, 0f, Progress);

            ArenaRect = Utils.CenteredRectangle(
                ArenaCTR,
                new Vector2(arenaWidth, arenaHeight)
            );

            topSide = new Line(ArenaRect.TopRight(), ArenaRect.TopLeft());
            bottomSide = new Line(ArenaRect.BottomLeft(), ArenaRect.BottomRight());
            leftSide = new Line(ArenaRect.TopLeft(), ArenaRect.BottomLeft());
            rightSide = new Line(ArenaRect.BottomRight(), ArenaRect.TopRight());
            Corners[0].Center = ArenaRect.TopLeft();
            Corners[1].Center = ArenaRect.TopRight();
            Corners[2].Center = ArenaRect.BottomLeft();
            Corners[3].Center = ArenaRect.BottomRight();


            NPC.Opacity = MathHelper.Lerp(1f, 0f, Progress);

            if (DeathTimer == MaxDeathTimer - 1)
            {
                SoundEngine.PlaySound(DTAssetLib.Impacts.StellarFox, NPC.Center);

                List<Vector2> Star2 = Polar.GenerateCurvedStar(5, 4, 90, NPC.Center, inwardPull: 0.5f, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                foreach (Vector2 p2 in Star2)
                {
                    Vector2 Vel = p2 - NPC.Center;

                    ConstitutionParticle Particle = new();
                    Particle.Initialize(NPC.Center, Vel, 1f, 30);
                    ParticleEngine.BehindProjectiles.Add(Particle);
                }

            }
            if (DeathTimer == MaxDeathTimer)
            {
                NPC.active = false;
            }
        }

        public  void TeleportFX()
        {
            SoundEngine.PlaySound(ConstitutionSounds.Teleport, NPC.Center);
            int points = 10; // 5 outer + 5 inner
            float outerRadius = 16f;
            float innerRadius = outerRadius * 0.4f;
            float rotationOffset = NPC.rotation;

            for (int i = 0; i < points; i++)
            {
                float angle = MathHelper.TwoPi * i / points + rotationOffset;
                float radius = (i % 2 == 0) ? outerRadius : innerRadius;

                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vector2 spawnPos = NPC.Center + direction * radius;
                Vector2 velocity = direction * 3f;

                List<Vector2> Star2 = Polar.GenerateCurvedStar(5, 4, 90, NPC.Center, inwardPull: 0.5f, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                foreach (Vector2 p2 in Star2)
                {
                    Vector2 Vel = p2 - NPC.Center;

                    ConstitutionParticle Particle = new();
                    Particle.Initialize(NPC.Center, Vel, 1f, 30);
                    ParticleEngine.BehindProjectiles.Add(Particle);
                }
            }

            StellarParticleUtils.FlatStar(NPC.Center, 1f, ParticleEngine.BehindProjectiles);
        }

        public void IdleAI()
        {
            Player player = Main.player[NPC.target];
            NPC.aiStyle = NPCAIStyleID.CursedSkull;
        }

        public int BeamBoomCount(bool Double, bool Half)
        {
            if (Half)
            {
                return 3;
            }
            if (Double)
            {
                return 12;
            }
            else
            {
                return 6;
            }
        }
        public void BeamBoomAI()
        {
            SoundEngine.PlaySound(ConstitutionSounds.Shoot1 with { PitchVariance = 0.4f }, NPC.Center);
            Opus.RadialSpreadProjectile(ModContent.ProjectileType<ConstitutionBeam>(), BeamBoomCount(Main.masterMode, DTUtils.ClassicMode()), NPC.Center, ConstitutionDamageValues.BeamDamage(), 10, 8, offset: Main.rand.NextFloat(MathHelper.TwoPi));
        }

        public int WallShotCount(bool Double, bool Half)
        {
            if (Half)
            {
                return 7;
            }
            if (Double)
            {
                return 13;
            }
            else
            {
                return 10;
            }
        }

        public int WallShootCount = 0;
        public void WallShootAI()
        {
            Player player = Main.player[NPC.target];
            Vector2[] tops = topSide.GetPointsAlongLine(WallShotCount(Main.masterMode, DTUtils.ClassicMode()));
            Vector2[] rights = rightSide.GetPointsAlongLine(WallShotCount(Main.masterMode, DTUtils.ClassicMode()));
            Vector2[] bottoms = bottomSide.GetPointsAlongLine(WallShotCount(Main.masterMode, DTUtils.ClassicMode()));
            Vector2[] lefts = leftSide.GetPointsAlongLine(WallShotCount(Main.masterMode, DTUtils.ClassicMode()));
            
            NPC.aiStyle = -1;
            NPC.Center = ArenaCTR;
            if (WallShootCount == 0)
            {
                TeleportFX();
            }
            WallShootCount++;

            // TOP
            if (WallShootCount == 60)
            {
                SoundEngine.PlaySound(ConstitutionSounds.WallWarn);
                for(int i = 0; i < tops.Length; i++)
                {
                    Vector2 shotpos = tops[i];
                    for(int t = 0; t < 8; t++)
                    {
                        ConstitutionParticle particle = new();
                        particle.Initialize(shotpos, new Vector2(0, 40), 1.6f, 60);
                        ParticleEngine.BehindProjectiles.Add(particle);

                    }
                }
                if (Main.expertMode)
                {
                    Vector2 v = player.Center - NPC.Center;
                    v.Normalize();
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, v * 4, ModContent.ProjectileType<StellarBomb>(), 35, 0);
                }
            }
            if (WallShootCount == 120)
            {
                for(int i = 0; i < tops.Length; i++)
                {
                    Vector2 shotpos = tops[i];
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), shotpos, new Vector2(0, 20), ModContent.ProjectileType<ConstitutionStarHostile_NoHoming>(), ConstitutionDamageValues.WallSweepDamage(), 0);
                }
            }

            // RIGHT
            if (WallShootCount == 180)
            {
                SoundEngine.PlaySound(ConstitutionSounds.WallWarn);
                for(int i = 0; i < rights.Length; i++)
                {
                    Vector2 shotpos = rights[i];
                    for(int t = 0; t < 8; t++)
                    {
                        ConstitutionParticle particle = new();
                        particle.Initialize(shotpos, new Vector2(-40, 0), 1.6f, 60);
                        ParticleEngine.BehindProjectiles.Add(particle);
                    }
                }
                if (Main.expertMode)
                {
                    Vector2 v = player.Center - NPC.Center;
                    v.Normalize();
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, v * 4, ModContent.ProjectileType<StellarBomb>(), 35, 0);
                }
            }
            if (WallShootCount == 240)
            {
                for(int i = 0; i < rights.Length; i++)
                {
                    Vector2 shotpos = rights[i];
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), shotpos, new Vector2(-20, 0), ModContent.ProjectileType<ConstitutionStarHostile_NoHoming>(), ConstitutionDamageValues.WallSweepDamage(), 0);
                }
            }

            //BOTTOM
            if (WallShootCount == 300)
            {
                SoundEngine.PlaySound(ConstitutionSounds.WallWarn);
                for(int i = 0; i < bottoms.Length; i++)
                {
                    Vector2 shotpos = bottoms[i];
                    for(int t = 0; t < 8; t++)
                    {
                        ConstitutionParticle particle = new();
                        particle.Initialize(shotpos, new Vector2(0, -40), 1.6f, 60);
                        ParticleEngine.BehindProjectiles.Add(particle);
                    }
                }
                if (Main.expertMode)
                {
                    Vector2 v = player.Center - NPC.Center;
                    v.Normalize();
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, v * 4, ModContent.ProjectileType<StellarBomb>(), 35, 0);
                }
            }
            if (WallShootCount == 360)
            {
                for(int i = 0; i < bottoms.Length; i++)
                {
                    Vector2 shotpos = bottoms[i];
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), shotpos, new Vector2(0, -20), ModContent.ProjectileType<ConstitutionStarHostile_NoHoming>(), ConstitutionDamageValues.WallSweepDamage(), 0);
                }
            }

            //LEFT
            if (WallShootCount == 420)
            {
                SoundEngine.PlaySound(ConstitutionSounds.WallWarn);
                for(int i = 0; i < lefts.Length; i++)
                {
                    Vector2 shotpos = lefts[i];
                    for(int t = 0; t < 8; t++)
                    {
                        ConstitutionParticle particle = new();
                        particle.Initialize(shotpos, new Vector2(40, 0), 1.6f, 60);
                        ParticleEngine.BehindProjectiles.Add(particle);
                    }
                }
                if (Main.expertMode)
                {
                    Vector2 v = player.Center - NPC.Center;
                    v.Normalize();
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, v * 4, ModContent.ProjectileType<StellarBomb>(), 35, 0);
                }
            }
            if (WallShootCount == 480)
            {
                for(int i = 0; i < lefts.Length; i++)
                {
                    Vector2 shotpos = lefts[i];
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), shotpos, new Vector2(20, 0), ModContent.ProjectileType<ConstitutionStarHostile_NoHoming>(), ConstitutionDamageValues.WallSweepDamage(), 0);
                }
            }

            if (WallShootCount >= 540)
            {
                
                return;
            }
        }

        Vector2 chargeDirection;
        bool charging = false;
        int chargeWindup = 30;
        int chargeDuration = 60;
        float chargeSpeed = 14f;
        float turnRate = 0.02f;
        public int DashCount = 0;
        public int DashCooldown = 0;
        public void DashAI()
        {
            Player player = Main.player[NPC.target];
            NPC.aiStyle = -1;

            if (DashCount >= 5)
            {
                charging = false;
                NPC.velocity = Vector2.Zero;
                NPC.aiStyle = NPCAIStyleID.CursedSkull;
                AITimer = 3200;
                return;
            }

            if (DashCooldown > 0)
            {
                DashCooldown--;
            }

            // --- TURNING OUTSIDE OF CHARGES ---
            if (!charging)
            {
                float desiredAngle = NPC.AngleTo(player.Center);
                float currentAngle = NPC.velocity.ToRotation();

                // soft turning toward the player
                float newAngle = currentAngle.AngleTowards(desiredAngle, MathHelper.ToRadians(4));
                NPC.velocity = newAngle.ToRotationVector2() * NPC.velocity.Length();
            }

            // --- CHARGE TRIGGER ---
            if (!charging && NPC.Distance(player.Center) < 1600f && DashCooldown <= 0)
            {
                chargeDirection = Vector2.Normalize(player.Center - NPC.Center);
                charging = true;
                chargeWindup = 10;    // short delay before burst
                DashCooldown = 150;
                SoundEngine.PlaySound(ConstitutionSounds.Dash, NPC.Center);
            }

            // --- WINDUP ---
            if (charging && chargeWindup > 0)
            {
                chargeWindup--;
                if (chargeWindup == 0)
                {
                    // commit to a direction
                    NPC.velocity = chargeDirection * chargeSpeed;
                }
            }

            // --- ACTIVE CHARGE ---
            if (charging && chargeWindup == 0)
            {
                chargeDuration--;

                // VERY slight steering, but not enough to prevent overshoot
                float desiredAngle = chargeDirection.ToRotation();
                float newAngle = NPC.velocity.ToRotation().AngleTowards(desiredAngle, turnRate);
                NPC.velocity = newAngle.ToRotationVector2() * chargeSpeed;
                NPC.rotation += 0.8f * NPC.direction;

                if (Main.GameUpdateCount % 20 == 0)
                {
                    Projectile Fire = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<StellarFireSlashHostile>(), ConstitutionDamageValues.DashSlashDamage(), 0f, ai2: 4);
                    Fire.scale = 0.4f;
                }

                if (chargeDuration <= 0)
                {
                    DashCount++;
                    charging = false;
                    chargeDuration = 35;
                    NPC.velocity *= 0.4f;
                }
            }            
        }

        public bool VolleyTele = false;
        public int Side = Main.rand.NextBool() ? 1 : -1;
        public void VolleyAI()
        {
            NPC.aiStyle = -1;

            Vector2 ArenaLeft = new Vector2(ArenaCTR.X + (ArenaRect.Width / 2), ArenaCTR.Y);
            Vector2 ArenaRight = new Vector2(ArenaCTR.X - (ArenaRect.Width / 2), ArenaCTR.Y);
            Vector2 Position = ArenaRight;

            if (Side == -1)
            {
                Position = ArenaRight;
            }
            if (Side == 1)
            {
                Position = ArenaLeft;
            }

            if (NPC.Center != Position && !VolleyTele)
            {
                NPC.Center = Position;
                NPC.velocity = Vector2.Zero;
                TeleportFX();
                VolleyTele = true;
            }

            if (VolleyTele)
            {
                if (AITimer % 120 == 0)
                {
                    SoundEngine.PlaySound(ConstitutionSounds.StellarVolley, NPC.Center);
                    if (Side == 1)
                    {
                        for (int u = 0; u < 10; u++)
                        {
                            Vector2 vel = new Vector2(Main.rand.NextFloat(-6, -0.25f), -15);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<StellarVolley>(), ConstitutionDamageValues.StellarVolleyDamage(), 8);
                        }
                    }
                    if (Side == -1)
                    {
                        for (int u = 0; u < 10; u++)
                        {
                            Vector2 vel = new Vector2(Main.rand.NextFloat(0.25f, 6), -15);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<StellarVolley>(), ConstitutionDamageValues.StellarVolleyDamage(), 8);
                        }
                    }
                }
            }

        }

        #region Eternity

        public void EternityMineAI(Vector2[] P)
        {
            foreach (Projectile mine in OrbitingMines)
            {
                if (mine.active)
                {
                    mine.Kill();
                }
            }


            OrbitingMines.Clear();

            for (int k = 0; k < P.Length; k++)
            {
                Projectile p = Projectile.NewProjectileDirect(
                    NPC.GetSource_FromAI(),
                    P[k],
                    Vector2.Zero,
                    ModContent.ProjectileType<StellarBomb>(),
                    ConstitutionDamageValues.EternityStellarBombDamage(),
                    5
                );

                p.timeLeft = 119;
                OrbitingMines.Add(p);
            }
        }

        public bool Flag2 = false;
        public bool ShotLance = false;
        public bool LancesEternity = false;
        public int u = 0;
        public void EternityLanceAI()
        {
            if (ShotLance)
            {
                LancesEternity = false;
                return;
            }

            LancesEternity = true;
            if (!Flag2)
            {
                if (LanceWarningOpacity < 1f)
                {
                    LanceWarningOpacity += 0.08f;
                }
                else
                {
                    Flag2 = true;
                }
            }
            else
            {
                if (LanceWarningOpacity > 0)
                {
                    LanceWarningOpacity -= 0.08f;
                }
                else
                {
                    Vector2[] Ps = bottomSide.GetPointsAlongLine(12);
                    if (u % 2 == 0)
                    {
                        Ps = topSide.GetPointsAlongLine(12);
                    }

                    SoundEngine.PlaySound(DTAssetLib.Impacts.MagicBeep);

                    Opus.RingSpreadProjectile(ModContent.ProjectileType<StarfuryClone>(), 6, player.MountedCenter, 200, ConstitutionDamageValues.EternityStarfuryCloneDamage(), 8, 8);

                    for (int i = 0; i < Ps.Length; i++)
                    {
                        Vector2 Pos = Ps[i];

                        Vector2 Dir = ArenaCTR - Pos;
                        Dir.Normalize();

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), Pos, Dir * 16, ModContent.ProjectileType<ConstitutionStarHostile_NoHoming>(), ConstitutionDamageValues.EternityLanceDamage(), 10);
                    }
                    u++;
                    ShotLance = true;
                }
            }
        }

        float off = 0f;
        public void EternityOrbitBursts()
        {
            off += 0.05f;
            Vector2 Ideal = ArenaCTR + new Vector2(500, 0).RotatedBy(off);
            Vector2 Opposite = ArenaCTR + new Vector2(-300, 0).RotatedBy(off);

            NPC.SmoothMoveToPoint(Ideal, 40f);

            if (AITimer % 20 == 0)
            {
                SoundEngine.PlaySound(ConstitutionSounds.StellarVolley);
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<HollowStar>(), 8, Ideal, 90, 4f, 16f);
            }
        }

        #endregion
    }

    public class ConstitutionFightScene : ModSceneEffect
    {
        public override bool IsSceneEffectActive(Player player)
        {

            foreach (NPC npc in Main.npc)
            {
                if(npc.ModNPC is ConstitutionBoss con && npc.active)
                {
                    if (ConstitutionBoss.ArenaRect.Contains((int)player.Center.X, (int)player.Center.Y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (IsSceneEffectActive(player))
            {
                Main.SceneMetrics.ShimmerMonolithState = 1;
            }
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override void SetStaticDefaults()
        {
            
        }
    }
}
