using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Rarity;
using Terraria.Localization;
using DestroyerTest.Content.Tools;
using DestroyerTest.Content.Tiles.Riftplate;
using System.Collections.Generic;
using DestroyerTest.Content.RiftArsenal;
using DestroyerTest.Content.Resources.Blueprints;
using System.Security.Cryptography.X509Certificates;

namespace DestroyerTest.Content.Scepter
{
	public class RiftScepter : ScepterItem
	{
		public override int Width => 56;
        public override int Height => 56;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 57;
            ShootCrit = 4;
            ThrowCrit = 14;
            KB = 2;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<RiftRarity1>();
            Item.shootSpeed = 20f;

            // Assign projectile types
            ShootID = ModContent.ProjectileType<RiftBolt>();
            ThrowID = ModContent.ProjectileType<RiftScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item25;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public bool Energized = false;

        public override void UpdateInventory(Player player)
        {
            var modPlayer = player.GetModPlayer<LivingShadowPlayer>();
            if (modPlayer.LivingShadowCurrent > 0)
            {
                Energized = true;
            }
            if (modPlayer.LivingShadowCurrent <= 0)
            {
                Energized = false;
            }
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
            if (Energized && player.altFunctionUse != 2)
            {
                type = ModContent.ProjectileType<RiftStar>();
            }
        }

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<ScepterData>()
				.AddIngredient<Item_Riftplate>(22)
				.AddTile<Tile_RiftConfiguratorWeaponry>()
			.Register();
		}
	}
} 