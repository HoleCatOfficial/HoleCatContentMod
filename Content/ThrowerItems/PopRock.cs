using DestroyerTest.Content.Projectiles.Weapon.Thrower;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.ThrowerItems
{
    public class PopRock : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 14f;
            Item.shoot = ModContent.ProjectileType<PopRockThrown>();
            Item.width = 32;
            Item.height = 32;
            Item.UseSound = SoundID.Item1;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.buyPrice(0, 0, 20, 0);
            Item.rare = ModContent.RarityType<RiftRarity>();
            Item.damage = 120;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Throwing;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Item_RiftStone>(10)
                .AddIngredient<Living_Shadow>(30)
                .Register();
        }
    }
}