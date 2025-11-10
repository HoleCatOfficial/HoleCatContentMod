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
using DestroyerTest.Content.Projectiles.DiscordScepter;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;

namespace DestroyerTest.Content.Scepter
{
	public class CelestialDiscord : ScepterItem
	{
		public override int Width => 52;
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
			ShootDMG = 65;
			ShootCrit = 2;
			ThrowCrit = 8;
			KB = 15;
			AdditiveValue = Item.sellPrice(silver: 80);
			Rarity = ModContent.RarityType<IncarnadineRarity>();

			ShootID = ModContent.ProjectileType<SolarDart>();
			ThrowID = ModContent.ProjectileType<CelestialDiscordThrown>();

			ShootSound = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/TeleportSetPosition") with { PitchVariance = 0.4f, MaxInstances = 0};
			ThrowSound = new SoundStyle("DestroyerTest/Assets/Audio/HellWeaponDash", 3) with { PitchVariance = 0.4f, MaxInstances = 0};

			// Refresh defaults after overriding values
			base.SetDefaults();
		}
		
		 public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
            if (player.altFunctionUse == 2) // Throwing mode
            {
                type = ModContent.ProjectileType<CelestialDiscordThrown>();
            }
            if (player.altFunctionUse != 2) // Shooting mode
            {
			    type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<VortexDart>(), ModContent.ProjectileType<StardustDartBig>(), ModContent.ProjectileType<NebulaFlameSpawner>() });
            }
		}

        public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.LunarBar, 16)
				.AddIngredient(ItemID.FragmentVortex, 8)
				.AddIngredient(ItemID.FragmentStardust, 8)
				.AddIngredient(ItemID.FragmentSolar, 8)
				.AddIngredient(ItemID.FragmentNebula, 8)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
    }
} 