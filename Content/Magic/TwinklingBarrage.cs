using DestroyerTest.Content.BossSummons;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Magic
{
    public class TwinklingBarrage : ModItem
    {
        public override void SetDefaults()
        {
            
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 4;
            Item.useAnimation = 20;
            Item.UseSound = SoundID.Item9;
            Item.mana = 25;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Magic;
            Item.SetWeaponValues(10, 6, 5);
            Item.shoot = ModContent.ProjectileType<StellarNeedle>();
            Item.shootSpeed = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;

            Item.rare = ModContent.RarityType<StellarRarity>();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = player.Center + Main.rand.NextVector2Circular(20, 20);
            
            type = Main.rand.NextBool(10) ? ModContent.ProjectileType<StellarNeedle>() : ModContent.ProjectileType<StellarShard>();

            if (type == ModContent.ProjectileType<StellarNeedle>())
            {
                velocity *= 2f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BladeChunk>()
                .AddIngredient(ItemID.ManaCrystal)
                .AddIngredient(ItemID.Leather, 3)
                .Register();
        }


    }
}