
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.Weapon.Summon;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.SummonItems
{
    public class ShadeArmada : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f;
        }

        public override void SetDefaults()
        {
            Item.damage = 170;
            Item.knockBack = 9f;
            Item.mana = 70;
            Item.width = 84;
            Item.height = 84;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.value = 18000;
            Item.rare = ModContent.RarityType<ShimmeringRarity>();
            Item.UseSound = new SoundStyle("DestroyerTest/Assets/Audio/SwordSounds/HellSword", 3);

            Item.autoReuse = true;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<TenebrisCrystalMinionBuff>();

            Item.shoot = ModContent.ProjectileType<TenebrisElementalMinion>();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = player.MountedCenter;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<TenebrisElementalMinion>(), damage, knockback, Main.myPlayer);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tenebris>(16)
                .Register();
        }
    }
}
