using DestroyerTest.Content.Projectiles.Weapon.Thrower;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.ThrowerItems
{
    public class HestiasBane : ModItem
    {

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 44;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ItemRarityID.Expert;
            Item.useTime = 120;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.knockBack = 11;
            Item.autoReuse = true;
            Item.damage = 500;
            Item.DamageType = DamageClass.Throwing;
            Item.crit = 22;
            Item.shoot = ModContent.ProjectileType<HestiasBaneThrown>();
            Item.shootSpeed = 12.5f;
            Item.noUseGraphic = true;
        }
    }
}