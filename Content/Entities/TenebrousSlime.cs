using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.RiftBiome;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using OpusLib.Content.Helpers;
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
			NPC.aiStyle = DestroyerTestMod.EternityIsActive ? -1 : NPCAIStyleID.Slime;
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

        int O = 0;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            O -= 20;
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

			Texture2D Tex = ModContent.Request<Texture2D>(Texture).Value;

			Main.EntitySpriteDraw(Tex, NPC.position - Main.screenPosition, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, Vector2.Zero, NPC.scale, SpriteEffects.None);

            Line Warn = new(NPC.Center, NPC.Center + new Vector2(0, 1300));
            if (NPC.ai[0] > 120 && NPC.ai[0] < 135)
            {
                float Opac = MathHelper.Lerp(0f, 1f, Utilities.Convert01To010(NPC.ai[2] / 15f));
                DTUtils.instance.ScrollingTextureSpine(Warn, DTAssetLib.ArrowTelegraphCont, drawColor with { A = 0 } * Opac, spriteBatch, BlendState.Additive, O, 0.3f);
            }
            
            
            return false;
        }

        int CurrentFrame = 0;
        public override void FindFrame(int frameHeight)
        {
            if (!DestroyerTestMod.EternityIsActive)
            {
                base.FindFrame(frameHeight);
                return;
            }
            else
            {
                NPC.frame.Y = CurrentFrame * frameHeight;
            }
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

		public enum EternityAIState
		{
			Float,
			Slam,
            Idle
		}

        EternityAIState state = EternityAIState.Idle;

        public override void AI()
        {
            NPC.TargetClosest();
            Player target = Main.player[NPC.target];

            if (DestroyerTestMod.EternityIsActive)
			{
                
                Color c = Color.White;
                switch (variant)
                {
                    case 0:
                        c = ColorLib.TenebrisBlue;
                        break;
                    case 1:
                        c = ColorLib.TenebrisMagenta;
                        break;
                    case 2:
                        c = ColorLib.TenebrisBeige;
                        break;
                }

                switch (state)
                {
                    case EternityAIState.Float:
                        {
                            NPC.noGravity = true;
                            NPC.ai[0]++;

                            if (NPC.ai[0] < 120)
                            {
                                
                                NPC.SmoothMoveToPoint(target.Center + new Vector2(0, -300), 15f, 100);
                                CurrentFrame = 1;
                            }
                            if (NPC.ai[0] > 120 && NPC.ai[0] < 135)
                            {
                                NPC.ai[2]++;
                                if (NPC.ai[0] == 121)
                                {
                                    SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost with { Pitch = -0.7f });
                                }
                            }
                            if (NPC.ai[0] >= 135)
                            {
                                state = EternityAIState.Slam;
                                NPC.ai[0] = 0;
                                NPC.ai[2] = 0;
                            }
                            break;
                        }
                    case EternityAIState.Slam:
                        {
                            if (!NPC.collideY)
                            {
                                NPC.velocity.Y = 30f;
                                NPC.velocity.X = 0;
                                CurrentFrame = 1;
                            }
                            else
                            {
                                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.Center);
                                state = EternityAIState.Idle;
                            }
                            break;
                        }
                    case EternityAIState.Idle:
                        {
                            NPC.noGravity = false;

                            NPC.ai[1]++;

                            if (NPC.ai[1] % 15 == 0)
                            {
                                if (CurrentFrame++ >= 1)
                                {
                                    CurrentFrame = 0;
                                }
                            }

                            if (NPC.ai[1] >= 240)
                            {
                                state = EternityAIState.Float;
                                NPC.ai[1] = 0;
                            }
                            break;
                        }
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {

        }
    }
}