using DestroyerTest.Content.Projectiles.Boss.NodeBoss.CursedFlame;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{

    public class Horizon : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.height = 39;
            Item.width = 39;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.shoot = ModContent.ProjectileType<CursedFireBomb>();
            Item.damage = 20;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shootSpeed = 0.004f;
        }
    }
}