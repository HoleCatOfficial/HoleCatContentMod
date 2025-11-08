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

namespace DestroyerTest.Content.Equips
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class TenebrousDemonVisor : ModItem
	{


		public override void SetStaticDefaults()
		{
			// If your head equipment should draw hair while drawn, use one of the following:

		}

		public override void SetDefaults()
		{
			Item.width = 26; // Width of the item
			Item.height = 20; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ModContent.RarityType<ShimmeringRarity>(); // The rarity of the item
			Item.defense = 10; // The amount of defense the item will give when equipped
		}

		// IsArmorSet determines what armor pieces are needed for the setbonus to take effect
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<TenebrousDemonChestplate>() && legs.type == ModContent.ItemType<TenebrousDemonChausses>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			if (player.TryGetModPlayer<TenebrousDemon>(out TenebrousDemon Demon))
			{
				Demon.Active = true;
			}
			player.setBonus = Language.GetTextValue("Mods.DestroyerTest.Items.TenebrousDemonVisor.SetBonus");
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<RiftGuardVisor>(1)
				.AddIngredient<Tenebris>(6)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}

	public class TenebrousDemon : ModPlayer
	{
		public bool Active;
		public bool Charge1, Charge2, Charge3;
		public const int ComboTierThreshold = 40;
		public const int ComboExpire = 180;
		public int ComboExpireTimer;
		public int ComboCounter;

		public bool SoundFlag1, SoundFlag2, SoundFlag3;
		public float MultiplicativeDamageBonus = 1f;
		public int AdditiveCritBonus;

		public override void ResetEffects() => Active = false;

		public override void PostUpdateEquips()
		{
			if (!Active)
			{
				ResetCharges();
				return;
			}

			ComboExpireTimer++;

			if (ComboExpireTimer >= ComboExpire)
			{
				SoundEngine.PlaySound(SoundID.DD2_WitherBeastHurt, Player.Center);
				ResetCharges();
			}

			if (Player.HeldItem.DamageType == DamageClass.Ranged)
			{
				HandleChargeProgression();
				ChargeEffects();
			}
			else
			{
				ResetCharges();
			}
		}

		private void HandleChargeProgression()
		{
			if (ComboCounter < ComboTierThreshold)
				return;

			// Step up through the charges.
			if (!Charge1 && !Charge2 && !Charge3)
			{
				Charge1 = true;
				SoundFlag1 = false; // enable new tier’s sound
			}
			else if (Charge1 && !Charge2)
			{
				Charge1 = false;
				Charge2 = true;
				SoundFlag2 = false;
			}
			else if (Charge2 && !Charge3)
			{
				Charge2 = false;
				Charge3 = true;
				SoundFlag3 = false;
			}

			ComboCounter = 0;
		}

		private void ResetCharges()
		{
			Charge1 = false;
			Charge2 = false;
			Charge3 = false;
			SoundFlag1 = false;
			SoundFlag2 = false;
			SoundFlag3 = false;
			ComboCounter = 0;
			ComboExpireTimer = 0;
			MultiplicativeDamageBonus = 1f;
			AdditiveCritBonus = 0;
		}

		public void ChargeEffects()
		{
			DTConfig cfg = ModContent.GetInstance<DTConfig>();
			if (cfg.EnableDebugMessages && Main.GameUpdateCount % 120 == 0)
			{
				Main.NewText($"Charge1:{Charge1} | Charge2:{Charge2} | Charge3:{Charge3} | Damage Mult:{MultiplicativeDamageBonus} | Crit +{AdditiveCritBonus} | Combo:{ComboCounter}");
			}

			// Clamp counter just in case.
			ComboCounter = Utils.Clamp(ComboCounter, 0, ComboTierThreshold + 1);

			// Apply bonuses.
			if (Charge3)
			{
				if (!SoundFlag3)
				{
					SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { Pitch = 2 }, Player.Center);
					SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Destitute") with { PitchVariance = 0.2f }, Player.Center);
					SoundFlag3 = true;
				}
				MultiplicativeDamageBonus = 1.4f;
				AdditiveCritBonus = 18;
				DustBurst(1f, 0);
				if (ComboCounter >= ComboTierThreshold + 1)
				{
					SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/NodeSpawn") with { PitchVariance = 0.2f }, Player.Center);
					ResetCharges();
				}
			}
			else if (Charge2)
			{
				if (!SoundFlag2)
				{
					SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { Pitch = 1 }, Player.Center);
					SoundFlag2 = true;
				}
				MultiplicativeDamageBonus = 1.2f;
				AdditiveCritBonus = 12;
				DustBurst(0.6f, 100);
			}
			else if (Charge1)
			{
				if (!SoundFlag1)
				{
					SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { Pitch = 0 }, Player.Center);
					SoundFlag1 = true;
				}
				MultiplicativeDamageBonus = 1.1f;
				AdditiveCritBonus = 6;
				DustBurst(0.4f, 200);
			}

			Player.GetDamage(DamageClass.Ranged) *= MultiplicativeDamageBonus;
			Player.GetCritChance(DamageClass.Ranged) += AdditiveCritBonus;
		}

		private void DustBurst(float intensity, int dustAlpha)
		{
			Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height,
				DustID.TintableDustLighted,
				(Player.velocity.X / 2) + Main.rand.NextFloat(-2, 2),
				(Player.velocity.Y / 2) + Main.rand.NextFloat(-2, 2),
				dustAlpha, ColorLib.TenebrisGradient, intensity);

			PRTLoader.NewParticle(
				DTUtils.ElectricArcs[Main.rand.Next(DTUtils.ElectricArcs.Length)],
				Main.rand.NextVector2FromRectangle(Player.Hitbox),
				Vector2.Zero, ColorLib.TenebrisGradient * intensity, 0.05f + intensity / 5);
		}

		public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			if (!Active)
				return;

			Vector2 drawPos = Player.Center - Main.screenPosition;
			drawPos.Y -= 200;
			Utils.DrawBorderString(Main.spriteBatch, $"Combo: {ComboCounter}", drawPos, ColorLib.TenebrisGradient, 1f, 0.5f, 0.5f);
		}
	}


	public class TenebrousDemonItemModifier : GlobalItem
	{
		public override bool InstancePerEntity => true;
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.TryGetModPlayer<TenebrousDemon>(out TenebrousDemon Demon) && item.DamageType == DamageClass.Ranged)
			{
				if (Demon.Charge3)
				{
					Projectile.NewProjectile(source, position, velocity.RotatedByRandom(1f), ModContent.ProjectileType<TenebrisStar>(), damage / 2, knockback, player.whoAmI, ai2: 1);
				}
			}
			return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
		}

	}

	public class TenebrousDemonHitTracker : GlobalProjectile
	{
		public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[projectile.owner];
			if (player.TryGetModPlayer<TenebrousDemon>(out TenebrousDemon Demon) && projectile.DamageType == DamageClass.Ranged)
			{
				Demon.ComboExpireTimer = 0;
				if (Demon.ComboCounter < TenebrousDemon.ComboTierThreshold)
				{
					Demon.ComboCounter++;
				}
			}
		}

	}
}