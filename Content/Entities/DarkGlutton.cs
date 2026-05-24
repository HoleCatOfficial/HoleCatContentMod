using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace DestroyerTest.Content.Entities
{
    // These three class showcase usage of the WormHead, WormBody and WormTail classes from Worm.cs
    internal class DarkGluttonHead : WormHead
    {
        public override int BodyType => ModContent.NPCType<DarkGluttonBody>();

        public override int TailType => ModContent.NPCType<DarkGluttonTail>();

        SoundStyle Roar = new SoundStyle("DestroyerTest/Assets/Audio/DPRoar") // The sound played when the worm roars, can be overridden by the tail or body if desired
        {
            Volume = 0.3f,
            PitchVariance = 1f,
            MaxInstances = 0
        };
        SoundStyle Kill = new SoundStyle("DestroyerTest/Assets/Audio/DPKill") // The sound played when the worm roars, can be overridden by the tail or body if desired
        {
            Volume = 0.3f,
            PitchVariance = 1f,
            MaxInstances = 0
        };

        public override void SetStaticDefaults()
        {
            immunities();
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            { // Influences how the NPC looks in the Bestiary
                CustomTexturePath = "DestroyerTest/Content/Entities/DarkGluttonHeadBestiary", // If the NPC is multiple parts like a worm, a custom texture for the Bestiary is encouraged.
                Position = new Vector2(40f, 24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 12f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
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
        public override void SetDefaults()
        {

            // Head is 10 defense, body 20, tail 30.
            NPC.CloneDefaults(NPCID.DiggerHead);
            //NPC.width = 10;
            NPC.lifeMax = 540;
            NPC.life = 540;
            NPC.defense = 30;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit12;
            NPC.DeathSound = Kill;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(DTUtils.GetModNPCLocalizationEntry(this, 1)),
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.DestroyerTest.Extras.ShadeWorldCommonBestiary")),
            });

            bestiaryEntry.Info.AddRange([
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption
            ]);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CorruptionModificationSystem.TenebrisSpawnRequirements(spawnInfo))
            {
                return 0.5f;
            }
            return 0f;
        }

        public override void OnSpawn(IEntitySource source)
        {

            base.OnSpawn(source);
        }

        public override void Init()
        {
            // Set the segment variance
            // If you want the segment length to be constant, set these two properties to the same value
            MinSegmentLength = 10;
            MaxSegmentLength = 20;

            CommonWormInit(this);
        }

        // This method is invoked from ExampleWormHead, ExampleWormBody and ExampleWormTail
        public static void CommonWormInit(Worm worm)
        {
            // These two properties handle the movement of the worm
            worm.MoveSpeed = 5.5f;
            worm.Acceleration = 0.045f;
        }

        Vector2 chargeDirection;
        bool charging = false;
        int chargeWindup = 30;
        int chargeDuration = 60;
        float chargeSpeed = 14f;
        float turnRate = 0.02f; // low = overshoot

        public bool MistakeFixed = false;
        public override void AI()
        {
            NPC.TargetClosest();
            if (MistakeFixed == false)
            {
                NPC.lifeMax = 1600;
                NPC.life = 1600;
                NPC.defense = 80;
                MistakeFixed = true;
            }

            Color[] Colors = new Color[]
            {
                ColorLib.TenebrisBeige,
                ColorLib.TenebrisBlue,
                ColorLib.TenebrisMagenta
            };

            if (Main.rand.NextBool(40))
            {
                Dust.NewDust(NPC.Center, 50, 50, DustID.FireworksRGB, NPC.velocity.X * 0.4f, NPC.velocity.Y * 0.4f, 100, Colors[Main.rand.Next(Colors.Length)], Main.rand.NextFloat(0.01f, 1.0f));
            }

           Player player = Main.player[NPC.target];

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
            if (!charging && NPC.Distance(player.Center) < 400f)
            {
                chargeDirection = Vector2.Normalize(player.Center - NPC.Center);
                charging = true;
                chargeWindup = 10;    // short delay before burst
                SoundEngine.PlaySound(Roar, NPC.Center);
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

                if (Main.GameUpdateCount % 10 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
                    Projectile Fire = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<TenebrisFlamesHostile_NoHoming>(), 20, 0f, ai2: 4);
                    Fire.scale = 0.65f;
                    Fire.timeLeft = 60;
                }

                if (chargeDuration <= 0)
                {
                    charging = false;
                    chargeDuration = 35;
                    NPC.velocity *= 0.4f; // slow down after missing
                }
            }


        }
        
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadeParticle>(), 3, 4, 17));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShimmeringSludge>(), 4, 2, 9));
        }
	}

    internal class DarkGluttonBody : WormBody
    {
        public override void SetStaticDefaults()
        {
            immunities();
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
            NPCID.Sets.RespawnEnemyID[NPC.type] = ModContent.NPCType<DarkGluttonHead>();
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

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerBody);
            NPC.lifeMax = 240;
            NPC.life = 240;
            NPC.defense = 30;
            NPC.aiStyle = -1;
        }


        public override void OnSpawn(IEntitySource source)
        {
            NPC.life = 240;
            base.OnSpawn(source);
        }
        public override void Init()
        {
            DarkGluttonHead.CommonWormInit(this);
        }

        public bool MistakeFixed = false;
        public override void AI()
        {
            if (MistakeFixed == false)
            {
                NPC.lifeMax = 1600;
                NPC.life = 1600;
                NPC.defense = 80;
                MistakeFixed = true;
            }
            Color[] Colors = new Color[]
            {
                ColorLib.TenebrisBeige,
                ColorLib.TenebrisBlue,
                ColorLib.TenebrisMagenta
            };

            if (Main.rand.NextBool(40))
            {
                Dust.NewDust(NPC.Center, 50, 50, DustID.TintableDustLighted, NPC.velocity.X * 0.4f, NPC.velocity.Y * 0.4f, 100, Colors[Main.rand.Next(Colors.Length)], Main.rand.NextFloat(0.01f, 1.0f));
            }
            base.AI();
        }
        
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadeParticle>(), 3, 4, 17));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShimmeringSludge>(), 4, 2, 9));
        }
	}

    internal class DarkGluttonTail : WormTail
    {
        public override void SetStaticDefaults()
        {
            immunities();
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
            NPCID.Sets.RespawnEnemyID[NPC.type] = ModContent.NPCType<DarkGluttonHead>();
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

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerTail);
            NPC.lifeMax = 240;
            NPC.life = 240;
            NPC.defense = 30;
            NPC.aiStyle = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            NPC.life = 240;
            base.OnSpawn(source);
        }

        public override void Init()
        {

            DarkGluttonHead.CommonWormInit(this);
        }
        
        public bool MistakeFixed = false;
        public override void AI()
        {
            if (MistakeFixed == false)
            {
                NPC.lifeMax = 1600;
                NPC.life = 1600;
                NPC.defense = 80;
                MistakeFixed = true;
            }
            Color[] Colors = new Color[]
            {
                ColorLib.TenebrisBeige,
                ColorLib.TenebrisBlue,
                ColorLib.TenebrisMagenta
            };

            if (Main.rand.NextBool(40))
            {
                Dust.NewDust(NPC.Center, 50, 50, DustID.TintableDustLighted, NPC.velocity.X * 0.4f, NPC.velocity.Y * 0.4f, 100, Colors[Main.rand.Next(Colors.Length)], Main.rand.NextFloat(0.01f, 1.0f));
            }
        }
	}
}