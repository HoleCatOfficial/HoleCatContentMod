using DestroyerTest.Content.Projectiles.Weapon.Thrower;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class TenebrisWaraxe : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item1;
            Item.width = 70;
            Item.height = 70;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.DamageType = DamageClass.Throwing;
            Item.damage = 300;
            Item.knockBack = 17;
            Item.crit = 4;

            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ModContent.RarityType<ShimmeringRarity>();
            Item.shoot = ModContent.ProjectileType<TenebrisWaraxeProjectile>();
            Item.shootSpeed = 42f;
            Item.ArmorPenetration = 15;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 3;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tenebris>(10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}