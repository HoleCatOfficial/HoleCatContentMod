using DestroyerTest.Common;
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
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using OpusLib;
using System.Data;
using DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss;
/*
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

        public void DamageScaling()
        {
            if(EternityIsActive())
            {
                StarDamage = 30;
                HomingStarDamage = 18;
                CloneDamage = 40;
                LightBoltDamage = 25;
                StarFuryDamage = 35;
            }
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
                    Main.EntitySpriteDraw(WhiteOutline, NPC.Center, null,  ColorLib.StellarFireGradientLooping(), outlineRotation, new Vector2(NPC.width / 2, NPC.height / 2), NPC.scale * 1.2f, effects, 0);
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

                    Main.EntitySpriteDraw(WhiteOutline, drawPos, null, NPC.GetAlpha( ColorLib.StellarFireGradientLooping()) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length), outlineRotation, new Vector2(NPC.width / 2, NPC.height / 2), NPC.scale * 1.2f, effects, 0);
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
            Piechart,
            StarTrace,
            StellarVolley,
            DoubleStar,
            Dash,
            LanceCross,
            MinefieldLegacy,
            StarFall,
            Constellations
        }

        public static int IdleFloat = (int)AttackState.idlefloat;
        public static int PieChart = (int)AttackState.Piechart;
        public static int StarTrace = (int)AttackState.StarTrace;
        public static int StellarVolley = (int)AttackState.StellarVolley;
        public static int DoubleStar = (int)AttackState.DoubleStar;
        public static int Dash = (int)AttackState.Dash;
        public static int LanceCross = (int)AttackState.LanceCross;
        public static int Minefield = (int)AttackState.LanceCross;
        public static int StarFall = (int)AttackState.StarFall;
        public static int Constellations = (int)AttackState.Constellations;

        /// <summary>
        /// Runs on classic difficulty and is the easiest. Attacks will use their classic mode variants.
        /// <para/> Is the most lax fight, with lots of Idle Time and only a small pool of attacks. Though their speed will pick up in the second phase.
        /// </summary>
        /// <param name="Phase2"></param>
        /// <returns></returns>
        public static List<int> ClassicAI(bool Phase2)
        {
            if (!Phase2)
            {
                return new List<int> { IdleFloat, DoubleStar, IdleFloat, PieChart, IdleFloat, StellarVolley };
            }
            else
            {
                return new List<int> { IdleFloat, Dash, DoubleStar, Dash, IdleFloat, PieChart, IdleFloat, StellarVolley };
            }
        }

        /// <summary>
        /// Runs on Expert difficulty. Attacks use Expert Mode Variants, classic mode attacks can also be used.
        /// <para/> The base experience. Runs attacks in pairs, doing two one after another before resting and doing another two in sequence.
        /// </summary>
        /// <param name="Phase2"></param>
        /// <returns></returns>
        public static List<int> ExpertAI(bool Phase2)
        {
            if (!Phase2)
            {
                return new List<int> { IdleFloat, DoubleStar, Dash, IdleFloat, Minefield, LanceCross, IdleFloat, StellarVolley, Dash };
            }
            else
            {
                return new List<int> { IdleFloat, PieChart, Dash, IdleFloat, PieChart, LanceCross, IdleFloat, StellarVolley, Constellations, Dash, Dash };
            }
        }

        /// <summary>
        /// Runs on master mode difficulty. Attacks can use Classic, Expert, or Master variants.
        /// </summary>
        /// <param name="Phase2"></param>
        /// <returns></returns>
        public static List<int> MasterAI(bool Phase2)
        {
            if (!Phase2)
            {
                return new List<int> { Dash, DoubleStar, Dash, StellarVolley, Minefield, LanceCross, StarTrace, StellarVolley, Dash };
            }
            else
            {
                return new List<int> { Constellations, PieChart, Dash, StarFall, PieChart, LanceCross, Constellations, StellarVolley, Dash };
            }
        }

        /// <summary>
        /// Runs on eternity mode. Attacks can use Classic, Expert, Master, or Eternity variants.
        /// </summary>
        /// <param name="Phase2"></param>
        /// <returns></returns>
        
        public static List<int> EternityAI(bool Phase2)
        {
            if (!Phase2)
            {
                return new List<int> { 0, 1 };
            }
            else
            {
                return new List<int> { 0, 1 };
            }
        }

        /// <summary>
        /// A catch-all AI for if the world is Running Masochist (Master + Eternity), A secret seed (such as For the Worthy), or Legendary mode (Master + For the Worthy).
        /// </summary>
        /// <param name="Phase2"></param>
        /// <returns></returns>

        public static List<int> AI_LegendaryMasoAndFTW(bool Phase2)
        {
            if (!Phase2)
            {
                return new List<int> { 0, 1};
            }
            else
            {
                return new List<int> { 0, 1 };
            }
        }

        #region AI Machine
       


        

        #endregion

        #region Music
        public void ModifyMusic()
        {
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
        }
        #endregion

        public void Success()
        {
            if (Targetplayer.dead)
            {
                DeathInterval--;
                if (DeathInterval <= 0)
                {
                    NPC.active = false;
                }
            }
        }

        public int StarDamage = 15;
        public int HomingStarDamage = 10;
        public int CloneDamage = 30;
        public int LightBoltDamage = 15;
        public int StarFuryDamage = 20;
        public int LanceCrossDamage = 10;
        public int LanceBurstDamage = 20;
        public int LanceSweepDamage = 30;
        public int MineDamage = 20;
        public int LightBirdDamage = 15;


        public int StarsCount1 = 3;
        public Vector2 Chargedir;
        public float TeleRadius = 600f;
        public bool Phase2 = false;
        public int Chance = 4;
        public float lanceradius = 500f;
        public float lancerotspeed = 0.05f;
        public bool HasPlayedPhase2Roar = false;

        public int AttackIntervalDefault = 20;
        public float TexRot = 0f;
        public AttackState currentState = AttackState.idlefloat;

        public override void OnSpawn(IEntitySource source)
        {
            Mod.Logger.Info("Constitution Spawned!");
        }

        public int DeathInterval = 10;
        public Player Targetplayer;
        public override void AI()
        {
            NPC.TargetClosest(true);
            if (NPC.HasValidTarget)
            {
                Targetplayer = Main.player[NPC.target];
            }
            else
            {
                Targetplayer = Main.player[0];
            }
            DTUtils Utility = new DTUtils();

            

            if (NPC.direction == -1)
            {
                NPC.rotation += MathHelper.Pi;
            }

            if (EternityIsActive())
            {
                AttackIntervalDefault = 8;
            }

            Success();

                if (NPC.Center.DistanceSQ(Targetplayer.Center) > 40000)
                {
                    TeleManager(Targetplayer, ref TeleCircumferencePoint);
                }

            NPC.rotation = (Chargedir * 4f).ToRotation() + MathHelper.PiOver4;


            if (NPC.life <= 0.50f * NPC.lifeMax)
            {
                Phase2 = true;
            }

            if (Phase2)
            {
                if (!HasPlayedPhase2Roar)
                {
                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionBoss/ConstitutionBossKill") with { PitchVariance = 1, MaxInstances = 1 });
                    HasPlayedPhase2Roar = true;
                }
                ColorGradientOverlaySystem.ColorVisibility = 0.7f;
            }

            if (NPC.life < NPC.lifeMax * 0.15f)
            {
                
            }

            Mod.Logger.Info($"Current State: {currentState}");



            ModifyMusic();
            Attack();

            int DustAmount = 3;
            if (NPC.type == ModContent.NPCType<ConstitutionBoss>())
            {
                if (Main.rand.NextBool(3))
                {
                    for (int g = 0; g < DustAmount; g++)
                    {
                        Dust.NewDust(NPC.position, NPC.Hitbox.Width + 10, NPC.Hitbox.Height + 10, DustID.Enchanted_Pink, 0, 0, 0, default, 1f);
                        Dust.NewDust(NPC.position, NPC.Hitbox.Width + 10, NPC.Hitbox.Height + 10, DustID.Enchanted_Gold, 0, 0, 0, default, 1f);
                    }
                }
            }
        }

        
        private void Attack()
        {
            switch (currentState)
            {
                case AttackState.idlefloat:
                    {
                        AI_IdleFloat();
                        if (AI_IdleFloat() == true)
                        {
                            currentState = AttackState.Piechart;
                            Reset();
                        }
                        break;
                    }
                case AttackState.Piechart:
                    {
                        AI_PieChartClassic();
                        if (AI_PieChartClassic() == true)
                        {
                            currentState = AttackState.StellarVolley;
                            Reset();
                        }
                        break;
                    }
                case AttackState.StellarVolley:
                    {
                        AI_StellarVolleyClassic();
                        if (AI_StellarVolleyClassic() == true)
                        {
                            currentState = AttackState.DoubleStar;
                            Reset();
                        }
                        break;
                    }
                case AttackState.DoubleStar:
                    {
                        AI_DoubleStarClassic();
                        if (AI_DoubleStarClassic() == true)
                        {
                            currentState = AttackState.idlefloat;
                            Reset();
                        }
                        break;
                    }
            }
        }

        private void Reset()
        {
            AI_IdleFloat_Timer = default;
            AI_IdleFloat_GetRandPos = default;
            //----------------------------
            AI_DoubleStarClassic_Count = default;
            //----------------------------
            AI_PieChartClassic_SetLocationFlag = default;
            AI_PieChartClassic_Timer = default;
            //----------------------------
            AI_StellarVolleyClassic_DecideLR = default;
            AI_StellarVolleyClassic_Timer = default;
        }

        #region Attack Methods

        public bool AI_IdleFloat_GetRandPos = false;
        public int AI_IdleFloat_Timer = 240;
        public bool AI_IdleFloat()
        {
            NPC.aiStyle = NPCAIStyleID.CursedSkull;
            if (AI_IdleFloat_Timer > 0)
            {
                if (NPC.Distance(Targetplayer.Center) > 30)
                {
                    NPC.velocity = Vector2.Lerp(NPC.Center, Targetplayer.Center, 0.05f);
                }
                else
                {
                    Vector2 RandPos = Targetplayer.Center;
                    if (!AI_IdleFloat_GetRandPos)
                    {
                        RandPos = Targetplayer.Center + Main.rand.NextVector2Circular(1000, 1000);
                        AI_IdleFloat_GetRandPos = true;
                    }
                    NPC.velocity = Vector2.Lerp(NPC.Center, RandPos, 0.01f);
                }
                AI_IdleFloat_Timer--;
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool AI_PieChartClassic_SetLocationFlag = false;
        public int AI_PieChartClassic_Timer = 120;
        public Vector2 AI_PieChartClassic_Center;
        public bool AI_PieChartClassic()
        {
            if (!AI_PieChartClassic_SetLocationFlag)
            {
                AI_PieChartClassic_Center = Targetplayer.Center + Main.rand.NextVector2Circular(600, 600);
                AI_PieChartClassic_SetLocationFlag = true;
            }

            if (AI_PieChartClassic_Timer > 0 && AI_PieChartClassic_SetLocationFlag)
            {
                NPC.Center = AI_PieChartClassic_Center;
                AI_PieChartClassic_Timer--;
                return false;
            }

            if (AI_PieChartClassic_Timer <= 0)
            {
                ShootLanceInward(ModContent.ProjectileType<GalantineLance>(), 3, AI_PieChartClassic_Center, 600, 15, 3, 0.05f, 3f, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                return true;
            }
            return false;
        }

        public void ShootLanceInward(int ID, int Amount, Vector2 CTR, float Radius, int Dmg = 0, int KB = 0, float Speed = 2f, float AI0 = 0f, float AI1 = 0f, float AI2 = 0f, bool RandomOffset = false)
        {
            float num = MathF.PI * 2f / (float)Amount;
            float num2 = (RandomOffset ? Main.rand.NextFloat(MathF.PI * 2f) : 0f);
            for (int i = 0; i < Amount; i++)
            {
                float num3 = num * (float)i + num2;
                Vector2 vector = CTR + new Vector2(Radius, 0f).RotatedBy(num3);
                Vector2 velocity = (CTR - vector).SafeNormalize(Vector2.Zero) * Speed;
                float Rot = velocity.ToRotation();
                Projectile.NewProjectile(NPC.GetSource_FromAI(), vector, velocity, ID, Dmg, KB, -1, AI0, Rot, AI2);
            }
        }

        public int AI_DoubleStarClassic_Count = 0;
        public const int AI_DoubleStarClassic_Interval = 60;
        public Vector2 AI_DoubleStarClassic_FlankUp;
        public Vector2 AI_DoubleStarClassic_FlankDown;
        public Vector2 AI_DoubleStarClassic_FlankUpToPlayer;
        public Vector2 AI_DoubleStarClassic_FlankDownToPlayer;
        public bool AI_DoubleStarClassic()
        {
            AI_DoubleStarClassic_FlankUp = NPC.Center + new Vector2(0, -25);
            AI_DoubleStarClassic_FlankUp = AI_DoubleStarClassic_FlankUp.RotatedBy(NPC.rotation);

            AI_DoubleStarClassic_FlankDown = NPC.Center + new Vector2(0, 25);
            AI_DoubleStarClassic_FlankDown = AI_DoubleStarClassic_FlankDown.RotatedBy(NPC.rotation);

            AI_DoubleStarClassic_FlankUpToPlayer = AI_DoubleStarClassic_FlankUp.AngleTo(Targetplayer.Center).ToRotationVector2() * 3;
            AI_DoubleStarClassic_FlankDownToPlayer = AI_DoubleStarClassic_FlankDown.AngleTo(Targetplayer.Center).ToRotationVector2() * 3;

            if (Main.GameUpdateCount % AI_DoubleStarClassic_Interval == 0)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), AI_DoubleStarClassic_FlankUp, AI_DoubleStarClassic_FlankUpToPlayer, ModContent.ProjectileType<ConstitutionStar>(), StarDamage, 4, ai2: 4);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), AI_DoubleStarClassic_FlankDown, AI_DoubleStarClassic_FlankDownToPlayer, ModContent.ProjectileType<ConstitutionStar>(), StarDamage, 4, ai2: 4);
                AI_DoubleStarClassic_Count++;
            }

            if (AI_DoubleStarClassic_Count >= 6)
            {
                return true;
            }
            return false;
        }

        public Vector2 AI_StellarVolleyPositionLeft;
        public Vector2 AI_StellarVolleyPositionRight;
        public bool AI_StellarVolleyClassic_LeftOrRight;
        public bool AI_StellarVolleyClassic_DecideLR = false;
        public Vector2 AI_StellarVolleyClassicShootDir;
        public int AI_StellarVolleyClassic_Timer = 240;

        public bool AI_StellarVolleyClassic()
        {
            AI_StellarVolleyPositionLeft = Targetplayer.Center + new Vector2(-300, -100);
            AI_StellarVolleyPositionRight = Targetplayer.Center + new Vector2(300, -100);

            
            if (!AI_StellarVolleyClassic_DecideLR)
            {
                AI_StellarVolleyClassic_LeftOrRight = Main.rand.NextBool(2);
                AI_StellarVolleyClassic_DecideLR = true;
            }

            if (AI_StellarVolleyClassic_DecideLR)
            {
                if (AI_StellarVolleyClassic_LeftOrRight)
                {
                    NPC.Center = AI_StellarVolleyPositionLeft;
                    AI_StellarVolleyClassicShootDir = NPC.Center + new Vector2(Main.rand.Next(100, 400), Main.rand.Next(1, 31));
                }
                else
                {
                    NPC.Center = AI_StellarVolleyPositionRight;
                    AI_StellarVolleyClassicShootDir = NPC.Center + new Vector2(Main.rand.Next(-400, -100), Main.rand.Next(1, 31));
                }

                if (AI_StellarVolleyClassic_Timer > 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, AI_StellarVolleyClassicShootDir, ModContent.ProjectileType<StellarVolley>(), 15, 2);
                    AI_StellarVolleyClassic_Timer--;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        /*
        bool CenterFlag = false;
        bool PieChartClassicFlag = false;
        Vector2 ClassicCenter;
        List<int> StellarJunkForSuckin = new List<int>{ ModContent.ProjectileType<StellarFlameHostile>(), ModContent.ProjectileType<GalantineLance>() };
        public void PieChart(Player player, int Mode = 0, int NumLances = 3, float Radius = 1000f, int ClassicExpertMasterWaitForLances = 100, int ClassicDelay = 60)
        {
            
            Vector2[] ExpertCenters = new Vector2[5];
            if (Mode == 0)
            {
                //Classic AI

                //--------------------------------------
                /*
                Sets ClassicCenter once.
                Counts ClassicDelay down to 0.
                Once ClassicDelay reaches 0, spawn NumLances number of lances equdistantly along the edges facing towards the center.
                Count down ClassicExperMasterWaitForLances to 0.
                Once it reaches 0, the attack is complete and the AI state can move to the next index of the AI list.

                During all of this, the NPC's center remains equal to the ClassicCenter

                //--------------------------------------
                if (!CenterFlag)
                {
                    ClassicCenter = Targetplayer.Center + Main.rand.NextVector2Circular(600, 600);
                    CenterFlag = true;
                }

                if (ClassicExpertMasterWaitForLances > 0 && CenterFlag)
                {
                    NPC.Center = ClassicCenter;
                }
                if (ClassicDelay > 0)
                {
                    ClassicDelay--;
                    PieChartClassicFlag = false;
                }
                if (ClassicDelay <= 0)
                {
                    if (!PieChartClassicFlag)
                    {
                        Opus.RingSpreadProjectile#(ModContent.ProjectileType<GalantineLance>(), 3, ClassicCenter, 500, 25, 3, 0.01f, 4f, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                        PieChartClassicFlag = true;
                    }
                    else
                    {
                        if (ClassicExpertMasterWaitForLances > 0)
                        {
                            ClassicExpertMasterWaitForLances--;
                        }
                        if (ClassicExpertMasterWaitForLances <= 0)
                        {
                            ClassicExpertMasterWaitForLances = 100;
                        }
                    }
                    ClassicDelay = 60;
                }
            }
            if (Mode == 1)
            {
                //Expert AI

                //--------------------------------------
                /*
                Sets 5 centers to fill ExpertCenters.
                Around each Center, run the classic AI.
                Lances though will have a higher speed modifier set, causing them all to intersect.
                

                NPC's center is not affected.

                //--------------------------------------
            }
            if (Mode == 2)
            {
                //Runs in master, but just uses Expert AI
            }
            if (Mode == 3)
            {
                //Eternity AI

                //--------------------------------------
                /*
                Sets ClassicCenter once.
                Player is confined inside the piechart while constitution begins sucking in matter, requiring you to dodge incoming projectiles.
                This creates a ball of light that, after the attack concludes, shrinks and explodes into Galantine Stars radially.

                During all of this, the NPC's center remains equal to the ClassicCenter
 
                //--------------------------------------
            }
            if (Mode == 4)
            {
                //Catch-all AI. Hardest / most unfair variation.
            }
            if (Mode > 4)
            {
                throw new Exception("Difficulty not Recognized. Use 4 for Masochist, Legendary, Death Mode, etc.");
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

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (EternityIsActive())
            {
                Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
                Main.EntitySpriteDraw(DTAssetLib.FireRing.Value, NPC.Center - Main.screenPosition, null,  ColorLib.StellarFireGradientLooping() * 0.5f, -TexRot, DTAssetLib.FireRing.Value.Size() / 2, 0.095f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(DTAssetLib.FireRing.Value, NPC.Center - Main.screenPosition, null,  ColorLib.StellarFireGradientLooping() * 0.25f, -TexRot * 2, DTAssetLib.FireRing.Value.Size() / 2, 0.085f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(DTAssetLib.FireRing.Value, NPC.Center - Main.screenPosition, null,  ColorLib.StellarFireGradientLooping() * 0.25f, TexRot * 1.5f, DTAssetLib.FireRing.Value.Size() / 2, 0.0805f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(DTAssetLib.FireRing.Value, NPC.Center - Main.screenPosition, null,  ColorLib.StellarFireGradientLooping() * 0.7f, -TexRot * 0.5f, DTAssetLib.FireRing.Value.Size() / 2, 0.08f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(DTAssetLib.FireRing.Value, NPC.Center - Main.screenPosition, null,  ColorLib.StellarFireGradientLooping() * 0.7f, TexRot, DTAssetLib.FireRing.Value.Size() / 2, 0.08f, SpriteEffects.None, 0);
                Opus.ReturnToDefaultDrawing(Main.spriteBatch);
            }
        }




        public override void OnKill()
        {

            Phase2 = false;
            ColorGradientOverlaySystem.ColorVisibility = 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StellarMatter>(), 2, 4, 35));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            Player plr = Main.player[NPC.target];
            PRTLoader.NewParticle(PRTLoader.GetParticleID<FlatStar>(), Main.rand.NextVector2FromRectangle(NPC.Hitbox), Vector2.Zero,  ColorLib.StellarFireGradientLooping(), 0.15f);
            IEntitySource src = NPC.GetSource_OnHurt(plr);
            for (int u = 0; u < 4; u++)
            {
                Gore.NewGore(src, NPC.Center, Main.rand.NextVector2Circular(2, 2), 16);
                Gore.NewGore(src, NPC.Center, Main.rand.NextVector2Circular(2, 2), 17);
            }                
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
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver4;
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
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver4;

            Color clr = new Color(105, 68, 186);
            Lighting.AddLight(Projectile.Center, (clr * 0.4f).ToVector3());

            if (Main.rand.NextBool(3))
            {
                int DustAmount = 8;
                for (int g = 0; g < DustAmount; g++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width - 10, Projectile.height - 10, DustID.Enchanted_Pink, 0, 0, 0, default, 1f);
                    Dust.NewDust(Projectile.position, Projectile.width - 10, Projectile.height - 10, DustID.Enchanted_Gold, 0, 0, 0, default, 1f);
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

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<GalantineBurn>(), 240);
            Projectile boom = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CloneCollisionBoom>(), 0, 0);
            Projectile.Kill();
        }

    }

    public class StarMine : ModProjectile
    {


        public override string Texture => DTUtils.NoTexture;
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
            lightColor =  ColorLib.StellarFireGradientLooping();

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
        public override string Texture => DTUtils.NoTexture; // Path to the texture for the projectile
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

            lightColor =  ColorLib.StellarFireGradientLooping();


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
*/

