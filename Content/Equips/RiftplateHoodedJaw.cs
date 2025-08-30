using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Riftplate;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Buffs;

namespace DestroyerTest.Content.Equips
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class RiftPlateHoodedJaw : ModItem
	{

		public override void SetStaticDefaults() {
			// If your head equipment should draw hair while drawn, use one of the following:
			// ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
			ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
			// ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
			// ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;
		}

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ModContent.RarityType<RiftRarity2>(); // The rarity of the item
			Item.defense = 8; // The amount of defense the item will give when equipped
		}
		
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			// Titan set
			if (body.type == ModContent.ItemType<RiftplateTitanBody>() &&
				legs.type == ModContent.ItemType<RiftplateTitanGreaves>())
			{
				return true;
			}

			// Agility set
			if (body.type == ModContent.ItemType<RiftplateAgilityArmor>() &&
				legs.type == ModContent.ItemType<RiftplateAgilityLeggings>())
			{
				return true;
			}

			// Anything else = not a valid set
			return false;
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player) {
			player.maxMinions += 3;
			if (player.body == ModContent.ItemType<RiftplateTitanBody>() &&
				player.legs == ModContent.ItemType<RiftplateTitanGreaves>())
			{
				TitanBonus(player);
			}
			else if (player.body == ModContent.ItemType<RiftplateAgilityArmor>() &&
					player.legs == ModContent.ItemType<RiftplateAgilityLeggings>())
			{
				AgilityBonus(player);
			}
		}
		
		private void TitanBonus(Player player)
		{
			player.GetDamage(DamageClass.Summon) *= 1.25f;
			player.statLifeMax2 += 15;
			player.AddBuff(ModContent.BuffType<RiftBallBuff>(), 3600);
			player.AddBuff(ModContent.BuffType<AirSeal>(), 3600);
		}

		private void AgilityBonus(Player player)
		{
			player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) *= 1.4f;
			player.moveSpeed *= 1.35f;
			player.AddBuff(ModContent.BuffType<RiftBallBuff>(), 3600);
			player.AddBuff(ModContent.BuffType<AirSeal>(), 3600);
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<Living_Shadow>(20)
				.AddIngredient<Item_Riftplate>(20)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}