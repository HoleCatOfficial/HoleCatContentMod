using System;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RangedItems
{
	public class TenebrousSniperRifle : ModItem
	{
        public override void SetDefaults()
        {
            Item.width = 226;
            Item.height = 62;
            Item.rare = ModContent.RarityType<ShimmeringRarity>();

            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.reuseDelay = 80;

            Item.UseSound = new SoundStyle($"DestroyerTest/Assets/Audio/GoliathPhantomHit")
            {
                Volume = 0.9f,
                PitchVariance = 0.2f,
                MaxInstances = 0
            };

            Item.DamageType = DamageClass.Ranged;
            Item.damage = 179;
            Item.knockBack = 9f;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<TenebrousBullet1>();
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 50f;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SniperRifle)
                .AddIngredient<Tenebris>(6)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }

		public override Vector2? HoldoutOffset() {
			return new Vector2(-50f, 0f);
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
            if (type == ProjectileID.Bullet)
            {
                type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<TenebrousBullet2>(), ModContent.ProjectileType<TenebrousBullet3>() });
            }
        }
	}
}