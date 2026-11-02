using DestroyerTest.Content.Projectiles.Weapon.Thrower;
using GlowmaskHelper.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.ThrowerItems
{
    [AutoloadGlowmask]
    public class VoltaicFenzim : ModItem
    {

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 62;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ItemRarityID.Blue;
            Item.useTime = 90;
            Item.useAnimation = 69;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.knockBack = 6;
            Item.autoReuse = true;
            Item.damage = 28;
            Item.DamageType = DamageClass.Throwing;
            Item.crit = 10;
            Item.shoot = ModContent.ProjectileType<VoltaicFenzimThrown>();
            Item.shootSpeed = 30f;
            Item.noUseGraphic = true;
        }

        public override bool? UseItem(Player player)
        {
            foreach (Projectile active in Main.projectile)
            {
                if (active.active && active.type == ModContent.ProjectileType<VoltaicFenzimThrown>() && active.owner == player.whoAmI)
                {
                    active.Kill();
                }
            }
            return true;
        }
    }
}