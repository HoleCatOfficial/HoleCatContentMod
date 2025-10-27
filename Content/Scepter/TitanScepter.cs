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
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter; // Add this line if CT3_Swing is in the Projectiles namespace

namespace DestroyerTest.Content.Scepter
{
	public class TitanScepter : ScepterItem
	{
		public override int Width => 54;
        public override int Height => 54;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 40;
            ShootCrit = 4;
            ThrowCrit = 14;
            KB = 2;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<WineRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<TitaniumShardBig>();
            ThrowID = ModContent.ProjectileType<TitanScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item25;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.TitaniumBar, 12)
                .AddIngredient(ItemID.WhitePearl)
                .AddTile(TileID.MythrilAnvil)
				.Register();
		}
    }
} 