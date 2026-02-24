using System.Collections.Generic;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Equips.ScepterAccessories;

namespace DestroyerTest.Content.Consumables
{
	// Basic code for a fishing crate
	// The catch code is in a separate ModPlayer class (ExampleFishingPlayer)
	// The placed tile is in a separate ModTile class
	public class HeliciteCrate : ModItem
	{
		public override void SetStaticDefaults() {
			// Disclaimer for both of these sets (as per their docs): They are only checked for vanilla item IDs, but for cross-mod purposes it would be helpful to set them for modded crates too
			ItemID.Sets.IsFishingCrate[Type] = true;
			ItemID.Sets.IsFishingCrateHardmode[Type] = true; // This is a crate that mimics a pre-hardmode biome crate, so this is commented out

			Item.ResearchUnlockCount = 10;
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tile_HeliciteCrate>());
			Item.width = 12; // The hitbox dimensions are intentionally smaller so that it looks nicer when fished up on a bobber
			Item.height = 12;
			Item.rare = ModContent.RarityType<RiftRarity2>();
			Item.value = Item.sellPrice(0, 2);
		}

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.Crates;
		}

		public override bool CanRightClick() {
			return true;
		}

		public override void ModifyItemLoot(ItemLoot itemLoot)
		{
			// Drop a special weapon/accessory etc. specific to this crate's theme (i.e. Sky Crate dropping Fledgling Wings or Starfury)
			int[] themedDrops = [
				//ModContent.ItemType<Accessories.ExampleBeard>(),
				//ModContent.ItemType<Accessories.ExampleStatBonusAccessory>()
			];
			//itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(1, themedDrops));

			// Drop coins
			itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 4, 5, 13));

			// Drop pre-hm ores, with the addition of one from ExampleMod
			IItemDropRule[] oreTypes = [
				ItemDropRule.Common(ItemID.AdamantiteOre, 1, 30, 50),
				ItemDropRule.Common(ItemID.TitaniumOre, 1, 30, 50),
				ItemDropRule.Common(ItemID.LihzahrdBrick, 1, 30, 50),
			];
			itemLoot.Add(new OneFromRulesRule(7, oreTypes));

			// Drop pre-hm bars (except copper/tin), with the addition of one from ExampleMod
			IItemDropRule[] oreBars = [
				ItemDropRule.Common(ItemID.MythrilBar, 1, 10, 21),
				ItemDropRule.Common(ItemID.OrichalcumBar, 1, 10, 21),
				ItemDropRule.Common(ItemID.PalladiumBar, 1, 10, 21),
				ItemDropRule.Common(ItemID.CobaltBar, 1, 10, 21),
				ItemDropRule.Common(ItemID.AdamantiteBar, 1, 10, 21),
				ItemDropRule.Common(ItemID.TitaniumBar, 1, 10, 21),
				ItemDropRule.Common(ModContent.ItemType<Item_HeliciteCrystal>(), 1, 10, 21),
			];
			itemLoot.Add(new OneFromRulesRule(4, oreBars));

			// Drop an "exploration utility" potion, with the addition of one from ExampleMod
			IItemDropRule[] explorationPotions = [
				ItemDropRule.Common(ItemID.ObsidianSkinPotion, 1, 2, 5),
				ItemDropRule.Common(ItemID.SpelunkerPotion, 1, 2, 5),
				ItemDropRule.Common(ItemID.HunterPotion, 1, 2, 5),
				ItemDropRule.Common(ItemID.GravitationPotion, 1, 2, 5),
				ItemDropRule.Common(ItemID.MiningPotion, 1, 2, 5),
				ItemDropRule.Common(ItemID.HeartreachPotion, 1, 2, 5),
				ItemDropRule.Common(ModContent.ItemType<HeliouricFlask>(), 1, 2, 5),
				ItemDropRule.Common(ModContent.ItemType<RiftFlask>(), 1, 2, 5)
			];

			// Pass the array to OneFromRulesRule
			itemLoot.Add(new OneFromRulesRule(4, explorationPotions));

			// Drop (pre-hm) resource potion
			IItemDropRule[] resourcePotions = [
				ItemDropRule.Common(ItemID.GreaterHealingPotion, 1, 5, 18),
				ItemDropRule.Common(ItemID.GreaterManaPotion, 1, 5, 18),
			];
			itemLoot.Add(new OneFromRulesRule(2, resourcePotions));

			// Drop (high-end) bait
			IItemDropRule[] highendBait = [
				ItemDropRule.Common(ItemID.JourneymanBait, 1, 2, 7),
				ItemDropRule.Common(ItemID.MasterBait, 1, 2, 7),
			];
			itemLoot.Add(new OneFromRulesRule(2, highendBait));

			IItemDropRule[] MiscLoot = [
				ItemDropRule.Common(ModContent.ItemType<HeliciteShank>(), 9, 1, 1),
				ItemDropRule.Common(ModContent.ItemType<HeliciteScroll>(), 4, 1, 1)
			];
			itemLoot.Add(new OneFromRulesRule(2, MiscLoot));
		}
	}
}