
﻿using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Common;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;

namespace DestroyerTest.Content.Equips
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class OrichalcumVisage : ModItem
	{

		public override void SetDefaults() {
			Item.width = 24; // Width of the item
			Item.height = 22; // Height of the item
			Item.value = Item.sellPrice(gold: 70); // How many coins the item is worth
			Item.rare = ModContent.RarityType<WineRarity>();
			Item.defense = 10; // The amount of defense the item will give when equipped
            Item.vanity = true;
		}

		// IsArmorSet determines what armor pieces are needed for the setbonus to take effect
		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ItemID.OrichalcumBreastplate && legs.type == ItemID.OrichalcumLeggings;
		}

		public override void UpdateArmorSet(Player player)
		{
			if (player.TryGetModPlayer<OrichalcumSetBonus>(out var Scptr))
			{
				Scptr.Active = true;
			}

			player.setBonus = Language.GetTextValue("Mods.DestroyerTest.Items.MythrilVisage.SetBonus");
		}

		public static readonly int SoloRangeBonus = 10;
		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SoloRangeBonus);
		public override void UpdateEquip(Player player)
        {
            ScepterClassStats.Range += SoloRangeBonus;
        }

		public override void AddRecipes() {
			CreateRecipe()
                .AddIngredient(ItemID.OrichalcumBar, 10)
                .AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}

	public class OrichalcumSetBonus : ModPlayer
	{
		public bool Active;
		public override void ResetEffects()
		{
			Active = false;
		}

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (Active)
			{
				if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse != 2)
				{
					float rotationStep = MathHelper.TwoPi / 8;
					float baseRotation = Main.rand.NextFloat(MathHelper.TwoPi);

					if (Main.rand.NextBool(12))
					{
						for (int i = 0; i < 8; i++)
						{
							float angle = rotationStep * i + baseRotation;
							Vector2 Position = Player.Center + new Vector2(50f, 0f).RotatedBy(angle);
							Vector2 Velocity = Main.MouseWorld - Position;
							Velocity = Velocity.ToRotation().ToRotationVector2() * 16f;
							Projectile.NewProjectile(source, Position, Velocity, ProjectileID.FlowerPetal, damage / 4, knockback, Player.whoAmI);
						}
					}
				}
			}
            return true;
        }


	}
}
