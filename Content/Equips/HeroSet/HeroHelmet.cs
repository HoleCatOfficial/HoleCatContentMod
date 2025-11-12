using System;
using System.Configuration;

using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;

namespace DestroyerTest.Content.Equips.HeroSet
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class HeroHelmet : ModItem
	{
		public int ParticleSpawnTimer = 0;
		public override void SetStaticDefaults() {
			// If your head equipment should draw hair while drawn, use one of the following:

		}

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ModContent.RarityType<GoliathRarity>(); // The rarity of the item
			Item.defense = 3; // The amount of defense the item will give when equipped
		}

        public override void UpdateEquip(Player player) {
            player.GetDamage(DamageClass.Melee) += 0.12f;
		}

		// IsArmorSet determines what armor pieces are needed for the setbonus to take effect
		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<HeroBreastplate>() && legs.type == ModContent.ItemType<HeroGreaves>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			ParticleSpawnTimer++;
			player.addDPS(25);
			player.moveSpeed *= 0.75f;
		}
	}
	public class HeroHelmetPlayer : ModPlayer
    {
        
    }

}
