using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Rarity;

namespace DestroyerTest.Content.Ammunitions
{
    public class SpiritArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.width = 6;
            Item.height = 18;
            Item.damage = 7;
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(copper: 8);
            Item.shoot = ModContent.ProjectileType<SpiritArrowProjectile>(); // The projectile that weapons fire when using this item as ammunition.
            Item.rare = ModContent.RarityType<LifeEchoRarity>();
            Item.shootSpeed = 4f;
            Item.ammo = AmmoID.Arrow;
        }

        public override void AddRecipes()
        {
            CreateRecipe(5)
                .AddIngredient(ItemID.WoodenArrow)
                .AddIngredient<LifeEcho>(2)
                .Register();
        }
    }
}