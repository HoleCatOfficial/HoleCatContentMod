using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using static Terraria.ModLoader.ModContent;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Common;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tools;
using System.Collections.Generic;

using Terraria.Localization;
using DestroyerTest.Content.Resources.Blueprints;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;

namespace DestroyerTest.Content.RiftArsenal
{
	public class RiftChakram : RechargeItem
	{

        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults() {
			Item.width = 46;
			Item.height = 46;
			Item.value = Item.sellPrice(gold: 2, silver: 50);
			Item.rare = ModContent.RarityType<RiftRarity1>();
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item169;
			Item.knockBack = 0;
			Item.autoReuse = true;
			Item.damage = 80;
			Item.DamageType = DamageClass.Ranged;
            Item.crit = 30;
			Item.shoot = ModContent.ProjectileType<RiftChakramThrown>();
            Item.shootSpeed = 20f;
			Item.noUseGraphic = true;
		}

        
		public override void UseItemFrame(Player player)
        {
            float animationSpeed = 8.0f;
            float progress = ((player.itemAnimationMax - player.itemAnimation) / (float)player.itemAnimationMax);
            progress = Math.Min(progress * animationSpeed, 1.0f);

            float startAngle = MathHelper.ToRadians(180f);

            float endAngle;

            if (player.direction == 1)
            {
                endAngle = MathHelper.ToRadians(270f);
            }
            else if (player.direction == -1)
            {
                endAngle = MathHelper.ToRadians(90f);
            }
            else
            {
                endAngle = startAngle;
            }

            float armRotation = MathHelper.Lerp(startAngle, endAngle, progress);

            if (progress == 1.0f)
            {
                armRotation = endAngle;
            }

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRotation);
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MiscData>()
                .AddIngredient<Item_Riftplate>(37)
                .AddTile<Tile_RiftConfiguratorWeaponry>()
			.Register();
        }
    }
} 