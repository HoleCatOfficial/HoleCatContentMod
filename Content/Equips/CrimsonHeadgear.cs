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

namespace DestroyerTest.Content.Equips
{

	[AutoloadEquip(EquipType.Head)]
	public class CrimsonHeadgear : ModItem
	{

		public override void SetStaticDefaults()
		{
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;

		}

		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 22;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<CrimsonSpecialRarity>();
			Item.defense = 6;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ItemID.CrimsonScalemail && legs.type == ItemID.CrimsonGreaves;
		}

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.DestroyerTest.Items.CrimsonHeadgear.SetBonus");
            player.GetDamage(ModContent.GetInstance<ScepterClass>()) *= 1.09f;
            player.lifeRegen += 10;
            ScepterClassStats.ThrowSpeedModifier *= 1.30f;
            ScepterClassStats.Range += 40;
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawOutlines = true;
		}

		public override void AddRecipes()
		{
            CreateRecipe()
				.AddIngredient(ItemID.CrimtaneBar, 15)
                .AddIngredient(ItemID.TissueSample, 10)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}