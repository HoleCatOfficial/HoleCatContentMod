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
using DestroyerTest.Content.Projectiles.player.ArmorSet;

namespace DestroyerTest.Content.Equips.AuraThiefSet
{

	[AutoloadEquip(EquipType.Head)]
	public class AuraThiefHeadress : ModItem
	{

		public override void SetStaticDefaults()
		{
			// If your head equipment should draw hair while drawn, use one of the following:
			// ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
			ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
																  //ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
																  // ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;

		}

		public override void SetDefaults()
		{
			Item.width = 32; // Width of the item
			Item.height = 22; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ModContent.RarityType<LifeEchoRarity>(); // The rarity of the item
			Item.defense = 3; // The amount of defense the item will give when equipped
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<AuraThiefBreastplate>() && legs.type == ModContent.ItemType<AuraThiefCuisses>();
		}

		public override void UpdateArmorSet(Player player)
		{
			if (player.TryGetModPlayer<AuraThiefScepterUsePlayer>(out AuraThiefScepterUsePlayer Scptr))
			{
				Scptr.Active = true;
			}
            player.DefaultSetBonusText(Item);
            player.GetDamage(ModContent.GetInstance<ScepterClass>()) += 0.08f;
			ScepterClassStats.ThrowSpeedModifier = 1.75f;
			player.buffImmune[BuffID.Frostburn] = true;
			player.buffImmune[BuffID.Frostburn2] = true;
			player.buffImmune[BuffID.Frozen] = true;
			player.buffImmune[BuffID.Chilled] = true;
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawOutlines = true;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<LifeEcho>(10)
				.AddIngredient(ItemID.Wood, 3)
				.AddIngredient(ItemID.FlinxFur, 7)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}

	public class AuraThiefScepterUsePlayer : ModPlayer
	{
		public bool Active = false;
        public override void ResetEffects()
        {
			Active = false;
        }
		public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			if (Active)
			{
				if (item.DamageType == ModContent.GetInstance<ScepterClass>())
				{
					if (Main.rand.NextBool(12))
					{
						SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/DAHit"), Player.Center);
						Projectile.NewProjectile(Entity.GetSource_ItemUse(item), position, velocity, ModContent.ProjectileType<AuraThiefFireball>(), 8, 4);
					}
				}
			}
		}
	}
}