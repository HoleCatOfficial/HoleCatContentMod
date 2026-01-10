using System;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Projectiles.Weapon.Ranged;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Blueprints;
using DestroyerTest.Content.RiftArsenal;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RangedItems
{
    public class RiftWretcher : RechargeItem
    {
        public override void SetDefaults()
        {
            Item.width = 150;
            Item.height = 46;
            Item.rare = ModContent.RarityType<RiftRarity1>();

            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;

            Item.UseSound = new SoundStyle($"DestroyerTest/Assets/Audio/HSZap")
            {
                Volume = 0.6f,
                PitchVariance = 0.4f,
                MaxInstances = 0
            };

            Item.DamageType = DamageClass.Ranged;
            Item.damage = 30;
            Item.knockBack = 4f;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<HeliouricSpreadProjectile>();
            Item.useAmmo = AmmoID.Gel;
            Item.shootSpeed = 17f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Flamethrower)
                .AddIngredient<MiscData>(3)
                .AddIngredient<RiftBattery>()
                .AddTile<Tile_RiftConfiguratorWeaponry>()
                .Register();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(8f, 0f);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<HeliouricSpreadProjectile>();

            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 125f;

			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) {
				position += muzzleOffset;
			}
        }
	}
}