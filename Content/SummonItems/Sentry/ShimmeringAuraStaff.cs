
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Common;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.MeleeWeapons;

using Terraria.Localization;
using DestroyerTest.Content.Entities;
using System.Linq;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.Weapon.Summon;
using DestroyerTest.Content.Projectiles.Weapon.Summon.Sentry;

namespace DestroyerTest.Content.SummonItems.Sentry
{
    public class ShimmeringAuraStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f; // The default value is 1, but other values are supported. See the docs for more guidance. 
        }

        public override void SetDefaults()
        {
            Item.damage = 230;
            Item.knockBack = 9f;
            Item.sentry = true;
            Item.mana = 130;
            Item.width = 42;
            Item.height = 42;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = 18000;
            Item.rare = ModContent.RarityType<ShimmeringRarity>();
            Item.UseSound = SoundID.DD2_DefenseTowerSpawn;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.shoot = ModContent.ProjectileType<ShimmeringFireAura>();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            position = Main.MouseWorld;
            player.LimitPointToPlayerReachableArea(ref position);

            Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);

            player.UpdateMaxTurrets();
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DD2LightningAuraT3Popper)
                .AddIngredient<Tenebris>(16)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
