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
using DestroyerTest.Content.Particles.Stellar;

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
            return 10;
        }

        public static int DashSlashDamage()
        {
            if (DTUtils.ClassicMode())
            {
                return 20;
            }
            if (Main.expertMode && !Main.masterMode)
            {
                return 25;
            }
            if (Main.masterMode)
            {
                return 30;
            }
            return 10;
        }
    }

    public class ConstitutionSounds
    {
        public static SoundStyle Shoot1 = new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionBoss/ConstitutionBossShootStars3");
        public static SoundStyle WallWarn = new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/CursedFlamesWarn");
        public static SoundStyle Teleport = new SoundStyle("DestroyerTest/Assets/Audio/Constitution/ConSwing", 6);
        public static SoundStyle Dash = DTAssetLib.SwordSounds.MagicSwing;
    
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

        public Line topSide = new Line(ArenaRect.TopLeft(), ArenaRect.TopRight());
        public Line bottomSide = new Line(ArenaRect.BottomLeft(), ArenaRect.BottomRight());
        public Line leftSide = new Line(ArenaRect.TopLeft(), ArenaRect.BottomLeft());
        public Line rightSide = new Line(ArenaRect.TopRight(), ArenaRect.BottomRight());
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            

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
            if (!player.dead )
            {
                Arena();
            }

            if (player.dead )
            {
                HandleDeath();
            }

            AITimer++;

            if (AITimer < 300 && AITimer >= 0)
            {
                IdleAI();
            }
            if (AITimer < 1200 && AITimer >= 300)
            {
                if (AITimer %  80 == 0)
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
                WallShootCount = 0;
                DashAI();
            }
            if (AITimer > 3500)
            {
                AITimer = 0;
            }

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;

            Music = MusicLoader.GetMusicSlot("DestroyerTest/Assets/Music/ConstitutionBoss");
        }

        

        public static Rectangle ArenaRect;
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

            topSide    = new Line(ArenaRect.TopLeft(), ArenaRect.TopRight());
            bottomSide = new Line(ArenaRect.BottomLeft(), ArenaRect.BottomRight());
            leftSide   = new Line(ArenaRect.TopLeft(), ArenaRect.BottomLeft());
            rightSide  = new Line(ArenaRect.TopRight(), ArenaRect.BottomRight());

            //Opus.RectDustRandom(DustID.TintableDustLighted, ArenaRect,  ColorLib.StellarFireGradientLooping(3f), 1f, 20);
            
            if (Main.rand.NextBool(3))
            {
                PRTLoader.NewParticle(StellarParticleIndex.ConstitutionParticle, Main.rand.NextVector2FromRectangle(ArenaRect), new Vector2(0, Main.rand.NextFloat(-1.5f, -0.1f)), (Color)default * 0.75f, 2.5f);
            }
        }

        public void HandleDeath()
        {
            if (ArenaRect.Width > 1)
            {
                ArenaRect.Width--;
            }
            if (ArenaRect.Height > 1)
            {
                ArenaRect.Height--;
            }

            NPC.Opacity -= 0.1f;

            if (NPC.Opacity == 0.1f)
            {
                SoundEngine.PlaySound(DTAssetLib.Impacts.StellarFox, NPC.Center);
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

                    PRTLoader.NewParticle(StellarParticleIndex.ConstitutionParticle, spawnPos, velocity, default, 1f);
                    PRTLoader.NewParticle(StellarParticleIndex.ConstitutionParticle, spawnPos, Vector2.Zero, default, 1f);
                }
                PRTLoader.NewParticle(StellarParticleIndex.FlatStar, NPC.Center, Vector2.Zero,  ColorLib.StellarFireGradientLooping(3f), 0.15f);
            }
            if (NPC.Opacity <= 0)
            {
                NPC.active = false;
            }


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

        public int WallShotCount(bool Double, bool Half)
        {
            if (Half)
            {
                return 8;
            }
            if (Double)
            {
                return 16;
            }
            else
            {
                return 12;
            }
        }

        public int WallShootCount = 0;
        public void WallShootAI()
        {
            Vector2[] tops = topSide.GetPointsAlongLine(WallShotCount(Main.masterMode, DTUtils.ClassicMode()));
            Vector2[] rights = rightSide.GetPointsAlongLine(WallShotCount(Main.masterMode, DTUtils.ClassicMode()));
            Vector2[] bottoms = bottomSide.GetPointsAlongLine(WallShotCount(Main.masterMode, DTUtils.ClassicMode()));
            Vector2[] lefts = leftSide.GetPointsAlongLine(WallShotCount(Main.masterMode, DTUtils.ClassicMode()));
            
            NPC.aiStyle = -1;
            NPC.Center = ArenaCTR;
            if (WallShootCount == 0)
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

                    PRTLoader.NewParticle(StellarParticleIndex.ConstitutionParticle, spawnPos, velocity, default, 1f);
                    PRTLoader.NewParticle(StellarParticleIndex.ConstitutionParticle, spawnPos, Vector2.Zero, default, 1f);
                }
                PRTLoader.NewParticle(StellarParticleIndex.FlatStar, NPC.Center, Vector2.Zero,  ColorLib.StellarFireGradientLooping(3f), 0.15f);
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
                        PRTLoader.NewParticle(StellarParticleIndex.ConstitutionParticle, shotpos, new Vector2(0, 40), default, 1.6f);
                    }
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
                        PRTLoader.NewParticle(StellarParticleIndex.ConstitutionParticle, shotpos, new Vector2(-40, 0), default, 1.6f);
                    }
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
                        PRTLoader.NewParticle(StellarParticleIndex.ConstitutionParticle, shotpos, new Vector2(0, -40), default, 1.6f);
                    }
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
                        PRTLoader.NewParticle(StellarParticleIndex.ConstitutionParticle, shotpos, new Vector2(40, 0), default, 1.6f);
                    }
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
                AITimer = 0;
                DashCount = 0;
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

    public class ConstitutionBCL : ModSystem
    {
        public override void PostSetupContent() 
        {
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
			Func<bool> downed = DownedBossSystem.downedConstitutionConditionbool;

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
