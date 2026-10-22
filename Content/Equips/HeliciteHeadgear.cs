using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Head)]
	public class HeliciteHeadgear : ModItem
	{
		public override void SetStaticDefaults() 
		{
			ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
		}

		public override void SetDefaults() 
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<HeliciteRarity>(); 
			Item.defense = 12;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<HeliciteRobe>() && legs.type == ModContent.ItemType<HeliciteChausses>();
		}

		public override void UpdateArmorSet(Player player) 
		{
			player.DefaultSetBonusText(player.armor[0]);
			player.GetModPlayer<HeliciteMage>().Active = true;
		}
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<Living_Shadow>(15)
				.AddIngredient<Item_HeliciteCrystal>(5)
				.AddIngredient<Item_RiftClay>(3)
                .AddIngredient(ItemID.Silk, 15)
				.AddTile<Tile_RiftConfigurator>()
				.Register();
		}
	}

	public class HeliciteMage : ModPlayer
	{
		public bool Active = false;

        public override void ResetEffects()
        {
			Active = false;
        }

		int Counter = 0;
        public override void PostUpdateEquips()
        {
			if (Active)
			{
				if (DestroyerTestMod.ArmorSetBonusHotKey.Current && Player.statLife < Player.statLifeMax2 && Player.CheckMana(5, true))
				{
					if (Player.gravity > 0f)
					{ 
						Player.gravity -= 0.01f;
					}
					Counter++;
					Player.velocity *= 0.2f;
					if (Counter % 5 == 0)
					{
						Player.statLife += 1;

						RegenHeart heart = new();
						heart.Initialize(Main.rand.NextVector2FromRectangle(Player.Hitbox), new Vector2(0f, -2f), OpusColorUtils.MultiLerp(Opus.Sine(1f, 0f), ColorLib.HeliciteColorMap), 0.4f);
						ParticleEngine.Particles.Add(heart);
					}
				}
				else
				{
					if (Player.gravity < 1f)
					{
						Player.gravity += 0.01f;
					}
				}
			}
			else
			{
				if (Player.gravity < 1f)
				{
					Player.gravity += 0.01f;
				}
			}
		}
	}
}