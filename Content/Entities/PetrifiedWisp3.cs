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
using System;
using Terraria.DataStructures;

namespace DestroyerTest.Content.Entities
{
    public class PetrifiedWisp3 : ModNPC
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
        }
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 30;
            NPC.damage = 20;
            NPC.defense = 15;
            NPC.lifeMax = 800;
            NPC.value = 1670f;
            NPC.knockBackResist = 0.8f;
            NPC.aiStyle = NPCAIStyleID.EnchantedSword;
            NPC.HitSound = new SoundStyle("DestroyerTest/Assets/Audio/StellarBow/StellarBowArrowImpact", 4) { MaxInstances = 0, PitchVariance = 0.4f };
            NPC.DeathSound = SoundID.Item74;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            NPC.velocity += Main.rand.NextVector2Circular(20, 20);
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
                float scale = MathHelper.Lerp(0.08f, 0.0005f, progress);
                Color color = ColorLib.Rift;

                Main.EntitySpriteDraw(
                    DTAssetLib.Cyclone(2).Value,
                    TrailPositions[i] - Main.screenPosition,
                    null,
                    color,
                    TextureRotationOffset,
                    DTAssetLib.Cyclone(2).Value.Size() / 2f,
                    scale,
                    SpriteEffects.None,
                    0
                );
            }

            Main.spriteBatch.Draw(
                DTAssetLib.Cyclone(2).Value,
                Center - Main.screenPosition,
                null,
                ColorLib.Rift,
                TextureRotationOffset,
                new Vector2(DTAssetLib.Cyclone(2).Value.Width / 2f, DTAssetLib.Cyclone(2).Value.Height / 2f),
                0.08f,
                SpriteEffects.None,
                1f
            );

            Opus.ReturnToDefaultDrawing(spriteBatch);
            
            for (int i = 0; i < TrailPositions.Count; i++)
			{
				float progress = i / (float)TrailLength;
				float scale = MathHelper.Lerp(0.1f, 0.001f, progress);
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
                new Vector2(DTAssetLib.FeatheredCircle.Value.Width / 2f, DTAssetLib.FeatheredCircle.Value.Height / 2f),
                0.1f,
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

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Living_Shadow>(), 1, 3, 10));
        }
    }
}