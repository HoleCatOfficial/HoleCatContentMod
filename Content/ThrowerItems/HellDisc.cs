using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Thrower;
using DestroyerTest.Content.Projectiles.Weapon.Thrower.HellDisc;
using DestroyerTest.Content.Remnants;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using GlowmaskHelper.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.ThrowerItems
{
    public class HellDisc : ModItem
    {
        int ThrowCount = 0;
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 25f;
            Item.shoot = ModContent.ProjectileType<HellDiscThrown>();
            Item.width = 32;
            Item.height = 28;
            Item.UseSound = DTAssetLib.SwordSounds.SpinWave;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.buyPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Orange;
            Item.damage = 200;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Throwing;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                ThrowCount++;
            }
            
        
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (proj.ModProjectile is HellDiscThrown main)
            {
                if (ThrowCount > 3)
                {
                    main.Special = true;
                    ThrowCount = 0;
                }
            }
            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            
            if (player.altFunctionUse != 2)
            {
                type = ModContent.ProjectileType<HellDiscThrown>();
            }
            else
            {
                type = ModContent.ProjectileType<HellDiscThrownAlt>();
                velocity *= 2f;
                damage = (int)(damage * 0.8f);
            }
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<HellDiscThrown>()] < 2 && player.ownedProjectileCounts[ModContent.ProjectileType<HellDiscThrownAlt>()] < 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HellArmorScrap>(6)
                .Register();
        }
        
    }
}