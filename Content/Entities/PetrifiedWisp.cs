using System.Collections.Generic;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftArsenal;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace DestroyerTest.Content.Entities
{
    public class PetrifiedWisp : ModNPC
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
            Banner = Type;
            NPCID.Sets.TrailCacheLength[Type] = 50;
            NPCID.Sets.TrailingMode[Type] = 3;
        }
        public override void SetDefaults()
        {
            switch (Tier)
            {
                case 1:
                    NPC.width = 40;
                    NPC.height = 400;
                    NPC.damage = 10;
                    break;
                case 2:
                    NPC.width = 60;
                    NPC.height = 60;
                    NPC.damage = 20;
                    break;
                case 3:
                    NPC.width = 110;
                    NPC.height = 110;
                    NPC.damage = 50;
                    break;
            }

            NPC.defense = 5;
            NPC.lifeMax = 1200;
            NPC.value = 100f;
            NPC.knockBackResist = 0.2f;
            NPC.aiStyle = -1;
            NPC.HitSound = new SoundStyle(DTAssetLib.AudioPath + "/PetrifiedWisp/Hit") { PitchVariance = 0.3f, MaxInstances = 0};
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            if (NPC.ai[0] == 0)
            {
                NPC.ai[0] = 3;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(DTUtils.GetModNPCLocalizationEntry(this, 1)),
            });
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            TextureRotationOffset -= 0.2f;

            if (NPC.IsABestiaryIconDummy)
            {

                Main.spriteBatch.Draw(DTAssetLib.FeatheredCircle.Value, NPC.Center - screenPos, null, ColorLib.Rift with { A = 0 }, TextureRotationOffset, DTAssetLib.FeatheredCircle.Value.Size() / 2f, 3f, SpriteEffects.None, 1f);
                Main.spriteBatch.Draw(DTAssetLib.FeatheredCircle.Value, NPC.Center - screenPos, null, Color.Black, TextureRotationOffset, DTAssetLib.FeatheredCircle.Value.Size() / 2f, 3f * 0.7f, SpriteEffects.None, 1f);
                return true;
            }

            DrawCrystalCore(spriteBatch, screenPos, NPC.Center);
            return true;
        }
        public void DrawCrystalCore(SpriteBatch spriteBatch, Vector2 screenPos, Vector2 Center)
        {
            // Helper method from a utility mod.

            float scale = 1f;
            switch (Tier)
            {
                case 1:
                    scale = 1f;
                    break;
                case 2:
                    scale = 2f;
                    break;
                case 3:
                    scale = 3f;
                    break;
            }

            


            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                float progress = i / (float)NPC.oldPos.Length;
                float trailscale = MathHelper.Lerp(scale, 0.0005f, progress);
                Color color = ColorLib.Rift with { A = 0 };

                Main.EntitySpriteDraw(DTAssetLib.FeatheredCircle.Value, NPC.OldCenter()[i] - screenPos, null, color, TextureRotationOffset, DTAssetLib.FeatheredCircle.Value.Size() / 2f, trailscale, SpriteEffects.None, 0);
            }

            Main.spriteBatch.Draw(DTAssetLib.FeatheredCircle.Value, Center - screenPos, null, ColorLib.Rift with { A = 0 }, TextureRotationOffset, DTAssetLib.FeatheredCircle.Value.Size() / 2f, scale, SpriteEffects.None, 1f);

            
            for (int i = 0; i < NPC.oldPos.Length; i++)
			{
				float progress = i / (float)NPC.oldPos.Length;
                float trailscale = MathHelper.Lerp(scale * 0.7f, 0.001f, progress); 
				Color color = Color.Black;

				Main.EntitySpriteDraw(DTAssetLib.FeatheredCircle.Value, NPC.OldCenter()[i] - screenPos, null, color, NPC.rotation, DTAssetLib.FeatheredCircle.Value.Size() / 2f, trailscale, SpriteEffects.None, 0);
			}

            Main.spriteBatch.Draw(DTAssetLib.FeatheredCircle.Value, Center - screenPos, null, Color.Black, NPC.rotation, DTAssetLib.FeatheredCircle.Value.Size() / 2f, scale * 0.7f, SpriteEffects.None, 1f);
        }
        public float TextureRotationOffset = 0f;

        ref float Tier => ref NPC.ai[0];
        public int time = 0;
        public int slowInterval = 90;

        public override void OnSpawn(IEntitySource source)
        {
            if (Tier != 3)
            {
                NPC.velocity += Main.rand.NextVector2Circular(8f, 8f);
            }
        }
        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];

            if (NPC.ai[0] == 0)
            {
                NPC.ai[0] = 3;
            }

            time++;
            
            
            if (time < 90)
            {
                NPC.velocity *= 0.95f;
            }
            if (time > 90 && time < 300)
            {
                NPC.SmoothMoveToPoint(player.MountedCenter, 6f, 150f);
            }
            if (time > 300)
            {
                time = 0;
                slowInterval = Main.rand.Next(9, 17) * 10;
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
				return 0.5f;
			}
			return 0f;
		}

        public override void HitEffect(NPC.HitInfo hit)
        {
            SoundStyle Death = new SoundStyle(DTAssetLib.AudioPath + "/PetrifiedWisp/Death", 13) { PitchVariance = 0.3f, MaxInstances = 0 };
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 32; i++)
                {
                    PixelParticle Pixel = new();
                    Pixel.Initialize(NPC.Center, Main.rand.NextVector2Circular(5f, 5f), ColorLib.Rift, 2f);
                    ParticleEngine.Particles.Add(Pixel);
                }

                int amount = Tier == 3 ? 2 : 3;
                for (int i = 0; i < amount; i++)
                {
                    switch (Tier)
                    {
                        case 3:
                            {
                                SoundEngine.PlaySound(Death);
                                NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PetrifiedWisp>(), ai0: 2);
                                break;
                            }
                        case 2:
                            {
                                SoundEngine.PlaySound(Death);
                                NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PetrifiedWisp>(), ai0: 1);
                                break;
                            }
                        case 1:
                            {
                                SoundEngine.PlaySound(Death);
                                break;
                            }
                    }
                    
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Living_Shadow>(), 1, 3, 10));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RiftWhipT1>(), 100, 1, 1));
        }
    }
}