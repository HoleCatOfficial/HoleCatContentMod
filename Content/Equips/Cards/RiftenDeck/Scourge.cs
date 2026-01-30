
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Terraria.GameContent.ItemDropRules;
using System.Collections.Generic;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Resources;

namespace DestroyerTest.Content.Equips.Cards.RiftenDeck
{
	public class Scourge : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 24;
			Item.maxStack = 1;
			Item.value = 100;
			Item.accessory = true;
            Item.rare = ModContent.RarityType<RiftRarity1>();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
            player.GetDamage(DamageClass.Ranged) *= 1.05f;
            player.whipRangeMultiplier *= 1.08f;
            player.maxMinions += 1;
            player.GetAttackSpeed(DamageClass.Ranged) *= 0.95f;
            player.GetArmorPenetration(DamageClass.Generic) += 6;
		}

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return incomingItem.type != ModContent.ItemType<ShineShadeDeck>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Living_Shadow>(60)
                .AddIngredient(ItemID.SoulofSight, 8)
            .Register();
        }
    }
}