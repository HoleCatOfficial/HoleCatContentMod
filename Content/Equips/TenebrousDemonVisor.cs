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
		public bool Active = false;
		public bool Charge1 = false;
		public bool Charge2 = false;
		public bool Charge3 = false;
		public const int ComboTierThreshold = 40;
		public const int ComboExpire = 180;
		public int ComboExpireTimer = 0;
		public int ComboCounter = 0;

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
					SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { Pitch = 0 }, Player.Center);
					SoundFlag1 = true;
				}
				Player.GetDamage(DamageClass.Ranged) *= 1.1f;
				Player.GetCritChance(DamageClass.Ranged) += 6;
				Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, DustID.TintableDustLighted, (Player.velocity.X / 2) + Main.rand.NextFloat(-2, 2), (Player.velocity.Y / 2) + Main.rand.NextFloat(-2, 2), 200, ColorLib.TenebrisGradient, 0.4f);
				PRTLoader.NewParticle(DTUtils.ElectricArcs[Main.rand.Next(DTUtils.ElectricArcs.Length)], Main.rand.NextVector2FromRectangle(Player.Hitbox), Vector2.Zero, ColorLib.TenebrisGradient * 0.4f, 0.05f);
			}
			if (Charge2)
			{
				if (!SoundFlag2)
				{
					SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { Pitch = 1 }, Player.Center);
					SoundFlag2 = true;
				}
				Player.GetDamage(DamageClass.Ranged) *= 1.2f;
				Player.GetCritChance(DamageClass.Ranged) += 12;
				Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, DustID.TintableDustLighted, (Player.velocity.X / 2) + Main.rand.NextFloat(-2, 2), (Player.velocity.Y / 2) + Main.rand.NextFloat(-2, 2), 100, ColorLib.TenebrisGradient, 0.6f);
				PRTLoader.NewParticle(DTUtils.ElectricArcs[Main.rand.Next(DTUtils.ElectricArcs.Length)], Main.rand.NextVector2FromRectangle(Player.Hitbox), Vector2.Zero, ColorLib.TenebrisGradient * 0.6f, 0.1f);
			}
			if (Charge3)
			{
				if (!SoundFlag3)
				{
					SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { Pitch = 2 }, Player.Center);
					SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Destitute") with { PitchVariance = 0.2f }, Player.Center);
					SoundFlag3 = true;
				}
				Player.GetDamage(DamageClass.Ranged) *= 1.4f;
				Player.GetCritChance(DamageClass.Ranged) += 18;
				Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, DustID.TintableDustLighted, (Player.velocity.X / 2) + Main.rand.NextFloat(-2, 2), (Player.velocity.Y / 2) + Main.rand.NextFloat(-2, 2), 0, ColorLib.TenebrisGradient, 1f);
				PRTLoader.NewParticle(DTUtils.ElectricArcs[Main.rand.Next(DTUtils.ElectricArcs.Length)], Main.rand.NextVector2FromRectangle(Player.Hitbox), Vector2.Zero, ColorLib.TenebrisGradient, 0.2f);
				if (ComboCounter >= 120)
				{
					SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/NodeSpawn") with { PitchVariance = 0.2f }, Player.Center);
					Charge1 = Charge2 = Charge3 = false;
					SoundFlag1 = SoundFlag2 = SoundFlag3 = false;
					ComboCounter = 0;
				}
			}
		}
		
		public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			Vector2 drawPos = Player.Center - Main.screenPosition;
			SpriteBatch spriteBatch = Main.spriteBatch;
			drawPos.Y -= 200;

			string text = $"Combo: {ComboCounter.ToString()}";

			if (Active)
			{
				Utils.DrawBorderString(spriteBatch, text, drawPos, ColorLib.TenebrisGradient, 1f, 0.5f, 0.5f);
			}
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
				if (Demon.ComboCounter < TenebrousDemon.ComboTierThreshold && !Demon.Charge3)
				{
					Demon.ComboCounter++;
				}
				if (Demon.ComboCounter <= 120 && Demon.Charge3)
				{
					Demon.ComboCounter++;
				}
			}
		}

	}
}