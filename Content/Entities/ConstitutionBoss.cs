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
using OpusLib.Content.Helpers;
using System.ComponentModel;
using Terraria.Social.Base;

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
            return 12;
        }
    }

    public class ConstitutionSounds
    {
        public static SoundStyle Shoot1 = new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionBoss/ConstitutionBossShootStars3");
    }

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
            NPC.width = 52;
            NPC.height = 50;
            NPC.aiStyle = -1;
            NPC.damage = 24;
            NPC.defense = 24;
            NPC.lifeMax = 6000;
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
            
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SimpleLine topSide = new SimpleLine(ArenaRect.TopLeft(), ArenaRect.TopRight());
            SimpleLine bottomSide = new SimpleLine(ArenaRect.BottomLeft(), ArenaRect.BottomRight());
            SimpleLine leftSide = new SimpleLine(ArenaRect.TopLeft(), ArenaRect.BottomLeft());
            SimpleLine rightSide = new SimpleLine(ArenaRect.TopRight(), ArenaRect.BottomRight());

            DTUtils.ScrollingTextureSpine(topSide, DTAssetLib.Streak(1),  ColorLib.StellarFireGradientLooping(3f), spriteBatch, BlendState.Additive);
            DTUtils.ScrollingTextureSpine(bottomSide, DTAssetLib.Streak(1),  ColorLib.StellarFireGradientLooping(3f), spriteBatch, BlendState.Additive);
            DTUtils.ScrollingTextureSpine(leftSide, DTAssetLib.Streak(1),  ColorLib.StellarFireGradientLooping(3f), spriteBatch, BlendState.Additive);
            DTUtils.ScrollingTextureSpine(rightSide, DTAssetLib.Streak(1),  ColorLib.StellarFireGradientLooping(3f), spriteBatch, BlendState.Additive);
            Utils.DrawBorderString(spriteBatch, AITimer.ToString(), (NPC.Center - new Vector2(0, 40)) - Main.screenPosition, Color.Red, 1f);
        }
        public int AITimer = 0;
        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            if (NPC.HasValidTarget)
            {
                if (!Flag1)
                {
                    ArenaCTR = player.Center;
                    Flag1 = true;
                }
            }
            Arena();

            AITimer++;

            if (AITimer < 600 && AITimer >= 0)
            {
                IdleAI();
            }
            if (AITimer < 1200 && AITimer >= 600)
            {
                if (AITimer %  120 == 0)
                {
                    BeamBoomAI();
                }
            }
            if (AITimer >= 3200)
            {
                AITimer = 0;
            }

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;

            Music = MusicLoader.GetMusicSlot("DestroyerTest/Assets/Music/ConstitutionBoss");
        }

        public  Rectangle ArenaRect;
        public void Arena()
        {
            Player player = Main.player[NPC.target];
            ArenaRect = Utils.CenteredRectangle(ArenaCTR, new Vector2(1500, 1500));
            float HalfWidth = 750f;
            float HalfHeight = 750f;

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


            //Opus.RectDustRandom(DustID.TintableDustLighted, ArenaRect,  ColorLib.StellarFireGradientLooping(3f), 1f, 20);
            

            
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
            Opus.RadialSpreadProjectile(ModContent.ProjectileType<ConstitutionBeam>(), BeamBoomCount(Main.masterMode, DTUtils.ClassicMode()), NPC.Center, ConstitutionDamageValues.BeamDamage(), 10, 8, RandomOffset: true);
        }

    }
}
