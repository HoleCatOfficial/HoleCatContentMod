using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class TrueSoulEdge : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item120;
            Item.width = 70; // The item texture's width.
            Item.height = 70; // The item texture's height.

            Item.useStyle = ItemUseStyleID.Swing; // The useStyle of the Item.
            Item.useTime = 20; // The time span of using the weapon. Remember in terraria, 60 frames is a second.
            Item.useAnimation = 20; // The time span of the using animation of the weapon, suggest setting it the same as useTime.
            Item.autoReuse = true; // Whether the weapon can be used more than once automatically by holding the use button.

            Item.DamageType = DamageClass.Melee; // Whether your item is part of the melee class.
            Item.damage = 140; // The damage your item deals.
            Item.knockBack = 6; // The force of knockback of the weapon. Maximum is 20
            Item.crit = 20; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.

            Item.value = Item.buyPrice(gold: 1); // The value of the weapon in copper coins.
            Item.rare = ModContent.RarityType<LifeEchoRarity>(); // Give this item our custom rarity.
            Item.shoot = ModContent.ProjectileType<TrueSoulEdgeCursorProjectile>();
            Item.shootSpeed = 1f;
            Item.channel = true;
        }
        public override bool MeleePrefix()
        {
            return true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SoulEdge>(1)
                .AddIngredient(ItemID.BrokenHeroSword, 1)
                .AddIngredient(ItemID.Ectoplasm, 30)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}