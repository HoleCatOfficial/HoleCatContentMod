using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Common;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DestroyerTest.Content.Projectiles;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using OpusLib;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Head)]
	public class TenebrousDemonVisor : ModItem
	{


		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 20;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<ShimmeringRarity>();
			Item.defense = 17;
		}
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<TenebrousDemonChestplate>() && legs.type == ModContent.ItemType<TenebrousDemonChausses>();
		}
		public override void UpdateArmorSet(Player player)
		{
			player.DefaultSetBonusText(player.armor[0]);
			if (player.TryGetModPlayer<TenebrousDemon>(out TenebrousDemon Demon))
			{
				Demon.Active = true;
			}
		}
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<RiftGuardVisor>(1)
				.AddIngredient<Tenebris>(6)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}

	public class TenebrousDemon : ModPlayer
	{
		public bool Active = false;
		public bool Charge1 = false;
		public bool Charge2 = false;
		public bool Charge3 = false;
		public const int ComboTierThreshold = 40;
		public const int ComboExpire = 180;
		public int ComboExpireTimer = 0;
		public int ComboCounter = 0;
		float AdvTierPitch = 0f;
		public SoundStyle AdvTier = new SoundStyle("DestroyerTest/Assets/Audio/Charge/Anvil") with { MaxInstances = 0 };
		public float ColorLerpProgress = 0f;

		public override void ResetEffects()
		{
			Active = false;
		}

		public override void PostUpdateEquips()
		{
			if (Active)
			{
				ComboExpireTimer++;
				if (ComboExpireTimer == ComboExpire)
				{
					SoundEngine.PlaySound(SoundID.DD2_WitherBeastHurt, Player.Center);
					ComboCounter = 0;
					AdvTierPitch = 0f;
					Charge1 = Charge2 = Charge3 = false;
					SoundFlag1 = SoundFlag2 = SoundFlag3 = false;
				}
				if (Player.HeldItem.DamageType == DamageClass.Ranged)
				{
					if (ComboCounter >= ComboTierThreshold && !Charge1 && !Charge2 && !Charge3)
					{
						Charge1 = true;
						ComboCounter = 0;
					}
					if (ComboCounter >= ComboTierThreshold && Charge1 && !Charge2 && !Charge3)
					{
						Charge2 = true;
						ComboCounter = 0;
					}
					if (ComboCounter >= ComboTierThreshold && Charge1 && Charge2 && !Charge3)
					{
						Charge3 = true;
						ComboCounter = 0;
					}

					ChargeEffects();
				}
				else
				{
					Charge1 = Charge2 = Charge3 = false;
					AdvTierPitch = 0f;
				}
			}
		}

		public bool SoundFlag1 = false;
		public bool SoundFlag2 = false;
		public bool SoundFlag3 = false;

		public void ChargeEffects()
		{
			if (Charge1)
			{
				if (!SoundFlag1)
				{
					SoundEngine.PlaySound(AdvTier with { Pitch = AdvTierPitch }, Player.Center);
					AdvTierPitch += 0.2f;

					TextScale = 1.25f;
					TextColor = Color.White;
					ColorLerpProgress = 0f;

					SoundFlag1 = true;
				}
				Player.GetDamage(DamageClass.Ranged) *= 1.1f;
				Player.GetCritChance(DamageClass.Ranged) += 6;
				Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, DustID.FireworksRGB, (Player.velocity.X / 2) + Main.rand.NextFloat(-2, 2), (Player.velocity.Y / 2) + Main.rand.NextFloat(-2, 2), 200, ColorLib.TenebrisGradient, 0.4f);
				if (Main.rand.NextBool(16))
                {
                    PRTLoader.NewParticle(DTUtils.ElectricArcs[Main.rand.Next(DTUtils.ElectricArcs.Length)], Main.rand.NextVector2FromRectangle(Player.Hitbox), Vector2.Zero, ColorLib.TenebrisGradient * 0.5f, 0.05f);
                }
			}
			if (Charge2)
			{
				if (!SoundFlag2)
				{
					SoundEngine.PlaySound(AdvTier with { Pitch = AdvTierPitch }, Player.Center);
					AdvTierPitch += 0.2f;

					TextScale = 1.25f;
					TextColor = Color.White;
					ColorLerpProgress = 0f;

					SoundFlag2 = true;
				}
				Player.GetDamage(DamageClass.Ranged) *= 1.1f;
				Player.GetCritChance(DamageClass.Ranged) += 6;
				Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, DustID.FireworksRGB, (Player.velocity.X / 2) + Main.rand.NextFloat(-2, 2), (Player.velocity.Y / 2) + Main.rand.NextFloat(-2, 2), 100, ColorLib.TenebrisGradient, 0.6f);
				if (Main.rand.NextBool(12))
                {
                    PRTLoader.NewParticle(DTUtils.ElectricArcs[Main.rand.Next(DTUtils.ElectricArcs.Length)], Main.rand.NextVector2FromRectangle(Player.Hitbox), Vector2.Zero, ColorLib.TenebrisGradient * 0.75f, 0.075f);
                }
			}
			if (Charge3)
			{
				if (!SoundFlag3)
				{
					SoundEngine.PlaySound(AdvTier with { Pitch = AdvTierPitch }, Player.Center);
					SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Destitute") with { PitchVariance = 0.2f }, Player.Center);

					TextScale = 1.25f;
					TextColor = Color.White;
					ColorLerpProgress = 0f;

					SoundFlag3 = true;
				}
				Player.GetDamage(DamageClass.Ranged) *= 1.2f;
				Player.GetCritChance(DamageClass.Ranged) += 10;
				Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, DustID.FireworksRGB, (Player.velocity.X / 2) + Main.rand.NextFloat(-2, 2), (Player.velocity.Y / 2) + Main.rand.NextFloat(-2, 2), 0, ColorLib.TenebrisGradient, 1f);
				if (Main.rand.NextBool(8))
                {
                    PRTLoader.NewParticle(DTUtils.ElectricArcs[Main.rand.Next(DTUtils.ElectricArcs.Length)], Main.rand.NextVector2FromRectangle(Player.Hitbox), Vector2.Zero, ColorLib.TenebrisGradient, 0.1f);
                }
				if (ComboCounter >= 120)
				{
					SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/HoleCatHookFreeze") with { PitchVariance = 0.2f }, Player.Center);
					Charge1 = Charge2 = Charge3 = false;
					SoundFlag1 = SoundFlag2 = SoundFlag3 = false;
					ComboCounter = 0;
					AdvTierPitch = 0f;
				}
			}
		}
		
		public float TextScale = 1f;
		public Color TextColor = ColorLib.TenebrisGradient;
		public void Text()
		{
			Vector2 drawPos = Player.Center - Main.screenPosition;
			SpriteBatch spriteBatch = Main.spriteBatch;
			drawPos.Y -= 200;

			string text = $"Combo: {ComboCounter.ToString()}";

			if (Active)
			{
				if (TextScale > 1f)
				{
					TextScale -= 0.005f;
					if (TextScale < 1f) 
					{
						TextScale = 1f;
					}
				}
				if (TextColor != ColorLib.TenebrisGradient)
				{
					ColorLerpProgress += 0.005f;
					if (ColorLerpProgress > 1f) ColorLerpProgress = 1f;
					float t = 1f - (1f - ColorLerpProgress) * (1f - ColorLerpProgress);
					TextColor = Color.Lerp(TextColor, ColorLib.TenebrisGradient, t);
				}
				Utils.DrawBorderString(spriteBatch, text, drawPos, TextColor, TextScale, 0.5f, 0.5f);
			}
		}
		public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			Text();	
		}

	}

	public class TenebrousDemonItemModifier : GlobalItem
	{
		public override bool InstancePerEntity => true;
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.TryGetModPlayer<TenebrousDemon>(out TenebrousDemon Demon) && item.DamageType == DamageClass.Ranged)
			{
				if (Demon.Active)
				{
					if (Demon.Charge3)
					{
						Projectile.NewProjectile(source, position, velocity.RotatedByRandom(1f), ModContent.ProjectileType<TenebrisStarFriendly>(), damage / 2, knockback, player.whoAmI, ai2: 1);
					}
				}
			}
			return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
		}

	}


	public class TenebrousDemonHitTracker : GlobalProjectile
	{
		public override bool InstancePerEntity => true;
		public float CPitch = 0f;
		public SoundStyle C = new SoundStyle("DestroyerTest/Assets/Audio/Charge/WoodyTick3") with { MaxInstances = 0 };
		public void ChargeSounds1()
		{
			SoundEngine.PlaySound(C with { Pitch = CPitch });
			CPitch += (1f / 300f);			
		}

		public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[projectile.owner];
			DTOptimizationsConfig OptCfg = ModContent.GetInstance<DTOptimizationsConfig>();
			if (player.TryGetModPlayer<TenebrousDemon>(out TenebrousDemon Demon) && projectile.DamageType == DamageClass.Ranged)
			{
				if (Demon.Active)
				{
					Demon.ComboExpireTimer = 0;
					ChargeSounds1();
					Demon.TextScale = 1.05f;
					if (Demon.ComboCounter < TenebrousDemon.ComboTierThreshold && !Demon.Charge3)
					{
						Demon.ComboCounter++;
					}
					if (Demon.ComboCounter <= 120 && Demon.Charge3)
					{
						Demon.ComboCounter++;
					}

					if (Demon.ComboCounter > 119 && Demon.Charge3)
					{
						CPitch = 0f;
					}

					if (Demon.Charge3 && OptCfg.DisableExcessParticles)
					{
						for (int t = 0; t < 7; t++)
						{
							PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), target.Center, (projectile.velocity.RotatedByRandom(0.2f) * 2), ColorLib.TenebrisGradient, 0.5f, 2);
						}
					}
				}
            }
			if (Demon.Active)
            {
                if (Demon.Charge3 && (projectile.type == ModContent.ProjectileType<TenebrisStarFriendly>() || projectile.type == ModContent.ProjectileType<TenebrisStarFriendly_NoHoming>()) && OptCfg.DisableExcessParticles)
					{
						for (int t = 0; t < 7; t++)
						{
							PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), target.Center, (projectile.velocity.RotatedByRandom(0.2f) * 2), ColorLib.TenebrisGradient, 0.5f, 2);
						}
					}
            }
		}

	}
}