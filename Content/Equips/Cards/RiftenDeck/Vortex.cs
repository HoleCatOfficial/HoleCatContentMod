
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
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using DestroyerTest.Content.RiftBiome.RiftDesertResources;

namespace DestroyerTest.Content.Equips.Cards.RiftenDeck
{
	public class Vortex : ModItem
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
            foreach (Item item in Main.item)
            {
                int tm = player.GetItemGrabRange(item);
                tm = (int)(tm * 1.10f);
            }
		}

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return incomingItem.type != ModContent.ItemType<ShineShadeDeck>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Item_RiftStone>(6)
                .AddIngredient<Item_RiftSilt>(6)
            .Register();
        }
    }
}