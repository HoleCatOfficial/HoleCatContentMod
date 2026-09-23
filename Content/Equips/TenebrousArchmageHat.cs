
using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.player.Accessory;
using DestroyerTest.Content.Projectiles.player.ArmorSet;
using DestroyerTest.Content.Projectiles.ShadeThrasherFriendly;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Steamworks;
﻿using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Head)]
	public class TenebrousArchmageHat : ModItem
	{

		public override void SetStaticDefaults()
		{
			ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; 
		}

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 20;
			Item.value = Item.sellPrice(gold: 70);
			Item.rare = ModContent.RarityType<ShimmeringRarity>();
			Item.defense = 29;
		}

        public override void UpdateEquip(Player player)
        {
			player.ScepterClass().ThrowSpeedModifier += 0.1f;
        }

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<TenebrousArchmageCoat>() && legs.type == ModContent.ItemType<TenebrousArchmagePants>();
		}

		public override void UpdateArmorSet(Player player)
		{
			player.DefaultSetBonusText(player.armor[0]);

            player.moveSpeed *= 1.1f;
            player.GetDamage<ScepterClass>() *= 1.15f;


            if (player.TryGetModPlayer<TenebrisScepterPlayer>(out TenebrisScepterPlayer scptr))
			{
				scptr.Active = true;
			}
		}

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
		public bool Flag1 = false;
		public int Cooldown = 0;
        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (Active)
			{
				if (item.DamageType.CountsAsClass<ScepterClass>())
				{
					if (Player.altFunctionUse == 2)
					{
                        if (Main.rand.NextBool(3))
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.3f), ModContent.ProjectileType<TenebrisFlamesFriendly>(), damage / 4, 3, Player.whoAmI);
                            }
                        }
                    }
				}
			}
            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }
    }

	public class TenebrousArchmageDrawLayer : PlayerDrawLayer
    {

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			Player player = drawInfo.drawPlayer;
			bool scepterActive = player.TryGetModPlayer<TenebrisScepterPlayer>(out var Scepter) && Scepter.Active;
			bool magicActive = player.TryGetModPlayer<TenebrisMagicPlayer>(out var Magic) && Magic.Active;
			return scepterActive || magicActive;
		}
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.FrozenOrWebbedDebuff);

        protected override void Draw(ref PlayerDrawSet drawInfo) 
        {
            Player player = drawInfo.drawPlayer;
			bool scepterActive = player.TryGetModPlayer<TenebrisScepterPlayer>(out var Scepter) && Scepter.Active;
			bool magicActive = player.TryGetModPlayer<TenebrisMagicPlayer>(out var Magic) && Magic.Active;
			if (scepterActive && drawInfo.shadow == 0)
            {
                DrawRuneRing(ref drawInfo, 1f, 0.25f, Scepter.Rot);
            }
			if (magicActive && drawInfo.shadow == 0)
            {
                DrawRuneRing(ref drawInfo, 1f, 0.25f, Magic.Rot);
            }
		}

        private void DrawRuneRing(ref PlayerDrawSet drawInfo, float Opacity = 1f, float Scale = 1f, float Rotation = 0f)
        {
            var Tex = DTAssetLib.RuneCircle.Value;

			var position = drawInfo.Center - Main.screenPosition;
			position = new Vector2((int)position.X, (int)position.Y);

            drawInfo.DrawDataCache.Add(new DrawData(
				Tex,
				position,
				null,
				Color.White with {A = 0} * Opacity,
				Rotation,
				Tex.Size() * 0.5f,
				Scale,
				SpriteEffects.None,
				0
			));
        }

    }
}
