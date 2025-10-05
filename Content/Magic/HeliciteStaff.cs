using System;
using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Blueprints;
using DestroyerTest.Content.RiftArsenal;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Tools;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Magic
{
	public class HeliciteStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.staff[Type] = true;
		}

		public override void SetDefaults() {
			// DefaultToStaff handles setting various Item values that magic staff weapons use.
			// Hover over DefaultToStaff in Visual Studio to read the documentation!
			Item.shoot = ModContent.ProjectileType<RiftStar2>();
            Item.useTime = 30;
            Item.useAnimation = 30;
			Item.width = 92;
			Item.height = 92;
			Item.autoReuse = true;
			Item.crit = 12;
			Item.rare = ModContent.RarityType<RiftRarity1>();
			Item.useStyle = ItemUseStyleID.Shoot;

			// Customize the UseSound. DefaultToStaff sets UseSound to SoundID.Item43, but we want SoundID.Item2.
			Item.UseSound = new SoundStyle($"DestroyerTest/Assets/Audio/RiftClaymorePowerStrike");

			Item.DamageType = DamageClass.Magic;
            Item.mana = 70;
            Item.damage = 30;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			int Count = 9;

			for (int i = 0; i < Count; i++)
			{
				float angle = MathHelper.TwoPi * i / Count;
				Vector2 projPos = Main.MouseWorld + 80 * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
				Vector2 dustStart = position;
				Vector2 dustEnd = projPos;
				int dustSteps = 20;
				for (int j = 0; j <= dustSteps; j++)
				{
					Vector2 dustPos = Vector2.Lerp(dustStart, dustEnd, j / (float)dustSteps);
					Dust.NewDustPerfect(dustPos, DustID.TintableDustLighted, Vector2.Zero, 150, ColorLib.Rift, 1.2f);
				}

				Vector2 Inward = Main.MouseWorld - projPos;
				Inward.Normalize();
				Projectile.NewProjectile(source, projPos, Inward * 0.5f, type, damage, knockback, ai2: 1);
			}
            return false;
        }
		
		
		public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RiftStaff>()
                .AddIngredient<Item_HeliciteCrystal>(15)
                .AddTile<Tile_RiftConfiguratorWeaponry>()
            .Register();
        }
	}
}