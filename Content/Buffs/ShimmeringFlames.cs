
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using Humanizer;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
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

		public static void ShimmerBurn(NPC npc)
		{
			if (npc.HasBuff<ShimmeringFlames>())
			{
				if (npc.TryGetGlobalNPC<SFTarget>(out var shimmer))
				{
					SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ShimmeringFlamesTierRaise") { Pitch = 1f - (shimmer.Stack / shimmer.MaxStack), MaxInstances = 0});
					Opus.RadialParticleRandomDir(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], 20, npc.Center, 0.75f, ColorLib.TenebrisGradient, 1f, 3f, 40, ai2: 2);
					if (shimmer.Stack < shimmer.MaxStack)
					{
						shimmer.Stack += 1;
					}
					if (shimmer.Stack >= shimmer.MaxStack)
					{
						SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/TenebrisImpact"));
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
				PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Main.rand.NextVector2FromRectangle(npc.Hitbox), new Vector2(0f, -0.1f), ColorLib.TenebrisGradient * 0.35f, 1.0f, 40, ai2: 2);

				if (Stack > 8)
				{
					int Chance = MaxStack - Stack;
					Chance = (int)MathHelper.Clamp(Chance, 1, MaxStack);
					if (Main.rand.NextBool(Chance))
					{
						PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), Main.rand.NextVector2FromRectangle(npc.Hitbox), new Vector2(0f, -1f).RotatedByRandom(0.05f), ColorLib.TenebrisGradient, 1.0f, ai1: 2);
					}
				}
				if (Main.rand.NextBool(6))
				{
					PRTLoader.NewParticle(PRTLoader.GetParticleID<SmallShine>(), Main.rand.NextVector2FromRectangle(npc.Hitbox), Vector2.Zero, Color.White, 0.25f);
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

		// Flag checking when life regen debuff should be activated
		public bool lifeRegenDebuff;

		public override void ResetEffects() {
			lifeRegenDebuff = false;
		}

		// Allows you to give the player a negative life regeneration based on its state (for example, the "On Fire!" debuff makes the player take damage-over-time)
		// This is typically done by setting player.lifeRegen to 0 if it is positive, setting player.lifeRegenTime to 0, and subtracting a number from player.lifeRegen
		// The player will take damage at a rate of half the number you subtract per second
		public override void UpdateBadLifeRegen() {
			if (lifeRegenDebuff)
			{
				

				PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Main.rand.NextVector2FromRectangle(Player.Hitbox), new Vector2(0f, -0.1f), ColorLib.TenebrisGradient * 0.35f, 1f, 40, ai2: 2);
				if (Main.rand.NextBool(6))
				{
					PRTLoader.NewParticle(PRTLoader.GetParticleID<SmallShine>(), Main.rand.NextVector2FromRectangle(Player.Hitbox), Vector2.Zero, Color.White, 0.5f);
				} 
				// These lines zero out any positive lifeRegen. This is expected for all bad life regeneration effects
				if (Player.lifeRegen > 0)
					Player.lifeRegen = 0;
				// Player.lifeRegenTime used to increase the speed at which the player reaches its maximum natural life regeneration
				// So we set it to 0, and while this debuff is active, it never reaches it
				Player.lifeRegenTime = 0;
				// lifeRegen is measured in 1/2 life per second. Therefore, this effect causes 8 life lost per second
				Player.lifeRegen -= 48;
			}
		}
	}
}