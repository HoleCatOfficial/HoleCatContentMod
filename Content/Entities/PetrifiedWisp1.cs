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
using Terraria.Audio;

namespace DestroyerTest.Content.Entities
{
    public class PetrifiedWisp1 : ModNPC
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            NPC.width = 110;
            NPC.height = 110;
            NPC.damage = 20;
            NPC.defense = 15;
            NPC.lifeMax = 1200;
            NPC.value = 100f;
            NPC.knockBackResist = 0.8f;
            NPC.aiStyle = NPCAIStyleID.Corite;
            NPC.HitSound = new SoundStyle("DestroyerTest/Assets/Audio/StellarBow/StellarBowArrowImpact", 4) { MaxInstances = 0, PitchVariance = 0.4f };
            NPC.DeathSound = SoundID.Item74;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(DTUtils.GetModNPCLocalizationEntry(this, 1)),
            });
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawCrystalCore(spriteBatch, NPC.Center);
            return true;
        }
        public void DrawCrystalCore(SpriteBatch spriteBatch, Vector2 Center)
        {
            // Helper method from a utility mod.
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            for (int i = 0; i < TrailPositions.Count; i++)
            {
                float progress = i / (float)TrailLength;
                float scale = MathHelper.Lerp(3.9f, 0.0005f, progress);
                Color color = ColorLib.Rift;

                Main.EntitySpriteDraw(
                    DTAssetLib.FeatheredCircle.Value,
                    TrailPositions[i] - Main.screenPosition,
                    null,
                    color,
                    TextureRotationOffset,
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
                ColorLib.Rift,
                TextureRotationOffset,
                DTAssetLib.FeatheredCircle.Value.Size() / 2f,
                3.9f,
                SpriteEffects.None,
                1f
            );

            Opus.ReturnToDefaultDrawing(spriteBatch);
            
            for (int i = 0; i < TrailPositions.Count; i++)
			{
				float progress = i / (float)TrailLength;
				float scale = MathHelper.Lerp(3f, 0.001f, progress);
				Color color = Color.Black;

				Main.EntitySpriteDraw(
					DTAssetLib.FeatheredCircle.Value,
					TrailPositions[i] - Main.screenPosition,
					null,
					color,
					NPC.rotation,
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
                NPC.rotation,
                DTAssetLib.FeatheredCircle.Value.Size() / 2f,
                3f,
                SpriteEffects.None,
                1f
            );
        }

        public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
        private const int TrailLength = 40;
        public float TextureRotationOffset = 0f;

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

            TextureRotationOffset -= 0.2f;
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
				return 0.5f;
			}
			return 0f;
		}

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PetrifiedWisp2>());
                }
            }
        }
    }
}