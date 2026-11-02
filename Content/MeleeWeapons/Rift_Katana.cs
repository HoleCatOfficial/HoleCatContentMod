using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class Rift_Katana : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 140;
            Item.DamageType = DamageClass.MeleeNoSpeed; // Deals melee damage
            Item.width = 74;
            Item.height = 122;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<RiftRarity>();
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Rift_Katana_Projectile>();
            Item.shootSpeed = 5f;
            Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
        }
    }
}
