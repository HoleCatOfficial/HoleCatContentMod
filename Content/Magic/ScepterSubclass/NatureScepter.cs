using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using static Terraria.ModLoader.ModContent;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Common; // Add this line if CT3_Swing is in the Projectiles namespace

namespace DestroyerTest.Content.Magic.ScepterSubclass
{
	public class NatureScepter : ScepterItem
	{
		public override int Width => 56;
        public override int Height => 52;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 13;
            ShootCrit = 4;
            ThrowCrit = 14;
            KB = 2;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ItemRarityID.LightRed;

            // Assign projectile types
            ShootID = ModContent.ProjectileType<NatureShot>();
            ThrowID = ModContent.ProjectileType<NatureScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item25;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.RichMahogany, 16)
				.AddIngredient(ItemID.WhitePearl)
				.AddIngredient(ItemID.JungleSpores, 12)
				.AddTile(TileID.WorkBenches)
				.Register();
		}


    }
} 