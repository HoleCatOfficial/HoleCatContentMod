using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Resources;
  
using DestroyerTest.Content.Dusts;
using DestroyerTest.Rarity;
using Terraria.GameInput;

using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;

namespace DestroyerTest.Content.MeleeWeapons
{
	public class Tenebrous_Katana : ModItem
	{
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults() {
			Item.damage = 200;
			Item.DamageType = DamageClass.MeleeNoSpeed; // Deals melee damage
			Item.width = 88;
			Item.height = 196;
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.channel = true; 
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<ShimmeringRarity>();
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Tenebrous_Katana_Projectile>();
			Item.shootSpeed = 5f;
			Item.channel = true;
            Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
		}

        public override bool CanUseItem(Player player)
        {
			return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void UpdateInventory(Player player)
        {
			var Config = ModContent.GetInstance<DTConfig>();
			if (Config.UnnerfTenebrousKatana == true) {
				Item.damage = 3680;
			} 
			if (Config.UnnerfTenebrousKatana == false) {
				Item.damage = 200;
			}
        }

        public override void AddRecipes() {
			CreateRecipe()
                .AddIngredient<Tenebris>(10)
				.AddIngredient<ShimmeringSludge>(12)
                .AddIngredient<Rift_Katana>(1)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}

	}

	public class TKPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
		{
			foreach (Projectile projectile in Main.projectile)
			{
				if (DestroyerTestMod.TenebrisTeleportKeybind.JustPressed && projectile.active && projectile.owner == Player.whoAmI && projectile.type == ModContent.ProjectileType<TenebrisClone>())
				{
					var StarBurstSound = new SoundStyle("DestroyerTest/Assets/Audio/RiftSwordMinionTeleport") with {
						Volume = 1.0f, 
						Pitch = 0.0f, 
						PitchVariance = 0.5f, 
					};
					SoundEngine.PlaySound(StarBurstSound);

					
					if (projectile.active && projectile.owner == Player.whoAmI && projectile.type == ModContent.ProjectileType<TenebrisClone>())
					{
							// Teleport the player to the projectile's center
							Player.position = projectile.Center - new Vector2(Player.width / 2, Player.height / 2);
							break;
					}
				}
			}
		}
    }
}
