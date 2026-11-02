using DestroyerTest.Content.Projectiles.Weapon.Thrower;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.ThrowerItems
{
    public class HypnicJerk : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 6f;
            Item.shoot = ModContent.ProjectileType<HypnicJerkThrown>();
            Item.width = 36;
            Item.height = 36;
            Item.UseSound = SoundID.Item1;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.buyPrice(0, 0, 20, 0);
            Item.rare = ModContent.RarityType<VesperRarity>();
            Item.damage = 16;
            Item.autoReuse = true;
        }
    }
}