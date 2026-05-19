using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader.Utilities;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Common;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.ItemDropRules;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using Terraria.GameContent;
using OpusLib;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Projectiles;
using Terraria.Audio;

namespace DestroyerTest.Content.Entities
{
    public class RiftObserver : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
        }
        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 32;
            NPC.damage = 20;
            NPC.defense = 15;
            NPC.lifeMax = 1200;
            NPC.value = 100f;
            NPC.knockBackResist = 0.2f;
            NPC.aiStyle = NPCAIStyleID.DemonEye;
            NPC.HitSound = SoundID.Item51;
            NPC.DeathSound = SoundID.Item62;
            NPC.noGravity = true;
        }

        private int frameIndex;

        public override void FindFrame(int frameHeight)
        {
            
            if (NPC.life < NPC.lifeMax / 2)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 10)
                {
                    NPC.frameCounter = 0;
                    frameIndex++;
                    if (frameIndex > 3)
                        frameIndex = 2;
                }
            }
            else
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    frameIndex++;
                    if (frameIndex > 1)
                        frameIndex = 0;
                }
            }
        

            NPC.frame.Y = frameIndex * frameHeight;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(DTUtils.GetModNPCLocalizationEntry(this, 1)),
            });
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effects = SpriteEffects.None;
            if (NPC.direction == -1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
            DrawCrystalCore(spriteBatch, NPC.Center);
            //Main.EntitySpriteDraw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2), NPC.scale, effects, 0);
            return true;
        }
        public void DrawCrystalCore(SpriteBatch spriteBatch, Vector2 Center)
        {
            DTUtils Utility = new DTUtils();
            
            for (int i = 0; i < TrailPositions.Count; i++)
			{
				float progress = i / (float)TrailLength;
				float scale = MathHelper.Lerp(0.5f, 0.0001f, progress);
				Color color = Color.Black;

				Main.EntitySpriteDraw(
					DTAssetLib.FeatheredCircle.Value,
					TrailPositions[i] - Main.screenPosition,
					null,
					color,
					TrailRotations[i],
					DTAssetLib.FeatheredCircle.Value.Size() / 2f,
					scale,
					SpriteEffects.None,
					0
				);
			}

            Main.spriteBatch.Draw(
                DTAssetLib.FeatheredCircle.Value,
                Center - Main.screenPosition,
                null,
                Color.Black,
                0f,
                new Vector2(DTAssetLib.FeatheredCircle.Value.Width / 2f, DTAssetLib.FeatheredCircle.Value.Height / 2f),
                0.5f,
                SpriteEffects.None,
                1f
            );
        }

        public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
        private const int TrailLength = 40;

        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            TrailPositions.Insert(0, NPC.Center);
            TrailRotations.Insert(0, NPC.rotation);

            while (TrailPositions.Count > TrailLength)
                TrailPositions.RemoveAt(TrailPositions.Count - 1);
            while (TrailRotations.Count > TrailLength)
                TrailRotations.RemoveAt(TrailRotations.Count - 1);

            Vector2 look = player.Center - NPC.Center;
            NPC.rotation = look.ToRotation();
            //NPC.spriteDirection = look.X > 0 ? 1 : -1;

            if (Main.rand.NextBool(200) && NPC.life < NPC.lifeMax / 2)
            {
                SoundEngine.PlaySound(SoundID.ForceRoarPitched with { PitchVariance = 0.5f } , NPC.Center);
                Opus.RingSpreadDust(ModContent.DustType<RiftDust>(), 30, NPC.Center, 10, 0, default, 1.2f, 3f, offset: NPC.rotation);
                NPC.velocity += look.ToRotation().ToRotationVector2() * 3f;
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, look.ToRotation().ToRotationVector2() * 2f, ModContent.ProjectileType<RiftSparkHostile>(), 14, 5f);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
            bool v = (ModContent.GetInstance<RiftSurface>().IsBiomeActive(spawnInfo.Player) || 
            ModContent.GetInstance<RiftUnderground>().IsBiomeActive(spawnInfo.Player) ||
            ModContent.GetInstance<RiftDesert>().IsBiomeActive(spawnInfo.Player) ||
            ModContent.GetInstance<RiftDesertUnderground>().IsBiomeActive(spawnInfo.Player) ||
            ModContent.GetInstance<RiftTundra>().IsBiomeActive(spawnInfo.Player));
			if (v)
			{
				return 0.3f;
			}
			return 0f;
		}

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Wraith);
                }
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Circular(6, 6), 99);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Living_Shadow>(), 1, 3, 10));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Item_RiftStone>(), 1, 1, 5));
        }
    }
}