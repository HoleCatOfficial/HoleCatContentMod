using DestroyerTest.Content.Projectiles.Weapon.Thrower;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.ThrowerItems
{
    public class FailedPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 8f;
            Item.shoot = ModContent.ProjectileType<FailedPotionThrown>();
            Item.width = 34;
            Item.height = 72;
            Item.UseSound = SoundID.Item106;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.rare = ModContent.RarityType<CorruptionSpecialRarity>();
            Item.damage = 16;
            Item.autoReuse = true;
        }


    }
}