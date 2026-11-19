
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Remnants;
using DestroyerTest.Content.SummonItems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{

    public class RibChainsaw : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (ModLoader.HasMod("QoLCompendium"))
            {
                ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<WyvernTail>();
            }


            ItemID.Sets.IsChainsaw[Type] = true;
        }
        public const int HoldoutDistance = 45;
        public override void SetDefaults()
        {
            Item.width = 118;
            Item.height = 42;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ItemRarityID.Purple;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.autoReuse = true;
            Item.reuseDelay = 10;
            Item.knockBack = 7; 
            Item.damage = 120;
            Item.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<RibChainsawHoldout>();

            Item.tileBoost = 45;
            Item.axe = 50;
        }

        public override bool MeleePrefix()
        {
            return true; 
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            type = ModContent.ProjectileType<RibChainsawHoldout>();
            velocity = Vector2.Normalize(velocity) * HoldoutDistance;

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer);
            return false;
        }

        public override void AddRecipes()
        {
            if (DTCrossMod.WGIIsLoaded)
            {
                CreateRecipe()
                    .AddIngredient<WyvernCorpseArtifact>(3)
                    .Register();
            }
        }
    }
}