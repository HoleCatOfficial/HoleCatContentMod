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
using DestroyerTest.Content.Projectiles;
using Terraria.Audio;

namespace DestroyerTest.Content.Equips
{

	[AutoloadEquip(EquipType.Head)]
	public class ShadowMonarchHeadgear : ModItem
	{

		public override void SetStaticDefaults()
		{
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 28;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<CorruptionSpecialRarity>();
			Item.defense = 6;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ItemID.ShadowScalemail && legs.type == ItemID.ShadowGreaves;
		}

        public override void UpdateArmorSet(Player player)
        {
            player.DefaultSetBonusText(player.armor[0]);
            player.GetCritChance(ModContent.GetInstance<ScepterClass>()) += 15;
            player.ScepterClass().ThrowSpeedModifier *= 1.30f;
            player.ScepterClass().Range += 40;
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawShadow = true;
		}

		public override void AddRecipes()
		{
            CreateRecipe()
				.AddIngredient(ItemID.DemoniteBar, 15)
                .AddIngredient(ItemID.ShadowScale, 10)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}