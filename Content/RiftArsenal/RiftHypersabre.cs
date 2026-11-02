using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using GlowmaskHelper.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RiftArsenal
{
    [AutoloadGlowmask]
    public class RiftHypersabre : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 230;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.width = 112;
            Item.height = 116;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<RiftRarity>();
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RiftHypersabreSwing>();
            Item.shootSpeed = 1f;
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile p = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            p.scale = player.GetAdjustedItemScale(Item);
            return false;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<Living_Shadow>(20)
            .AddIngredient<Item_Riftplate>(10)
            .AddIngredient<ShadowCircuitry>(3)
            .AddTile<Tile_RiftConfigurator>()
            .Register();
        }

    }
}
