using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.PetrifiedSet
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class PetrifiedChaplet : ModItem
	{


		public override void SetStaticDefaults()
		{
			// If your head equipment should draw hair while drawn, use one of the following:
			//ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
			// ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
			ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
			// ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;

		}

		public override void SetDefaults()
		{
			Item.width = 32; // Width of the item
			Item.height = 28; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 5; // The amount of defense the item will give when equipped
		}

		// IsArmorSet determines what armor pieces are needed for the setbonus to take effect
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<PetrifiedChestplate>() && legs.type == ModContent.ItemType<PetrifiedGreaves>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			if (player.TryGetModPlayer<PetrifiedShieldPlayer>(out PetrifiedShieldPlayer Shield))
            {
                Shield.Active = true;
            }
            ScepterClassStats.Range += 8;
            player.lavaImmune = true;
            player.setBonus = Language.GetText("Mods.DestroyerTest.Items.PetrifiedChaplet.SetBonus").Value;
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawOutlines = true;
		}


	}
	
	public class PetrifiedShieldPlayer : ShieldPlayer
    {
        public override int MaxDurability => 400;
        private int _durability = 400;
		public override int Durability
		{
			get => _durability;
			set => _durability = Math.Clamp(value, 0, MaxDurability);
		}

        public override int Radius => 160;
        public override Color themeColor => ColorLib.JavelinEnergy;
        public override NetworkText[] DeathMSGs => new NetworkText[]
        {
            NetworkText.FromLiteral($"{Player.name} was sucked dry."),
            NetworkText.FromLiteral($"{Player.name} gave a little too much in return for too little."),
            NetworkText.FromLiteral($"{Player.name} got folded like a chair."),
            NetworkText.FromLiteral($"{Player.name} didnt have it in them to sustain their shield.")
        };
        public override int RechargeHealthTax => 5;
    }
}