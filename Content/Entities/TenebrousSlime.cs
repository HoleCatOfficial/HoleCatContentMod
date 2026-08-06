using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.RiftBiome;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace DestroyerTest.Content.Entities
{

	public class TenebrousSlime : ModNPC
	{

		public override void SetStaticDefaults() {
			immunities();
            Main.npcFrameCount[NPC.type] = 2;
			
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() {
				Velocity = 1f,
				Direction = 1
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
			NPCID.Sets.ShimmerTransformToNPC[Type] = -1;
            Banner = Type;
            BannerItem = Mod.Find<ModItem>("Item_TenebrousSlimeBanner").Type;

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
				new FlavorTextBestiaryInfoElement("Originating from the Shade World, this mindless glob of sludge seeks to explore, but prefers not to be in the light, as is common with life in the shade world."),
				new FlavorTextBestiaryInfoElement("In addition to freeing the moon lord from imprisonment, breaking the seal also tore open holes across space, allowing enemies from the shade world to enter yours."),
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns
			});
		}

		public int variant = 0;

		public override void SetDefaults()
		{
			NPC.width = 74;
			NPC.height = 52;
			NPC.aiStyle = NPCAIStyleID.Slime;
			NPC.damage = 15;
			NPC.defense = 12;
			NPC.lifeMax = 300;
			NPC.HitSound = SoundID.Item154;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.noGravity = false;
			NPC.lavaImmune = true;
			variant = Main.rand.Next(3);
			NPC.Opacity = 0.75f;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			switch(variant) 
			{ 
				case 0:
					drawColor = ColorLib.TenebrisBlue;
					break;
                case 1:
                    drawColor = ColorLib.TenebrisMagenta;
                    break;
                case 2:
                    drawColor = ColorLib.TenebrisBeige;
                    break;
            }
			return true;
        }

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
            DTUtils Utility = new DTUtils();
            if (spawnInfo.Player.ZoneCorrupt == true && DTFlags.TenebrisCanSpawnInWorldEvilBiome == true)
            {
                return 0.1f;
            }
			return 0f;
		}


        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {

        }
    }
}