using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Projectiles.EntitiesProjectiles;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.Scepter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using OpusLib;
using OpusLib.Content.Helpers;
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

	public class PossessedScepter : ModNPC
	{
		public override void SetStaticDefaults()
        {
            Banner = Type;
            Main.npcFrameCount[Type] = 4;
            BannerItem = Mod.Find<ModItem>("Item_PossesedScepterBanner").Type;
        }

		public override void SetDefaults() {
			NPC.width = 66;
			NPC.height = 66;
			NPC.aiStyle = -1;
			NPC.damage = 34;
			NPC.defense = 0;
			NPC.lifeMax = 1250;
			NPC.HitSound = SoundID.NPCHit44;
			NPC.DeathSound = SoundID.NPCDeath43;
            NPC.noGravity = true;
			NPC.lavaImmune = true;
            NPC.noTileCollide = true;
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {

			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
			{
				new FlavorTextBestiaryInfoElement("A scepter, perhaps belonging to an ancient royal. Now its dusty form has been animated by tormented spirits.")
			});
		}

		public override void FindFrame(int frameHeight)
        {
            int startFrame = 0;
            int finalFrame = 3;
            int frameSpeed = 3;
            NPC.frameCounter += 1f;
            if (NPC.frameCounter > frameSpeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y > finalFrame * frameHeight)
                {
                    NPC.frame.Y = startFrame * frameHeight;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
			Opus.DrawNPCShadowsRotating(NPC, NPC.frame, 2f, ColorLib.PossessedScepterColor, 0.2f);
			Opus.ReturnToDefaultDrawing(spriteBatch);
            return true;
        }


        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

            if (player.ZoneDungeon && DownedBossSystem.downedPlanteraBoss)
            {
                return 0.1f; // Or whatever spawn chance you want in the Dungeon
            }

            return 0f; // Prevent spawning otherwise
        }

		public Line Dir;
        public override void AI()
        {
			NPC.TargetClosest();
			Player player = Main.player[NPC.target];
			Dir = new Line(NPC.Center, player.Center);

			float IdealRot = Dir.GetLineRotation;
			float RotDiff = MathF.Atan2(MathF.Sin(IdealRot - NPC.rotation), MathF.Cos(IdealRot - NPC.rotation));

			if (Math.Abs(RotDiff) >= 0.1f)
			{
				float spriteOffset = MathHelper.PiOver4;

				float idealRot = Dir.GetLineRotation;
				float currentRot = NPC.rotation - spriteOffset;

				float rotDiff = MathF.Atan2(
					MathF.Sin(idealRot - currentRot),
					MathF.Cos(idealRot - currentRot)
				);

				currentRot += rotDiff * 0.1f;

				NPC.rotation = MathHelper.WrapAngle(currentRot) + spriteOffset;
			}
			else
			{
				NPC.rotation = IdealRot + MathHelper.PiOver4;
			}

            NPC.ai[0]++;

			int t = (int)NPC.ai[0];
			float ShootDist = 600f * 600f;

			if (t < 300)
			{
				NPC.velocity = Dir.GetLineRotation.ToRotationVector2() * Opus.Sine(1f, 2f);
			}
			if (t > 300)
			{
				NPC.velocity = Dir.GetLineRotation.ToRotationVector2() * Opus.Sine(2f, 5f);
				if (NPC.DistanceSQ(player.Center) <= ShootDist)
				{
					if (t % 60 == 0)
					{
						SoundEngine.PlaySound(SoundID.Zombie103, NPC.Center);
						Shoot();
					}
				}
			}
        }

		private void Shoot()
		{
			Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Dir.GetLineRotation.ToRotationVector2() * 10, ModContent.ProjectileType<PosessedScepterSoulShot>(), NPC.damage / 2, 2);
		}

        public override void OnKill()
        {
            NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.DungeonSpirit);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GhoulishScepter>(), 6, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpiritBauble>(), 18, 1, 1));
        }


    }
}