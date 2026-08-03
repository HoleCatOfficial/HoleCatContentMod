using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class Committment : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 116; // The item texture's width.
            Item.height = 106; // The item texture's height.

            Item.useStyle = ItemUseStyleID.Shoot; // The useStyle of the Item.
            Item.SetSpecialMeleeStats();
            Item.autoReuse = true; // Whether the weapon can be used more than once automatically by holding the use button.

            Item.DamageType = ModContent.GetInstance<DTTrueMeleeClass>(); // Whether your item is part of the melee class.
            Item.damage = 110; // The damage your item deals.
            Item.knockBack = 4f; // The force of knockback of the weapon. Maximum is 20
            Item.crit = 6; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.

            Item.value = Item.buyPrice(gold: 16); // The value of the weapon in copper coins.
            Item.rare = ModContent.RarityType<StellarRarity>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<CommittmentSwing>();
            Item.channel = true;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (player.altFunctionUse == 2)
            {
                damage += 100;
            }

        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Vector2 T = Main.MouseWorld - player.Center;
                T.Normalize();
                velocity = T * (20 + player.GetTotalAttackSpeed(DamageClass.Melee));
                type = ModContent.ProjectileType<CommittmentThrow>();

            }
            else
            {
                velocity = default;
                type = Item.shoot;
                damage = 80;
            }
        }


        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.UseSound = SoundID.Item82;
            }
            else
            {
                Item.UseSound = null;
            }
            return player.ownedProjectileCounts[Item.shoot] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<CommittmentThrow>()] < 1;
        }

        public override bool MeleePrefix()
        {
            return true;
        }



        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Constitution>()
                .AddIngredient(ItemID.PearlstoneBlock, 8)
                .AddIngredient(ItemID.EbonstoneBlock, 8)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddIngredient(ItemID.PixieDust, 8)
                .Register();

            CreateRecipe()
                .AddIngredient<Constitution>()
                .AddIngredient(ItemID.PearlstoneBlock, 8)
                .AddIngredient(ItemID.CrimstoneBlock, 8)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddIngredient(ItemID.PixieDust, 8)
                .Register();
        }


    }
}
