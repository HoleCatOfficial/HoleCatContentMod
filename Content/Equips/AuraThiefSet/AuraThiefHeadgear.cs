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
using DestroyerTest.Common;

namespace DestroyerTest.Content.Equips.AuraThiefSet
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class AuraThiefHeadgear : ModItem
	{

		public int ParticleSpawnTimer = 0;


		public override void SetStaticDefaults() {
			// If your head equipment should draw hair while drawn, use one of the following:
			//ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
			// ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
			//ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
			// ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;

		}

		public override void SetDefaults() {
			Item.width = 36;
			Item.height = 28;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<LifeEchoRarity>();
			Item.defense = 4;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<AuraThiefBreastplate>() && legs.type == ModContent.ItemType<AuraThiefCuisses>();
		}

		public override void UpdateArmorSet(Player player) 
		{
			player.DefaultSetBonusText(Item);
			player.GetDamage(DamageClass.Melee) += 0.11f;
			player.buffImmune[BuffID.Frostburn] = true;
			player.buffImmune[BuffID.Frozen] = true;
			player.buffImmune[BuffID.Chilled] = true;
		}

         public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<LifeEcho>(16)
                .AddIngredient(ItemID.Wood, 10)
                .AddIngredient(ItemID.FlinxFur, 10)
				.AddTile(TileID.Anvils)
				.Register();
        }
	}
}