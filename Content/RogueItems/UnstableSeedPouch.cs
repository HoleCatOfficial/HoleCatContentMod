using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RogueItems
{
    public class UnstableSeedPouch : ModItem
    {

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 36;
            Item.value = Item.sellPrice(silver: 50);
            Item.rare = ModContent.RarityType<CorruptionSpecialRarity>();
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = DTAssetLib.SwordSounds.SwiftSwing with { PitchVariance = 0.3f, MaxInstances = 0};
            Item.knockBack = 0f;
            Item.autoReuse = true;
            Item.damage = 3;
            Item.DamageType = DamageClass.Throwing;
            Item.crit = 10;
            Item.shoot = ModContent.ProjectileType<UnstableSeedProjectile>();
            Item.shootSpeed = 9f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 9; i++)
            {
                Projectile.NewProjectile(source, position, new Vector2(Main.rand.NextFloat(0.4f, 1.1f) * Item.shootSpeed * player.direction, Main.rand.NextFloat(-1.1f, -0.7f) * Item.shootSpeed), type, damage, knockback, player.whoAmI);
            }
            return false;
        }


    }
}