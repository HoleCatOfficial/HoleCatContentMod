
﻿using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using DestroyerTest.Content.Projectiles.player.ArmorSet;
using OpusLib;
using System.Collections.Generic;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Head)]
	public class ForgottenCrown : ModItem
	{
        public override void SetStaticDefaults()
		{
			ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
		}
		public override void SetDefaults() {
			Item.width = 24;
			Item.height = 22;
			Item.value = Item.sellPrice(gold: 70);
			Item.rare = ModContent.RarityType<PearlRarity>();
			Item.defense = 3;
		}
		public override bool IsArmorSet(Item head, Item body, Item legs) 
        {
			return body.type == ModContent.ItemType<ForgottenPlatemail>() && legs.type == ModContent.ItemType<ForgottenGreaves>();
		}

		public static readonly int SoloRangeBonus = 10;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SoloRangeBonus);
		public override void UpdateArmorSet(Player player) 
        {
			if (player.TryGetModPlayer<ForgottenCrownPlayer>(out var Crown))
            {
                Crown.Active = true;
                Crown.JumpBoost = true;
            }
			player.DefaultSetBonusText(player.armor[0]);
		}

        public override void UpdateEquip(Player player)
        {
            if (player.TryGetModPlayer<ForgottenCrownPlayer>(out var Crown))
            {
                Crown.Active = true;
                Crown.Imbue = true;
            }
        }

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.PlatinumBar, 4)
            .Register();
        }
	}

    public class ForgottenCrownPlayer : ModPlayer
    {
        public bool Active = false;
        public bool JumpBoost = false;

        //Ignore these two. Theyre unrelated to the jump boost.
        public bool Imbue = false;
        public int cooldown = 0;

        public override void ResetEffects()
        {
            Active = false;
            JumpBoost = false;
            Imbue = false;

            if (cooldown > 0)
            {
                cooldown--;
            }
        }

        public override void PostUpdateEquips()
        {
            if (Active)
            {
                Player.GetJumpState<ForgottenSetExtraJump>().Enable();
            }
        }
    }


    public class ForgottenSetExtraJump : ExtraJump
	{
		public override Position GetDefaultPosition() => new After(BlizzardInABottle);

		public override IEnumerable<Position> GetModdedConstraints() {
			// By default, modded extra jumps set to be between two vanilla extra jumps (via After and Before) are ordered in load order.
			// This hook allows you to organize where this extra jump is located relative to other modded extra jumps that are also
			// placed between the same two vanilla extra jumps.
			yield return new Before(ExtraJump.GoatMount);
		}

		public override float GetDurationMultiplier(Player player) {
			// Use this hook to set the duration of the extra jump
			// The XML summary for this hook mentions the values used by the vanilla extra jumps
			return 1.65f;
		}

		public override void UpdateHorizontalSpeeds(Player player) {
			// Use this hook to modify "player.runAcceleration" and "player.maxRunSpeed"
			// The XML summary for this hook mentions the values used by the vanilla extra jumps
			player.runAcceleration *= 3.18f;
			player.maxRunSpeed *= 1.9f;
		}

		public override void OnStarted(Player player, ref bool playSound) {
			// Use this hook to trigger effects that should appear at the start of the extra jump
			// This example mimics the logic for spawning the puff of smoke from the Cloud in a Bottle
			int offsetY = player.height;
			if (player.gravDir == -1f)
				offsetY = 0;

			offsetY -= 16;

			for (int i = 0; i < 10; i++) {
				Dust dust = Dust.NewDustDirect(player.position + new Vector2(-34f, offsetY), 102, 32, DustID.Cloud, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 100, Color.Gray, 1.5f);
				dust.velocity = dust.velocity * 0.5f - player.velocity * new Vector2(0.1f, 0.3f);
			}

			SpawnCloudPoof(player, player.Top + new Vector2(-16f, offsetY));
			SpawnCloudPoof(player, player.position + new Vector2(-36f, offsetY));
			SpawnCloudPoof(player, player.TopRight + new Vector2(4f, offsetY));
		}

		private static void SpawnCloudPoof(Player player, Vector2 position) {
			Gore gore = Gore.NewGoreDirect(player.GetSource_FromThis(), position, -player.velocity, Main.rand.Next(11, 14));
			gore.velocity.X = gore.velocity.X * 0.1f - player.velocity.X * 0.1f;
			gore.velocity.Y = gore.velocity.Y * 0.1f - player.velocity.Y * 0.05f;
		}

		public override void ShowVisuals(Player player) {
			// Use this hook to trigger effects that should appear throughout the duration of the extra jump
			// This example mimics the logic for spawning the dust from the Blizzard in a Bottle
			int offsetY = player.height - 6;
			if (player.gravDir == -1f)
				offsetY = 6;

			Vector2 spawnPos = new Vector2(player.position.X, player.position.Y + offsetY);

			for (int i = 0; i < 2; i++) {
				SpawnBlizzardDust(player, spawnPos, 0.1f, i == 0 ? -0.07f : -0.13f);
			}

			for (int i = 0; i < 3; i++) {
				SpawnBlizzardDust(player, spawnPos, 0.6f, 0.8f);
			}

			for (int i = 0; i < 3; i++) {
				SpawnBlizzardDust(player, spawnPos, 0.6f, -0.8f);
			}
		}

		private static void SpawnBlizzardDust(Player player, Vector2 spawnPos, float dustVelocityMultiplier, float playerVelocityMultiplier) {
			Dust dust = Dust.NewDustDirect(spawnPos, player.width, 12, DustID.Snow, player.velocity.X * 0.3f, player.velocity.Y * 0.3f, newColor: Color.Gray);
			dust.fadeIn = 1.5f;
			dust.velocity *= dustVelocityMultiplier;
			dust.velocity += player.velocity * playerVelocityMultiplier;
			dust.noGravity = true;
			dust.noLight = true;
		}
	}

    public class ForgottenCrownOwnedProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        

        public override void PostAI(Projectile projectile)
        {
            
        }
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[projectile.owner];
            DTConfig cfg = ModContent.GetInstance<DTConfig>();
            if (player.TryGetModPlayer<ForgottenCrownPlayer>(out var Crown) && projectile.owner == player.whoAmI && projectile.DamageType == ModContent.GetInstance<ScepterClass>() && projectile.type != ModContent.ProjectileType<ExplodingIcicle>())
            {
                if (Crown.Active && Crown.cooldown <= 0)
                {
                    Opus.RadialSpreadDust(DustID.Ice, 10, target.Center, 0, Color.White, 1f, 2, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                    for(int o = 0; o < Main.rand.Next(3, 6); o++)
                    {
						Vector2 speed = new Vector2(0, -3.5f).RotatedByRandom(1f);
                        Projectile.NewProjectile(player.GetSource_Misc("Crown Icicles"), target.Center, speed, ModContent.ProjectileType<ExplodingIcicle>(), projectile.damage / 2, 4, projectile.owner);
                    }
                    Crown.cooldown = 120;
                }
                if (Crown.Active && Crown.Imbue)
                {
                    target.AddBuff(BuffID.Frostburn, 300);
                }
            }
            
        }
    }
}
