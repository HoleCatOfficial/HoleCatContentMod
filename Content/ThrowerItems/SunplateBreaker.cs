using DestroyerTest.Content.Projectiles.Weapon.Thrower;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.ThrowerItems
{
    public class SunplateBreaker : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<SunplateBreakerThrown>();
            Item.width = 70;
            Item.height = 70;
            Item.UseSound = SoundID.Item1;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Blue;
            Item.DamageType = DamageClass.Throwing;
            Item.damage = 20;
            Item.autoReuse = true;
            Item.crit = 15;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SunplateBlock, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}