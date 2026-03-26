using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class RiftPlateBerserkerHelm : ModItem
	{


		public override void SetStaticDefaults() {
			// If your head equipment should draw hair while drawn, use one of the following:

		}

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ModContent.RarityType<RiftRarity2>(); // The rarity of the item
			Item.defense = 10; // The amount of defense the item will give when equipped
		}

        public override void UpdateEquip(Player player) {
			player.GetCritChance(DamageClass.Melee) += 4;

		}

		
		public override bool IsArmorSet(Item head, Item body, Item legs) {
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

		public static string Key = "Mods.DestroyerTest.Items.RiftPlateBerserkerHelm";
		public override void UpdateArmorSet(Player player)
		{
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
			player.GetDamage(DamageClass.Melee) += 0.15f;
            player.GetModPlayer<RiftBerserkerRunSpeeds>().Slow = true;
            player.AddBuff(ModContent.BuffType<RiftBallBuff>(), 3600);
			player.setBonus = Language.GetTextValue($"{Key}.SetBonusTitan");
		}

		private void AgilityBonus(Player player)
		{
			player.GetDamage(DamageClass.Melee) += 0.08f;
			player.GetAttackSpeed(DamageClass.Melee) += 0.12f;
			player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.10f;
			player.GetModPlayer<RiftBerserkerRunSpeeds>().Fast = true;
			player.AddBuff(ModContent.BuffType<RiftBallBuff>(), 3600);
            player.setBonus = Language.GetTextValue($"{Key}.SetBonusAgility");
        }

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<Living_Shadow>(20)
				.AddIngredient<Item_Riftplate>(20)
                .AddTile<Tile_RiftConfiguratorArmory>()
                .Register();
		}
	}

	public class RiftBerserkerRunSpeeds : ModPlayer
	{
		public bool Slow = false;
        public bool Fast = false;
        public override void ResetEffects()
        {
			Slow = false;
			Fast = false;
        }

        public override void PostUpdateRunSpeeds()
        {
            if (Fast)
			{
                Player.moveSpeed += 0.2f;
            }
            if (Slow)
            {
                Player.moveSpeed -= 0.15f;
            }
        }
	}
}