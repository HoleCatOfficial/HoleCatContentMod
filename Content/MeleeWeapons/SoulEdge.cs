using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{

    public class SoulEdge : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {

            Item.UseSound = SoundID.Item71;
            Item.width = 40;
            Item.height = 40;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20; 
            Item.useAnimation = 20;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 15;
            Item.knockBack = 6;
            Item.crit = 6;

            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ModContent.RarityType<LifeEchoRarity>();
            Item.shoot = ModContent.ProjectileType<SoulEdgeProjectile>();
            Item.shootSpeed = 0.02f;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WoodenSword, 1)
                .AddIngredient(ItemID.FlinxFur, 2)
                .AddIngredient<LifeEcho>(20)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}