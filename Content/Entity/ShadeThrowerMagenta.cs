using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.RiftBiome;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace DestroyerTest.Content.Entity
{
	public class ShadeThrowerMagenta : ModNPC
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
            Main.npcFrameCount[Type] = 4;
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
				new FlavorTextBestiaryInfoElement("Originating from the Shade World, this shapeshifter is attempting to mimic an Ichor Sticker."),
				new FlavorTextBestiaryInfoElement("In addition to freeing the moon lord from imprisonment, breaking the seal also tore open holes across space, allowing enemies from the shade world to enter yours."),
			});

			bestiaryEntry.Info.AddRange([
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson
			]);
		}

		SoundStyle Kill = new SoundStyle("DestroyerTest/Assets/Audio/TPKill")
		{
			Volume = 0.3f,
			PitchVariance = 1f,
			MaxInstances = 0
		};

		SoundStyle Hit = new SoundStyle("DestroyerTest/Assets/Audio/TPHit")
		{
			Volume = 0.3f,
			PitchVariance = 1f,
			MaxInstances = 0
		};

        public override void SetDefaults()
        {
            NPC.width = 70;
            NPC.height = 60;
            NPC.damage = 55;
            NPC.defense = 26;
            NPC.lifeMax = 1700;
            NPC.HitSound = Hit;
            NPC.DeathSound = Kill;
            NPC.noGravity = true;
            NPC.aiStyle = 5;
            // Sets the above
            NPC.lavaImmune = false;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0.25f;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
            DTUtils Utility = new DTUtils();
            if (spawnInfo.Player.ZoneCrimson && (spawnInfo.Player.ZoneDirtLayerHeight || spawnInfo.Player.ZoneRockLayerHeight) == true && Utility.TenebrisCanSpawnInWorldEvilBiome == true)
            {
                return 0.1f;
            }
			return 0f;
		}
        
        public override void FindFrame(int frameHeight) {
			int startFrame = 0;
			int finalFrame = 3;
			int frameSpeed = 5;
			NPC.frameCounter += 0.5f;
			NPC.frameCounter += NPC.velocity.Length() / 10f;
			if (NPC.frameCounter > frameSpeed) {
				NPC.frameCounter = 0;
				NPC.frame.Y += frameHeight;

				if (NPC.frame.Y > finalFrame * frameHeight) {
					NPC.frame.Y = startFrame * frameHeight;
				}
			}
		}


        public override void AI() {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];

            NPC.rotation = 0.05f * NPC.velocity.Length();

            if (Main.GameUpdateCount % 120 == 0) {
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ChargeBreak") with { PitchVariance = 1f, Volume = 3f });
                Projectile.NewProjectile(Entity.GetSource_FromAI(), NPC.Center,
                    (player.Center - NPC.Center) * 0.005f,
                    ModContent.ProjectileType<TenebrisStar>(), 26, 8, ai2: 2);
            }
        }

		

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 120, true, false);
        }
    }
}