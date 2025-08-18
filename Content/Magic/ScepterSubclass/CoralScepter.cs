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
using DestroyerTest.Content.Resources; // Add this line if CT3_Swing is in the Projectiles namespace

namespace DestroyerTest.Content.Magic.ScepterSubclass
{
	public class CoralScepter : ScepterItem
	{
		public override int Width => 34;
        public override int Height => 34;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 9;
            ShootCrit = 2;
            ThrowCrit = 8;
            KB = 2;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ItemRarityID.LightRed;

            // Assign projectile types
            ShootID = ModContent.ProjectileType<WaterShot>();
            ThrowID = ModContent.ProjectileType<CoralScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item25;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.Coral, 12)
				.AddIngredient<EnchantedScepter>()
				.AddCondition(Condition.InBeach)
                .AddCondition(Condition.InRain)
				.Register();
		}
    }
} 