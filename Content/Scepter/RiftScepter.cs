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
 
using System.Security.Cryptography.X509Certificates;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;

namespace DestroyerTest.Content.Scepter
{
	public class RiftScepter : ScepterItem, IRechargeFunctionality
    {
        public bool Energized
        {
            get
            {
                return Main.LocalPlayer.GetModPlayer<Recharge>().Energized;
            }
        }
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
            ShootID = ModContent.ProjectileType<RiftScepterSun>();
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
            Item.shootSpeed = 2f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
        public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<Item_Riftplate>(22)
				.AddTile<Tile_RiftConfigurator>()
			.Register();
		}
	}
} 