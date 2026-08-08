using DestroyerTest.Common;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace DestroyerTest.Content.Entities
{
	internal class RiftDiggerHead : WormHead
	{
		public override int BodyType => ModContent.NPCType<RiftDiggerBody>();

		public override int TailType => ModContent.NPCType<RiftDiggerTail>();

		SoundStyle Roar = new SoundStyle("DestroyerTest/Assets/Audio/WormRoar")
		{
			Volume = 0.4f,
			PitchVariance = 0.2f,
			MaxInstances = 3
		};

		public override void SetStaticDefaults() {
			var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers() { 
				CustomTexturePath = "DestroyerTest/Content/Entities/RiftDiggerHeadBestiary", 
				Position = new Vector2(40f, 24f),
				PortraitPositionXOverride = 0f,
				PortraitPositionYOverride = 12f
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
            Banner = Type;
            BannerItem = Mod.Find<ModItem>("Item_RiftDiggerBanner").Type;
        }

		public override void SetDefaults() {
			// Head is 10 defense, body 20, tail 30.
			NPC.CloneDefaults(NPCID.DiggerHead);
			NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit12;
            NPC.DeathSound = SoundID.NPCDeath26;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(DTUtils.GetModNPCLocalizationEntry(this, 1)),
            });
        }

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			// Check if the player is in the Shimmer zone and the Cultist Boss has been defeated
			if (ModContent.GetInstance<RiftSurface>().IsBiomeActive(spawnInfo.Player)) // Ensure the Cultist Boss has been defeated
			{
					// Set spawn chance relative to standard overworld night monsters
					return SpawnCondition.OverworldDaySlime.Chance; // 10% of regular zombie spawn rate
				
			}
			return 0f; // Prevent spawning otherwise
		}

		public override void Init() {
			// Set the segment variance
			// If you want the segment length to be constant, set these two properties to the same value
			MinSegmentLength = 6;
			MaxSegmentLength = 12;

			CommonWormInit(this);
		}

		// This method is invoked from ExampleWormHead, ExampleWormBody and ExampleWormTail
		internal static void CommonWormInit(Worm worm) {
			// These two properties handle the movement of the worm
			worm.MoveSpeed = 5.5f;
			worm.Acceleration = 0.045f;
		}

		private int attackCounter;
		public override void SendExtraAI(BinaryWriter writer) {
			writer.Write(attackCounter);
		}

		public override void ReceiveExtraAI(BinaryReader reader) {
			attackCounter = reader.ReadInt32();
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (attackCounter < 120 && NPC.HasValidTarget)
            {
                Vector2 V = NPC.Center.DirectionTo(Main.player[NPC.target].Center);

				Main.EntitySpriteDraw(DTAssetLib.GlowCone.Value, NPC.Center - Main.screenPosition, null, Color.Red, V.ToRotation() + MathHelper.PiOver4, DTAssetLib.GlowCone.Value.Size() / 2, MathHelper.Lerp(0f, 1.4f, (float)attackCounter / 120f), SpriteEffects.None);
            }
            return true;
        }

		public override void AI() 
		{
			NPC.TargetClosest();
			if (NPC.HasValidTarget)
			{
				Player player = Main.player[NPC.target];
				Vector2 Ideal = player.MountedCenter + new Vector2(0f, 200f);

                attackCounter++;

				if (attackCounter < 120)
				{
					NPC.SmoothMoveToPoint(Ideal, 16f);
				}
				else
				{
					if (attackCounter == 121)
					{
						SoundEngine.PlaySound(Roar, NPC.position);
						Vector2 V = NPC.Center.DirectionTo(player.Center);
						NPC.velocity += V * 10f;
					}
					
					if (attackCounter > 240)
					{
						attackCounter = 0;
					}
				}
			}
		}

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CarbonizedFlesh>(), 3, 6, 8));
        }
    }

	internal class RiftDiggerBody : WormBody
	{
		public override void SetStaticDefaults() {
			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers() {
				Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
			NPCID.Sets.RespawnEnemyID[NPC.type] = ModContent.NPCType<RiftDiggerHead>();
		}

		public override void SetDefaults() {
			NPC.CloneDefaults(NPCID.DiggerBody);
			NPC.aiStyle = -1;

			// Extra body parts should use the same Banner value as the main ModNPC.
			Banner = ModContent.NPCType<RiftDiggerHead>();
		}

		public override void Init() {
			RiftDiggerHead.CommonWormInit(this);
		}
	}

	internal class RiftDiggerTail : WormTail
	{
		public override void SetStaticDefaults() {
			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers() {
				Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
			NPCID.Sets.RespawnEnemyID[NPC.type] = ModContent.NPCType<RiftDiggerHead>();
		}

		public override void SetDefaults() {
			NPC.CloneDefaults(NPCID.DiggerTail);
			NPC.aiStyle = -1;

			// Extra body parts should use the same Banner value as the main ModNPC.
			Banner = ModContent.NPCType<RiftDiggerHead>();
		}

		public override void Init() {
			RiftDiggerHead.CommonWormInit(this);
		}
	}
}