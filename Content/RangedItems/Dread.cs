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
    public class Dread : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 110;
            Item.height = 36;
            Item.rare = ModContent.RarityType<ShimmeringRarity>();

            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;

            Item.UseSound = new SoundStyle($"DestroyerTest/Assets/Audio/NightmareRose/CursedFlameShoot")
            {
                Volume = 0.9f,
                PitchVariance = 0.2f,
                MaxInstances = 0
            };

            Item.DamageType = DamageClass.Ranged;
            Item.damage = 80;
            Item.knockBack = 9f;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<TenebrisFlamesFriendly_NoHoming>();
            Item.useAmmo = AmmoID.Gel;
            Item.shootSpeed = 10f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Flamethrower)
                .AddIngredient<Tenebris>(6)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-35f, 0f);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<TenebrisFlamesFriendly_NoHoming>();
        }
	}
}