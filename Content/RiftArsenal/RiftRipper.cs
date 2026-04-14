using System;
using System.Collections.Generic;
using DestroyerTest.Common;

using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Blueprints;
using DestroyerTest.Content.RiftBiome;

using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Tools;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RiftArsenal
{
    public class RiftRipper : ModItem, IRechargeFunctionality
    {
        public bool Energized
        {
            get
            {
                return Main.LocalPlayer.GetModPlayer<Recharge>().Energized;
            }
        }

        public override void SetDefaults()
        {
            Item.width = 100;
            Item.height = 36;
            Item.rare = ModContent.RarityType<RiftRarity1>();

            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;

            Item.UseSound = new SoundStyle($"DestroyerTest/Assets/Audio/RiftRipperShot")
            {
                PitchVariance = 0.2f,
                MaxInstances = 3,
            };

            Item.DamageType = DamageClass.Ranged;
            Item.damage = 60;
            Item.knockBack = 5f;
            Item.noMelee = true;

            Item.shoot = ProjectileID.PurificationPowder; 
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-20f, 0f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RevolverData>()
                .AddIngredient<ShadowCircuitry>(6)
                .AddIngredient<Item_Riftplate>(12)
                .AddTile<Tile_RiftConfiguratorWeaponry>()
            .Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.rand.NextBool(16) && Energized)
            {
                SoundEngine.PlaySound(new SoundStyle($"DestroyerTest/Assets/Audio/RiftRipperBoom") { Volume = 0.6f, PitchVariance = 0.2f, MaxInstances = 3 }, position);
                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectile(source, position, (velocity * 0.25f).RotatedByRandom(0.1f), ModContent.ProjectileType<RiftBolt2>(), damage / 4, knockback, player.whoAmI);
                }
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

    }
}