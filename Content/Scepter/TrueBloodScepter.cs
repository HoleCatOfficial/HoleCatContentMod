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
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Content.Projectiles.EntitiesProjectiles;

namespace DestroyerTest.Content.Scepter
{
	public class TrueBloodScepter : ScepterItem
	{
		public override int Width => 114;
        public override int Height => 94;

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
            Rarity = ModContent.RarityType<CerisePinkRarity>();
            Item.shootSpeed = 20f;

            // Assign projectile types
            ShootID = ModContent.ProjectileType<EimvurBloodProjectile>();
            ThrowID = ModContent.ProjectileType<RiftScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item25;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void ShootDefaults()
        {
            base.ShootDefaults();
            Item.shootSpeed = 120;
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