using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Content.Projectiles;

using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.Projectiles.Weapon.Melee.Quixotism;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using GlowmaskHelper.Content;

namespace DestroyerTest.Content.MeleeWeapons
{
    [AutoloadGlowmask]
    public class SpiritOfJustice : ModItem
    {
        public int attackType = 0;
        public bool CanParry = true;
        public int ParryCooldown = 0;
        public const int MaxParryCooldown = 300;


        public override void SetDefaults()
        {
            Item.width = 92;
            Item.height = 92;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 80;
            Item.knockBack = 8f;
            Item.crit = 26;

            Item.value = Item.buyPrice(gold: 16);
            Item.rare = ModContent.RarityType<VesperRarity>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<SpiritOfJusticeSwing>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, attackType);
            attackType = (attackType + 1) % 2;
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            if (ParryCooldown > 0)
            {
                CanParry = false;
                ParryCooldown--;
                
                if (ParryCooldown == 1)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath7);
                }
            }
            else
            {
                CanParry = true;
            }
        }
        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.HallowedBar, 16)
            .AddIngredient<Vesper>(16)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
