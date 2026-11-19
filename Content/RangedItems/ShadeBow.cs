
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RangedItems
{
    public class ShadeBow : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 76;
            Item.rare = ModContent.RarityType<ShimmeringRarity>();

            Item.useTime = 8;
            Item.useAnimation = 8; 
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = new SoundStyle("DestroyerTest/Assets/Audio/ChargeBreak")
            {
                Volume = 0.9f,
                PitchVariance = 0.2f,
                MaxInstances = 3,
            };
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 65;
            Item.knockBack = 5f;
            Item.noMelee = true;

            Item.shoot = ProjectileID.PurificationPowder; 
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.Arrow; 
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShimmeringSludge>(12)
                .AddIngredient<Tenebris>(9)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }

        public override bool RangedPrefix()
        {
            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(2f, -2f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<TenebrisStarFriendly>(), Item.damage / 8, 2, player.whoAmI, ai2: 1);
            return true;
        }
      
    }
}