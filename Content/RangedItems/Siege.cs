using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RangedItems
{
	public class Siege : ModItem
	{
		public override void SetStaticDefaults() {
			AmmoID.Sets.SpecificLauncherAmmoProjectileFallback[Type] = ItemID.RocketLauncher;
			AmmoID.Sets.SpecificLauncherAmmoProjectileMatches.Add(Type, new Dictionary<int, int> {
				{ ItemID.RocketI, ModContent.ProjectileType<TenebrisRocketProjectile_NoTileDestroy>()},
                { ItemID.RocketIII, ModContent.ProjectileType<TenebrisRocketProjectile_NoTileDestroy>()},
                { ItemID.RocketII, ModContent.ProjectileType<TenebrisRocketProjectile_TileDestroy>()},
                { ItemID.RocketIV, ModContent.ProjectileType<TenebrisRocketProjectile_TileDestroy>()},
			});
		}

		public override void SetDefaults() {
			Item.DefaultToRangedWeapon(ProjectileID.RocketI, AmmoID.Rocket, singleShotTime: 120, shotVelocity: 10f, hasAutoReuse: true);
			Item.width = 132;
			Item.height = 50;
			Item.damage = 4000;
			Item.knockBack = 4f;
			Item.UseSound = new SoundStyle("DestroyerTest/Assets/Audio/SiegeShoot") { MaxInstances = 0, PitchVariance = 0.2f };
			Item.value = Item.buyPrice(gold: 40);
			Item.rare = ItemRarityID.Yellow;
		}
        
        int Cooldown = 0;
        public override void UpdateInventory(Player player)
        {
            if (Cooldown > 0)
            {
                Cooldown--;
                if (Cooldown == 60)
                {
                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/SiegeReload"), player.Center);
                }
            }
        }

        public override bool? UseItem(Player player)
        {
            Cooldown = 240;
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            return Cooldown <= 0;
        }

		public override Vector2? HoldoutOffset() {
			return new Vector2(5f, -25f);
		}

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.RocketLauncher, 1)
                .AddIngredient<Tenebris>(5)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}
}