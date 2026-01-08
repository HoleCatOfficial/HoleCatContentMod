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
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Content.Projectiles.Weapon.Scepter; // Add this line if CT3_Swing is in the Projectiles namespace

namespace DestroyerTest.Content.Scepter
{
	public class OnyxScepter : ScepterItem
	{
		public override int Width => 40;
        public override int Height => 40;

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
            ShootID = ModContent.ProjectileType<OnyxWave>();
            ThrowID = ModContent.ProjectileType<OnyxScepterThrown>();

            // Optional: change sounds
            ShootSound = DTAssetLib.Impacts.MagicBeep with { MaxInstances = 0, PitchVariance = 0.4f };
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.DarkShard, 6)
				.AddIngredient(ItemID.SoulofNight, 12)
				.AddIngredient<ShadowScepter>()
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}


    }
} 