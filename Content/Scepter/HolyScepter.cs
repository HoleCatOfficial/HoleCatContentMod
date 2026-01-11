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
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;

namespace DestroyerTest.Content.Scepter
{
	public class HolyScepter : ScepterItem
	{
		public override int Width => 46;
        public override int Height => 46;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 75;
            ShootCrit = 14;
            ThrowCrit = 34;
            KB = 2;
            AdditiveValue = Item.sellPrice(silver: 80);
            Item.rare = ModContent.RarityType<WineRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<HolyOrb>();
            ThrowID = ModContent.ProjectileType<HolyScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.NPCDeath55;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 15)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
    }
} 