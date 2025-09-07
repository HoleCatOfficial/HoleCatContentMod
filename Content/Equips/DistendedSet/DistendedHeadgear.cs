using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System;
using InnoVault.PRT;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using Terraria.Audio;

namespace DestroyerTest.Content.Equips.DistendedSet
{

	[AutoloadEquip(EquipType.Head)]
	public class DistendedHeadgear : ModItem
	{

		public override void SetStaticDefaults()
		{
			// If your head equipment should draw hair while drawn, use one of the following:
			// ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
			//ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
            // ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;

		}

		public override void SetDefaults()
		{
			Item.width = 32; // Width of the item
			Item.height = 22; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ModContent.RarityType<CrimsonSpecialRarity>(); // The rarity of the item
			Item.defense = 6; // The amount of defense the item will give when equipped
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<DistendedBodyArmor>() && legs.type == ModContent.ItemType<DistendedCuisses>();
		}

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.DestroyerTest.EquipSetBonus.Distended");
            player.GetDamage(ModContent.GetInstance<ScepterClass>()) += 0.10f;
            player.GetCritChance(ModContent.GetInstance<ScepterClass>()) += 10;
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawOutlines = true;
		}

		public override void AddRecipes()
		{
            CreateRecipe()
				.AddIngredient(ItemID.CrimsonHelmet, 1)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}