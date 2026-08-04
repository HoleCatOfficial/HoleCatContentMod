using DestroyerTest.Content.Projectiles.AmmoProjectiles.RiftRocket;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Ammunitions
{
    public class RiftRocket : ModItem
    {

        public override void SetStaticDefaults()
        {
            AmmoID.Sets.IsSpecialist[Type] = true;

            AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[ItemID.RocketLauncher].Add(Type, ModContent.ProjectileType<RiftRocketProjectile>());
            AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[ItemID.GrenadeLauncher].Add(Type, ModContent.ProjectileType<RiftGrenadeProjectile>());
            AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[ItemID.ProximityMineLauncher].Add(Type, ModContent.ProjectileType<RiftMineProjectile>());
            AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[ItemID.SnowmanCannon].Add(Type, ModContent.ProjectileType<RiftSnowmanRocketProjectile>());

            AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[ItemID.Celeb2].Add(Type, ProjectileID.Celeb2RocketLarge);
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 14;
            Item.damage = 40;
            Item.knockBack = 4f;
            Item.consumable = true;
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(copper: 50);
            Item.ammo = AmmoID.Rocket;
        }

        public override void AddRecipes()
        {
            CreateRecipe(100)
                .AddIngredient(ItemID.RocketIV, 100)
                .AddIngredient<Item_Riftplate>(25)
                .AddIngredient<SunscorchedCinder>(5)
                .AddTile<Tile_RiftConfigurator>()
                .Register();
        }
    }
}