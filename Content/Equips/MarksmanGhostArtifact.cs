using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.player.ArmorSet;
using DestroyerTest.Rarity.Scepter;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using DestroyerTest.Content.Projectiles.player.Accessory;

namespace DestroyerTest.Content.Equips
{
    public class MarksmanGhostArtifact : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 40;
            Item.maxStack = 1;
            Item.value = 100;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += 0.15f;
            player.GetArmorPenetration(DamageClass.Ranged) += 10;
            player.GetCritChance(DamageClass.Ranged) += 12;
            player.GetModPlayer<MarksmanGhostArtifactPlayer>().Active = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBar, 10)
                .AddIngredient(ItemID.TitaniumBar, 4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class MarksmanGhostArtifactPlayer : ModPlayer
    {
        public bool Active;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Active)
            {
                if (Main.rand.NextBool(5) && item.useAmmo == AmmoID.Bullet)
                {
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<GhostBullet>(), damage, knockback, Player.whoAmI);
                    return false;
                }
                if (Main.rand.NextBool(5) && item.useAmmo == AmmoID.StyngerBolt)
                {
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SpectralStyngerBolt>(), damage, knockback, Player.whoAmI);
                    return false;
                }
            }
            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }
    }
}
