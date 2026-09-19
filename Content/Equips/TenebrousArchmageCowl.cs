
﻿using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using Steamworks;
using DestroyerTest.Common;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Buffs;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using DestroyerTest.Content.Projectiles.player.ArmorSet;
using Terraria.Audio;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Head)]
	public class TenebrousArchmageCowl : ModItem
	{

        public override void SetStaticDefaults() 
		{
			ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;

		}

		public override void SetDefaults() {
			Item.width = 28;
			Item.height = 22;
			Item.value = Item.sellPrice(gold: 70);
			Item.rare = ModContent.RarityType<ShimmeringRarity>();
			Item.defense = 29;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<TenebrousArchmageCoat>() && legs.type == ModContent.ItemType<TenebrousArchmagePants>();
		}

        public override void UpdateEquip(Player player)
        {
			player.GetModPlayer<TenebrisArchmageManaCost>().Active = true;
        }

		public override void UpdateArmorSet(Player player) 
		{
			player.DefaultSetBonusText(player.armor[0]);
			if (player.TryGetModPlayer<TenebrisMagicPlayer>(out var Magic))
            {
                Magic.Active = true;
            }
		}

		public override void AddRecipes() {
			CreateRecipe()
                .AddIngredient<Tenebris>(8)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}

	public class TenebrisArchmageManaCost : ModPlayer
	{
        public bool Active = false;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
        {
            if (Active)
			{
				reduce -= (int)(item.mana * 0.84f);
			}
        }
    }

	public class TenebrisMagicPlayer : ModPlayer
    {
		public bool Active = false;
		public override void ResetEffects()
		{
			Active = false;
		}

		public float Rot = 0;

		public int Cooldown = 0;
		public override void PostUpdateEquips()
		{
			Rot += 0.05f * Player.direction;
			if (Active)
			{
				Player.statManaMax2 += 100;

				if (Cooldown > 0)
				{
					Cooldown--;
				}

				if (Cooldown == 1)
				{
					SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Corpse/TeleportSetPosition") with { PitchVariance = 0.5f }, Player.Center);
				}

				if (DestroyerTestMod.ArmorSetBonusHotKey.JustPressed)
				{
					if (Cooldown <= 0)
					{
						SoundEngine.PlaySound(DTAssetLib.ChargeBreak, Player.Center);
						Opus.RingSpreadProjectile(ModContent.ProjectileType<MageClone>(), 3, Player.Center, 200, 10, 0, 0, offset: 0f);
						Cooldown = 1000;
					}
				}
			}
		}
    }
}
