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

namespace DestroyerTest.Content.Scepter
{
	public class ObsidianOculus : ScepterItem
	{
		public override int Width => 48;
        public override int Height => 48;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 17;
            ShootCrit = 4;
            ThrowCrit = 14;
            KB = 2;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<PearlRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<ObsidianShard>();
            ThrowID = ModContent.ProjectileType<ObsidianOculusThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item25;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.Obsidian, 12)
				.AddIngredient(ItemID.SilverBar, 8)
				.AddIngredient<LifeEcho>(2)
				.AddTile(TileID.WorkBenches)
				.Register();
		}


    }
} 