using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
 
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Common;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;  

namespace DestroyerTest.Content.Scepter
{
	public class BalanceScepter : ScepterItem
	{
		public override int Width => 54;
        public override int Height => 54;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

		public override void SetDefaults()
		{
			base.SetDefaults();
			ShootDMG = 90;
			ShootCrit = 2;
			ThrowCrit = 8;
			KB = 2;
			AdditiveValue = Item.sellPrice(silver: 80);
			Rarity = ModContent.RarityType<WineRarity>();

			ShootID = ModContent.ProjectileType<BalanceBolt>();
			ThrowID = ModContent.ProjectileType<BalanceScepterThrown>();

			ShootSound = new SoundStyle(DTAssetLib.AudioPath + "/HopeScabbardTele") { PitchVariance = 0.5f, MaxInstances = 0 };
            ThrowSound = SoundID.Item169;

			base.SetDefaults();
		}

		int Count = 0;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
			Count++;
            if (player.altFunctionUse != 2)
			{
				type = Count % 2 == 0 ? ModContent.ProjectileType<SoulOfLight_Projectile>() : ModContent.ProjectileType<SoulOfNight_Projectile>();
            }
        }

        public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.SoulofLight, 9)
                .AddIngredient(ItemID.LightShard)
                .AddIngredient(ItemID.SoulofNight, 9)
                .AddIngredient(ItemID.DarkShard)
                .AddIngredient(ItemID.IronBar, 8)
				.AddTile(TileID.Anvils)
				.Register();
		}
    }
} 