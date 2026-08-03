
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using Humanizer;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using ReLogic.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
	public class ShimmeringFlames : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
			BuffID.Sets.CanBeRemovedByNetMessage[Type] = true;
		}
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<SFPlayer>().lifeRegenDebuff = true;
		}

		public int CheckTimer = 20;
		public override void Update(NPC target, ref int buffIndex)
		{
			if (target.TryGetGlobalNPC<SFTarget>(out var modNPC))
			{
				modNPC.lifeRegenDebuff = true;

				if (modNPC.Stack <= 0)
				{
					if (CheckTimer <= 0)
					{
                        modNPC.lifeRegenDebuff = false;
                        //target.DelBuff(buffIndex);
					}
					else
					{
                        
                        CheckTimer--;
					}
                }
			}
		}

		public static void ShimmerBurn(NPC npc, bool Sound = true)
		{
			if (npc.HasBuff<ShimmeringFlames>())
			{
				if (npc.TryGetGlobalNPC<SFTarget>(out var shimmer))
				{
					if (Sound)
					{
						SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ShimmeringFlamesTierRaise") { Pitch = 1f - (shimmer.Stack / shimmer.MaxStack), MaxInstances = 1 }, npc.Center);
					}

					for (int i = 0; i < 8; i++)
					{
						Vector2 Dir = Main.rand.NextVector2Circular(3, 3);
                        Vector2 Dir2 = Main.rand.NextVector2Circular(3, 3);

                        Fire F = new Fire();
						F.PrepareFire(npc.Center, Dir, Math.Sign(Dir.X), 0.15f, ColorLib.TenebrisGradient, 0.75f, 80, FireDrawMode.Additive, PixelLayer.AboveNPCs);
						ParticleEngine.Particles.Add(F);

						if (!DTOptimizationsConfig.instance.DisableExcessParticles)
						{
							TenebrousCloudParticle C = new();
							C.Initialize(npc.Center, Dir2, ColorLib.TenebrisGradient, 0.8f, 0.3f, 120);
							ParticleEngine.Particles.Add(C);
						}
                    }

					Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 6, npc.Center, 75, ColorLib.TenebrisGradient, 1f, 3f);
					if (shimmer.Stack < shimmer.MaxStack)
					{
						shimmer.Stack += 1;
					}
					if (shimmer.Stack >= shimmer.MaxStack)
					{
						if (Sound)
						{
							SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/TenebrisImpact") { MaxInstances = 1 }, npc.Center);
						}
					}

				}
			}
			else
			{
				npc.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 9999);
			}
		}
	}

	public class SFTarget : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool lifeRegenDebuff;
		public int Stack = 0;
		public int MaxStack = 16;
		public int MinTimer = 120;

        public override void ResetEffects(NPC npc) 
		{
            lifeRegenDebuff = false;
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			if (lifeRegenDebuff)
			{
				Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive,SpriteSortMode.Immediate);
				DynamicSpriteFont spriteFont = FontAssets.MouseText.Value;
				Vector2 Size = spriteFont.MeasureString(Stack.ToString());
				Utils.DrawBorderString(spriteBatch, Stack.ToString(), (npc.Bottom + new Vector2(0, 20)) - Main.screenPosition, ColorLib.TenebrisGradient, 1f, 0.5f, 0.5f);
				Opus.ReturnToDefaultDrawing(spriteBatch);
			}
		}

        public override void AI(NPC npc)
        {
            if (lifeRegenDebuff)
			{
				if (MinTimer > 0)
				{
					MinTimer--;
				}
				if (Main.GameUpdateCount % 60 == 0 && Stack > 0 && MinTimer <= 0)
				{
					Stack--;
				}
			}
			else
			{
				Stack = 0;
			}

			if (Stack > 0)
			{
				lifeRegenDebuff = true;
			}
        }


        public override void UpdateLifeRegen(NPC npc, ref int damage) 
		{
            if (lifeRegenDebuff) 
			{
                Fire fire = new Fire();
                fire.PrepareFire(Main.rand.NextVector2FromRectangle(npc.Hitbox), new Vector2(0f, -0.1f), Main.rand.Next(1, 3), 0.08f, ColorLib.TenebrisGradient * 0.8f, 0.5f, 40, FireDrawMode.Additive, PixelLayer.AbovePlayer);
                ParticleEngine.ShaderParticles.Add(fire);

				if (Main.rand.NextBool(2) && !DTOptimizationsConfig.instance.DisableExcessParticles)
				{
					TenebrousCloudParticle C = new();
					C.Initialize(Main.rand.NextVector2FromRectangle(npc.Hitbox), new Vector2(0f, -2f), ColorLib.TenebrisGradient, 0.5f, 0.15f, 120);
					ParticleEngine.ShaderParticles.Add(C);
				}

				if (Stack > 8)
				{
					int Chance = MaxStack - Stack;
					Chance = (int)MathHelper.Clamp(Chance, 1, MaxStack);
					if (Main.rand.NextBool(Chance) && !DTOptimizationsConfig.instance.DisableExcessParticles)
					{
                        Spark Spark = new Spark();

                        Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(npc.Hitbox), new Vector2(0f, -1f).RotatedByRandom(0.05f), 0f, ColorLib.TenebrisGradient, 1f, false, 40, SparkDrawMode.Additive);
                        ParticleEngine.ShaderParticles.Add(Spark);
					}
				}
				if (Main.rand.NextBool(6) && !DTOptimizationsConfig.instance.DisableExcessParticles)
				{
					SmallShine Shine = new SmallShine();
					Shine.Prepare(Main.rand.NextVector2FromRectangle(npc.Hitbox), Vector2.Zero, Color.White, 0.25f);
                    ParticleEngine.ShaderParticles.Add(Shine);
                }

                if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				float t = Stack / (float)MaxStack;
				int DamageAmt = (int)MathHelper.Lerp(10, 90, t);

                npc.lifeRegen -= DamageAmt;
            }
        }
    }

	public class SFPlayer : ModPlayer
	{
		public bool lifeRegenDebuff;

		public override void ResetEffects() 
		{
			lifeRegenDebuff = false;
		}

		public override void UpdateBadLifeRegen() {
			if (lifeRegenDebuff)
			{

                Fire fire = new Fire();
                fire.PrepareFire(Main.rand.NextVector2FromRectangle(Player.Hitbox), new Vector2(0f, -0.1f), Main.rand.Next(1, 3), 0.08f, ColorLib.TenebrisGradient * 0.8f, 0.5f, 40, FireDrawMode.Additive, PixelLayer.AboveNPCs);
				ParticleEngine.ShaderParticles.Add(fire);

                if (Main.rand.NextBool(2) && !DTOptimizationsConfig.instance.DisableExcessParticles)
                {
                    TenebrousCloudParticle C = new();
                    C.Initialize(Main.rand.NextVector2FromRectangle(Player.Hitbox), new Vector2(0f, -2f), ColorLib.TenebrisGradient, 0.5f, 0.15f, 120);
                    ParticleEngine.ShaderParticles.Add(C);
                }

                if (Main.rand.NextBool(6) && !DTOptimizationsConfig.instance.DisableExcessParticles)
				{
					SmallShine Shine = new SmallShine();
					Shine.Prepare(Main.rand.NextVector2FromRectangle(Player.Hitbox), Vector2.Zero, Color.White, 0.5f);
					ParticleEngine.ShaderParticles.Add(Shine);
				} 
			
				if (Player.lifeRegen > 0)
					Player.lifeRegen = 0;
				
				Player.lifeRegenTime = 0;
		
				Player.lifeRegen -= 48;
			}
		}
	}
}