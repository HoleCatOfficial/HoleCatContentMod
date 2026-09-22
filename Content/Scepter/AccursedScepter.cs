using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

using DestroyerTest.Content.Projectiles;
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Rarity.Scepter;
using OpusLib;

namespace DestroyerTest.Content.Scepter
{
    public class AccursedScepter : ScepterItem
    {
        public override int Width => 56;
        public override int Height => 56;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 90;
            ShootCrit = 6;
            ThrowCrit = 8;
            KB = 8;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<WineRarity>();


            // Assign projectile types
            ShootID = ModContent.ProjectileType<AccursedScepterShadowflame>();
            ThrowID = ModContent.ProjectileType<AccursedScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.DD2_EtherianPortalSpawnEnemy;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                Opus.RingSpreadProjectile(ModContent.ProjectileType<AccursedScepterShadowflame>(), 3, Main.MouseWorld, 200, damage, knockback, -4f, offset: Main.rand.NextFloat(MathHelper.TwoPi));
            }
            else
            {
                return base.Shoot(player, source, position, velocity, type, damage, knockback);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AncientBattleArmorMaterial, 1)
                .AddIngredient(ItemID.CobaltBar, 4)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
               .AddIngredient(ItemID.AncientBattleArmorMaterial, 1)
               .AddIngredient(ItemID.PalladiumBar, 4)
               .AddTile(TileID.Anvils)
               .Register();
        }
    }
}