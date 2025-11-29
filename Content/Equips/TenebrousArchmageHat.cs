
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
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.ShadeThrasherFriendly;

namespace DestroyerTest.Content.Equips
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class TenebrousArchmageHat : ModItem
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
			Item.width = 30; // Width of the item
			Item.height = 20; // Height of the item
			Item.value = Item.sellPrice(gold: 70); // How many coins the item is worth
			Item.rare = ModContent.RarityType<ShimmeringRarity>(); // The rarity of the item
			Item.defense = 16; // The amount of defense the item will give when equipped
			Item.vanity = true;
		}

		//IsArmorSet determines what armor pieces are needed for the setbonus to take effect
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<TenebrousArchmageCoat>() && legs.type == ModContent.ItemType<TenebrousArchmagePants>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			if (player.TryGetModPlayer<TenebrisScepterPlayer>(out TenebrisScepterPlayer scptr))
			{
				scptr.Active = true;
			}
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<Tenebris>(8)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}
	
	public class TenebrisScepterPlayer : ModPlayer
    {
		public bool Active = false;
		public override void ResetEffects()
		{
			Active = false;
		}


		public float Rot = 0;
		public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			if (Active)
			{
				Main.EntitySpriteDraw(DTAssetLib.RuneCircle.Value, Player.Center - Main.screenPosition, null, Color.White, Rot, DTAssetLib.RuneCircle.Value.Size() / 2, 0.25f, SpriteEffects.None, 0);
			}
		}

		public bool Flag1 = false;
		public override void UpdateEquips()
		{
			Rot += 0.05f * Player.direction;
			if (Active)
			{
				Player.GetDamage(ModContent.GetInstance<ScepterClass>()) *= 1.15f;
				Player.moveSpeed += 0.4f;
				if (!Flag1)
				{
					Projectile.NewProjectile(Player.GetSource_None(), Player.Center, Vector2.One, ModContent.ProjectileType<ShadeThrasherFriendlyHead>(), 120, 7);
					Flag1 = true;
				}
                
			}

		}
		
		public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (Active)
            {
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<TenebrisStar>(), 5, Player.Center, 14, 4, 6, AI2: 1, RandomOffset: true);
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (Active)
            {
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<TenebrisStar>(), 3, Player.Center, 10, 4, 6, AI2: 1, RandomOffset: true);
            }
        }
    }
}
