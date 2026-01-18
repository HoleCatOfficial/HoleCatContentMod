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
using DestroyerTest.Content.Equips.AuraThiefSet;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using DestroyerTest.Common;
using OpusLib;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Head)]
	public class RiftHalo : ModItem
	{

		public int ParticleSpawnTimer = 0;


		public override void SetStaticDefaults() {
			// If your head equipment should draw hair while drawn, use one of the following:
			//ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
			// ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
			ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
			// ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;

		}

		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 16;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<RiftRarity1>();
			Item.defense = 14;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<RiftVeil>();
		}

		public override void UpdateArmorSet(Player player) {
			player.AddBuff(ModContent.BuffType<RiftBallBuff>(), 3600);
			player.GetDamage(ModContent.GetInstance<ScepterClass>()) *= 1.12f;
			ScepterClassStats.Range += 100;
			ScepterClassStats.ThrowSpeedModifier *= 1.45f; 
			if (player.TryGetModPlayer<RiftHaloPlayer>(out var Halo))
			{
				Halo.Active = true;
			}
			player.DefaultSetBonusText(player.armor[0]);
		}

        public override void UpdateEquip(Player player)
        {
			player.GetCritChance(ModContent.GetInstance<ScepterClass>()) += 22;
			player.GetDamage(ModContent.GetInstance<ScepterClass>()) *= 1.05f;
        }


        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }

		public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<Item_RiftStone>(16)
                .AddIngredient<Living_Shadow>(20)
				.AddTile(TileID.MythrilAnvil)
				.Register();
        }
	}

	public class RiftHaloPlayer : ModPlayer
	{
		public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateEquips()
        {
            if (Active)
			{
				Lighting.AddLight(Player.Center, ColorLib.Rift.ToVector3() * Opus.Sine(0.2f, 0.6f));
				Player.buffImmune[ModContent.BuffType<HeliouricShock>()] = true;
			}
        }

	}
}