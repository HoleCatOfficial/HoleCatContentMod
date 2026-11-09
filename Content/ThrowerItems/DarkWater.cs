using DestroyerTest.Content.Projectiles.Weapon.Thrower;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.ThrowerItems
{
    public class DarkWater : ModItem
    {

        public override void SetStaticDefaults()
        {

        }


        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.value = Item.sellPrice(silver: 50);
            Item.rare = ItemRarityID.Green;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.knockBack = 6;
            Item.autoReuse = true;
            Item.damage = 32;
            Item.DamageType = DamageClass.Throwing;
            Item.crit = 10;
            Item.shoot = ModContent.ProjectileType<DarkWaterThrown>();
            Item.shootSpeed = 15f;
            Item.noUseGraphic = true;
        }


    }
}