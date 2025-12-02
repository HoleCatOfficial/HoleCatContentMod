using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles; // Add this line if CT3_Swing is in the Projectiles namespace
using DestroyerTest.Rarity;
using DestroyerTest.Common;
using System;
using DestroyerTest.Content.Projectiles.Weapon.Melee;

namespace DestroyerTest.Content.MeleeWeapons
{
	public class ConstantineScythe : ModItem
	{
        public override void SetStaticDefaults()
        {
        }

		private enum ScytheMode
		{
			Melee,
			Minion
		}

		private ScytheMode currentMode = ScytheMode.Melee;

		//Weapon Properties
		public override void SetDefaults()
		{
			// Common Properties
			Item.width = 56;
			Item.height = 52;
			Item.value = Item.sellPrice(gold: 2, silver: 50);
			Item.rare = ModContent.RarityType<TestRarity>();

			// Use Properties
			// Note that useTime and useAnimation for this item don't actually affect the behavior because the held projectile handles that. 
			// Each attack takes a different amount of time to execute
			// Conforming to the item useTime and useAnimation makes it much harder to design
			// It does, however, affect the item tooltip, so don't leave it out.
			Item.useTime = 100;
			Item.useAnimation = 100;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.UseSound = new SoundStyle($"DestroyerTest/Assets/Audio/CS_Slice") with
			{
				Volume = 1.0f,
				Pitch = 0.0f,
				PitchVariance = 0.5f,
			}; // The sound when the weapon is being used.

			// Weapon Properties
			Item.knockBack = 30;  // The knockback of your sword, this is dynamically adjusted in the projectile code.
			Item.autoReuse = true; // This determines whether the weapon has autoswing
			Item.damage = 480; // The damage of your sword, this is dynamically adjusted in the projectile code.
			Item.DamageType = DamageClass.Melee; // Deals melee damage
			Item.crit = 16; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.
			Item.noMelee = true;  // This makes sure the item does not deal damage from the swinging animation
			Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand

			// Projectile Properties
			Item.shoot = ModContent.ProjectileType<ConstantineScytheProjectile>(); // The sword as a projectile
		}

        public override bool AltFunctionUse(Player player)
		{
			return true; // Right click is always allowed
		}

		public override bool CanUseItem(Player player)
		{
			int minionType = ModContent.ProjectileType<ConstantineScytheMinionProjectile>();
			bool minionAlive = player.ownedProjectileCounts[minionType] > 0;

			if (player.altFunctionUse == 2) // Right click
			{
				if (minionAlive)
				{
					// Kill the minion
					for (int i = 0; i < Main.maxProjectiles; i++)
					{
						Projectile proj = Main.projectile[i];
						if (proj.active && proj.owner == player.whoAmI && proj.type == minionType)
						{
							proj.Kill();
						}
					}
					//SoundEngine.PlaySound(SoundID.Item14, player.Center);
				}
				else
				{
					currentMode = currentMode == ScytheMode.Melee ? ScytheMode.Minion : ScytheMode.Melee;
					string modeText = currentMode == ScytheMode.Melee ? "Melee Mode" : "Minion Mode";
					CombatText.NewText(player.getRect(), Color.LightGreen, modeText, true, false);
				}

				return false; // Do not use the item directly on right click
			}

			// Left click behavior depends on mode
			if (currentMode == ScytheMode.Melee)
			{
				// Prevent swinging if minion exists
				if (minionAlive)
					return false;

				Item.useStyle = ItemUseStyleID.Shoot;
				Item.UseSound = new SoundStyle($"DestroyerTest/Assets/Audio/CS_Slice")
				{
					Volume = 1f,
					Pitch = 0f,
					PitchVariance = 0.5f
				};
			}
			else // Minion mode
			{
				if (minionAlive) // Prevent multiple minions
					return false;

				Item.UseSound = SoundID.Item119;
			}

			return true;
		}

		public override bool? UseItem(Player player)
		{
			FullChargeImmunity(player);
			return true;
		}


		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (currentMode == ScytheMode.Minion)
			{
				player.SpawnMinionOnCursor(source, player.whoAmI, ModContent.ProjectileType<ConstantineScytheMinionProjectile>(), damage, knockback);
				return false;
			}

			return true; // Melee mode uses the normal scythe projectile
		}
		
		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.NextBool(2))
			{
				// Emit dusts when the sword is swung
				Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Granite);
			}
		}

		public SoundStyle Tick = new SoundStyle("DestroyerTest/Assets/Audio/IncreaseTick") with { PitchVariance = 0.7f, Volume = 0.1f, MaxInstances = 1200 };

		public bool hasplayefullsound = false;
		public override void UpdateInventory(Player player)
		{
			var modPlayer = player.GetModPlayer<MeleeImmunityPlayer>();

			if (player.HeldItem == Item)
			{
				if (modPlayer.Timer < modPlayer.ImmunityThreshold2)
				{
					FullCharge = false;

					SoundEngine.PlaySound(Tick, player.Center);
					modPlayer.Timer += (int)(1 * (1f + modPlayer.TimeSpeed));
					modPlayer.Timer = Math.Min(modPlayer.Timer, modPlayer.ImmunityThreshold2);
				}
				if (modPlayer.Timer >= modPlayer.ImmunityThreshold2)
				{
					if (hasplayefullsound == false)
					{
						SoundEngine.PlaySound(SoundID.Item129, player.Center);
						CombatText.NewText(player.getRect(), Color.Aquamarine, "Immunity on next swing!", true, false);
						hasplayefullsound = true;
					}
					FullCharge = true;
				}
			}

			base.UpdateInventory(player);
		}

		public bool FullCharge = false;

		public void FullChargeImmunity(Player me)
		{
			var modPlayer = me.GetModPlayer<MeleeImmunityPlayer>();
			if (modPlayer.Timer >= modPlayer.ImmunityThreshold2 && FullCharge == true)
			{

				me.immune = true;
				me.immuneTime = 60;
				modPlayer.Timer = 0;
				FullCharge = false;
				hasplayefullsound = false;
			}
			else
			{
				ResetOnUse(me);
			}
		}

		public void ResetOnUse(Player me)
		{
			
			var modPlayer = me.GetModPlayer<MeleeImmunityPlayer>();
			SoundEngine.PlaySound(SoundID.Item130, me.Center);
			modPlayer.Timer = 0;
		}

    }
} 